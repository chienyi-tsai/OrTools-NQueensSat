using System;

using Google.OrTools.Sat;


namespace NQueensSatLib
{
    // [START solution_printer]
    public class SolutionPrinter : CpSolverSolutionCallback
    {
        public SolutionPrinter(IntVar[] queens)
        {
            queens_ = queens;
        }

        public override void OnSolutionCallback()
        {
            Console.WriteLine($"Solution {SolutionCount_}");
            for (int i = 0; i < queens_.Length; ++i)
            {
                for (int j = 0; j < queens_.Length; ++j)
                {
                    if (Value(queens_[j]) == i)
                    {
                        Console.Write("Q");
                    }
                    else
                    {
                        Console.Write("_");
                    }
                    if (j != queens_.Length - 1)
                        Console.Write(" ");
                }
                Console.WriteLine("");
            }
            SolutionCount_++;
        }

        public int SolutionCount()
        {
            return SolutionCount_;
        }

        private int SolutionCount_;
        private IntVar[] queens_;
    }
    // [END solution_printer]

    public static class NQueensSatLibClass
    {
        public static void DoIt()
        {
            // Constraint programming engine
            // [START model]
            CpModel model = new CpModel();
            // [START model]

            // [START variables]
            int BoardSize = 8;
            // There are `BoardSize` number of variables, one for a queen in each
            // column of the board. The value of each variable is the row that the
            // queen is in.
            IntVar[] queens = new IntVar[BoardSize];
            for (int i = 0; i < BoardSize; ++i)
            {
                queens[i] = model.NewIntVar(0, BoardSize - 1, $"x{i}");
            }
            // [END variables]

            // Define constraints.
            // [START constraints]
            // All rows must be different.
            model.AddAllDifferent(queens);

            // No two queens can be on the same diagonal.
            LinearExpr[] diag1 = new LinearExpr[BoardSize];
            LinearExpr[] diag2 = new LinearExpr[BoardSize];
            for (int i = 0; i < BoardSize; ++i)
            {
                diag1[i] = LinearExpr.Affine(queens[i], /*coeff=*/1, /*offset=*/i);
                diag2[i] = LinearExpr.Affine(queens[i], /*coeff=*/1, /*offset=*/-i);
            }

            model.AddAllDifferent(diag1);
            model.AddAllDifferent(diag2);
            // [END constraints]

            // [START solve]
            // Creates a solver and solves the model.
            CpSolver solver = new CpSolver();
            SolutionPrinter cb = new SolutionPrinter(queens);
            // Search for all solutions.
            solver.StringParameters = "enumerate_all_solutions:true";
            // And solve.
            solver.Solve(model, cb);
            // [END solve]

            // [START statistics]
            Console.WriteLine("Statistics");
            Console.WriteLine($"  conflicts : {solver.NumConflicts()}");
            Console.WriteLine($"  branches  : {solver.NumBranches()}");
            Console.WriteLine($"  wall time : {solver.WallTime()} s");
            Console.WriteLine($"  number of solutions found: {cb.SolutionCount()}");
            // [END statistics]
        }


    }
}
