using System;
using System.Collections.Generic;
using System.IO;

namespace CosmOS_Projekt
{
    internal class Filesystem
    {

        private string path = Commands.currentDirectory;

        public Filesystem()
        {
            InitializeCommands();
        }

        private Dictionary<string, Action<string[]>> commandMap;

        // forbidden chars for files
        public char[] forbiddenchars = new char[]{
            '$',
            '%',
            '_',
        };

        public void InitializeCommands()
        {
            commandMap = new Dictionary<string, Action<string[]>>
            {
                { "help", args => helpCommand() },
                { "free", args => freeCommand() },
                { "type", args => typeCommand() },
                { "ls", args => lsCommand(args) },
                { "dir", args => lsCommand(args) },
                { "cat", args => catCommand(args) },
                { "touch", args => touchCommand(args) },
                { "mkdir", args => mkDirCommand(args) },
                { "rm", args => rmCommand(args) },
                { "mv", args => mvCommand(args) }
            };
        }
        public void fileCommands(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Missing arguments, try \"file help\" for a quick view of all file commands!");
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

        private void helpCommand()
        {
            Console.WriteLine(
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
                    "rm f || rm d - deletes given file or directory.\n" +
                    "mv [file] [dirToMove] - moves a file to another dir\n");
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

        private void lsCommand(string[] args)
        {

            // check if user specified a path
            // if a path is specified, use it, otherwise dont use it
            if (args.Length == 3)
            {
                path += args[2];
            }

            // check if the given file is valid (white space or not even specified)
            if (!checkString(path)) return;

            try
            {
                if (!Directory.Exists(path))
                {
                    Console.WriteLine("Error: The specified path does not exist.");
                    return;
                }

                var filesList = Directory.GetFiles(path);
                var directoriesList = Directory.GetDirectories(path);

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
                        File.WriteAllText(path, text);
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

                foreach(char c in forbiddenchars)
                {
                    if (path.Contains(c))
                    {
                        Console.WriteLine("Filename contains invalid character!");
                        return;
                    }
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

                        path += input;

                        // Überprüfen, ob die Datei existiert
                        if (!checkFile(path)) return;

                        File.Delete(path);
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

                        path += input;

                        // Sicherstellen, dass der Pfad mit einem '\' endet, falls nicht vorhanden
                        if (!path.EndsWith('\\')) path += '\\';

                        // Überprüfen, ob das Verzeichnis existiert
                        if (!checkDir(path)) return;

                        Directory.Delete(path);
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
        
        private void mvCommand(string[] args)
        {
            // Überprüfen, ob genügend Argumente übergeben wurden
            // file mv [filename/path] [pathToMove]
            if (!checkArgs(args, 4)) return;

            // Überprüfen, ob das dritte und vierte Argument gültige Strings sind
            if (!checkString(args[2]) || !checkString(args[3])) return;

            // Extrahiere den Dateinamen aus args[2]
            string sourcePath = Path.Combine(path, args[2]);
            string fileName = Path.GetFileName(args[2]);
            string destinationPath = Path.Combine(@"0:\", args[3]);

            try
            {
                // Überprüfe, ob die Quelldatei und das Zielverzeichnis existieren
                if (checkFile(sourcePath) && checkDir(destinationPath))
                {
                    // Überprüfen, ob die Datei bereits im Zielverzeichnis existiert
                    if (File.Exists(destinationPath + "\\" + fileName))
                    {
                        Console.WriteLine("File already exists in the specified path. Do you want to replace it? (y/n)");
                        string response = Console.ReadLine().ToLower();

                        if (response != "y")
                        {
                            Console.WriteLine("File move operation cancelled.");
                            return;
                        }
                    }

                    // Kopiere die Datei ins Zielverzeichnis und lösche die Originaldatei
                    File.Copy(sourcePath, destinationPath, true);
                    File.Delete(sourcePath);

                    Console.WriteLine("File moved successfully!");
                }
                else
                {
                    Console.WriteLine("Source file or destination path not found.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
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