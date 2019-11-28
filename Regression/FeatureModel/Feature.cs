using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureClass
{
    public class Feature
    {
        public string Name { get; set; }
        public Feature(string _name)
        {
            Name = _name;
        }
        public string GetName()
        {
            return Name;
        }
    }
}
