using System;
using System.Collections.Generic;
using System.Linq;
using Google.OrTools.ConstraintSolver;

namespace aiso
{
    public class Binoxxo
    {
        private string[, ] emptyGame = { { "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "", "", "" }
        };

        private string[, ] game1 = { { "", "1", "", "", "", "", "", "", "", "" },
            { "", "", "", "0", "", "", "", "", "", "" },
            { "", "1", "1", "", "", "", "", "", "", "" },
            { "", "", "", "", "0", "0", "", "", "", "0" },
            { "1", "", "", "", "", "", "1", "1", "", "" },
            { "", "1", "", "", "1", "", "", "", "", "" },
            { "", "", "", "0", "", "", "1", "", "", "" },
            { "", "0", "", "", "", "", "", "0", "", "0" },
            { "", "", "", "", "0", "", "", "", "", "" },
            { "0", "", "", "", "", "", "", "", "", "0" }
        };

        private string[, ] game2 = { { "", "", "0", "0", "", "", "", "", "", "" },
            { "", "", "", "0", "", "", "", "", "", "" },
            { "0", "", "", "", "1", "", "", "1", "", "" },
            { "", "", "", "", "", "0", "", "", "", "" },
            { "0", "", "", "0", "", "", "", "", "", "" },
            { "", "", "", "0", "", "", "", "", "1", "" },
            { "", "", "", "", "", "", "", "1", "1", "" },
            { "1", "", "", "", "", "", "", "1", "", "" },
            { "", "", "", "", "", "0", "", "", "", "0" },
            { "", "", "", "", "1", "", "0", "", "", "" }
        };

        private string[, ] game3 = { { "1", "", "", "", "", "", "0", "0", "", "" },
            { "", "", "", "0", "", "", "", "", "1", "" },
            { "", "", "0", "", "", "", "", "", "", "" },
            { "", "", "", "0", "", "1", "", "", "", "0" },
            { "1", "", "1", "", "", "1", "", "", "1", "" },
            { "", "", "1", "", "1", "", "", "", "", "" },
            { "", "", "", "", "1", "1", "", "", "1", "" },
            { "", "", "", "", "", "", "", "", "", "0" },
            { "", "1", "", "1", "", "", "", "", "", "" },
            { "", "1", "", "", "1", "", "", "1", "1", "" }
        };

        public void solve()
        {
            Solver solver = new Solver("Binoxxo");
            IntVar[, ] board = this.createBoard(solver, this.game3);

            int boardWidth = board.GetLength(0);
            int boardHeight = board.GetLength(1);

            IEnumerable<int> cell = Enumerable.Range(0, boardWidth);
            IEnumerable<int> range = Enumerable.Range(0, boardHeight);

            // ensure that all of them dont have more then 2 X or 0 in a row
            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < board.GetLength(1) - 1; y++)
                {
                    if (y + 2 != boardWidth)
                    {
                        IntExpr columnExpression = board[y, x] + board[y + 1, x] + board[y + 2, x];
                        solver.Add(solver.MakeBetweenCt(columnExpression, 1, 2));
                    }
                    if (y + 2 != boardHeight)
                    {
                        IntExpr rowExpression = board[x, y] + board[x, y + 1] + board[x, y + 2];
                        solver.Add(solver.MakeBetweenCt(rowExpression, 1, 2));
                    }
                }
            }

            foreach (int x in cell)
            {
                // cannot have more x then 0 -> boardSize / 2 == amount of x'es
                solver.Add((from y in range select board[x, y]).ToArray().Sum() == boardWidth / 2);
            }

            // all different rows
            solver.Add(
                solver.MakeAllDifferent(
                    (from x in cell select(
                        (from y in range select board[x, y] * Convert.ToInt64(Math.Pow(10, y + 1)))
                        .ToArray().Sum().Var()
                    ))
                    .ToArray()
                )
            );

            foreach (int y in range)
            {
                solver.Add((from x in range select board[x, y]).ToArray().Sum() == boardHeight / 2);
            }

            // all different columns
            solver.Add(
                solver.MakeAllDifferent(
                    (from y in range select(
                        (from x in cell select board[x, y] * Convert.ToInt64(Math.Pow(10, y + 1)))
                        .ToArray().Sum().Var()
                    ))
                    .ToArray()
                )
            );

            DecisionBuilder db = solver.MakePhase(board.Flatten(), Solver.INT_VAR_SIMPLE, Solver.INT_VALUE_SIMPLE);
            solver.NewSearch(db);
            while (solver.NextSolution())
            {
                Util.PrettyPrint(board);
            }

            solver.EndSearch();
        }

        private IntVar[, ] createBoard(Solver solver, string[, ] game)
        {
            int xSize = game.GetLength(0);
            int ySize = game.GetLength(1);
            IntVar[, ] board = solver.MakeIntVarMatrix(xSize, ySize, 0, 1);

            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    string preValue = game[x, y];
                    if (!string.IsNullOrEmpty(preValue))
                    {
                        int value = Convert.ToInt32(preValue);
                        solver.Add(board[x, y] == value);
                    }
                }
            }
            return board;
        }
    }
}