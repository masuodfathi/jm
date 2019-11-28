using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regression.FeatureModel
{

    public class SPLOT_Reader
    {
        List<string> ConsPair = new List<string>();
        List<string> featurPair = new List<string>();
        public SPLOT_Reader(string path)
        {
            
            string FeatureTag = "";
            string constraints = "";
            XmlDocument xml = new XmlDocument();
            
            
            xml.Load(path);
            foreach (XmlNode node in xml.DocumentElement)//get all text in <feature_tree> tag
            {
                if (node.Name == "feature_tree")
                {
                    FeatureTag = node.InnerText;
                    featurPair = GetFeaturePair(FeatureTag);
                }
                else if (node.Name == "constraints")
                {
                    constraints = node.InnerText;
                    ConsPair = GetConstraintsPair(constraints);
                }
                

            }
            
        }
        private List<string> GetFeaturePair(string featureTag)
        {
            List<string> fPair = new List<string>();
            StringReader sr = new StringReader(featureTag);

            string line;
            List<string> Stack = new List<string>();
            while (true)// read untill reach its end
            {
                line = sr.ReadLine(); //Read FeatureTag line by line 
                if (line != null)
                {
                    if (line == "")
                    {
                        continue;
                    }
                    string d = GetDakheleP(line);// Get text inside Braces
                    if (Stack.Count < 1)// if we are reading for first time
                    {
                        Stack.Add(d);
                    }
                    else
                    {
                        while (true)
                        {
                            int c = Stack.Count - 1;
                            if (ISparent(Stack[c], d))
                            {
                                fPair.Add(Stack[c].Trim() + "," + d.Trim());
                                Stack.Add(d);
                                break;
                            }
                            else
                            {
                                Stack.RemoveAt(Stack.Count - 1);
                            }
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            return fPair;
        }
        private bool ISparent(string parent, string child)
        {
            bool isPar = false;
            List<string> separatedP = new List<string>();
            List<string> separatedC = new List<string>();
            separatedC = separator(child);
            if (parent == "R" && separatedC.Count == 1)
            {
                return true;
            }
            else
            {
                separatedP = separator(parent);
            }

            if (separatedP.Count == separatedC.Count - 1)
            {
                isPar = true;
                for (int i = 0; i < separatedP.Count; i++)
                {
                    if (separatedP[i] != separatedC[i])
                    {
                        isPar = false;
                    }
                }
            }
            return isPar;
        }
        private List<string> separator(string parent)
        {
            List<string> s = new List<string>();
            string[] a = parent.Split('_');
            foreach (var item in a)
            {
                int count = item.Count(x => char.IsDigit(x));
                if (count == item.Length && count > 0)
                {
                    s.Add(item);
                }
            }
            return s;
        }
        private List<string> GetConstraintsPair(string constraints)
        {
            StringReader SR = new StringReader(constraints);
            string line;
            List<string> features = new List<string>();
            List<string> Allpairs = new List<string>();
            while (true)
            {
                line = SR.ReadLine();
                if (line != null)
                {
                    if (line == "")
                    {
                        continue;
                    }
                    features = separatorConstraints(line);
                    if (features.Count > 0)
                    {
                        for (int i = 0; i < features.Count; i++)
                        {
                            for (int j = 0; j < features.Count; j++)
                            {
                                if (i == j)
                                {
                                    continue;
                                }
                                Allpairs.Add(features[i].Trim() + "," + features[j].Trim());
                            }
                        }
                    }
                }
                else
                {
                    break;
                }
            }
            return Allpairs;
        }
        private List<string> separatorConstraints(string line)
        {
            List<string> Features = new List<string>();
            string[] hole = line.Split(':');
            string[] features = hole[1].Split(new string[] { "OR" }, StringSplitOptions.RemoveEmptyEntries);
            string a = features[0].Replace("~", "");
            foreach (var feature in features)
            {
                string f = feature.Replace("~", "");
                Features.Add(f.Trim());
            }
            return Features;
        }
        private string GetDakheleP(string line)
        {
            string input = line;
            if (input != null && input != "")
            {
                string output = input.Split('(', ')')[1];
                return output;
            }
            else
            {
                return "";
            }

        }
        public List<string> GetPairs()
        {
            List<string> union = new List<string>();
            union = featurPair.Union(ConsPair).ToList();
            return union;
        }
    }
}
