using ir2cil.Lexer;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ir2cil.Parser.NodeTypes
{
    internal class Function : BaseNode
    {
        List<BaseNode> stmts = new();
        List<Tuple<Token, Token>> arguments;
        string returnType;
        string name;
        Mono.Cecil.MethodDefinition method;
        Dictionary<string, VariableDefinition> variables = new();

        public Function(string returnType, string name, List<Tuple<Token, Token>> arguments)
        {
            this.name = name;
            this.returnType = returnType;
            this.arguments = arguments;
        }

        public void InsertNode(BaseNode node)
        {
            stmts.Add(node);
        }

        public int GetParameterIndexFromName(string s)
        {
            for(int i = 0; i < arguments.Count; i++) 
            {
                if (s == arguments[i].Item1.value) return i;
            }

            return -1;
        }

        public override void Codegen()
        {
            Console.WriteLine("Generating function");
            Module parentModule = GetParentModule();

            method = new Mono.Cecil.MethodDefinition(name, MethodAttributes.Public | MethodAttributes.Static, GetTypeSystem().TranslateLLVMTypeIntoILType(returnType));

            parentModule.wrapperClass.Methods.Add(method);
            RegisterMethodDefinition(name, method);
            method.Body.InitLocals = true;

            if (name == "main")
            {
                parentModule.assembly.EntryPoint = method;
            }

            // arguments
            foreach (var el in arguments)
            {
                method.Parameters.Add(new ParameterDefinition(el.Item1.value, ParameterAttributes.None, GetTypeSystem().TranslateLLVMTypeIntoILType(el.Item2.value)));
            }

            // create the method body
            var il = method.Body.GetILProcessor();
            
            foreach(BaseNode stmt in stmts)
            {
                stmt.Codegen();
            }
        }

        public override string ToString()
        {
            String result = returnType + " " + name + "(";

            if(arguments.Count > 0)
            {
                result += arguments[0].Item2.value + " " + arguments[0].Item1.value;

                for(int i = 1; i < arguments.Count; ++i)
                {
                    result += ", " + arguments[0].Item2.value + " " + arguments[0].Item1.value;
                }
            }

            result += "):\n";

            foreach (BaseNode node in stmts)
            {
                result += "    " + node.ToString();
            }
            return result;
        }

        public override Mono.Cecil.MethodDefinition GetParentFunctionMethodDefinition()
        {
            return method;
        }

        public override void RegisterVariable(string name, VariableDefinition variable)
        {
            variables[name] = variable;
            method.Body.Variables.Add(variable);
        }

        public override void PopulateParents()
        {
            foreach(BaseNode node in stmts)
            {
                node.parent = this;
                node.PopulateParents();
            }
        }

        public override void LoadOntoStack(Token value)
        {
            Console.WriteLine("Loading token:" + value);
            var ilproc = method.Body.GetILProcessor();

            if (value.type == Lexer.TokenType.LocalIdentifier)
            {
                int argIdx = GetParameterIndexFromName(value.value);
                if (argIdx == -1)
                {
                    var variable = GetVariable(value);
                    ilproc.Emit(OpCodes.Ldloc, variable);
                }
                else
                {
                    ilproc.Emit(OpCodes.Ldarg, argIdx);
                }
            }

            if (value.type == Lexer.TokenType.IntegerLiteral)
            {
                ilproc.Emit(OpCodes.Ldc_I4, Int32.Parse(value.value));
            }
        }

        public override VariableDefinition GetVariable(Token value)
        {
            if(value.type == Lexer.TokenType.LocalIdentifier)
            {
                return variables[value.value];
            }
            else
            {
                throw new SyntaxErrorException("Could not find variable " + value.value);
            }
        }
    }
}
