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
    enum ComparisonType
    {
        Icmp,
        None,
    }

    enum CondType
    {
        Eq,
        Ne,
        Ugt,
        Uge,
        Ult,
        Ule,
        Sgt,
        Sge,
        Slt,
        Sle,

        None
    }

    internal class Comparison : BaseNode
    {
        public static bool IsComparison(string opName)
        {
            return GetComparisonTypeFromString(opName) != ComparisonType.None;
        }
        public static string GetStringFromComparisonType(ComparisonType opName)
        {
            switch (opName)
            {
                case ComparisonType.Icmp: return "icmp";
            }

            return "none";
        }
        public static string GetStringFromCondType(CondType opName)
        {
            switch (opName)
            {
                case CondType.Eq: return "eq";
                case CondType.Ne: return "ne";
                case CondType.Ugt: return "ugt";
                case CondType.Uge: return "uge";
                case CondType.Ult: return "ult";
                case CondType.Ule: return "ule";
                case CondType.Sgt: return "sgt";
                case CondType.Sge: return "sge";
                case CondType.Slt: return "slt";
                case CondType.Sle: return "sle";

                case CondType.None: return "none";
            }

            return "none";
        }

        public static CondType GetCondTypeFromString(string str)
        {
            switch (str)
            {
                 case "eq" : return CondType.Eq;
                 case "ne" : return CondType.Ne;
                 case "ugt" : return CondType.Ugt;
                 case "uge" : return CondType.Uge;
                 case "ult" : return CondType.Ult;
                 case "ule" : return CondType.Ule;
                 case "sgt" : return CondType.Sgt;
                 case "sge" : return CondType.Sge;
                 case "slt" : return CondType.Slt;
                 case "sle" : return CondType.Sle;
                 case "none": return CondType.None;
            }

            return CondType.None;
        }

        public static ComparisonType GetComparisonTypeFromString(string opName)
        {
            switch (opName)
            {
                case "icmp": return ComparisonType.Icmp;
            }

            return ComparisonType.None;
        }


        public Comparison(ComparisonType type, CondType condType, Token operandTypes, Token lhs, Token rhs)
        {
            if(condType == CondType.None)
            {
                throw new ArgumentException("Invalid condition type provided");
            }

            this.type = type;
            this.condType = condType;
            this.lhs = lhs;
            this.rhs = rhs;
            this.operandTypes = operandTypes;
        }

        public ComparisonType type;
        public CondType condType;
        public Token lhs;
        public Token rhs;
        public Token operandTypes;

        public override string ToString()
        {
            return GetStringFromComparisonType(type) + " " + GetStringFromCondType(condType) + " " + operandTypes.value + " " + lhs.value + " " + rhs.value;
        }

        public override void Codegen()
        {
            LoadOntoStack(lhs);
            LoadOntoStack(rhs);

            var ilproc = GetParentFunctionMethodDefinition().Body.GetILProcessor();

            switch (condType)
            {
                case CondType.Eq:
                    ilproc.Emit(OpCodes.Ceq);
                    break;
                case CondType.Ne:
                    ilproc.Emit(OpCodes.Ceq);
                    ilproc.Emit(OpCodes.Ldc_I4_0);
                    ilproc.Emit(OpCodes.Ceq);
                    break;
                case CondType.Sgt:
                    ilproc.Emit(OpCodes.Cgt);
                    break;
                case CondType.Slt:
                    ilproc.Emit(OpCodes.Clt);
                    break;
                case CondType.Sge:
                    ilproc.Emit(OpCodes.Clt);
                    ilproc.Emit(OpCodes.Ldc_I4_0);
                    ilproc.Emit(OpCodes.Ceq);
                    break;
                case CondType.Sle:
                    ilproc.Emit(OpCodes.Cgt);
                    ilproc.Emit(OpCodes.Ldc_I4_0);
                    ilproc.Emit(OpCodes.Ceq);
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
