using System;
using Sys = Cosmos.System;

namespace CosmOS_Projekt
{
    public class Kernel : Sys.Kernel
    {
        public static DateTime momentOfStart;
        public static Sys.FileSystem.CosmosVFS fs;  // Static variable for filesystem

        protected override void BeforeRun()
        {
            fs = new Cosmos.System.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);

            Console.WriteLine("Cosmos booted successfully. Type a line of text to get it echoed back.");
            momentOfStart = DateTime.Now;
        }

        protected override void Run()
        {
            var input = Console.ReadLine();
            string[] args = input.Split(' ');

            if (args.Length < 1) return;

            Commands command = new Commands();
            command.commands(args);
        }
    }
}