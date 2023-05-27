using ir2cil.Lexer;
using Mono.Cecil;
using Mono.Cecil.Cil;
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
        FRem,

        None,
    }

    internal class BinaryOperation : BaseNode
    {
        public static bool IsBinaryOperation(string opName)
        {
            return GetBinaryOpFromString(opName) != BinaryOperationType.None;
        }
        public static string GetStringFromBinaryOp(BinaryOperationType opName)
        {
            switch (opName)
            {
                case BinaryOperationType.Add: return "add";
                case BinaryOperationType.Sub: return "sub";
                case BinaryOperationType.Mul: return "mul";
                case BinaryOperationType.UDiv: return "udiv";
                case BinaryOperationType.SDiv: return "sdiv";
                case BinaryOperationType.URem: return "urem";
                case BinaryOperationType.SRem: return "srem";
                case BinaryOperationType.FRem: return "frem";
            }

            return "none";
        }

        public static BinaryOperationType GetBinaryOpFromString(string opName)
        {
            switch (opName)
            {
                case "add": return BinaryOperationType.Add;
                case "sub": return BinaryOperationType.Sub;
                case "mul": return BinaryOperationType.Mul;
                case "udiv": return BinaryOperationType.UDiv;
                case "sdiv": return BinaryOperationType.SDiv;
                case "urem": return BinaryOperationType.URem;
                case "srem": return BinaryOperationType.SRem;
                case "frem": return BinaryOperationType.FRem;
            }

            return BinaryOperationType.None;
        }


        public BinaryOperation(BinaryOperationType type, Token operandTypes, Token lhs, Token rhs)
        {
            this.type = type;
            this.lhs = lhs;
            this.rhs = rhs;
            this.operandTypes = operandTypes;
        }

        public BinaryOperationType type;
        public Token lhs;
        public Token rhs;
        public Token operandTypes;

        public override string ToString()
        {
            return GetStringFromBinaryOp(type) + " " + lhs.value + " " + rhs.value;
        }

        public override void Codegen()
        {
            LoadOntoStack(lhs);
            LoadOntoStack(rhs);

            var ilproc = GetParentFunctionMethodDefinition().Body.GetILProcessor();

            switch (type)
            {
                case BinaryOperationType.Add:
                    ilproc.Emit(OpCodes.Add);
                    break;
                case BinaryOperationType.Sub:
                    ilproc.Emit(OpCodes.Sub);
                    break;
                case BinaryOperationType.Mul:
                    ilproc.Emit(OpCodes.Mul);
                    break;
                case BinaryOperationType.SDiv:
                    ilproc.Emit(OpCodes.Div);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        public override TypeReference GetILType()
        {
            return GetTypeSystem().TranslateLLVMTypeIntoILType(operandTypes.value);
        }
    }
}
