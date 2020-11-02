using JMetalCSharp.Core;
using JMetalCSharp.Encoding.SolutionType;
using JMetalCSharp.Utils.Wrapper;
using JMetalCSharp.Utils;
using System;

namespace JMetalCSharp.Problems.Regression
{
    public class Regression: Problem
    {
        int numberOfVariables;
        int numberOfPairs;
        int[,] Matrix;
        public Regression(string solutionType,int numberofvariable,int numberofpair,int[,] matrix)
        {
            
            this.numberOfVariables = numberofvariable;
            this.numberOfPairs = numberofpair;
            NumberOfVariables = numberofvariable;
            NumberOfObjectives = 2;
            NumberOfConstraints = 0;
            ProblemName = "regression";

            //Matrix = new int[numberofvariable, numberOfPairs];
            //Matrix = initialArray(numberOfVariables, numberOfPairs);
            Matrix = matrix;

            UpperLimit = new double[numberofvariable];
            LowerLimit = new double[numberofvariable];
            for (int i = 0; i < numberofvariable; i++)
            {
                LowerLimit[i] = 0.0;
                UpperLimit[i] = 1.0;
            }
            if (solutionType == "Int")
                this.SolutionType = new IntSolutionType(this);
            else
            {
                Console.WriteLine("Error: solution type " + solutionType + " is invalid");
                Logger.Log.Error("Solution type " + solutionType + " is invalid");
                return;
            }
        }
        int[,] initialArray(int x, int y)
        {
            Random rnd = new Random();
            int[,] f = new int[x, y];
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    f[i, j] = rnd.Next(0, 2);
                }
            }
            return f;
        }
        public override void Evaluate(Solution solution)
        {
            XReal x = new XReal(solution);
            
            double[] f = new double[NumberOfObjectives];
            float[] coverage = new float[numberOfPairs]; // motaqayeri ke neshan dahande tedade pair haye poshesh dade shode mibashad
            float cost = 0;

            for (int var = 0; var < NumberOfVariables; var++)
            {
                if (x.GetValue(var) >= 1)
                {
                    
                    cost++;
                    for (int i = 0; i < numberOfPairs; i++)
                    {
                        if (Matrix[i, var] == 1)
                        {
                            coverage[i] = 1;
                            
                        }
                    }

                    
                }
            }
            for (int j = 0; j < coverage.Length; j++)
            {
                f[0] +=  coverage[j];
            }
            f[0] = numberOfPairs - f[0];// uncoverage

            f[1] = cost;
            if (f[1] == 0)
            {
                f[0] = numberOfPairs;
                f[1] = numberOfVariables;
            }
            f[0] /= numberOfPairs;
            f[1] /= numberOfVariables;
            f[0] = Math.Round(f[0], 2);
            f[1] = Math.Round(f[1], 2);
            //f[0] = f[0] - 1;
            
            Console.WriteLine(f[0] + " : " + f[1]);
            solution.Objective[0] = f[0];
            solution.Objective[1] = f[1];
            
            
        }
    }
}
