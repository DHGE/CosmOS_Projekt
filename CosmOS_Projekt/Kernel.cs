using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;

namespace CosmOS_Projekt
{
    public class Kernel : Sys.Kernel
    {

        string versionString = "Version 1.0";
        DateTime momentOfStart;

        protected override void BeforeRun()
        {
            Console.WriteLine("Cosmos booted successfully. Type a line of text to get it echoed back.");
            momentOfStart = DateTime.Now;
        }

        protected override void Run()
        {
            var input = Console.ReadLine();

            string[] args = input.Split(' ');

            

        }

        public void commands(string[] args)
        {

        }

    }
}
