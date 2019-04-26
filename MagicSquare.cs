using System.Collections.Generic;
using Google.OrTools;
using Google.OrTools.ConstraintSolver;

namespace aiso
{
	public class MagicSquare
	{
		public void Solve(int size = 3)
		{
			Solver solver = new Solver("MagicSquare");
			IntVar[, ] board = new IntVar[size, size];

			// create board
			for (int x = 0; x < size; x++)
			{
				for (int y = 0; y < size; y++)
				{
					board[x, y] = solver.MakeIntVar(1, size * size);
				}
			}

			IntVarVector flatten = board.Flatten();

			// constraints
			solver.Add(solver.MakeAllDifferent(flatten));

			Constraint c = new Constraint(solver);

			IntExpr expression = null;
			for (int x = 0; x < size; x++)
			{
				IntExpr rowSums = null;
				IntExpr columnSums = null;
				for (int y = 0; y < size; y++)
				{
					rowSums = rowSums == null ? board[x, y] : rowSums + board[x, y];
					columnSums = columnSums == null ? board[y, x] : columnSums + board[y, x];
				}

				if (expression == null)
				{
					expression = rowSums;
				}
				else
				{
					solver.Add(expression == rowSums);
					solver.Add(expression == columnSums);
				}
			}

			DecisionBuilder db = solver.MakePhase(flatten, Solver.INT_VAR_SIMPLE, Solver.INT_VALUE_SIMPLE);
			solver.NewSearch(db);
			while (solver.NextSolution())
			{
				Util.PrettyPrint(board);
			}

			solver.EndSearch();
		}
	}
}