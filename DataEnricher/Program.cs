using Interfaces;
using System;
using System.Threading.Tasks;
using Utility;

namespace DataEnricher
{
    class Program
    {
        static IRunnableAsync runnable = null;
        static async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += AppDomain_ProcessExit;
            runnable = new WeatherEnricher();
            await runnable.RunAsync();
            await new WeatherEnricher().RunAsync();
        }
        private static void AppDomain_ProcessExit(object sender, EventArgs e)
        {
            runnable.Dispose();
            ChannelFactory.Dispose();
            Console.WriteLine("The process has ended.");
        }
    }
}
