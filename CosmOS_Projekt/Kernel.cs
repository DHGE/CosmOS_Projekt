using System;
using Sys = Cosmos.System;
using CosmOS_Projekt.Userverwaltung;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace CosmOS_Projekt
{
    public class Kernel : Sys.Kernel
    {
        public static DateTime momentOfStart;
        public static Sys.FileSystem.CosmosVFS fs;  // Static variable for filesystem
        public static List<User> users;
        public static User currentUser = null;

        protected override void BeforeRun()
        {
            Console.Clear();

            users = new List<User>();

            fs = new Cosmos.System.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);

            Console.WriteLine("Cosmos booted successfully.");
            momentOfStart = DateTime.Now;
        }

        protected override void Run()
        {
            checkUsers();
            while(currentUser == null)
            {
                currentUser = UserControls.LoginUser();
            }

            Console.Write($"{Commands.currentDirectory}> "); // Zeigt immer den aktuellen Pfad an

            var input = Console.ReadLine();
            string[] args = input.Split(' ');

            if (args.Length < 1) return;

            Commands command = new Commands();
            command.commands(args);
            ConsoleKeyInfo key = Console.ReadKey();
            if ((key.Modifiers & ConsoleModifiers.Control) != 0 && key.Key == ConsoleKey.C)
            {
                return;
            }
        }

        private void checkUsers()
        {
            if (!File.Exists(@"0:\Config\config.txt"))
            {
                fs.CreateFile(@"0:\Config\config.txt");
            }

            try
            {
                users = UserControls.getAllUsers();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading users: {ex.Message}");
                return;
            }

            // Count admins in the users list
            int adminCnt = users.Count(user => user.Permission == 1);

            if (adminCnt == 0)
            {
                Console.WriteLine("There is no Admin account!\nPlease create one now!\n");
                UserCommands.createCommand(1);  // Prompt to create an admin
            }
        }
    }
}