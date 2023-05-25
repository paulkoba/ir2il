using ir2cil.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ir2cil.Parser.NodeTypes
{
    // https://llvm.org/docs/LangRef.html#binary-operations
    enum BinaryOperationType
    {
        Add,
        Sub,
        Mul,
        UDiv,
        SDiv,
        URem,
        SRem,
        FRem
    }

    internal class BinaryOperation
    {
        BinaryOperation(BinaryOperationType type, Token lhs, Token rhs)
        {
            this.type = type;
        }

        public BinaryOperationType type;
        public Token lhs;
        public Token rhs;
    }
}
