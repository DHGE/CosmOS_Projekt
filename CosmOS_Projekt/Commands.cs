using System;
using System.Collections.Generic;
using System.Linq;
using Sys = Cosmos.System;
using CosmOS_Projekt.Userverwaltung;
using System.IO;

namespace CosmOS_Projekt
{
    internal class Commands
    {
        private string currentVersion = "Version 1.2";
        public static string currentDirectory = @"0:\"; // Startverzeichnis
        private Dictionary<string, Action<string[]>> commandMap;
        private Filesystem filesys;
        UserCommands userCommands;

        public Commands()
        {
            filesys = new Filesystem();
            userCommands = new UserCommands();
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
                { "cd", args => cdCommand(args) },
                { "file", args => InitializeFilesystem(args) },
                { "user", args => InitializeUsers(args) },
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
                    Console.Write($"{currentDirectory}> "); // Zeigt immer den aktuellen Pfad an
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

        private void cdCommand(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: cd [directory]");
                return;
            }

            string target = args[1];

            if (target == "..")
            {
                if (currentDirectory != @"0:\")
                {
                    currentDirectory = Directory.GetParent(currentDirectory)?.FullName ?? currentDirectory;
                }
            }
            else
            {
                if (!target.EndsWith("\\"))
                {
                    target += "\\";
                }

                string newPath = Path.Combine(currentDirectory, target);

                if (Directory.Exists(newPath))
                {
                    currentDirectory = newPath;
                }
                else
                {
                    Console.WriteLine($"Directory '{target}' does not exist.");
                }
            }
        }

        private void helpCommand()
        {
            Console.WriteLine("This project has a variety of commands, see below to get a quick understanding of their abilities:\n\n" +
                              "Basic Commands:\n" +
                              "runtime - outputs the runtime of this shell\n" +
                              "version - outputs the current version of the system\n" +
                              "shutdown || exit - shuts down the system\n" +
                              "echo - outputs the given text\n\n" +
                              "cd [directory] - changes to specified directory\n" +
                              "cd .. - moves one directory up\n\n" +
                              "Filesystem Commands:\n" +
                              "file help - list all filesystem commands\n\n" +
                              "User Commands:\n" +
                              "user help - list all user specific commands\n");
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
                Console.WriteLine(); // Leere Zeile, falls kein Text angegeben
            }
        }

        private void InitializeFilesystem(string[] args)
        {
            filesys.fileCommands(args);
        }

        private void InitializeUsers(string[] args)
        { 
            userCommands.userCommands(args);
        }
    }
}
