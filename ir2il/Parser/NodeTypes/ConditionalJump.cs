using ir2cil.Lexer;
using ir2il;
using Mono.Cecil.Cil;
using System;


namespace ir2cil.Parser.NodeTypes
{
    internal class ConditionalJump : BaseNode
    {
        public static bool IsJump(string opName)
        {
            return opName == "br";
        }

        public ConditionalJump(Token type, Token what, Token where1, Token where2)
        {
            this.type = type;
            this.what = what;
            this.where1 = where1;
            this.where2 = where2;
        }

        public Token type;
        public Token what;
        public Token where2;
        public Token where1;

        public Instruction location;

        public override string ToString()
        {
            return "br " + this.type.value + " " + this.what.value + " label " + where1.value + " label " + where2.value + "\n";
        }

        public override void Codegen()
        {
            LoadOntoStack(what);
            var ilproc = GetParentFunctionMethodDefinition().Body.GetILProcessor();
            location = ilproc.Body.Instructions[ilproc.Body.Instructions.Count - 1];
        }
        public override void CodegenPass2()
        {
            var ilproc = GetParentFunctionMethodDefinition().Body.GetILProcessor();
            // Note that we should reverse the insertion order as we use insertAfter instead of emit.
            ilproc.InsertAfter(location, ilproc.Create(OpCodes.Br, GetLabelMarkerByName(where2)));
            ilproc.InsertAfter(location, ilproc.Create(OpCodes.Brtrue, GetLabelMarkerByName(where1)));
        }
    }
}
