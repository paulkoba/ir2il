using ir2cil.Lexer;
using System;


namespace ir2cil.Parser.NodeTypes
{
    internal class Ret : BaseNode
    {
        public static bool IsRet(string opName)
        {
            return opName == "ret";
        }

        public Ret(Token type, Token value)
        {
            this.type = type;
            this.value = value;
        }

        public Token type;
        public Token value;

        public override string ToString()
        {
            return "ret " + type.value + " " + value.value + "\n";
        }
        public override void Codegen()
        {
            if(type.value != "void")
            {
                LoadOntoStack(value);
            }

            var ilproc = GetParentFunctionMethodDefinition().Body.GetILProcessor();
            ilproc.Emit(Mono.Cecil.Cil.OpCodes.Ret);
        }
    }
}
