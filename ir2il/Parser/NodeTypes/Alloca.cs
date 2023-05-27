using ir2cil.Lexer;
using ir2il;
using Mono.Cecil.Cil;
using System;


namespace ir2cil.Parser.NodeTypes
{
    internal class Alloca : BaseNode
    {
        public static bool IsAlloca(string opName)
        {
            return opName == "alloca";
        }

        public Alloca(Token identifier, Token type)
        {
            this.identifier = identifier;
            this.type = type;
        }

        public Token type;
        public Token identifier;

        public override string ToString()
        {
            return identifier.value + " = alloca " + type.value + "\n";
        }

        public override void Codegen()
        {
            var ilType = GetTypeSystem().TranslateLLVMTypeIntoILType(type.value);
            var variableDefinition = new VariableDefinition(ilType);

            parent.RegisterVariable(identifier.value, variableDefinition);
        }
    }
}
