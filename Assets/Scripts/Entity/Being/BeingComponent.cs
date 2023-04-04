using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Being
{
    /// <summary>
    /// Parts of a being that need knowledge of itself;
    /// </summary>
    public abstract class BeingComponent
    {
        protected BeingActor self;

        /// <summary>
        /// Call this when a component is added to a being
        /// </summary>
        /// <param name="self"></param>
        public virtual void Initialize(BeingActor self)
        {
            this.self = self;
        }

        /// <summary>
        /// Call this when a component is removed from a being
        /// </summary>
        public virtual void Detatch()
        {

        }
    }
}
