using ir2cil.Lexer;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ir2cil.Parser.NodeTypes
{
    internal class Load : BaseNode
    {
        public static bool IsLoad(string opName)
        {
            return opName == "load";
        }

        public Load(Token type1, Token type2, Token where)
        {
            this.type1 = type1;
            this.type2 = type2;
            this.where = where;
        }

        public Token type1;
        public Token type2;
        public Token where;

        public override string ToString()
        {
            return "load " + type1.value + ", " + type2.value + ", " + where.value;
        }

        public override void Codegen()
        {
            var v = GetVariable(where);
            var ilproc = GetParentFunctionMethodDefinition().Body.GetILProcessor();
            ilproc.Emit(Mono.Cecil.Cil.OpCodes.Ldloc, v);
        }

        public override TypeReference GetILType()
        {
            return GetTypeSystem().TranslateLLVMTypeIntoILType(type1.value);
        }
    }
}
