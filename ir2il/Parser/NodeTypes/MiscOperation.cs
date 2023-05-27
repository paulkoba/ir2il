using ir2cil.Lexer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ir2cil.Parser.NodeTypes
{
    enum MiscOperationType
    {
        Store,
        Ret,

        None,
    }

    internal class MiscOperation : BaseNode
    {
        public static bool IsMiscOperation(string opName)
        {
            return GetMiscOperationFromString(opName) != MiscOperationType.None;
        }
        public static MiscOperationType GetMiscOperationFromString(string opName)
        {
            switch (opName)
            {
                case "store": return MiscOperationType.Store;
                case "ret": return MiscOperationType.Ret;
            }

            return MiscOperationType.None;
        }


        public MiscOperation(MiscOperationType type, Token operandTypes, Token lhs, Token rhs)
        {
            if(type == MiscOperationType.None)
            {
                throw new NotImplementedException("Operation type not supported");
            }
            this.type = type;
            this.lhs = lhs;
            this.rhs = rhs;
            this.operandTypes = operandTypes;
        }

        public MiscOperationType type;
        public Token lhs;
        public Token rhs;
        public Token operandTypes;

        public override string ToString()
        {
            return "    " + lhs + " " + type.ToString() + " " + rhs + "\n";
        }
    }
}
