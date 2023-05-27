using ir2cil.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ir2cil.Parser.NodeTypes
{
    // https://llvm.org/docs/LangRef.html#unary-operations
    enum UnaryOperationType
    {
        Fneg,

        None,
    }

    internal class UnaryOperation : BaseNode
    {
        public static bool IsUnaryOperation(string opName)
        {
            return GetUnaryOpFromString(opName) != UnaryOperationType.None;
        }
        public static UnaryOperationType GetUnaryOpFromString(string opName)
        {
            switch (opName)
            {
                case "fneg": return UnaryOperationType.Fneg;
            }

            return UnaryOperationType.None;
        }


        public UnaryOperation(UnaryOperationType type, Token operandTypes, Token rhs)
        {
            this.type = type;
            this.rhs = rhs;
            this.operandTypes = operandTypes;
        }

        public UnaryOperationType type;
        public Token rhs;
        public Token operandTypes;


        public override string ToString()
        {
            return type.ToString() + " " + type + " " + rhs.value;
        }
    }
}
