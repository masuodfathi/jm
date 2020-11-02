using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regression.FeatureModel
{
    class SortFun
    {
        public List<string> Fun = new List<string>();
        public SortFun(List<string> _fun)
        {
            _fun.Sort();
            Fun = _fun;
        }
    }
}
