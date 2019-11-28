using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using FeatureClass;

namespace Regression.FeatureModel
{
    public class SaveEvaluation
    {
        public FilterTestCase FilterTestCase { set; get; }
        public int AllTestCount { get; set; }
        _Application EP = new _Excel.Application();
        Workbook WB;
        Worksheet WS1;
        _Excel.Range Range;
        object misValue = System.Reflection.Missing.Value;
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(int handel, out int ProcessId);

        string Path;
        string MyCoverage;
        string MyCost;
        string MyFDE;
        string RandCoverage;
        string RandCost;
        string RandFDE;
        string FileName;
        string Reusability;
        string EstimateTime;
        string M9;
        string M10;
        public SaveEvaluation()
        {

        }
        public SaveEvaluation(string _path, string _myCoverage, string _myCost, string _myFDE, string _randCoverage, string _randCost, string _randFDE,string _filename,double _reusability,string _time)
        {
            Path = _path;
            MyCoverage = _myCoverage;
            MyCost = _myCost;
            MyFDE = _myFDE;
            RandCoverage = _randCoverage;
            RandCost = _randCost;
            RandFDE = _randFDE;
            FileName = _filename;
            Reusability = _reusability.ToString();
            EstimateTime = _time;
            EP.Visible = true;
        }
        public void Run()
        {
            WB = EP.Workbooks.Open(Path, 0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
            _Excel.Sheets sheets = WB.Sheets;

            WS1 = (_Excel.Worksheet)WB.Worksheets.get_Item(1);

            Save(WS1);
        }
        private void Save(Worksheet ws)
        {
            int col = GetCol(WS1);
            string[,] matrix = MakeArray();

            Range c1 = ws.Cells[1, col];
            Range c2 = ws.Cells[14, col];

            ws.get_Range(c1, c2).Value2 = matrix;
            
            int[] rowConvertToint = { 3, 4, 5, 7, 8, 9, 10, 11, 12, 13 ,14};
            for (int i = 0; i < rowConvertToint.Length; i++)
            {
                try
                {
                    ws.Cells[rowConvertToint[i], col].TextToColumns();
                    ws.Cells[rowConvertToint[i], col].NumberFormat = "0.00";
                }
                catch (Exception)
                {
                    continue;
                }
                
            }
            WB.Save();
            WB.Close();
            Exit();
            
        }

        private string[,] MakeArray()
        {
            string[,] M = new string[14,1];
            string filename = FileName;
            M[0,0] = filename;
            M[1, 0] = EstimateTime;
            M[2, 0] = MyCoverage;
            M[3, 0] = MyCost;
            M[4, 0] = MyFDE;
            M[5, 0] = "_";
            M[6, 0] = RandCoverage;
            M[7, 0] = RandCost;
            M[8, 0] = RandFDE;

            M[9, 0] = M1();
            M[10, 0] = M2();
            M[11, 0] = M3();
            M[12, 0] = Reusability;
            M[13, 0] = M9Calculate();
            
            return M;
            
        }

        private string M10Calculate()
        {
            throw new NotImplementedException();
        }

        private string M9Calculate()
        {
            int ObCount = FilterTestCase.ObsoleteTestCases.Count;
            int ReusibleCount = FilterTestCase.ReUsableTestCases.Count;
            int Retestablecount = FilterTestCase.RetestableTestCases.Count;
            int allTestCount = AllTestCount;

            double a = (allTestCount - (Retestablecount + ReusibleCount)) / ObCount;
            M9 = Math.Round(a, 2).ToString();
            return M9;
        }

        private string M3()
        {
            double c1 = Convert.ToDouble(MyFDE);
            double c2 = Convert.ToDouble(RandFDE);
            double m1 = (c1 - c2) / c2;
            m1 = Math.Round(m1, 2);
            return m1.ToString();
        }

        private string M2()
        {
            double c1 = Convert.ToDouble(MyCost);
            double c2 = Convert.ToDouble(RandCost);
            double m1 = (c1 - c2) / c2;
            m1 = Math.Round(m1, 2);
            return m1.ToString();
        }

        private string M1()
        {
            double c1 = 1- Convert.ToDouble(MyCoverage);
            double c2 = 1- Convert.ToDouble(RandCoverage);
            double m1 = (c1 - c2) / c2;
            m1 = Math.Round(m1, 2);
            return m1.ToString();
        }

        private int GetCol(Worksheet ws)
        {
            Range = ws.UsedRange;
            return Range.Columns.Count+1;
        }
        public void Exit()
        {
            //WB.Close(true, misValue, misValue);
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
