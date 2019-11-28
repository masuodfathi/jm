using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureClass
{
    public class Compare
    {
        List<string> ChangedFeature = new List<string>();
        public List<string> TestName = new List<string>();
        public List<Pair> SamePairs = new List<Pair>();
        public List<Pair> initialSamePairs = new List<Pair>();
        public List<Pair> NewPairs = new List<Pair>();
        public List<Pair> RemovedPairs = new List<Pair>();
        public List<Pair> ChangedPairs = new List<Pair>();

        public Compare(featureModel featureModel1, PaireSet _newPairs,List<string> _changedFeatures)
        {
            featureModel f1 = featureModel1;
            TestName = featureModel1.TestName;
            PaireSet nPairs = _newPairs;
            ChangedFeature = _changedFeatures;
            GetSamePairs(f1, nPairs);
            GetRemovePairs(nPairs, f1, RemovedPairs);
            GetChangedPairs(_changedFeatures);
            RemoveChFeatureFromSame();
            GetNewPairs(f1,nPairs,NewPairs);
            
            
        }
        private void GetRemovePairs(PaireSet nPairs, featureModel f1, List<Pair> removedPairs)
        {
            for (int i = 0; i < f1.Pairs.Count; i++)
            {
                bool check = false;
                for (int j = 0; j < nPairs.PairsList.Count; j++)
                {
                    if (f1.Pairs[i].Feature1 == nPairs.PairsList[j].Feature1 && f1.Pairs[i].Feature2 == nPairs.PairsList[j].Feature2)
                    {
                        check = true;
                        break;
                    }
                    if (f1.Pairs[i].Feature2 == nPairs.PairsList[j].Feature1 && f1.Pairs[i].Feature1 == nPairs.PairsList[j].Feature2)
                    {
                        check = true;
                        break;
                    }
                }
                if (!check)
                {
                    removedPairs.Add(f1.Pairs[i]);
                }
            }
        }
        
        private void GetSamePairs(featureModel f1, PaireSet f2)
        {
            for (int i = 0; i < f1.Pairs.Count; i++)
            {
                for (int j = 0; j < f2.PairsList.Count; j++)
                {
                    if (f1.Pairs[i].Feature1 == f2.PairsList[j].Feature1 && f1.Pairs[i].Feature2 == f2.PairsList[j].Feature2)
                    {
                        SamePairs.Add(f1.Pairs[i]);
                        break;
                    }
                    if (f1.Pairs[i].Feature2 == f2.PairsList[j].Feature1 && f1.Pairs[i].Feature1 == f2.PairsList[j].Feature2)
                    {
                        SamePairs.Add(f1.Pairs[i]);
                        break;
                    }
                }
            }
            initialSamePairs = SamePairs.ToList();
        }
        private void RemoveChFeatureFromSame()
        {
            for (int i = SamePairs.Count-1; i >= 0; i--)
            {
                for (int j = 0; j < ChangedFeature.Count; j++)
                {
                    if (SamePairs[i].Feature1 == ChangedFeature[j] || SamePairs[i].Feature2 == ChangedFeature[j])
                    {
                        SamePairs.RemoveAt(i);
                        break;
                    }
                }
                
            }
        }
        private void GetNewPairs(featureModel f1, PaireSet f2,List<Pair> _list)
        {
            bool c=true;
            for (int i = 0; i < f2.PairsList.Count; i++)
            {
                for (int j = 0; j < f1.Pairs.Count; j++)
                {
                    c = true;
                    if (f1.Pairs[j].Feature1 == f2.PairsList[i].Feature1 && f1.Pairs[j].Feature2 == f2.PairsList[i].Feature2)
                    {
                        c = false;
                        break;
                    }
                    if (f1.Pairs[j].Feature2 == f2.PairsList[i].Feature1 && f1.Pairs[j].Feature1 == f2.PairsList[i].Feature2)
                    {
                        c = false;
                        break;
                    }
                    
                }
                if (c)
                {
                    _list.Add(f2.PairsList[i]);
                }
                
            }
        }
        private void GetChangedPairs(List<string> _changedFeatures)
        {
            for (int i = 0; i < initialSamePairs.Count; i++)
            {
                for (int j = 0; j < _changedFeatures.Count; j++)
                {
                    if (initialSamePairs[i].Feature1==_changedFeatures[j] || initialSamePairs[i].Feature2 == _changedFeatures[j])
                    {
                        ChangedPairs.Add(initialSamePairs[i]);
                        break;
                    }
                }
            }
        }
    }
}
