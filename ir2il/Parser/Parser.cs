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
            return s == "i8" || s == "i16" || s == "i32" || s == "i64";
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

            if (tokens[currentToken].type != TokenType.LeftParenthese)
            {
                throw new SyntaxErrorException("Expected '(', got " + tokens[currentToken].type);
            }

            // TODO: Function parameter support
            while (tokens[currentToken].type != TokenType.RightParenthese)
            {
                ++currentToken;
            }

            // Here we intentially skip attributes & metadata, I do not plan to support them in any form

            while (tokens[currentToken].type != TokenType.LeftBrace)
            {
                Console.WriteLine("Skipped token " + tokens[currentToken].value);
                ++currentToken;
            }

            while (tokens[currentToken].type != TokenType.RightBrace)
            {
                BaseNode stmt = ParseStmt();
            }

            ++currentToken;

            return new Function(returnType, new List<string>());
        }

        BaseNode ParseStmt()
        {
            Console.WriteLine("Skipped token " + tokens[currentToken].value);
            
            ++currentToken;

            return null;
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
                Console.WriteLine("Skipped token " + tokens[currentToken].value);
                ++currentToken;
            }

            ++currentToken;

            return null;
        }
    }
}
