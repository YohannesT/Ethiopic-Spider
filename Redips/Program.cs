using System;
using Redips.Crawler;

namespace Redips
{
    class Program
    {
        static void Main(string[] args)
        {       
            Init(args);

            Console.WriteLine("Waiting for results...");
            var key = new ConsoleKeyInfo();
            while (key.Key != ConsoleKey.Q)
                key = Console.ReadKey();
        }

        private static void Init(string[] args)
        {
            var spiderCommand = new SpiderCommand();

            if (args.Length >= 1)
            {
                spiderCommand.StartCrawling(args);
            }
        }
    }
}
