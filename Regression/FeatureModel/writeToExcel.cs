using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using FeatureClass;
using System.Text.RegularExpressions;

namespace Regression.FeatureModel
{
    public class writeToExcel
    {
        public int FeatureNumber;
        public int TestCasesCount;
        _Application EP = new _Excel.Application();
        Workbook WB;
        Worksheet WS1;
        Worksheet WS2;
        Worksheet WS3;
        object misValue = System.Reflection.Missing.Value;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(int handel, out int ProcessId);
        public writeToExcel()
        {

            WB = EP.Workbooks.Add(misValue);
            WB.Worksheets.Add(misValue,WB.Worksheets[1]);
            WB.Worksheets.Add(misValue,WB.Worksheets[2]);
            
            
            WS1 = WB.Worksheets.get_Item(1);
            WS2 = WB.Worksheets.get_Item(2);
            WS3 = WB.Worksheets.get_Item(3);
            WriteWorkSheet1();
            WriteWorkSheet2();
            WriteWorkSheet3();
            
            
            Save();
            Exit();
        }
        public writeToExcel(List<string> pairs , string path)
        {

            EP.Visible = true;
            WB = EP.Workbooks.Add(misValue);
            WB.Worksheets.Add(misValue, WB.Worksheets[1]);
            WB.Worksheets.Add(misValue, WB.Worksheets[2]);

            WS1 = WB.Worksheets.get_Item(1);
            WS2 = WB.Worksheets.get_Item(2);
            WS3 = WB.Worksheets.get_Item(3);
            List<string> FeatureList = GetFeatures(pairs);

            WriteWorkSheet1(pairs);
            List<string> newpairs =  WriteWorkSheet2(pairs,FeatureList);
            List<string> featuresOfSheet2 = GetFeatures(newpairs);
            WriteWorkSheet3(FeatureList,featuresOfSheet2);

            Save(path);
            Exit();
        }
        
        public writeToExcel(string[,] oldMatrix,List<Pair> newPair,string path)
        {
            EP.Visible = true;
            WB = EP.Workbooks.Add(misValue);
            WB.Worksheets.Add(misValue, WB.Worksheets[1]);
            WB.Worksheets.Add(misValue, WB.Worksheets[2]);

            WS1 = WB.Worksheets.get_Item(1);
            WS2 = WB.Worksheets.get_Item(2);
            WS3 = WB.Worksheets.get_Item(3);

            string[,] newVersion = GetMatrixNewversion(oldMatrix,newPair);
            //matrix sheet2 az version qabl be sorate kamel ba testcases va pairs darone matrix hast
            int row = newVersion.GetLength(0);
            int col = newVersion.GetLength(1);

            WritefullArray(WS1, row, col, newVersion);//write in sheet1
            List<string> pairsList = getPairFromMatrix(newVersion);
            List<string> featureList = GetFeatures(pairsList);

            List<string> newpairs = WriteWorkSheet2(pairsList,featureList);
            List<string> featuresOfSheet2 = GetFeatures(newpairs);
            WriteWorkSheet3(featureList,featuresOfSheet2);
            Save(path);
            Exit();
        }

        private List<string> getPairFromMatrix(string[,] newVersion)
        {
            int RowSize = newVersion.GetLength(0);
            List<string> pairs = new List<string>();
            for (int i = 1; i < RowSize; i++)
            {
                pairs.Add(newVersion[i, 0]);
            }
            return pairs;
        }

        private string[,] GetMatrixNewversion(string[,] oldMatrix, List<Pair> newPair)
        {
            int LastTestnumber = GetLastTestName(oldMatrix);
            List<string> NTest = GenerateTestcases(newPair.Count,LastTestnumber);
            int row = oldMatrix.GetLength(0) + newPair.Count;
            int col = oldMatrix.GetLength(1) + NTest.Count;
            int oldrow = oldMatrix.GetLength(0);
            int oldcol = oldMatrix.GetLength(1);
            string[,] matrix = new string[row, col];
            
            //dar in for tamame matrix ra ba "0" por mikonim
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    matrix[i, j] = "0";
                }
            }
            //dar in for maqaddir oldmatrix ra bedone taqir dakhele matrix rikhtim
            for (int i = 0; i < oldcol ; i++)
            {
                for (int j = 0; j < oldrow; j++)
                {
                    matrix[j, i] = oldMatrix[j, i];
                }
            }
            //dar in for name testcase haye jadidi ra be matrix ezafe mikonim
            for (int i = oldcol; i < col; i++)
            {
                matrix[0, i] = NTest[i-oldcol];
            }
            //yek list tasadofi az 0 o 1 ijad mikonim baraye mavarede azmone jadid
            List<string> testBit = GenerateMatrix(newPair.Count, NTest.Count);
            int indexTestBit = 0;
            for (int i = oldrow; i < row; i++)
            {
                for (int j = oldcol; j < col; j++)
                {
                    
                    string a = testBit[indexTestBit];
                    matrix[i, j] = testBit[indexTestBit];
                    indexTestBit++;
                }
            }
            //in for baraye sabte name pair ha dar matrix ast
            List<string> pairsName = getPairListString(newPair);
            for (int i = oldrow; i < row; i++)
            {
                matrix[i, 0] = pairsName[i - oldrow];
            }
            return matrix;
        }

        private int GetLastTestName(string[,] oldMatrix)
        {
            int lastcol = oldMatrix.GetLength(1);
            string testName = oldMatrix[0, lastcol-1];
            string numberOfTest = Regex.Match(testName, @"\d+").Value;
            int RN =int.Parse(numberOfTest);
            return RN;
        }

        //public writeToExcel(List<Pair> pairsHaveTest, List<Pair> pairsNeedTest, string path)
        //{
        //    EP.Visible = true;
        //    WB = EP.Workbooks.Add(misValue);
        //    WB.Worksheets.Add(misValue, WB.Worksheets[1]);
        //    WB.Worksheets.Add(misValue, WB.Worksheets[2]);

        //    WS1 = WB.Worksheets.get_Item(1);
        //    WS2 = WB.Worksheets.get_Item(2);
        //    WS3 = WB.Worksheets.get_Item(3);

        //    String[,] m = GenerateMatrixNewversion(pairsHaveTest,pairsNeedTest.Count());

        //    WriteWorkSheet1(m);
        //    Save(path);
        //    Exit();
        //}

        //public writeToExcel(List<Pair> changedPairs, List<Pair> samePairs, List<Pair> newPairs, string path)
        //{
        //    EP.Visible = true;
        //    WB = EP.Workbooks.Add(misValue);
        //    WB.Worksheets.Add(misValue, WB.Worksheets[1]);
        //    WB.Worksheets.Add(misValue, WB.Worksheets[2]);

        //    WS1 = WB.Worksheets.get_Item(1);
        //    WS2 = WB.Worksheets.get_Item(2);
        //    WS3 = WB.Worksheets.get_Item(3);

        //    IEnumerable<Pair> pairsHaveTest = changedPairs.Union(samePairs).ToList();//tarkibe Changed pairs va same pairs chon hardo testcase daran
        //    String[,] m = GenerateMatrixNewversion(pairsHaveTest.ToList(), newPairs.Count());

        //    List<string> pairsString = getPairListString(pairsHaveTest.ToList());
        //    List<string> pairsStringnew = getPairListString(newPairs);
        //    pairsString = pairsString.Union(pairsStringnew).ToList();
        //    WriteWorkSheet1(m);
        //    List<string> testName = new List<string>();
        //    List<string> NewtestName = new List<string>();
        //    WriteWorkSheet1(pairsString,testName,NewtestName);
        //    Save(path);
        //    Exit();
        //}

        //private void WriteWorkSheet1(List<string> pairsString, List<string> testName, List<string> newtestName)
        //{

        //}

        private List<string> getPairListString(List<Pair> pairs)
        {
            List<string> p = new List<string>();
            for (int i = 0; i < pairs.Count(); i++)
            {
                p.Add(pairs[i].Feature1 + "," + pairs[i].Feature2);
            }
            return p;
        }

        //private string[,] GenerateMatrixNewversion(List<Pair> pairsHaveTest, int newPCount)
        //{
        //    int havePcount = pairsHaveTest.Count();
        //    int haveTcount = pairsHaveTest[0].TestCases.Count();
        //    int row = havePcount + newPCount;
        //    List<string> newTest = GenerateTestcases(newPCount,haveTcount.ToString());
        //    int col = haveTcount + newTest.Count();
            

        //    string[,] matrix = new string[row, col];
        //    for (int i = 0; i < row; i++)
        //    {
        //        for (int j = 0; j < col; j++)
        //        {
        //            if (i < havePcount)
        //            {
        //                if (j < haveTcount)
        //                {
        //                    matrix[i, j] = pairsHaveTest[i].TestCases[j].ToString();
        //                }
        //                else
        //                {
        //                    matrix[i, j] = "0";
        //                }
                        
        //            }
        //            else
        //            {
        //                matrix[i, j] = "0";
        //            }
                    
        //        }
        //    }
        //    List<string> tvalue = GenerateMatrix(row - havePcount, col - haveTcount);
        //    int c = 0;
        //    for (int i = havePcount; i < row; i++)
        //    {
        //        for (int j = haveTcount; j < col; j++)
        //        {
                    
        //            matrix[i, j] = tvalue[c];
        //            c++;

        //        }
        //    }
        //    return matrix;
        //}

        private List<string> GetFeatures(List<string> pairs)
        {
            List<string> features = new List<string>();
            for (int i = 0; i < pairs.Count; i++)
            {
                string[] f = pairs[i].Split(',');
                for (int j = 0; j < f.Length; j++)
                {
                    features.Add(f[j]);
                }

            }
            features = features.Distinct().ToList();
            return features;
        }

        private void WriteWorkSheet1()
        {
            int random = Random(7, 10);
            FeatureNumber = random;
            List<string> pairs = GeneratePairs(random);
            List<string> testcases = GenerateTestcases(random);
            TestCasesCount = testcases.Count;
            List<string> matrix = GenerateMatrix(pairs.Count, testcases.Count);
            Writepairs(pairs);
            Writetestcases(testcases);
            WriteMatrix(matrix, pairs.Count, testcases.Count, WS1);
        }

        private void WriteWorkSheet1(List<string> interactions)
        {
            List<string> features = GetFeatures(interactions);
            FeatureNumber = features.Count;
            List<string> pairs = interactions;
            List<string> testcases = GenerateTestcases(FeatureNumber);
            TestCasesCount = testcases.Count;
            List<string> matrix = GenerateMatrix(pairs.Count, testcases.Count);
            Writepairs(pairs);
            Writetestcases(testcases);
            WriteMatrix(matrix, pairs.Count, testcases.Count,WS1);
        }
        private void WriteWorkSheet1(string[,] matrix)
        {
            WriteMatrix(matrix, matrix.GetLength(0), matrix.GetLength(1), WS1);
        }
        private void WriteWorkSheet1(List<string> pairs, string[,] matrix)
        {

        }
        private void WriteWorkSheet2()
        {

        }

        private List<string> WriteWorkSheet2(List<string> pairs, List<string> featurelist)
        {
            List<string> randomPairs = getRandomFromSheeet1(pairs);
            List<string> newPairs = GetNewPairs(featurelist,pairs.Count);
            newPairs.AddRange(randomPairs);
            string[,] pairsArray = new string[newPairs.Count,1];
            for (int i = 0; i < newPairs.Count; i++)
            {
                pairsArray[i,0] = newPairs[i];
            }
            int row = newPairs.Count;
            WriteArray(WS2, row, 1, pairsArray);
            return newPairs;
        }

        private List<string> GetNewPairs(List<string> featurelist , int PairsCount)
        {
            int newpairsnumber = Random(PairsCount / 5, PairsCount / 4);
            List<string> newpairs = new List<string>();
            int name = 1;
            for (int i = 1; i < newpairsnumber; i++)
            {
                
                bool duplicate = true;
                string pair =string.Format("f{0}",name);
                name++;
                for (int j = 0; j < featurelist.Count; j++)
                {
                    if (pair == featurelist[j])
                    {
                        duplicate = false;
                        i--;
                        break;
                    }
                }
                if (duplicate)
                {
                    pair += ","+featurelist[Random(1,featurelist.Count-1)];
                    newpairs.Add(pair);
                }
                
            }
            return newpairs;
        }

        private List<string> getRandomFromSheeet1(List<string> pairs)
        {
            List<string> sheet2PairsGottenFromSheet1 = new List<string>();
            sheet2PairsGottenFromSheet1.Add(pairs[0]);
            for (int i = 1; i < pairs.Count; i++)
            {
                
                int r = Random(0,6);
                if (r > 2)
                {
                    sheet2PairsGottenFromSheet1.Add(pairs[i]);
                }
            }
            return sheet2PairsGottenFromSheet1;
        }

        private void WriteWorkSheet3(List<string> featuresListSheet1 ,List<string> featuresListSheet2)
        {
            List<string> selected = new List<string>();
            for (int i = 0; i < featuresListSheet1.Count; i++)
            {
                int r = Random(1,3);
                if (r == 1)
                {
                    selected.Add(featuresListSheet1[i]);
                }

            }
            selected = selected.Select(c => c).Intersect(featuresListSheet2).ToList();
            WriteMatrix(selected, 1, selected.Count,WS3);

            // write without matrix
            //for (int i = 0; i < selected.Count; i++)
            //{

            //    Write(WS3, i + 1, 1, selected[i]);
            //}
        }

        private void WriteWorkSheet3()
        {
            List<string> select = new List<string>();
            for (int i = 0; i < FeatureNumber; i++)
            {
                int r = Random(2);
                if (r == 1)
                {
                    select.Add("f" + i);
                }

            }
            for (int i = 0; i < select.Count; i++)
            {
                Write(WS3, 1, i+1, select[i]);
            }
        }

        private void WriteMatrix(List<string> matrix , int row , int col,Worksheet ws)
        {
            int c = 0;
            string[,] matrixTest = new string[row, col];

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    //Write(WS1, i+2, j+2, matrix[c]);
                    matrixTest[i, j] = matrix[c];
                    c++;
                }
            }
            WriteArray(ws, row, col, matrixTest);
            
        }
        private void WriteMatrix(string[,] matrix, int row, int col, Worksheet ws)
        {
            WriteArray(ws, row, col, matrix);
        }
        private void Writetestcases(List<string> testcases)
        {
            int len = testcases.Count;
            for (int i = 0; i < len; i++)
            {
                Write(WS1, 1, i+2, testcases[i]);
            }
            
        }

        private void Writepairs(List<string> pairs)
        {
            int len = pairs.Count;
            for (int i = 0 ; i < len; i++)
            {
                Write(WS1, i+2, 1, pairs[i]);
            }
        }

        private List<string> GenerateMatrix(int pairsCount, int testcasescount)
        {
            int length = pairsCount * testcasescount;
            List<string> value = new List<string>();
            for (int i = 0; i < length; i++)
            {
                int r = Random(0,testcasescount/2);
                string m = "0";
                if (r < 1)
                {
                    m = "1";
                }
                value.Add(m);
                 
            }
            return value;
        }

        private List<string> GenerateTestcases(int number,int StartNumber = 0)
        {
            StartNumber++;
            List<string> testCases = new List<string>();
            int min = number / 3;
            //int max = number - min;
            int max = number;
            min = (min == 0) ? 1 : min;
            
            int numberOfTest = Random(min, max);
            
            for (int i = StartNumber; i < numberOfTest+StartNumber; i++)
            {
                string testCase = string.Format("{0}-{1}", "t", i);
                testCases.Add(testCase);
            }
            return testCases;
        }

        public void Write(Worksheet ws , int row,int col, string text)
        {
            try
            {
                ws.Cells[row, col].Value2 = text;
            }
            catch (Exception e)
            {
                
            }
            
        }
        public void WriteArray(Worksheet ws , int row , int col , string[,] matrix)
        {
            
            int startCell = 1;
            int addition = 0; //if we start to write frome cell[1,1] we haven't needed to add any number to end cell number 
            if (ws == WS1)
            {
                startCell = 2;
                addition = 1;
            }
            _Excel.Range c1 = ws.Cells[startCell, startCell];
            _Excel.Range c2 = ws.Cells[row+addition, col+addition];
            
            ws.get_Range(c1,c2).Value2 = matrix;

            ///////////////////////////////////////////change cell format to number
            //for (int i = 1; i < col; i++)
            //{
            //    ws.Columns[i].TextToColumns();
            //    ws.Columns[i].NumberFormat = "0";
            //}
            //ws.get_Range(c1, c2).TextToColumns();//just one column
            //ws.get_Range(c1, c2).NumberFormat = "0";
            
        }
        public void WritefullArray(Worksheet ws, int row, int col, string[,] matrix)
        {
            int startCell = 1;

            _Excel.Range c1 = ws.Cells[startCell, startCell];
            _Excel.Range c2 = ws.Cells[row, col];

            ws.get_Range(c1, c2).Value2 = matrix;
        }
        public bool Save()
        {
            string Filename =System.Windows.Forms.Application.StartupPath+"\\doc\\"+ DateTime.Now.Ticks.ToString() + ".xlsx";
            try
            {
                WB.SaveAs(new System.IO.FileInfo(Filename));
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }

        }

        private void Save(string path)
        {
            try
            {
                WB.SaveAs(new System.IO.FileInfo(path));
                
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public List<string> GeneratePairs(int numberOfFeatures)
        {
            List<string> pairs = new List<string>();
            for (int i = 0; i < numberOfFeatures; i++)
            {
                for (int j = i+1; j < numberOfFeatures; j++)
                {
                    string feature = "f" + i + ",f" + j;
                    pairs.Add(feature);
                }
            }
            return pairs;
        }

        public int Random()
        {
            int rnd;
            rnd = JMetalCSharp.Utils.JMetalRandom.Next(0 , 1);
            return rnd;
        }

        public int Random(int max)
        {
            int rnd;
            rnd = JMetalCSharp.Utils.JMetalRandom.Next(0 , max);
            return rnd;
        }

        public int Random(int min,int max)
        {
            int rnd;
            rnd = JMetalCSharp.Utils.JMetalRandom.Next(min , max);
            return rnd;
        }

        public void Exit()
        {
            WB.Close(true, misValue, misValue);
            EP.Quit();
            int prid;
            GetWindowThreadProcessId(EP.Hwnd, out prid);
            Process[] Allprocess = Process.GetProcessesByName("excel");
            foreach (var process in Allprocess)
            {
                if (process.Id == prid)
                {
                    process.Kill();
                }
            }
            Allprocess = null;
        }

    }
}
