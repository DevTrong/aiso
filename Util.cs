using System;
using Google.OrTools.ConstraintSolver;

namespace aiso
{
    public class Util
    {
        public static void PrettyPrint(IntVar[, ] board)
        {
            for (int x = 0; x < board.GetLength(0); x++)
            {
                for (int y = 0; y < board.GetLength(1); y++)
                {
                    Console.Write(String.Format("[{0}]", (board[x, y].Value())));
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}