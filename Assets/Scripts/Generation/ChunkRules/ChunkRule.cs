using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generation
{
    [System.Serializable]
    public class ChunkRule
    {
        public enum RuleType
        {
            AtLeastOne,
            AtMostOne
        }
        public RuleType Rule;
        public int[] RelatedBoundaries;

        public ChunkRule()
        {
            RelatedBoundaries = new int[0];
        }
    }
}
