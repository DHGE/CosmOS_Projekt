using System;
using System.Collections.Generic;
using System.IO;

namespace CosmOS_Projekt
{
    internal class Filesystem
    {
        private Dictionary<string, Action<string[]>> commandMap;
        private string path = @"0:\";

        public void InitializeCommands()
        {
            commandMap = new Dictionary<string, Action<string[]>>
            {
                { "free", args => freeCommand() },
                { "type", args => typeCommand() },
                { "ls", args => lsCommand(args.Length > 2 ? args[2] : "") },
                { "dir", args => lsCommand(args.Length > 2 ? args[2] : "") },
                { "cat", args => catCommand(args) },
                { "touch", args => touchCommand(args) },
                { "mkdir", args => mkDirCommand(args) },
                { "rm", args => rmCommand(args) }
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
                if (!Directory.Exists(directoryPath))
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
            // check if user specified a path
            if (!checkArgs(args, 3)) return;

            // check if the given file is valid (white space or not even specified)
            if (!checkString(path)) return;

            path += args[2];

            // check if file exists
            if (!checkFile(path)) return;

            // print out the content of file
            try
            {
                string content = File.ReadAllText(path);
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

            while (input.ToLower() != "w" || input.ToLower() != "r")
            {
                // get the input
                if (input.ToLower() == "r")
                {
                    Console.WriteLine("Enjoy reading...");
                    return;
                }
                else if (input.ToLower() == "w")
                {
                    var text = "";
                    while (text != "quit\n")
                        // append the file with the given text
                        try
                        {
                            File.AppendAllText(path, text);
                            text = Console.ReadLine();
                            text += '\n';
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                        }
                    while(text != "quit\n")
                    // append the file with the given text
                    try
                    {
                        File.AppendAllText(path, text);
                        text = Console.ReadLine();  
                        text += '\n';
                    }
                    catch (Exception e)
                    {
                         Console.WriteLine(e.ToString());
                    }
                    Console.WriteLine("Quit editing");
                    return;
                }
                else
                {
                    Console.WriteLine("Invalid operation, please try one of the above!");
                    input = Console.ReadLine();
                }
            }
        }

        private void touchCommand(string[] args)
        {
            // check if user specified a path
            if (!checkArgs(args, 3)) return;

            // check if the given file is valid (white space or not even specified)
            if (!checkString(path)) return;

            path += args[2];

            try
            {
                // if the file already exists, do nothing
                if (File.Exists(path))
                {
                    Console.WriteLine("File already exists!");
                    return;
                }
                Kernel.fs.CreateFile(path);
                Console.WriteLine("Successfully created file!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void mkDirCommand(string[] args)
        {
            // check if user specified a path
            if (!checkArgs(args, 3)) return;

            // check if the given file is valid (white space or not even specified)
            if (!checkString(path)) return;

            path += args[2];

            // add a \ at the end to make sure its a valid directory
            if (!path.EndsWith('\\')) { path += '\\'; }

            try
            {
                // if the directory already exists, do nothing
                if (Directory.Exists(path))
                {
                    Console.WriteLine("Directory already exists!");
                    return;
                }
                Kernel.fs.CreateDirectory(path);
                Console.WriteLine("Successfully created directory!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void rmCommand(string[] args)
        {
            // Überprüfen, ob genügend Argumente übergeben wurden
            if (!checkArgs(args, 3)) return;

            // Überprüfen, ob das dritte Argument ein gültiger String ist
            if (!checkString(args[2])) return;

            string operation = args[2].ToLower();

            // Wechsel zu switch-case für besseren Stil
            switch (operation)
            {
                case "f":
                    try
                    {
                        Console.WriteLine("Which file do you want to delete?");
                        var input = Console.ReadLine();

                        // Überprüfen, ob die Eingabe gültig ist
                        if (!checkString(input)) return;

                        string filePath = Path.Combine(path, input);

                        // Überprüfen, ob die Datei existiert
                        if (!checkFile(filePath))
                        {
                            Console.WriteLine("File does not exist.");
                            return;
                        }

                        File.Delete(filePath);
                        Console.WriteLine("Successfully deleted file!");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error while deleting file: {e.Message}");
                    }
                    break;

                case "d":
                    try
                    {
                        Console.WriteLine("Which directory do you want to delete?");
                        var input = Console.ReadLine();

                        // Überprüfen, ob die Eingabe gültig ist
                        if (!checkString(input)) return;

                        string dirPath = Path.Combine(path, input);

                        // Sicherstellen, dass der Pfad mit einem '\' endet, falls nicht vorhanden
                        if (!dirPath.EndsWith('\\')) dirPath += '\\';

                        // Überprüfen, ob das Verzeichnis existiert
                        if (!checkDir(dirPath))
                        {
                            Console.WriteLine("Directory does not exist.");
                            return;
                        }

                        Directory.Delete(dirPath);
                        Console.WriteLine("Successfully deleted directory!");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error while deleting directory: {e.Message}");
                    }
                    break;

                default:
                    Console.WriteLine("Invalid operation. Please try one of the following:\n" +
                                      "f - for deleting a file\n" +
                                      "d - for deleting a directory");
                    break;
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
            if (File.Exists(path)) return true;
            Console.WriteLine("File does not exist!");
            return false;
        }

        bool checkDir(string path)
        {
            if (Directory.Exists(path)) return true;
            Console.WriteLine("Directory does not exist!");
            return false;
        }

        bool checkArgs(string[] args, int index)
        {
            if (args.Length >= index) return true;
            Console.WriteLine("Missing arguments");
            return false;
        }
    }
}