using Interfaces;
using System;
using Utility;

namespace ContentFilters
{
    class Program
    {
        static IRunnable runnable = null;
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += AppDomain_ProcessExit;
            runnable = new ContentFilter();
            runnable.Run();
        }

        private static void AppDomain_ProcessExit(object sender, EventArgs e)
        {
            runnable.Dispose();
            ChannelFactory.Dispose();
            Console.WriteLine("The process has ended.");
        }
    }
}
