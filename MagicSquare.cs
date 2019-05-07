using System;
using System.Collections.Generic;
using System.Linq;
using Google.OrTools;
using Google.OrTools.ConstraintSolver;

namespace aiso
{
	public class MagicSquare
	{
		public void Solve(int size = 3)
		{
			Solver solver = new Solver("MagicSquare");
			IntVar[, ] board = solver.MakeIntVarMatrix(size, size, 1, size * size);
			IntVarVector flatten = board.Flatten();

			IEnumerable<int> range = Enumerable.Range(0, size);

			// constraints
			solver.Add(solver.MakeAllDifferent(flatten));

			IntExpr targetExpression = (from x in range select(board[x, 0])).ToArray().Sum();
			foreach (int x in range)
			{
				IntExpr rowSum = (from y in range select board[x, y]).ToArray().Sum();
				IntExpr columnSum = (from y in range select board[y, x]).ToArray().Sum();
				solver.Add(targetExpression == rowSum);
				solver.Add(targetExpression == columnSum);
			}

			solver.Add(
				(from x in range select board[x, x]).ToArray().Sum() == targetExpression
			);
			solver.Add(
				(from x in range select board[size - 1 - x, x]).ToArray().Sum() == targetExpression
			);

			DecisionBuilder db = solver.MakePhase(flatten, Solver.INT_VAR_SIMPLE, Solver.INT_VALUE_SIMPLE);
			solver.NewSearch(db);
			while (solver.NextSolution())
			{
				Util.PrettyPrint(board);
			}

			solver.EndSearch();

			Console.WriteLine(solver.Solutions());
		}
	}
}