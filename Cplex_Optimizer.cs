using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using BaseClasses;
//using ILOG.Concert;
//using ILOG.CPLEX;

//using Accord.Math.Optimization;
//using Accord;

using Extreme.Mathematics.LinearAlgebra;
using Extreme.Mathematics.Optimization;
using Extreme.Mathematics;


namespace BaseClasses
{
    public class Cplex_Optimizer
    {
        public double[] PreW;
        public double[] myRet;
        public List<Symbols> mySymbols;
        public double[][] VarCov;        
        public double ExpVar;
        public double BestObj;
        public double[] myTC = new double[] { 0.001, 0.005 };//{ 0.005, 0.01 }
        public double PortVar;
        public double Weight = 10.0;

        /*
        public double[] Run_Opt()
        {
            
            Cplex CPL = new Cplex();
            double[] ResW = new double[mySymbols.Count()];

            //Define the Variables

            //Decision Variables
            INumVar[] X = CPL.NumVarArray(PreW.Count(),0.0,1.0);            
            INumVar[] BS = CPL.NumVarArray(2, System.Double.MinValue, System.Double.MaxValue);

            //Dependant Variables
            INumVar[] RD = new INumVar[myRet.Count()];

            //Objective Function Expresion
            INumExpr Prof = CPL.ScalProd(X, RD);
            Prof = CPL.Sum(Prof,CPL.Sum(CPL.Negative(CPL.Prod(BS[0],(0.05))) , CPL.Negative(CPL.Prod(BS[1],(0.1)))));
            

            int i=0;
            //Create Buy Constraint
            INumExpr BB = CPL.Prod(CPL.Sum(CPL.Abs(CPL.Diff(X[i], PreW[i])), CPL.Diff(X[i], PreW[i])),0.5);            
            for(i=1; i<PreW.Count(); i++){
                BB = CPL.Sum(BB, (CPL.Prod(CPL.Sum(CPL.Abs(CPL.Diff(X[i], PreW[i])), CPL.Diff(X[i], PreW[i])), 0.5)));
            }
            //Add Constraint to Model
            CPL.AddEq(0.0, CPL.Sum(BB, CPL.Negative(BS[0])));

            //Create Sell Constraint
            INumExpr SS = CPL.Prod(CPL.Sum(CPL.Abs(CPL.Diff(PreW[i], X[i])), CPL.Diff(PreW[i], X[i])), 0.5);
            for (i = 1; i < PreW.Count(); i++)
            {
                SS = CPL.Sum(BB, (CPL.Prod(CPL.Sum(CPL.Abs(CPL.Diff(PreW[i], X[i])), CPL.Diff(PreW[i], X[i])), 0.5)));
            }
            //Add Constraint to Model
            CPL.AddEq(0.0, CPL.Sum(SS, CPL.Negative(BS[1])));

            //Add Objective Function to Model
            CPL.Maximize(Prof);

            //Create the variance-Covariance Expression : (X')*(Sigma)*(X)
            INumExpr[] tmpA = new INumExpr[mySymbols.Count()];
            for (i = 0; i < mySymbols.Count(); i++)
            {
                tmpA[i] = CPL.ScalProd(VarCov[i], X);
            }

            INumExpr tmpB = CPL.Prod(X[0],tmpA[0]);

            for (i = 1; i < mySymbols.Count(); i++)
            {
                tmpB = CPL.Sum(tmpB, CPL.Prod(X[i], tmpA[i]));
            }
            //Add Portfolio Variance Constraint
            CPL.AddEq(ExpVar, tmpB);

            //Add Sgima(X[i]) equals to 1 Constraint
            INumExpr tmpXSig = CPL.Sum(X);
            CPL.AddEq(1, tmpXSig);

            if (CPL.Solve())
            {
                for (i = 0; i < mySymbols.Count(); i++)
                {
                    ResW[i] = CPL.GetValue(X[i]);
                }
                BestObj = CPL.ObjValue;
            }
            

            return ResW;
        }
        */
        /*
        public double[] Run_Opt_Accord()
        {
            int SC = mySymbols.Count + 1;
            double[] X = new double[SC];
            int ConstraintsCount = 4;            
            double[] ResW = new double[SC];
            
            
            List<LinearConstraint> list = new List<LinearConstraint>();

            double [,] Tvar = new double[SC,SC];
            for(int i=0; i<SC; i++){
                for(int j=0; j<SC; j++){
                    Tvar[i,j] = VarCov[i][j];
                }
            }

            int[] Indices1 = new int[SC];
            int[] Indices2 = new int[3 * SC + 2];
            int[] Indices3 = new int[3 * SC + 2];

            double[] Coef1 = new double[SC];
            double[] Coef2 = new double[3 * SC + 2];
            double[] Coef3 = new double[3 * SC + 2];

            double SumPre = 0;
            double[] c = { 2, 3 };
                        

           // QuadraticObjectiveFunction objective = new QuadraticObjectiveFunction(SC + 2, function: (x) => x.InnerProduct(c), gradient: (x) => c );
 

            




            for (int i = 0; i < SC; i++)
            {
                Indices1[i] = i;
                Indices2[i] = i; Indices2[i + SC] = i + SC; Indices2[i + 2 * SC] = i + 2 * SC;
                Indices3[i] = i; Indices3[i + SC] = i + 3 * SC; Indices3[i + 2 * SC] = i + 4 * SC;

                Coef1[i] = 1;
                Coef2[i] = 1; Coef2[i + SC] = 1; Coef2[i + 2 * SC] = 1;
                Coef3[i] = -1; Coef3[i + SC] = 1; Coef3[i + 2 * SC] = 1;

                SumPre += PreW[i];

                list.Add(new LinearConstraint(numberOfVariables: 3)
                {
                    //VariablesAtIndices = new int[] { 0, 1 }, // index 0 (x) and index 1 (y)
                    VariablesAtIndices = new int[] { i, SC + i, 2 * SC + i }, // index 0 (X[0]) and index 1 (X[1])

                    CombinedAs = new double[] { 1, -1, 1 }, // when combined as 1x -1y
                    ShouldBe = ConstraintType.EqualTo,
                    Value = PreW[i]
                });

                list.Add(new LinearConstraint(numberOfVariables: 3)
                {
                    //VariablesAtIndices = new int[] { 0, 1 }, // index 0 (x) and index 1 (y)
                    VariablesAtIndices = new int[] { i, 3*SC + i, 4 * SC + i }, // index 0 (X[0]) and index 1 (X[1])

                    CombinedAs = new double[] { 1, 1, -1 }, 
                    ShouldBe = ConstraintType.EqualTo,
                    Value = PreW[i]
                });
            }

            Indices2[3 * SC] = 5 * SC; Indices2[3 * SC + 1] = 5 * SC + 1;
            Indices3[3 * SC] = 5 * SC; Indices3[3 * SC + 1] = 5 * SC + 1;

            Coef2[3 * SC] = -2; Coef2[3 * SC + 1] = -2;
            Coef3[3 * SC] = -2; Coef3[3 * SC + 1] = -2;

            list.Add(new LinearConstraint(numberOfVariables: SC)
            {                
                VariablesAtIndices = Indices1, // index 0 (X[0]) and index 1 (X[1])
                CombinedAs = Coef1, // when combined as 1x -1y
                ShouldBe = ConstraintType.EqualTo,
                Value = 1
            });


            list.Add(new LinearConstraint(numberOfVariables: 3 * SC + 2)
            {
                VariablesAtIndices = Indices2, // index 0 (X[0]) and index 1 (X[1])
                CombinedAs = Coef2, // when combined as 1x -1y
                ShouldBe = ConstraintType.EqualTo,
                Value = SumPre
            });


            list.Add(new LinearConstraint(numberOfVariables: 3 * SC + 2)
            {
                VariablesAtIndices = Indices3, // index 0 (X[0]) and index 1 (X[1])
                CombinedAs = Coef3, // when combined as 1x -1y
                ShouldBe = ConstraintType.EqualTo,
                Value = -SumPre
            });

            List<QuadraticConstraint> kk = new List<QuadraticConstraint>();
            kk.Add(new QuadraticConstraint(function,){

            }

            // Create objective function
            function = new QuadraticObjectiveFunction();



            return ResW;
        }
        */
        /*
        public double[] Run_Opt_FuncLib()
        {
            int SC = mySymbols.Count + 1;
            Variable[] X = new Variable[SC];
            Variable B = new Variable("Buy");
            Variable S = new Variable("Sell");

            Function f = - B * 0.005 - S * 0.01;
            Function expcnt1 = 0;
            Function[] expcnt2 = new Function[SC];

            for (int i = 0; i < SC; i++)
            {
                X[i] = new Variable("X" + i);
                Function tmpf = X[i] * myRet[i];
                f = f + tmpf;

                expcnt1 = expcnt1 + X[i];
                for (int j = 0; j < SC; j++)
                {
                    expcnt2[i] = expcnt2[i] + VarCov[i][j] * X[j];
                }
            }
            
            
            
            Function expcnt22 = 0;
            for (int i = 0; i < SC; i++)
            {
                expcnt22 = expcnt22 + X[i] * expcnt2[i];
            }

            Function expcnt3 = 0;
            for (int i = 0; i < SC; i++)
            {
                expcnt3 = expcnt3 + 0.5 * ((X[i] - PreW[i]) + Function.Abs(X[i] - PreW[i]));
            }
            expcnt3 = expcnt3 - B;

            Function expcnt4 = 0;
            for (int i = 0; i < SC; i++)
            {
                expcnt4 = expcnt4 + 0.5 * ((PreW[i] - X[i]) + Function.Abs(PreW[i] - X[i]));
            }
            expcnt4 = expcnt4 - S;
            
            IpoptOptimizer o = new IpoptOptimizer();
            
            o.Constraints.Add(expcnt1 == 1.0);
            o.Constraints.Add(expcnt22 == ExpVar);
            o.Constraints.Add(expcnt3 == 0.0);
            o.Constraints.Add(expcnt4 == 0.0);

            VariableAssignment[] VarAs = new VariableAssignment[SC];           
            Random r = new Random(1);
            for (int i = 0; i < SC; i++)
            {
                o.Variables.Add(X[i]);
                o.Constraints.Add(X[i] >= 0.0);
                VarAs[i] = new VariableAssignment(X[i], r.NextDouble());        
            }
            // Specify variables and objective functions and add constraints. Derivatives are computed automatically.            
            o.Variables.Add(B);
            o.Variables.Add(S);

            o.ObjectiveFunction = f;
            
            // Prepare the optimizer.
            //PreparedOptimizer po = o.Prepare();

            // Run the prepared optimizer from many different points.

            IOptimizerResult or = o.Run(VarAs);

            double[] ResW = new double[SC];

            for (int i = 0; i < SC; i++)
            {
                ResW[i] = or.OptimalPoint[X[i]];
            }           


            return ResW;
        }
        */
        
        public double[] Run_Opt_Extreme()
        {
            int SC = mySymbols.Count + 1;
            double[] ResW = new double[mySymbols.Count];
            
            NonlinearProgram nlp2 = new NonlinearProgram(SC);

            Func<Vector, double> objectiveFunction = X => -X.DotProduct(myRet) + Weight*(myTC[0] * 0.5 * (Vector.Sum((X - PreW) + Vector.Abs(X - PreW))) + myTC[1] * 0.5 * (Vector.Sum((PreW - X) + Vector.Abs(PreW - X))));
                                    

            nlp2.ObjectiveFunction = objectiveFunction;
            //nlp2.ObjectiveGradient = objectiveGradient;
            
                        
            // Add constraint X[0]+X[1]+...=1
            nlp2.AddNonlinearConstraint(X => X.Sum(), ConstraintType.Equal, 1.0);

            for (int i = 0; i < SC; i++)
            {
                List<double> Coefs = new List<double>();                
                for (int j = 0; j < SC; j++)
                {
                    if (i != j) Coefs.Add(0); else Coefs.Add(1);
                }
                nlp2.AddLinearConstraint(i.ToString(), Coefs, ConstraintType.GreaterThanOrEqual, 0.0);
            }

            double[,] KK = new double[SC, SC];
            for (int w1 = 0; w1 < SC; w1++)
            {
                for (int w2 = 0; w2 < SC; w2++)
                {
                    KK[w1,w2] = VarCov[w1][w2];
                }
            }
            if (Matrix.Determinant(Matrix.Create(KK)) == 0)
            {
                int k = 0;
            }

            Func<Vector, double> ttm = X => { double y = 0; for (int i = 0; i < SC; i++) { y += X[i] * Vector.DotProduct(Vector.Create(VarCov[i]), X); } return y; };

            nlp2.AddNonlinearConstraint(ttm, ConstraintType.LessThanOrEqual, ExpVar);/*,
                (X, y) =>
                {
                    for (int i = 0; i < SC; i++)
                    {
                        double tmpk = 0;
                        tmpk = 2 * VarCov[i][i] * X[i];
                        for (int j = 0; j < SC; j++)
                        {
                            if (i != j)
                            {
                                tmpk += ((VarCov[i][j] + VarCov[j][i]) * X[i]);
                            }
                        };
                        y[i] = tmpk;
                    };
                    return y;
                });
            */
            Vector InitGuess = Vector.Create(SC);
            for (int i = 0; i < SC; i++)
            {
                InitGuess[i] = 0.0;
            }
            
            nlp2.ExtremumType = ExtremumType.Maximum;
            nlp2.InitialGuess = InitGuess;
            Vector solution;
            
            solution = nlp2.Solve();
            ResW = solution.ToArray();
            BestObj = nlp2.OptimalValue;
            
            return ResW;
        }
    }
}
