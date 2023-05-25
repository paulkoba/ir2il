using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ir2cil.Parser.NodeTypes
{
    internal class Function : BaseNode
    {
        List<BaseNode> stmts;
        List<string> arguments;
        string returnType;

        public Function(string returnType, List<string> arguments)
        {
            this.returnType = returnType;
            this.arguments = arguments;
        }

        public void InsertNode(BaseNode node)
        {
            stmts.Add(node);
        }

        public override void Codegen()
        {
            Console.WriteLine("Generating function");
            Module parentModule = GetParentModule();


            var mainMethod = new MethodDefinition("NotMain",
                Mono.Cecil.MethodAttributes.Public | Mono.Cecil.MethodAttributes.Static, parentModule.assembly.MainModule.TypeSystem.Void);

            parentModule.wrapperClass.Methods.Add(mainMethod);

            // create the method body
            var il = mainMethod.Body.GetILProcessor();

            il.Append(il.Create(OpCodes.Ret));
        }
    }
}
