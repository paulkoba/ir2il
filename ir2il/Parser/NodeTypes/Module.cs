﻿using Cecilifier.Runtime;
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

        private void GenerateBoilerplate()
        {
            mp = new ModuleParameters { Architecture = TargetArchitecture.AMD64, Kind = ModuleKind.Console, ReflectionImporterProvider = new SystemPrivateCoreLibFixerReflectionProvider() };
            assembly = AssemblyDefinition.CreateAssembly(new AssemblyNameDefinition(name, Version.Parse(version)), Path.GetFileName(filename), mp);

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

            // define the 'Main' method and add it to 'Program'
            var mainMethod = new MethodDefinition("Main",
                Mono.Cecil.MethodAttributes.Public | Mono.Cecil.MethodAttributes.Static, mainModule.TypeSystem.Void);

            wrapperClass.Methods.Add(mainMethod);

            // add the 'args' parameter
            var argsParameter = new ParameterDefinition("args",
                Mono.Cecil.ParameterAttributes.None, mainModule.Import(typeof(string[])));

            mainMethod.Parameters.Add(argsParameter);

            // create the method body
            il = mainMethod.Body.GetILProcessor();

            il.Append(il.Create(OpCodes.Nop));
            il.Append(il.Create(OpCodes.Ldstr, "Hello"));

            var writeLineMethod = il.Create(OpCodes.Call, mainModule.Import(typeof(Console).GetMethod("WriteLine", new[] { typeof(string) })));

            // call the method
            il.Append(writeLineMethod);

            il.Append(il.Create(OpCodes.Nop));
            il.Append(il.Create(OpCodes.Ret));

            // set the entry point and save the module
            assembly.EntryPoint = mainMethod;
        }

        public override Module GetParentModule()
        {
            return this;
        }
    }
}