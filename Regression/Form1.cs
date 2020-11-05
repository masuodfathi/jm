using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JMetalRunners.NSGAII;
using FeatureClass;
using Regression.FeatureModel;
using System.IO;
using System.Security.Cryptography;
using System.Threading;

namespace Regression
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        featureModel fmO;
        Compare compair;
        FilterTestCase filter;
        computingRandom RC;
        List<string> Var;
        int[] Faults;
        string CurrentFileName;
        double Reusability;
        string FileName;
        private List<string> GetPairsList(List<Pair> pairs)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < pairs.Count; i++)
            {
                list.Add(pairs[i].Feature1 + "," + pairs[i].Feature2);
            }
            return list;
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            CleareLabel();

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Open Matrix File";
            openFileDialog1.DefaultExt = "xlsx";
            openFileDialog1.Filter = "Excel files (*.xlsx)|*.xlsx";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileName = getFileName( openFileDialog1.FileName.Split('\\').Last());
                string path = openFileDialog1.FileName;
                CurrentFileName = path.Split('\\').Last();
                label1.Text = path;

                OpenExcel a = new OpenExcel(label1.Text);
                fmO = new featureModel(a.OldMatrix);
                PaireSet newPairs = new PaireSet(a.newPairs);
                compair = new Compare(fmO, newPairs, a.ChangedFeatureList);
                filter = new FilterTestCase(compair);

                Faults = setFaults(compair.ChangedPairs.Count);

                List<string> list1Source = new List<string>();
                for (int i = 0; i < compair.NewPairs.Count; i++)
                {
                    list1Source.Add(compair.NewPairs[i].Feature1 + "," + compair.NewPairs[i].Feature2);
                }

                listBox1.DataSource = list1Source;
                listBox2.DataSource = filter.RetestableTestCases;
                listBox3.DataSource = filter.ReUsableTestCases;
                listBox4.DataSource = filter.ObsoleteTestCases;
                listBox5.DataSource = GetPairsList(compair.ChangedPairs);
                listBox6.DataSource = GetPairsList(compair.SamePairs);
                listBox7.DataSource = GetPairsList(compair.RemovedPairs);
                int[] rowcol = new int[2];
                string[] arg = new string[1];
                arg[0] = "Regression";
                
                rowcol[0] = filter.Matrix.GetLength(0);
                rowcol[1] = filter.Matrix.GetLength(1);
                NSGAII.Matrix = filter.Matrix;

                NSGAII.RowCol = rowcol;
                NSGAII.Main(arg);

                //JMetalCSharp.Core.SolutionSet s = new JMetalCSharp.Core.SolutionSet();
                List<string> fun = new List<string>();
                Var = NSGAII.variables;
                fun = NSGAII.fun;
                //fun.Sort();
                listBox8.DataSource = Var;
                listBox9.DataSource = SeperateFun(fun);
                

                
                RC = new computingRandom(fmO, compair,filter,Faults);
                double Randomreusability = RC.GetReusability(fmO.Pairs,RC.Variable.ToList());
                LRCover.Text = RC.UnCoverage;
                LRCost.Text = RC.Cost;
                LRFDE.Text = RC.FDE.ToString();
                string[] algo = listBox9.Items[0].ToString().Split("\t"[0]);

                int[] ourVariable = new int[compair.TestName.Count];
                ourVariable = SetOurVariable(filter.removedTest, Var[0]);
                
                
                computingRandom mr = new computingRandom();

                double MyFDE = mr.GetFDE(ourVariable, Faults, compair.ChangedPairs);
                double MyReusability = mr.GetReusability(fmO.Pairs, ourVariable.ToList(), compair.ChangedPairs.Count);
                //double MyReusability = mr.GetReusability(compair.ChangedPairs, ourVariable.ToList(),compair.ChangedPairs.Count);
                Reusability = (MyReusability - Randomreusability) / Randomreusability;
                MyFDE = Math.Round(MyFDE, 2);
                Reusability = Math.Round(Reusability, 2);
                Lcoverage.Text = algo[0];
                Lcost.Text = algo[1].Trim();
                LFDE.Text = MyFDE.ToString();


                //Appearence -------------------------------------------------------------
                if (Convert.ToDouble(RC.UnCoverage) > Convert.ToDouble(algo[0]))
                {
                    tick1.Text = "\u2714";
                    tick1.ForeColor = Color.FromArgb(80, 200, 80);
                }
                else
                {
                    tick1.Text = "X";
                    tick1.ForeColor = Color.FromArgb(200, 80, 80);
                }
                if (Convert.ToDouble(RC.Cost) > Convert.ToDouble(algo[1]))
                {
                    tick2.Text = "\u2714";
                    tick2.ForeColor = Color.FromArgb(80, 200, 80);
                }
                else
                {
                    tick2.Text = "X";
                    tick2.ForeColor = Color.FromArgb(200, 80, 80);
                }
                if (Convert.ToDouble(RC.FDE) < Convert.ToDouble(MyFDE))
                {
                    tick3.Text = "\u2714";
                    tick3.ForeColor = Color.FromArgb(80, 200, 80);
                }
                else
                {
                    tick3.Text = "X";
                    tick3.ForeColor = Color.FromArgb(200, 80, 80);
                }
                //Appearence -------------------------------------------------------------
                //SaveCompairViaRandom
                SaveCompairViaRandom(MyReusability,Randomreusability);
                saveResult(path);
      
                int numberoftest = 0;
                string variable = Var[0];
                for (int i = 0; i < Var[0].Length; i++)
                {
                    if (variable[i].ToString() == "1")
                    {
                        numberoftest++;
                    }
                }
                //int numberOfSelectedTest =  filter.RetestableTestCases.Count;
                double newCover = 1 - Convert.ToDouble(algo[0]);
                VersionEvaluation Ve = new VersionEvaluation(fmO.Pairs, compair.initialSamePairs, newPairs.PairsList.Count, numberoftest, Faults, compair.ChangedPairs, MyFDE, MyReusability, newCover);
                SaveVersionEval SEV = new SaveVersionEval(Ve.M5,Ve.M6,Ve.M7,Ve.M8,CurrentFileName);
                SEV.Run();
                
                Dolabel();
            }
        }

        private int[] SetOurVariable(List<int> removedTest, string v)
        {
            
            List<int> variable = new List<int>();

            foreach (var item in v)
            {
                variable.Add(int.Parse(item.ToString()));
            }

            for (int i = 0; i < removedTest.Count; i++)
            {
                variable.Insert(removedTest[i], 0);
            }

            int[] t = new int[variable.Count];
            for (int i = 0; i < variable.Count; i++)
            {
                t[i] = variable[i];
            }

            return t;
        }

        private int[] setFaults(int count)
        {
            int[] f = new int[count];
            for (int i = 0; i < count; i++)
            {
                int r = JMetalCSharp.Utils.JMetalRandom.Next(1, 5);
                int r1 = JMetalCSharp.Utils.JMetalRandom.Next(1, 15);
                
                if (true)
                {
                    f[i] = 1;
                }
            }
            return f;
        }

        private void saveResult(string path)
        {
            FeatureModel.SaveResult saveResult = new SaveResult(path);
        }

        private void Dolabel()
        {
            label23.Text = NSGAII.estimatedTime.ToString() + " ms";
            LcoverageC.Text = listBox9.Items.Count.ToString();
            LremoveC.Text = listBox7.Items.Count.ToString();
            LsameC.Text = listBox6.Items.Count.ToString();
            LchangedC.Text = listBox5.Items.Count.ToString();
            LpairC.Text = listBox1.Items.Count.ToString();
            LretestC.Text = listBox2.Items.Count.ToString();
            LreuseC.Text = listBox3.Items.Count.ToString();
            LobsolC.Text = listBox4.Items.Count.ToString();
        }

        private void CleareLabel()
        {
            label23.Text = "";
            label23.Text = "";
            LcoverageC.Text = "";
            LremoveC.Text = "";
            LsameC.Text = "";
            LchangedC.Text = "";
            LpairC.Text = "";
            LretestC.Text = "";
            LreuseC.Text = "";
            LobsolC.Text = "";
            listBox1.DataSource = null;
            
            foreach (Control item in Controls)
            {
                if (item is ListBox)
                {
                    ((ListBox)item).DataSource = null;
                }
            }
        }

        private List<string> seperateFun(List<string> fun)
        {
            List<string> f = new List<string>();
            string[] s = new string[2];
            for (int i = 0; i < fun.Count(); i++)
            {
                s = fun[i].Split(' ');
                string coverageCost = "";
                //coverageCost = string.Format("{0}       {1}", s[0], s[1]);

                for (int j = 0; j < 2; j++)
                {

                    coverageCost += s[j] + "  \t  ";

                }
                f.Add(coverageCost.TrimEnd());
            }
            return f;
        }
        private List<string> SeperateFun(List<string> fun)
        {
            List<string> f = new List<string>();
            string[] s = new string[2];
            for (int i = 0; i < fun.Count(); i++)
            {
                s = fun[i].Split(' ');
                string coverageCost = "";
                //coverageCost = string.Format("{0}       {1}", s[0], s[1]);

                for (int j = 0; j < 2; j++)
                {

                    coverageCost += s[j] + "  \t  ";

                }
                f.Add(coverageCost.TrimEnd());
            }
            return f;
        }
        private void Button2_Click(object sender, EventArgs e)
        {
            FeatureModel.writeToExcel r = new FeatureModel.writeToExcel();
            label4.Text = "Features Number: " + r.FeatureNumber.ToString();
            label5.Text = "TastCases Number: " + r.TestCasesCount.ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                
                FeatureModel.SPLOT_Reader s = new FeatureModel.SPLOT_Reader(openFileDialog1.FileName);
                List<string> pairs = s.GetPairs();
                listBox1.DataSource = pairs;
                label18.Text  = openFileDialog1.FileName;
                
                FileName = getFileName(openFileDialog1.FileName.Split('\\').Last());
                
                
                label19.Text = pairs.Count.ToString();
                SaveFileDialog saveFlDialog = new SaveFileDialog();
                saveFlDialog.FileName = FileName;
                saveFlDialog.Filter = "Excell File|*.xlsx";
                saveFlDialog.Title = "Save a Feature model";
                if (saveFlDialog.ShowDialog() == DialogResult.OK)
                {
                    
                    string filename = saveFlDialog.FileName;
                    FeatureModel.writeToExcel writeToExcel = new FeatureModel.writeToExcel(pairs, filename);
                }

            }


        }

        private string getFileName(string v)
        {
            string[] n = v.Split('.');
            string name = "";
            for (int i = 0; i < n.Length - 1; i++)
            {
                name += n[i];
            }
            return name;
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            fmO = null;


            RC = null;
            
            
            SaveFileDialog saveFlDialog = new SaveFileDialog();
            saveFlDialog.FileName = FileName;
            if (saveFlDialog.ShowDialog() == DialogResult.OK)
            {
                //saveFlDialog.FileName = FileName + "-V";
                string path = saveFlDialog.FileName;
                path += "-v";
                writeToExcel wr = new writeToExcel(filter.matrixNewversion,compair.NewPairs,path);
            }
        }

        private void ListBox8_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox9.Items.Count > 0)
            {


                int i = listBox8.SelectedIndex;
                string[] algo = listBox9.Items[i].ToString().Split("\t"[0]);

                int[] ourVariable = new int[compair.TestName.Count];
                ourVariable = SetOurVariable(filter.removedTest, Var[i]);

                computingRandom mr = new computingRandom();

                double MyFDE = mr.GetFDE(ourVariable, Faults, compair.ChangedPairs);
                MyFDE = Math.Round(MyFDE, 2);
                Lcoverage.Text = algo[0];
                Lcost.Text = algo[1].Trim();
                LFDE.Text = MyFDE.ToString();



                //Appearence -------------------------------------------------------------
                if (Convert.ToDouble(RC.UnCoverage) > Convert.ToDouble(algo[0]))
                {
                    tick1.Text = "\u2714";
                    tick1.ForeColor = Color.FromArgb(80, 200, 80);
                }
                else
                {
                    tick1.Text = "X";
                    tick1.ForeColor = Color.FromArgb(200, 80, 80);
                }
                if (Convert.ToDouble(RC.Cost) > Convert.ToDouble(algo[1]))
                {
                    tick2.Text = "\u2714";
                    tick2.ForeColor = Color.FromArgb(80, 200, 80);
                }
                else
                {
                    tick2.Text = "X";
                    tick2.ForeColor = Color.FromArgb(200, 80, 80);
                }
                if (Convert.ToDouble(RC.FDE) < Convert.ToDouble(MyFDE))
                {
                    tick3.Text = "\u2714";
                    tick3.ForeColor = Color.FromArgb(80, 200, 80);
                }
                else
                {
                    tick3.Text = "X";
                    tick3.ForeColor = Color.FromArgb(200, 80, 80);
                }
            }
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            //string path = @"C:\Users\masuo_esp0vb3\Desktop\jm\Evaluation\Evaluation-Q1.xlsx";
            string path = @"C:\Users\masuo_esp0vb3\Desktop\Evaluation-Q1.xlsx";
            string time = NSGAII.estimatedTime.ToString();
            SaveEvaluation sv = new SaveEvaluation(path, Lcoverage.Text, Lcost.Text, LFDE.Text,LRCover.Text,LRCost.Text,LRFDE.Text,CurrentFileName,Reusability,time);
            sv.FilterTestCase = filter;
            sv.AllTestCount = compair.ChangedPairs[0].TestCases.Count;
            sv.Run();
        }
        private void SaveCompairViaRandom(double myR,double randR)
        {
            //string path = @"C:\Users\masuo_esp0vb3\Desktop\jm\Evaluation\Evaluation-Q1.xlsx";
            string path = @"C:\Users\masuod\Desktop\Evaluation-Q1.xlsx";
            string time = NSGAII.estimatedTime.ToString();
            SaveEvaluation sv = new SaveEvaluation(path, Lcoverage.Text, Lcost.Text, LFDE.Text, LRCover.Text, LRCost.Text, LRFDE.Text, CurrentFileName, Reusability, time);
            sv.FilterTestCase = filter;
            sv.AllTestCount = compair.ChangedPairs[0].TestCases.Count;
            sv.MyUsability = myR.ToString();
            sv.RandUsability = randR.ToString();
            sv.Run();
        }
        private void Button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog opndialog = new OpenFileDialog();
            if (opndialog.ShowDialog() == DialogResult.OK)
            {
                
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            
        }

        private void listBox10_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void button9_Click(object sender, EventArgs e)
        {
            
        }
    }
}
