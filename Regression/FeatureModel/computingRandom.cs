using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FeatureClass;

namespace Regression.FeatureModel
{
    class computingRandom
    {
        //Tedade testCase ha dar sheet1.
        int TestNumber;

        //Baraye makhraje cost estefade mikonim.
        int RetastAbleNumber;

        //Arraye be andaze chenged pairs ha ke niaz be test darand. Harja 1 bashe mani vojode fault ast.
        int[] Faults;

        //Arraye ee ke neshan midahad ba estefade az variable random kodam pais ha cover mishavand(tamame pair haye sheet1).
        string[] Coveraged;

        //Meqdare nahaee baraye UnCoverage
        public string UnCoverage;

        //Meqdare nahaee baraye cost
        public string Cost;

        //Meqdare nahaee FDE
        public double FDE;

        //Variable ee ke be sorate randome baraye arzyabi ejad shode(koromozom).
        public int[] Variable;

        //pair haa ee ke dar ravashe ma shenasaee shodan ke bayad test shavand.
        List<Pair> PairsNeedCover;

        int PairsCount;

        public computingRandom()
        {

        }

        public computingRandom(featureModel fmo,Compare co,FilterTestCase ft, int[] faults)
        {
            PairsCount = fmo.Pairs.Count;
            TestNumber = fmo.testcases.Count();
            RetastAbleNumber = ft.RetestableTestCases.Count;
            PairsNeedCover = co.ChangedPairs;
            Faults = faults;
            List<double> cc = new List<double>();
            cc = GetCoverageCost(fmo.Matrix);
            UnCoverage = cc[0].ToString();
            Cost = cc[1].ToString();
            //FaultsDetectionEfficiency
            double fde = GetFDE(Variable);
            FDE = Math.Round(fde, 2);
        }

        private double GetFDE(int[] variable)
        {
            List<Pair> pairs = new List<Pair>();
            
            for (int i = 0; i < Faults.Count(); i++)
            {
                if (Faults[i] == 1)
                {
                    pairs.Add(PairsNeedCover[i]);
                }
            }
            int[] faultsCovered = new int[pairs.Count];//Faults which are covered.
            for (int i = 0; i < variable.Length; i++)
            {
                if (variable[i] == 1)
                {
                    for (int j = 0; j < pairs.Count; j++)
                    {
                        if (pairs[j].TestCases[i] == 1)
                        {
                            faultsCovered[j] = 1;
                        }
                    }
                }
            }
            int numberOfCoverage = countOne(faultsCovered);
            
            int numberAllfaults = countOne(Faults);
            
            double fde = 0;
            fde = (double)numberOfCoverage / (double)numberAllfaults ;
            return fde;
        }

        public double GetFDE(int[] variable,int[] faults,List<Pair> PairsNeedCover)
        {
            List<Pair> pairs = new List<Pair>();

            for (int i = 0; i < faults.Count(); i++)
            {
                if (faults[i] == 1)
                {
                    pairs.Add(PairsNeedCover[i]);
                }
            }
            int[] faultsCovered = new int[pairs.Count];//that Faults which are covered.
            for (int i = 0; i < variable.Length; i++)
            {
                if (variable[i] == 1)
                {
                    for (int j = 0; j < pairs.Count; j++)
                    {
                        if (pairs[j].TestCases[i] == 1)
                        {
                            faultsCovered[j] = 1;
                        }
                    }
                }
            }
            int numberOfCoverage = countOne(faultsCovered);

            int numberAllfaults = countOne(faults);

            double fde = 0;
            fde = (double)numberOfCoverage / (double)numberAllfaults;
            return fde;
        }

        public List<double> GetCoverageCost(string[,] matrix)
        {
            double cov = 0;
            double cost = 0;
            int[] variable = new int[TestNumber];
            string[] coverage = new string[PairsNeedCover.Count];
            for (int i = 0; i < TestNumber; i++)
            {
                int xr = JMetalCSharp.Utils.JMetalRandom.Next(1, 2);
                //int xr = Random(1);
                if (xr == 1)
                {
                    variable[i] = xr;
                }
            }
            Variable = variable;
            string[,] m = setMatrix(matrix);
            int row = m.GetLength(0);
            int col = m.GetLength(1);
            for (int i = 0; i < TestNumber; i++)
            {
                if (variable[i] == 1)
                {
                    cost++;
                    for (int j = 0; j < PairsNeedCover.Count; j++)
                    {
                        if (PairsNeedCover[j].TestCases[i] == 1)
                        {
                            coverage[j] = "1";
                            break;
                        } 
                    }
                }
            }
            Coveraged = coverage;
            for (int i = 0; i < coverage.Count(); i++)
            {
                if (coverage[i] == "1")
                {
                    cov++;
                }
            }
            cov = cov / PairsNeedCover.Count;
            cost = cost / RetastAbleNumber;
            cov = 1 - cov;
            cov = Math.Ceiling(cov * 100) / 100;
            cost = Math.Ceiling(cost * 100) / 100;
            
            List<double> Result = new List<double>();
            Result.Add(cov);
            Result.Add(cost);
            return Result;
        }

        internal double GetReusability(List<Pair> pairs, List<int> testCases,int tedadPairs = 1)
        {
            List<int> cols = GetCols(testCases);
            double reusability = 0;
            int I;
            if (tedadPairs>1)
            {
                I = tedadPairs;
            }
            else
            {
                I = pairs.Count;
            }
            
            int T = cols.Count;
            double[] Ii = new double[T];

            int row = pairs.Count;
            int col = cols.Count;
            string[,] matrix = new string[row , col];

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    matrix[i, j] = pairs[i].TestCases[cols[j]].ToString();
                }
            }
            for (int x = 0; x < col; x++)
            {
                int numberofOnes = 0;
                for (int w = 0; w < row; w++)
                {
                    if (matrix[w,x] == "1")
                    {
                        numberofOnes++;
                    }
                }
                Ii[x] = numberofOnes;
            }
            double sum = 0;
            for (int i = 0; i < Ii.Length; i++)
            {
                sum += Ii[i];
            }
            reusability = sum / (T * I);
            reusability = Math.Round(reusability, 9);
            return reusability;
        }

        private List<int> GetCols(List<int> testCases)
        {
            List<int> cols = new List<int>();
            for (int i = 0; i < testCases.Count; i++)
            {
                if (testCases[i] == 1)
                {
                    cols.Add(i);
                }
            }
            return cols;
        }

        //internal double GetReusability()
        //{
        //    double reusability = 0;

        //    int I = PairsCount;
        //    int T = TestNumber;
        //    double[] Ii = new double[T];

        //    int row = I;
        //    int col = T;
        //    string[,] matrix = new string[row, col];

        //    for (int i = 0; i < row; i++)
        //    {
        //        for (int j = 0; j < col; j++)
        //        {
        //            matrix[i, j] = initialSamePairs[i].TestCases[reservedTestCases[j]].ToString();
        //        }
        //    }
        //    for (int x = 0; x < col; x++)
        //    {
        //        int numberofOnes = 0;
        //        for (int w = 0; w < row; w++)
        //        {
        //            if (matrix[w, x] == "1")
        //            {
        //                numberofOnes++;
        //            }
        //        }
        //        Ii[x] = numberofOnes;
        //    }
        //    double sum = 0;
        //    for (int i = 0; i < Ii.Length; i++)
        //    {
        //        sum += Ii[i];
        //    }
        //    reusability = sum / (T * I);
        //    reusability = Math.Round(reusability, 2);
        //    return reusability;
        //}

        private string[,] setMatrix(string[,] matrix)
        {
            int row = matrix.GetLength(0)-1;
            int col = matrix.GetLength(1)-1;
            string[,] mtx = new string[row, col];
            for (int i = 1; i < matrix.GetLength(0); i++)
            {
                for (int j = 1; j < matrix.GetLength(1); j++)
                {
                    mtx[i - 1, j - 1] = matrix[i,j];
                }
            }
            return mtx;
        }

        public int Random()
        {
            int rnd;
            rnd = JMetalCSharp.Utils.JMetalRandom.Next(0, 1);
            return rnd;
        }

        public int Random(int max)
        {
            int rnd;
            rnd = JMetalCSharp.Utils.JMetalRandom.Next(0, max);
            return rnd;
        }
        private int countOne(List<string> list)
        {
            int count = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == "1")
                {
                    count++;
                }
            }
            return count;
        }
        private int countOne(List<int> list)
        {
            int count = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == 1)
                {
                    count++;
                }
            }
            return count;
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
