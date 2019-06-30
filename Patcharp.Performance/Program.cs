using System;

namespace Patcharp.Performance
{
    class Program
    {
        static void Main(string[] args)
        {
            PerformanceBenchmarks.Run(new Patcharp());

            Console.ReadLine();
        }
    }
}
