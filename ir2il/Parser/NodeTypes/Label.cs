using ir2cil.Lexer;
using System;


namespace ir2cil.Parser.NodeTypes
{
    internal class Label : BaseNode
    {
        public Label(Token name)
        {
            this.name = name;
        }

        public Token name;

        public override string ToString()
        {
            return name.value + ":\n";
        }
        public override void Codegen()
        {
            var ilproc = GetParentFunctionMethodDefinition().Body.GetILProcessor();
            ilproc.Emit(Mono.Cecil.Cil.OpCodes.Nop);

            RegisterLabelMarker(name, ilproc.Body.Instructions[ilproc.Body.Instructions.Count - 1]);
        }
    }
}
