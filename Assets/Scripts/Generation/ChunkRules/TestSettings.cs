using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generation
{
    public class TestSettings
    {
        public int MaxAtLeastRuleBreaks;

        public TestSettings(int maxAtLeastRuleBreaks)
        {
            MaxAtLeastRuleBreaks = maxAtLeastRuleBreaks;
        }

        public static TestSettings InitialSeed => new TestSettings(int.MaxValue);
        public static TestSettings DefaultValidation => new TestSettings(1);
    }
}
