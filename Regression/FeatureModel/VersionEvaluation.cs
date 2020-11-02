using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeatureClass;

namespace Regression.FeatureModel
{
    class VersionEvaluation
    {
        List<Pair> OldPairs = new List<Pair>();
        List<Pair> NewPairs = new List<Pair>();
        int NumberOfnewPair;
        double NumberselectedTest;
        int[] FaultVariable;
        List<Pair> ChangedPairs;
        double newCover;
        double newCost;
        double FdeNew;
        double NewReusability;
        public double M5;
        public double M6;
        public double M7;
        public double M8;
        public VersionEvaluation()
        {

        }
        public VersionEvaluation(string path)
        {

        }
        public VersionEvaluation(List<Pair> _oldPairs , List<Pair> _newPairs , int newpairCount , int _numberselectedTest , int[] _faultVariable ,List<Pair> _changedPairs , double _fdeNew,double _newReusability, double _newCoverage)
        {
            newCover = _newCoverage;
            //newCost = _newCost;
            OldPairs = _oldPairs;
            NewPairs = _newPairs;
            NumberOfnewPair = newpairCount;
            NumberselectedTest = _numberselectedTest;
            FaultVariable = _faultVariable;
            ChangedPairs = _changedPairs;
            FdeNew = _fdeNew;
            NewReusability = _newReusability;
            M5 = GetKeepedCoverage();
            M6 = GetCostReduced();
            M7 = GetKeepedFDE();
            M8 = GetKeepedReusability();
        }

        private double GetKeepedReusability()
        {
            List<int> oldtestList = new List<int>();
            for (int i = 0; i < OldPairs[0].TestCases.Count; i++)
            {
                oldtestList.Add(1);
            }
            computingRandom cm = new computingRandom();
            double oldreusability = cm.GetReusability(OldPairs,oldtestList);
            double R = NewReusability / oldreusability;
            return R;
        }

        private double GetKeepedFDE()
        {
            double oldFde = GetFDE(FaultVariable);
            double m = (FdeNew) / oldFde;
            return m;
        }

        private double GetCostReduced()
        {
            double NTold = OldPairs[0].TestCases.Count;
            double rc = (NTold - NumberselectedTest) / NTold;
            rc = Math.Round(rc, 2);
            return rc;
        }

        private double GetKeepedCoverage()
        {
            double kc = 0;

            //double newCover = GetCoverage(NewPairs, NumberOfnewPair);
            double oldCover = GetCoverage(OldPairs, OldPairs.Count);
            kc = (newCover) / oldCover;
            return kc;
        }

        public double GetCoverage(List<Pair> pairSet , int newPairSize = 0)
        {
            bool[] coverlist = new bool[pairSet.Count];
            double coverage = 0;
            for (int i = 0; i < pairSet.Count; i++)
            {
                for (int j = 0; j < pairSet[0].TestCases.Count; j++)
                {
                    if (pairSet[i].TestCases[j] == 1)
                    {
                        coverlist[i] = true;
                        break;
                    }
                    
                }
            }
            for (int i = 0; i < coverlist.Length; i++)
            {
                if (coverlist[i])
                {
                    coverage++;
                }
            }
            coverage = coverage / newPairSize;
            return coverage;
        }

        private double GetFDE(int[] faults)
        {
            List<Pair> pairs = new List<Pair>();
            List<Pair> PairsNeedCover = ChangedPairs;
            int testCount = ChangedPairs[0].TestCases.Count;
            for (int i = 0; i < faults.Count(); i++)
            {
                if (faults[i] == 1)
                {
                    pairs.Add(PairsNeedCover[i]);
                }
            }

            int[] faultsCovered = new int[pairs.Count];//that Faults which are covered.

            for (int i = 0; i < testCount ; i++)
            {
                for (int j = 0; j < pairs.Count; j++)
                {
                    if (pairs[j].TestCases[i] == 1)
                    {
                        faultsCovered[j] = 1;
                    }
                }
            }
                 
            

            int numberOfCoverage = countOne(faultsCovered);

            int numberAllfaults = countOne(FaultVariable);

            double fde = 0;
            fde = (double)numberOfCoverage / (double)numberAllfaults;
            return fde;
        }
        private int countOne(int[] list)
        {
            int count = 0;
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i] == 1)
                {
                    count++;
                }
            }
            return count;
        }
    }
}
