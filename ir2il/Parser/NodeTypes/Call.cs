using ir2cil.Lexer;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;

namespace ir2cil.Parser.NodeTypes
{
    internal class Call : BaseNode
    {
        public static bool IsCall(string opName)
        {
            return opName == "call";
        }

        public Call(Token returnType, Token name, List<Tuple<Token, Token>> parameters)
        {
            this.returnType = returnType;
            this.name = name;
            this.parameters = parameters;
        }

        public Token returnType;
        public Token name;
        public List<Tuple<Token, Token>> parameters;

        public override string ToString()
        {
            string output = "call " + returnType.value + " " + name.value + " ( ";
            if(parameters.Count > 0) 
            {
                output += parameters[0].Item1.value + " " + parameters[0].Item2.value;
                for(int i = 1; i < parameters.Count; i++)
                {
                    output += ", " + parameters[i].Item1.value + " " + parameters[i].Item2.value;
                }
            }

            output += " )\n";

            return output;
        }
        public override TypeReference GetILType()
        {
            return GetTypeSystem().TranslateLLVMTypeIntoILType(returnType.value);
        }

        public override void Codegen()
        {
            if(name.value == "llvm.dbg.declare")
            {
                return;
            }


            foreach(var param in parameters)
            {
                LoadOntoStack(param.Item1);
            }

            var ilproc = GetParentFunctionMethodDefinition().Body.GetILProcessor();
            ilproc.Emit(OpCodes.Call, GetMethodDefinitionByName(name.value));
        }
    }
}
