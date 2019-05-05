using System;
using Google.OrTools;

namespace aiso
{
    class Program
    {
        static void Main(string[] args)
        {
            MagicSquare magicSquare = new MagicSquare();
            // magicSquare.Solve(3);

            Binoxxo binoxxo = new Binoxxo();
            binoxxo.solve();
        }
    }
}