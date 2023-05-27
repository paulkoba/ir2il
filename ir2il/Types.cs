using ir2cil.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;

namespace ir2il
{
    internal class Types
    {
        public Types(AssemblyDefinition assembly)
        {
           this.assembly = assembly;
        }

        public AssemblyDefinition assembly;

        public TypeReference TranslateLLVMTypeIntoILType(string type)
        {
            switch (type) 
            {
            case "i1":
                return assembly.MainModule.TypeSystem.Boolean;
            case "i8":
                return assembly.MainModule.TypeSystem.Byte;
            case "i16":
                return assembly.MainModule.TypeSystem.Int16;
            case "i32":
                return assembly.MainModule.TypeSystem.Int32;
            case "i64":
                return assembly.MainModule.TypeSystem.Int64;
            default:
                throw new ArgumentException("Couldn't convert type " + type);
            }
        }
    }
}
