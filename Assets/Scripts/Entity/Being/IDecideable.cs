using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Being
{
    public interface IDecideable
    {
        public abstract float Weight { get; }

        public abstract void Decide();
    }
}
