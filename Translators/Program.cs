﻿using Interfaces;
using System;
using Utility;

namespace Translators
{
    class Program
    {
        static IRunnable runnable = null;
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += AppDomain_ProcessExit;
            runnable = new AirlineTranslators();
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
