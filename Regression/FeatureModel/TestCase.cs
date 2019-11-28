using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regression.FeatureModel
{
    public class TestCase
    {
        string Name;
        List<string> Sequence;
        public TestCase(string _name,List<string> _sequence)
        {
            Name = _name;
            Sequence = _sequence;
        }
    }
}
