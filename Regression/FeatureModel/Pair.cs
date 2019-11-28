using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureClass
{
    public class Pair
    {
        public string Feature1;
        public string Feature2;
        public List<int> TestCases = new List<int>();
        public Pair (string _feature1,string _feature2,List<int> _testCases)
        {
            Feature1 = _feature1;
            Feature2 = _feature2;
            TestCases = _testCases;
        }
        public Pair(string _pair, List<int> _testCases)
        {
            string[] featureString = new string[2];
            
            string[] f = _pair.Split(',');
            for (int j = 0; j < f.Length; j++)
            {
                featureString[j] = f[j];
            }
            Feature1 = f[0];
            Feature2 = f[1];
            TestCases = _testCases;
        }
        public Pair(string _pair)
        {
            string[] featureString = new string[2];

            string[] f = _pair.Split(',');
            for (int j = 0; j < f.Length; j++)
            {
                featureString[j] = f[j];
            }
            Feature1 = f[0];
            Feature2 = f[1];
            
        }
    }
}
