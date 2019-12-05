using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeatureClass
{
    public class FilterTestCase
    {
        public int[,] Matrix;//matrix ke be nsgaii ersal mishavad
        public List<string> RetestableTestCases = new List<string>();
        public List<string> ReUsableTestCases = new List<string>();
        public List<string> ObsoleteTestCases = new List<string>();
        public List<int> reservedTestCases = new List<int>();
        public List<int> removedTest = new List<int>();
        public List<int> ColOfRetestableTestcases;
        public string[,] matrixNewversion;
        List<string> TestName = new List<string>();
        int[,] InitMatrix;
        Compare CompairResualt;


        public FilterTestCase(Compare CompareResult)
        {
            CompairResualt = CompareResult;
            TestName = CompareResult.TestName;
            List<Pair> all = CompairResualt.initialSamePairs;
            GetRetestable(CompareResult.ChangedPairs, TestName);
            SetRemovedTest(CompairResualt.ChangedPairs[0].TestCases.Count);
            GetReusable(CompareResult.SamePairs, TestName);
            GetObsolete(CompareResult.RemovedPairs, TestName);
            GetNewVersionMatrix();
            
        }

        private void SetRemovedTest(int numberOfAllTest)
        {
            List<int> r = new List<int>();
            
            for (int i = 0; i < numberOfAllTest; i++)
            {
                r.Add(i);
            }
            removedTest = r.Except(reservedTestCases).ToList();
        }

        public void GetNewVersionMatrix()
        {
            int row = CompairResualt.initialSamePairs.Count();
            int col = CompairResualt.ChangedPairs[0].TestCases.Count();
            //int col = ReUsableTestCases.Count() + RetestableTestCases.Count();
            string[,] Tmatrix = new string[row,col];
            
            for (int i = 0; i < col; i++)
            {
                for (int j = 0; j < row; j++)
                {
                    Tmatrix[j, i] = CompairResualt.initialSamePairs[j].TestCases[i].ToString();
                }
            }
            
            
            List<int> testName = new List<int>();
            
            for (int i = 0; i < col; i++)
            {
                bool moaser = false;
                for (int j = 0; j < row; j++)
                {
                   
                    
                    
                    if (Tmatrix[j,i] == "1")
                    {
                        moaser = true;
                        break;
                    }
                }
                if (moaser)
                {
                    testName.Add(i);
                }
            }

            matrixNewversion = new string[row+1,testName.Count+1];

            for (int i = 1; i < testName.Count+1; i++)
            {
                for (int j = 1; j < row+1; j++)
                {
                    matrixNewversion[j,i] = Tmatrix[j-1,testName[i-1]];
                }
            }

            for (int i = 1; i < testName.Count+1; i++)
            {
                matrixNewversion[0, i] = CompairResualt.TestName[testName[i-1]];
            }
            for (int i = 1; i < row+1; i++)
            {
                matrixNewversion[i, 0] = getStringOfPair(CompairResualt.initialSamePairs[i-1]);
            }

        }

        private string getStringOfPair(Pair pair)
        {
            string p = pair.Feature1 + "," + pair.Feature2;
            return p;
        }

        private void GetObsolete(List<Pair> removedPairs, List<string> testName)
        {
            //InitialMatrix(removedPairs);
            //List<int> cols = GetColNumber(InitMatrix);
            List<int> cools = new List<int>();
            for (int i = 0; i < CompairResualt.ChangedPairs[0].TestCases.Count; i++)
            {
                cools.Add(i);
            }
            //cols = cols.Except(reservedTestCases).ToList();
            cools = cools.Except(reservedTestCases).ToList();
            ObsoleteTestCases = GetTestCasesOf(cools);
        }
        private void GetReusable(List<Pair> samePairs, List<string> testName)
        {
            InitialMatrix(samePairs);
            List<int> cols = GetColNumber(InitMatrix);
            cols = cols.Except(reservedTestCases).ToList();
            ReUsableTestCases = GetTestCasesOf(cols);
            reservedTestCases = reservedTestCases.Union(cols).ToList();
        }
        public void GetRetestable(List<Pair> ChangedPairs, List<string> _testCases)
        {
            InitialMatrix(ChangedPairs);//yek matrix az 0 o 1 haye mojod dar liste har pair ke marboot be changedpairs  hast ejad mikonad
            List<int> cols = ColOfRetestableTestcases = reservedTestCases = GetColNumber(InitMatrix);// shomare colume haye moaser bar roye changed pairhara peyda mikonad
            Matrix = new int[ChangedPairs.Count, cols.Count];
            for (int i = 0; i < cols.Count; i++)
            {
                for (int j = 0; j < ChangedPairs.Count; j++)
                {
                    Matrix[j, i] = InitMatrix[j, cols[i]];
                }
            }

            RetestableTestCases = GetTestCasesOf(cols);
            
        }
        private List<int> GetColNumber(int[,] matrix)
        {
            List<int> colnumber = new List<int>();
            for (int col = 0; col < matrix.GetLength(1); col++)
            {
                
                for (int row = 0; row < matrix.GetLength(0); row++)
                {
                    
                    if (matrix[row,col] >= 1)
                    {
                        
                        colnumber.Add(col);
                        break;
                    }
                }
            }
            return colnumber;
        }
        private List<string> GetTestCasesOf(List<int> ColNumber)
        {
            List<string> testCases = new List<string>();
            for (int i = 0; i < ColNumber.Count; i++)
            {
                testCases.Add(TestName[ColNumber[i]]);
            }
            return testCases;
        }
        private void InitialMatrix(List<Pair> _pairs)
        {
            int pcount = _pairs.Count;
            int tcount = _pairs[0].TestCases.Count;

            InitMatrix = new int[pcount, tcount];
            for (int i = 0; i < pcount; i++)
            {
                for (int j = 0; j < tcount; j++)
                {
                    InitMatrix[i, j] = _pairs[i].TestCases[j];
                }
            }

        }
    }
}
