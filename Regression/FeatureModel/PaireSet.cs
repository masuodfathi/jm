using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureClass
{
    public class PaireSet
    {
        public List<Pair> PairsList = new List<Pair>();
        public PaireSet(List<string> _pairs)
        {
            for (int i = 0; i < _pairs.Count; i++)
            {
                Pair p = new Pair(_pairs[i]);
                PairsList.Add(p);
            }
        }
    }
}
