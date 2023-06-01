using Cecilifier.Runtime;
using ir2il;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ir2cil.Parser.NodeTypes
{
    internal class Module : BaseNode
    {
        List<BaseNode> children = new();
        string name, version, filename;
        Types types = null;
        public Module() { }

        public void InsertNode(BaseNode node)
        {
            children.Add(node);
        }

        public void SetBuildParameters(string name, string version, string filename)
        {
            this.name = name;
            this.version = version;
            this.filename = filename;
        }

        public ModuleParameters mp = null;
        public AssemblyDefinition assembly = null;
        public TypeDefinition wrapperClass = null;

        public override void Codegen()
        {
            Console.WriteLine("Started codegen...");

            GenerateBoilerplate();
            
            foreach (BaseNode node in children) 
            {
                node.Codegen();
            }
        }

        public override void CodegenPass2()
        {
            Console.WriteLine("Second pass...");

            foreach (BaseNode node in children)
            {
                node.CodegenPass2();
            }

            assembly.Write(filename);
        }

        public override void PopulateParents()
        {
            foreach(BaseNode node in children)
            {
                if (node == null) continue;
                node.parent = this;
                node.PopulateParents();
            }
        }

        public override Types GetTypeSystem()
        {
            return types;
        }

        private void GenerateBoilerplate()
        {
            mp = new ModuleParameters { Architecture = TargetArchitecture.AMD64, Kind = ModuleKind.Console, ReflectionImporterProvider = new SystemPrivateCoreLibFixerReflectionProvider() };
            assembly = AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition(name, Version.Parse(version)), Path.GetFileName(filename), mp);
            types = new Types(assembly);
            var mainModule = assembly.MainModule;
            wrapperClass = new TypeDefinition("", name + "_wrapper", TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.NotPublic, mainModule.TypeSystem.Object);
            assembly.MainModule.Types.Add(wrapperClass);

            var ctor = new MethodDefinition(".ctor", Mono.Cecil.MethodAttributes.Public | Mono.Cecil.MethodAttributes.HideBySig | Mono.Cecil.MethodAttributes.SpecialName | Mono.Cecil.MethodAttributes.RTSpecialName, mainModule.TypeSystem.Void);

            var il = ctor.Body.GetILProcessor();

            il.Append(il.Create(OpCodes.Ldarg_0));

            // call the base constructor
            il.Append(il.Create(OpCodes.Call, mainModule.Import(typeof(object).GetConstructor(Array.Empty<Type>()))));

            il.Append(il.Create(OpCodes.Nop));
            il.Append(il.Create(OpCodes.Ret));

            wrapperClass.Methods.Add(ctor);
        }

        public override Module GetParentModule()
        {
            return this;
        }

        public override string ToString()
        {
            string result = "Module " + name + "\n";

            foreach(BaseNode node in children)
            {
                result += node.ToString();
            }

            return result;
        }

        private Dictionary<string, MethodDefinition> methods = new();

        public override void RegisterMethodDefinition(string name, MethodDefinition method)
        {
            methods[name] = method;
        }

        public override MethodDefinition GetMethodDefinitionByName(string name)
        {
            return methods[name];
        }
    }
}
