using Microsoft.Office.Interop.Excel;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using _Excel = Microsoft.Office.Interop.Excel;

namespace Regression
{
    internal class SaveVersionEval
    {
        _Application EP = new _Excel.Application();
        Workbook WB;
        Worksheet WS;
        _Excel.Range Range;
        object misValue = System.Reflection.Missing.Value;
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(int handel, out int ProcessId);

        private double m5;
        private double m6;
        private double m7;
        private double m8;
        private string FileName;
        const string Path = @"C:\Users\masuo_esp0vb3\Desktop\jm\Evaluation\Evaluation-Q2.xlsx";

        public SaveVersionEval(double m5, double m6, double m7, double m8, string _fileName)
        {
            this.m5 = m5;
            this.m6 = m6;
            this.m7 = m7;
            this.m8 = m8;
            FileName = _fileName;
        }
        public bool Run()
        {
            WB = EP.Workbooks.Open(Path, 0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
            _Excel.Sheets sheets = WB.Sheets;

            WS = (_Excel.Worksheet)WB.Worksheets.get_Item(1);

            if (Write(WS))
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        private bool Write(Worksheet _wS)
        {
            string[,] M = new string[5,1];
            M[0,0] = FileName;
            M[1,0] = m5.ToString();
            M[2,0] = m6.ToString();
            M[3,0] = m7.ToString();
            M[4,0] = m8.ToString();

            int[] rowConvertToint = { 2, 3, 4, 5 };

            int col = GetCol(_wS);
            Range c1 = _wS.Cells[1, col];
            Range c2 = _wS.Cells[5, col];
            try
            {
                _wS.get_Range(c1, c2).Value2 = M;

                for (int i = 0; i < rowConvertToint.Length; i++)
                {
                    try
                    {
                        _wS.Cells[rowConvertToint[i], col].TextToColumns();
                        _wS.Cells[rowConvertToint[i], col].NumberFormat = "0.00";
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                WB.Save();
                WB.Close();
                Exit();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
            
            

        }
        private int GetCol(Worksheet ws)
        {
            Range = ws.UsedRange;
            foreach (_Excel.Range r in Range.Columns)
            {
                if (r.Value2[1, 1] == null)
                {
                    return r.Column;
                }
                else
                {
                    //i++;
                }
            }
            return Range.Columns.Count + 1;
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