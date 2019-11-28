using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;

namespace Regression.FeatureModel
{
    class SaveResult
    {
        string Path;
        _Application EP = new _Excel.Application();
        Workbook WB;
        Worksheet WS1;
        object misValue = System.Reflection.Missing.Value;
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(int handel, out int ProcessId);
        public SaveResult(string _path)
        {
            Path = _path;


            WB = EP.Workbooks.Open(Path, 0, false, 5, "", "", false, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, true, false, false);
            _Excel.Sheets sheets = WB.Sheets;

            WS1 = (_Excel.Worksheet)sheets.Add(Type.Missing, sheets[3], Type.Missing, Type.Missing);
            
            

            WS1 = (_Excel.Worksheet)WB.Worksheets.get_Item(4);
            string[,] a = new string[1, 1];
            a[0, 0] = "value";
            WritefullArray(WS1, 1, 1, a);

            WB.Save();
            WB.Close();


            //Save();
            //Exit();
        }
        public void WritefullArray(Worksheet ws, int row, int col, string[,] matrix)
        {
            int startCell = 1;

            _Excel.Range c1 = ws.Cells[startCell, startCell];
            _Excel.Range c2 = ws.Cells[row, col];

            ws.get_Range(c1, c2).Value2 = matrix;
        }
        private bool Save()
        {
            string Filename = Path;
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
        private void Exit()
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
