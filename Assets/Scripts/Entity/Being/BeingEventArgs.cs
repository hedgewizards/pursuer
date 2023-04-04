using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Being
{
    public class BeingEventArgs : EventArgs
    {
    }

    public class BeingActionArgs : BeingEventArgs
    {
        public string Name;

        public BeingActionArgs(string name)
        {
            Name = name;
        }
    }

    public class BeingStanceChangeArgs : BeingEventArgs
    {
        public string OldName;
        public string NewName;

        public BeingStanceChangeArgs(string oldName, string newName)
        {
            OldName = oldName;
            NewName = newName;
        }
    }
}
