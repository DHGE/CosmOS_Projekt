using System;
using System.Collections.Generic;
using System.Linq;
using Sys = Cosmos.System;

namespace CosmOS_Projekt
{
    internal class Commands
    {
        private string currentVersion = "Version 1.0";
        private Dictionary<string, Action<string[]>> commandMap;

        public Commands()
        {
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            commandMap = new Dictionary<string, Action<string[]>>
            {
                { "help", args => helpCommand() },
                { "runtime", args => runtimeCommand() },
                { "version", args => Console.WriteLine(currentVersion) },
                { "shutdown", args => shutdownCommand() },
                { "exit", args => shutdownCommand() },
                { "echo", args => echoCommand(args) },
                { "tempsave", args => tempsaveCommand() },
                { "file", args => InitializeFilesystem(args) }
            };
        }

        public void commands(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Try \"help\" for a quick view of all commands!");
                return;
            }

            string command = args[0].ToLower();

            if (commandMap.ContainsKey(command))
            {
                try
                {
                    commandMap[command](args);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error executing command '{command}': {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Unknown command, try \"help\" for a quick view of all commands!");
            }
        }

        private void helpCommand()
        {
            Console.WriteLine("This project has a variety of commands, see below to get a quick understanding of their abilities:\n\n" +
                              "Basic Commands:\n" +
                              "runtime - outputs the runtime of this shell\n" +
                              "version - outputs the current version of the system\n" +
                              "shutdown || exit - shuts down the system\n" +
                              "echo - outputs the given text\n" +
                              "tempsave - idk\n\n" +
                              "Filesystem Commands:\n" +
                              "To access the specific commands for the filesystem use the following format\n" +
                              "file [OPTIONS]..\n" +
                              "[OPTIONS] - specifies the specific commands\n" +
                              "free - outputs the free memory of the system\n" +
                              "type - outputs the type of the system\n\n" +
                              "ls [PATH] - list of all files in directory\n" +
                              "dir [PATH] - list of all files in directory\n" +
                              "If no Path is specified, it returns all file inside the 0:\\ DOS drive\n\n" +
                              "cat [PATH] - reads content from a given file.\n" +
                              "Is also used to write into a given file.\n" +
                              "There is no need to specify a drive since CosmOS uses the DOS drive naming system.\n\n" +
                              "touch [PATH] - creates a new file inside the given path.\n" +
                              "mkdir [PATH] - creates a new directory.\n" +
                              "rm f || rm d - deletes given file or directory.\n");
        }

        private void runtimeCommand()
        {
            TimeSpan span = DateTime.Now - Kernel.momentOfStart;
            Console.WriteLine(span);
        }

        private void shutdownCommand()
        {
            Sys.Power.Shutdown();
        }

        private void echoCommand(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine(string.Join(" ", args.Skip(1)));
            }
            else
            {
                Console.WriteLine(); // If no additional text, just print an empty line.
            }
        }

        private void tempsaveCommand()
        {
            // Assuming Memory is properly initialized and accessible
            Memory mem = new Memory();
            mem.writeAt(0, 4);
            Console.WriteLine(mem.readAt(0).ToString());
        }

        private void InitializeFilesystem(string[] args)
        {
            Filesystem filesys = new Filesystem();
            filesys.InitializeCommands();
            filesys.fileCommands(args);
        }
    }
}
