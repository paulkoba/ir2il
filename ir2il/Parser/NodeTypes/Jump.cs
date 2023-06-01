using ir2cil.Lexer;
using ir2il;
using Mono.Cecil.Cil;
using System;


namespace ir2cil.Parser.NodeTypes
{
    internal class Jump : BaseNode
    {
        public static bool IsJump(string opName)
        {
            return opName == "br";
        }

        public Jump(Token where)
        {
            this.where = where;
        }

        public Token where;
        public Instruction location;

        public override string ToString()
        {
            return "br label " + this.where.value + "\n";
        }

        public override void Codegen()
        {
            var ilproc = GetParentFunctionMethodDefinition().Body.GetILProcessor();
            location = ilproc.Body.Instructions[ilproc.Body.Instructions.Count - 1];
        }
        public override void CodegenPass2()
        {
            var ilproc = GetParentFunctionMethodDefinition().Body.GetILProcessor();
            ilproc.InsertAfter(location, ilproc.Create(OpCodes.Br, GetLabelMarkerByName(where)));
        }
    }
}
