using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys = Cosmos.System;

namespace CosmOS_Projekt
{
    internal class Commands
    {
        private string currentVersion = "Version 1.0";
        
        Memory mem = new Memory();
        Filesystem filesys = new Filesystem();
        public void commands(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Try \"help\" for a quick view of all commands!");
                return;
            }

            // The first argument is the command, the rest are command-specific arguments.
            string command = args[0].ToLower();

            switch (command)
            {
                case "help":
                    helpCommand();
                    break;
                case "runtime":
                    runtimeCommand();
                    break;
                case "version":
                    Console.WriteLine(currentVersion);
                    break;
                case "shutdown":
                case "exit":
                    shutdownCommand();
                    break;
                case "echo":
                    echoCommand(args);
                    break;
                case "tempsave":
                    tempsaveCommand();
                    break;
                case "file":
                    filesys.fileCommands(args);
                    break;
                default:
                    Console.WriteLine("Unknown command, try \"help\" for a quick view of all commands!");
                    break;
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
                              "type - outputs the type of the system\n" +
                              "fs - list of all files in directory\n");
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
            // Print all arguments after "echo" as a single line.
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
            mem.writeAt(0, 4);
            Console.WriteLine(mem.readAt(0).ToString());
        }
    }
}
