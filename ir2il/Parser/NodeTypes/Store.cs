using ir2cil.Lexer;
using Mono.Cecil.Cil;
using System;


namespace ir2cil.Parser.NodeTypes
{
    internal class Store : BaseNode
    {
        public static bool IsStore(string opName)
        {
            return opName == "store";
        }

        public Store(Token type, Token value, Token where)
        {
            this.type = type;
            this.value = value;
            this.where = where;
        }

        public Token type;
        public Token value;
        public Token where;

        public override string ToString()
        {
            return "store " + type.value + " " + value.value + ", " + where.value + "\n";
        }

        public override void Codegen()
        {
            LoadOntoStack(value);

            var v = GetVariable(where);
            var ilproc = GetParentFunctionMethodDefinition().Body.GetILProcessor();

            ilproc.Emit(OpCodes.Stloc, v);
        }
    }
}
