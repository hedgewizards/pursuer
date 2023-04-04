using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generation
{
    [Serializable]
    public class GenerationStep
    {
        public int Attempts = 1;
        public bool PreferStartFromOrigin = false;
        public int AllowedSteps;
        public int MinimumLoopSize;
    }
}
