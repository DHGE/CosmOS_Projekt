using System;
using System.Collections.Generic;
using System.IO;

namespace CosmOS_Projekt
{
    internal class Filesystem
    {
        private Dictionary<string, Action<string[]>> commandMap;

        public void InitializeCommands()
        {
            commandMap = new Dictionary<string, Action<string[]>>
            {
                { "free", args => freeCommand() },
                { "type", args => typeCommand() },
                { "ls", args => lsCommand(args.Length > 2 ? args[2] : "") },
                { "cat", args => catCommand(args) } 
            };
        }
        public void fileCommands(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Missing arguments, try \"help\" for a quick view of all commands!");
                return;
            }

            string command = args[1].ToLower();

            try
            {
                if (commandMap.TryGetValue(command, out var commandAction))
                {
                    commandAction(args);
                }
                else
                {
                    Console.WriteLine("Unknown command, try \"help\" for a quick view of all commands!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing command '{command}': {ex.Message}");
            }
        }

        private void freeCommand()
        {
            try
            {
                if (Kernel.fs == null)
                {
                    throw new InvalidOperationException("Filesystem is not initialized.");
                }

                var available_space = Kernel.fs.GetAvailableFreeSpace(@"0:\");
                Console.WriteLine("Available Free Space: " + available_space);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving free space: " + ex.Message);
            }
        }

        private void typeCommand()
        {
            try
            {
                var fs_type = Kernel.fs.GetFileSystemType(@"0:\");
                Console.WriteLine("File System Type: " + fs_type);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving file system type: " + ex.Message);
            }
        }

        private void lsCommand(string path)
        {
            string directoryPath = string.IsNullOrEmpty(path) ? @"0:\" : @"0:\" + path;

            try
            {
                if (!Directory.Exists(directoryPath))//test lol
                {
                    Console.WriteLine("Error: The specified path does not exist.");
                    return;
                }

                var filesList = Directory.GetFiles(directoryPath);
                var directoriesList = Directory.GetDirectories(directoryPath);

                if (filesList.Length == 0 && directoriesList.Length == 0)
                {
                    Console.WriteLine("The directory is empty.");
                }
                else
                {
                    foreach (var directory in directoriesList)
                    {
                        Console.WriteLine("DIR: " + directory);
                    }

                    foreach (var file in filesList)
                    {
                        Console.WriteLine("FILE: " + file);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while listing files: " + ex.Message);
            }
        }

        private void catCommand(string[] args)
        {
            // check if user specified a file
            if(!checkArgs(args, 3)) return;

            var path = args[2];

            // check if the given file is valid (white space or not even specified)
            if (!checkString(path)) return;
            // check if file exists
            if (!checkFile(path)) return;

            string fullPath = @"0:\" + path;

            // print out the content of file
            try
            {
                string content = File.ReadAllText(fullPath);
                Console.WriteLine("File size: " + content.Length);
                Console.WriteLine("Content: " + content);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading file: " + e.Message);
            }

            // check what the user wants to do
            Console.WriteLine("\nWhat do you want to do?\n" +
                              "Write - w | Read - r");
            var input = Console.ReadLine();

            // get the input
            if (input.ToLower() == "r") return;
            else if (input.ToLower() != "w" || input.ToLower() != "r")
            {
                Console.WriteLine("Invalid operation, please try one of the above!");
                return;
            }
            else
            {
                var text = "\n";
                while(text != "quit\n")
                {
                    // append the file with the given text
                    try
                    {
                        File.AppendAllText(fullPath, text);
                        text = Console.ReadLine();  
                        text += '\n';
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
                Console.WriteLine("Quit editing");
            }
        }

        bool checkString(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Console.WriteLine("Error: No file path specified.");
                return false;
            }
            return true;
        }
        
        bool checkFile(string path)
        {
            var filePath = @"0:\" + path;
            if (File.Exists(filePath)) return true;
            Console.WriteLine("File does not exist!");
            return false;
        }

        bool checkDir(string path)
        {
            var filePath = @"0:\" + path;
            if (Directory.Exists(filePath)) return true;
            Console.WriteLine("Directory does not exist!");
            return false;
        }

        bool checkArgs(string[] args, int index)
        {
            if (args.Length == index) return true;
            Console.WriteLine("Missing arguments");
            return false;
        }
    }
}
