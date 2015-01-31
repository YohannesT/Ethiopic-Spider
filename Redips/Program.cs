using System;
using Spider.Crowler;
using Spider.Redips;

namespace Spider
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

            if (args.Length == 1)
            {
                spiderCommand.StartCrawling(args);
            }
            else if (args.Length == 2)
            {

                spiderCommand.StartCrawling(args);   
            }
            else if (args.Length > 2)
            {
                spiderCommand.StartCrawling(args);
            }
        }
    }
}
