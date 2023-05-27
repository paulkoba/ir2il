using ir2cil.Lexer;
using ir2il;
using Mono.Cecil.Cil;
using System;
using System.Reflection.Metadata;

namespace ir2cil.Parser.NodeTypes
{
    internal class BaseNode
    {
        public BaseNode parent = null;

        public BaseNode() { }

        public virtual void Codegen()
        { 
            throw new NotImplementedException();
        }

        public virtual void PopulateParents() { }

        public virtual Module GetParentModule()
        {
            return parent.GetParentModule();
        }

        public override string ToString()
        {
            return "BaseNode";
        }

        public virtual Mono.Cecil.MethodDefinition GetParentFunctionMethodDefinition()
        {
            if (parent == null)
            {
                throw new NotImplementedException();
            }
            else
            {
                return parent.GetParentFunctionMethodDefinition();
            }
        }

        public virtual void RegisterVariable(string name, VariableDefinition variable)
        {
            if (parent == null)
            {
                throw new NotImplementedException();
            }
            else
            {
                parent.RegisterVariable(name, variable);
            }
        }

        public virtual Types GetTypeSystem()
        {
            if (parent == null)
            {
                throw new NotImplementedException();
            }
            else
            {
                return parent.GetTypeSystem();
            }
        }

        public virtual void LoadOntoStack(Token value)
        {
            if (parent == null)
            {
                throw new NotImplementedException();
            }
            else
            {
                parent.LoadOntoStack(value);
            }
        }

        public virtual VariableDefinition GetVariable(Token value)
        {
            if (parent == null)
            {
                throw new NotImplementedException();
            }
            else
            {
                return parent.GetVariable(value);
            }
        }

        public virtual Mono.Cecil.TypeReference GetILType()
        {
            throw new NotSupportedException("GetILType() is not supported for this type of node: ");
        }

        public virtual Mono.Cecil.MethodDefinition GetMethodDefinitionByName(string name)
        {
            if (parent == null)
            {
                throw new NotImplementedException();
            }
            else
            {
                return parent.GetMethodDefinitionByName(name);
            }
        }

        public virtual void RegisterMethodDefinition(string name, Mono.Cecil.MethodDefinition method)
        {
            if (parent == null)
            {
                throw new NotImplementedException();
            }
            else
            {
                parent.RegisterMethodDefinition(name, method);
            }
        }
    }
}
