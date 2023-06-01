using ir2cil.Lexer;
using ir2cil.Parser.NodeTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono;
using System.Runtime.InteropServices;
using ir2il.Parser.NodeTypes;
using System.Threading;

namespace ir2cil.Parser
{
    internal class Parser
    {
        List<Token> tokens;
        int currentToken = 0;

        public Parser(List<Token> tokens) 
        {
            this.tokens = tokens;
        }

        public Module Parse()
        {
            Module module = new();

            while (currentToken < tokens.Count) 
            {
                BaseNode node = ParseTopLevel();
                if(node != null)
                {
                    module.InsertNode(node);
                }
            }

            return module;
        }

        private bool IsValidType(string s)
        {
            // TODO: ???
            return s == "i8" || s == "i16" || s == "i32" || s == "i64" || s == "void";
        }

        BaseNode ParseTopLevel()
        {
            while (tokens[currentToken].type == TokenType.Newline)
            {
                ++currentToken;
            }

            switch (tokens[currentToken].type)
            {
                case TokenType.GlobalIdentifier:
                    throw new NotImplementedException("Global variable support is not yet implemented");
                case TokenType.Keyword:
                    if (tokens[currentToken].value == "define")
                    {
                        return ParseMethod();
                    }

                    if (tokens[currentToken].value == "attributes")
                    {
                        return ParseAttributes();
                    }

                    if (tokens[currentToken].value == "declare")
                    {
                        return ParseDeclare();
                    }

                    throw new NotImplementedException("Unexpected keyword '" + tokens[currentToken].value + "' (" + tokens[currentToken].type.ToString() + ")");
                case TokenType.EOF:
                    ++currentToken;
                    return null;
                default:
                    throw new NotImplementedException("No handler for token " + tokens[currentToken].type + " implemented");
            }
        }

        /*
         * define [linkage] [PreemptionSpecifier] [visibility] [DLLStorageClass]
         * [cconv] [ret attrs]
         * <ResultType> @<FunctionName> ([argument list])
         * [(unnamed_addr|local_unnamed_addr)] [AddrSpace] [fn Attrs]
         * [section "name"] [partition "name"] [comdat [($name)]] [align N]
         * [gc] [prefix Constant] [prologue Constant] [personality Constant]
         * (!name !N)* { ... }
         */
        BaseNode ParseMethod()
        {
            while (tokens[currentToken].type == TokenType.Keyword && LexerUtils.StringToKeyword(tokens[currentToken].value) != Keyword.None) 
            {
                // Ignore the [linkage] [PreemptionSpecifier] [visibility] [DLLStorageClass] [cconv] [ret attrs] part
                ++currentToken;
            }

            string returnType = tokens[currentToken].value;
            if (!IsValidType(returnType))
            {
                throw new NotSupportedException("Token " + returnType + " does not describe a valid type");
            }

            ++currentToken;

            if (tokens[currentToken].type != TokenType.GlobalIdentifier)
            {
                throw new SyntaxErrorException("Expected function name, got '" + tokens[currentToken].value + "' (" + tokens[currentToken].type.ToString() + ")");
            }

            string functionName = tokens[currentToken].value;
            ++currentToken;

            var args = ParseArgumentList();

            while (tokens[currentToken].type != TokenType.LeftBrace)
            {
                ++currentToken;
            }

            ++currentToken;

            var func = new Function(returnType, functionName, args);

            ConsumeWhitespace();
            while (tokens[currentToken].type != TokenType.RightBrace)
            {
                BaseNode stmt = ParseStmt();
                func.InsertNode(stmt);
                ConsumeWhitespace();
            }

            ++currentToken;

            return func;
        }

        Jump ParseNormalJump()
        {
            ++currentToken;
            var where = tokens[currentToken];

            while (tokens[currentToken].type != TokenType.Newline)
            {
                ++currentToken;
            }
            ++currentToken;

            return new Jump(where);
        }

        ConditionalJump ParseCondJump()
        {
            var type = tokens[currentToken];
            ++currentToken;
            var what = tokens[currentToken];
            ++currentToken;
            ++currentToken;
            ++currentToken;
            var where1 = tokens[currentToken];
            ++currentToken;
            ++currentToken;
            ++currentToken;
            var where2 = tokens[currentToken];

            while (tokens[currentToken].type != TokenType.Newline)
            {
                ++currentToken;
            }
            ++currentToken;

            return new ConditionalJump(type, what, where1, where2);
        }

        BaseNode ParseJump()
        {
            ++currentToken;
            if (tokens[currentToken].value == "label")
            {
                return ParseNormalJump();
            }
            else
            {
                return ParseCondJump();
            }
        }

        BinaryOperation ParseBinaryOperation()
        {
            Token operation = tokens[currentToken];

            ++currentToken;

            while (LexerUtils.StringToKeyword(tokens[currentToken].value) != Keyword.None)
            {
                ++currentToken;
            }

            Token type = tokens[currentToken];
            ++currentToken;

            Token op1 = tokens[currentToken];
            ++currentToken;

            if (tokens[currentToken].type != TokenType.Comma)
            {
                throw new SyntaxErrorException("Expected ',', got '" + tokens[currentToken].value + "' (" + tokens[currentToken].type.ToString() + ")");
            }

            ++currentToken;

            Token op2 = tokens[currentToken];

            while (tokens[currentToken].type != TokenType.Newline)
            {
                ++currentToken;
            }
            ++currentToken;

            return new BinaryOperation(BinaryOperation.GetBinaryOpFromString(operation.value), type, op1, op2);
        }

        UnaryOperation ParseUnaryOperation()
        {
            Token operation = tokens[currentToken];

            // There can be an unknown amount of keywords depending on the operation, however the last 4 tokens on the line are guaranteed to be <ty> <op1>, <op2>
            while (tokens[currentToken].type != TokenType.Newline)
            {
                ++currentToken;
            }
            var type = tokens[currentToken - 2];
            var op1 = tokens[currentToken - 1];

            return new UnaryOperation(UnaryOperation.GetUnaryOpFromString(operation.value), type, op1);
        }

        Alloca ParseAlloca(Token identifier)
        {
            ++currentToken;
            Token token = tokens[currentToken];
            ++currentToken;
            // TODO: Implement support for arrays / structs
            while (tokens[currentToken].type != TokenType.Newline)
            {
                ++currentToken;
            }
            ++currentToken;

            return new Alloca(identifier, token);
        }

        Ret ParseRet()
        {
            ++currentToken;
            Token token = tokens[currentToken];
            ++currentToken;
            Token value = tokens[currentToken];
            ++currentToken;
            // TODO: Implement support for arrays / structs
            while (tokens[currentToken].type != TokenType.Newline)
            {
                ++currentToken;
            }
            ++currentToken;

            return new Ret(token, value);
        }

        MiscOperation ParseMiscOperation()
        {
            Token operation = tokens[currentToken];
            ++currentToken;
            var op1 = tokens[currentToken];
            ++currentToken;

            if (tokens[currentToken].type != TokenType.Comma)
            {
                throw new SyntaxErrorException("Expected ',', got '" + tokens[currentToken].value + "' (" + tokens[currentToken].type.ToString() + ")");
            }
            ++currentToken;
            var op2 = tokens[currentToken];
           
            // There can be trailing metadata / keywords for some operations of this type
            while (tokens[currentToken].type != TokenType.Newline)
            {
                ++currentToken;
            }

            return new MiscOperation(MiscOperation.GetMiscOperationFromString(operation.value), operation, op1, op2);
        }

        Store ParseStore()
        {
            ++currentToken;
            Token token = tokens[currentToken];
            ++currentToken;
            Token value = tokens[currentToken];
            ++currentToken;
            if (tokens[currentToken].type != TokenType.Comma)
            {
                throw new SyntaxErrorException("Expected ',', got '" + tokens[currentToken].value + "' (" + tokens[currentToken].type.ToString() + ")");
            }
            ++currentToken; // always ptr
            ++currentToken;
            Token where = tokens[currentToken];
            ++currentToken;
            // TODO: Implement support for arrays / structs
            while (tokens[currentToken].type != TokenType.Newline)
            {
                ++currentToken;
            }
            ++currentToken;

            return new Store(token, value, where);
        }

        Load ParseLoad()
        {
            ++currentToken;
            Token type1 = tokens[currentToken];
            ++currentToken;
            if (tokens[currentToken].type != TokenType.Comma)
            {
                throw new SyntaxErrorException("Expected ',', got '" + tokens[currentToken].value + "' (" + tokens[currentToken].type.ToString() + ")");
            }
            ++currentToken; 
            Token type2 = tokens[currentToken];
            ++currentToken;
            Token where = tokens[currentToken];
            ++currentToken;
            // TODO: Implement support for arrays / structs
            while (tokens[currentToken].type != TokenType.Newline)
            {
                ++currentToken;
            }
            ++currentToken;

            return new Load(type1, type2, where);
        }

        BaseNode ParseDeclare()
        {
            Console.WriteLine("Skipping declare");
            while (tokens[currentToken].type != TokenType.Newline)
            {
                ++currentToken;
            }
            ++currentToken;

            return null;
        }

        List<Tuple<Token, Token>> ParseArgumentList()
        {
            List<Tuple<Token, Token>> parameters = new();

            // Parse argument list
            if (tokens[currentToken].type != TokenType.LeftParenthese)
            {
                throw new SyntaxErrorException("Expected '(', got '" + tokens[currentToken].value + "' (" + tokens[currentToken].type.ToString() + ")");
            }
            ++currentToken;

            while (tokens[currentToken].type != TokenType.RightParenthese)
            {
                if (tokens[currentToken].value == "metadata" || tokens[currentToken].type == TokenType.Newline)
                {
                    while (tokens[currentToken].type != TokenType.Comma && tokens[currentToken].type != TokenType.RightParenthese)
                    {
                        ++currentToken;
                    }
                    if (tokens[currentToken].type != TokenType.RightParenthese) ++currentToken;
                    continue;
                }

                if (tokens[currentToken].type == TokenType.RightParenthese)
                {
                    ++currentToken;
                    break;
                }

                Token type = tokens[currentToken];
                ++currentToken;

                while (LexerUtils.StringToKeyword(tokens[currentToken].value) != Keyword.None)
                {
                    ++currentToken;
                }

                Token parameterName = tokens[currentToken];
                ++currentToken;

                if (tokens[currentToken].type == TokenType.RightParenthese)
                {
                    ++currentToken;
                    parameters.Add(new Tuple<Token, Token>(parameterName, type));
                    break;
                }

                if (tokens[currentToken].type != TokenType.Comma)
                {
                    throw new SyntaxErrorException("Expected ',', got '" + tokens[currentToken].value + "' (" + tokens[currentToken].type.ToString() + ")");
                }
                ++currentToken;

                parameters.Add(new Tuple<Token, Token>(parameterName, type));
            }

            // Skip attributes
            while (tokens[currentToken].type != TokenType.Newline && tokens[currentToken].type != TokenType.LeftBrace)
            {
                ++currentToken;
            }
            if(tokens[currentToken].type != TokenType.LeftBrace) ++currentToken;

            return parameters;
        }
        Call ParseCall()
        {
            ++currentToken;

            // Skip stuff like noundef that we have no use for
            while (!IsValidType(tokens[currentToken].value))
            {   
                ++currentToken;
            }

            Token token = tokens[currentToken];
            ++currentToken;
            
            Token value = tokens[currentToken];
            ++currentToken;

            return new Call(token, value, ParseArgumentList());
        }

        BaseNode ParseStmt()
        {
            if (tokens[currentToken].type == TokenType.Label)
            {
                var l = tokens[currentToken];
                ++currentToken;
                return new Label(l);
            }

            if (tokens[currentToken].type == TokenType.LocalIdentifier)
            {
                Token identifier = tokens[currentToken];
                ++currentToken;

                if (tokens[currentToken].type != TokenType.Equals)
                {
                    throw new SyntaxErrorException("Expected '=', got '" + tokens[currentToken].value + "' (" + tokens[currentToken].type.ToString() + ")");
                }
                ++currentToken;

                if (BinaryOperation.IsBinaryOperation(tokens[currentToken].value))
                {
                    return new VariableDeclaration(identifier, ParseBinaryOperation());
                }
                else if (UnaryOperation.IsUnaryOperation(tokens[currentToken].value))
                {
                    return new VariableDeclaration(identifier, ParseUnaryOperation());
                }
                else if (Alloca.IsAlloca(tokens[currentToken].value))
                {
                    return ParseAlloca(identifier);
                }
                else if (Load.IsLoad(tokens[currentToken].value))
                {
                    return new VariableDeclaration(identifier, ParseLoad());
                }
                else if (Call.IsCall(tokens[currentToken].value))
                {
                    return new VariableDeclaration(identifier, ParseCall());
                }
                else if (Comparison.IsComparison(tokens[currentToken].value))
                {
                    return new VariableDeclaration(identifier, ParseCompare());
                }
                else
                {
                    throw new NotImplementedException("Not yet implemented.");
                }
            }
            
            if (Ret.IsRet(tokens[currentToken].value))
            {
                return ParseRet();
            }
            else if (Store.IsStore(tokens[currentToken].value))
            {
                return ParseStore();
            }
            else if (Call.IsCall(tokens[currentToken].value)) 
            {
                return ParseCall();    
            }
            else if (ConditionalJump.IsJump(tokens[currentToken].value))
            {
                return ParseJump();
            }

            ++currentToken;
            return ParseMiscOperation();
        }

        // TODO: ???
        BaseNode ParseAttributes()
        {
            ++currentToken;

            if (tokens[currentToken].type != TokenType.AttributeGroup)
            {
                throw new SyntaxErrorException("Expected attribute group name, got '" + tokens[currentToken].value + "' (" + tokens[currentToken].type.ToString() + ")");
            }

            string name = tokens[currentToken].value;

            ++currentToken;

            if (tokens[currentToken].type != TokenType.Equals)
            {
                throw new SyntaxErrorException("Expected '=', got '" + tokens[currentToken].value + "' (" + tokens[currentToken].type.ToString() + ")");
            }

            ++currentToken;

            if (tokens[currentToken].type != TokenType.LeftBrace)
            {
                throw new SyntaxErrorException("Expected left brace, got '" + tokens[currentToken].value + "' (" + tokens[currentToken].type.ToString() + ")");
            }

            while (tokens[currentToken].type != TokenType.RightBrace)
            {
                ++currentToken;
            }

            ++currentToken;

            return null;
        }

        Comparison ParseCompare()
        {
            Token identifier = tokens[currentToken];
            ++currentToken;
            CondType cond = Comparison.GetCondTypeFromString(tokens[currentToken].value);
            ++currentToken;
            Token type = tokens[currentToken];
            ++currentToken;
            Token lhs = tokens[currentToken];
            ++currentToken;
            if (tokens[currentToken].type != TokenType.Comma)
            {
                throw new SyntaxErrorException("Expected comma, got '" + tokens[currentToken].value + "' (" + tokens[currentToken].type.ToString() + ")");
            }
            ++currentToken;
            Token rhs = tokens[currentToken];

            while (tokens[currentToken].type != TokenType.Newline)
            {
                ++currentToken;
            }
            ++currentToken;

            return new Comparison(Comparison.GetComparisonTypeFromString(identifier.value), cond, type, lhs, rhs);
        }

        void ConsumeWhitespace()
        {
            while (tokens[currentToken].type == TokenType.Newline)
            {
                ++currentToken;
            }
        }
    }
}
