using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public virtual void LoadVariableOntoStackByName(string name)
        {
            if (parent == null)
            {
                throw new NotImplementedException();
            }
            else
            {
                parent.LoadVariableOntoStackByName(name);
            }
        }

        public virtual void PopulateParents() { }

        public virtual Module GetParentModule() 
        { 
            return parent.GetParentModule(); 
        }
    }
}
