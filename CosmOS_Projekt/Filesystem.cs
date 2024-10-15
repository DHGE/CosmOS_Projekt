using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmOS_Projekt
{
    internal class Filesystem
    {
        public void fileCommands(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Missing arguments, try \"help\" for a quick view of all commands!");
            }

            string command = args[1].ToLower();

            switch (command)
            {
                case "free":
                    freeCommand();
                    break;
                case "type":
                    typeCommand();
                    break;
                case "fs":
                    fsCommand();
                    break;
                case "dirList":
                    dirListing();
                    break;
                case "vi":
                    readFile(args[2]);
                    break;
                default:
                    Console.WriteLine("Unknown command, try \"help\" for a quick view of all commands!");
                    break;
            }
        }

        private void freeCommand()
        {
            var available_space = Kernel.fs.GetAvailableFreeSpace(@"0:\");
            Console.WriteLine("Available Free Space: " + available_space);
        }

        private void typeCommand()
        {
            var fs_type = Kernel.fs.GetFileSystemType(@"0:\");
            Console.WriteLine("File System Type: " + fs_type);
        }

        private void fsCommand()
        {
            var files_list = Directory.GetFiles(@"0:\");
            foreach (var file in files_list)
            {
                Console.WriteLine(file);
            }
        }

        private void dirListing()
        {
            var files_list = Directory.GetFiles(@"0:\");
            var directory_list = Directory.GetDirectories(@"0:\");

            foreach (var file in files_list)
            {
                Console.WriteLine(file);
            }
            foreach (var directory in directory_list)
            {
                Console.WriteLine(directory);
            }
        }


        //TODO crash beheben bei File-Angabe
        private void readFile(string dir)
        {
            var files_list = Directory.GetFiles(@dir);
            try
            {
                foreach (var file in files_list)
                {
                    var content = File.ReadAllText(file);

                    Console.WriteLine("File name: " + file);
                    Console.WriteLine("File size: " + content.Length);
                    Console.WriteLine("Content: " + content);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

    }
}
