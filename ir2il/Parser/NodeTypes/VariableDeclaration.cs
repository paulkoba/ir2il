using ir2cil.Lexer;
using ir2cil.Parser.NodeTypes;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ir2il.Parser.NodeTypes
{
    internal class VariableDeclaration : BaseNode
    {
        Token name;
        BaseNode rhs;
        public VariableDeclaration(Token name, BaseNode rhs)
        {
            this.name = name;
            this.rhs = rhs;
        }

        public override string ToString()
        {
            return this.name.value + " = " + rhs.ToString() + "\n";
        }

        public override void Codegen()
        {
            var variableDefinition = new VariableDefinition(rhs.GetILType());

            parent.RegisterVariable(name.value, variableDefinition);

            rhs.Codegen();

            var ilproc = GetParentFunctionMethodDefinition().Body.GetILProcessor();
            ilproc.Emit(OpCodes.Stloc, variableDefinition);
        }

        public override void PopulateParents()
        {
            rhs.parent = this;
            rhs.PopulateParents();
        }
    }
}
