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
        private Commands command;

        protected override void BeforeRun()
        {
            Console.Clear();

            users = new List<User>();

            fs = new Cosmos.System.FileSystem.CosmosVFS();
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);

            Console.WriteLine("Cosmos booted successfully.");
            momentOfStart = DateTime.Now;
            command = new Commands();
        }

        protected override void Run()
        {
            checkUsers();
            while(currentUser == null)
            {
                currentUser = UserControls.LoginUser();
            }

            Console.Write($"{Commands.currentDirectory}> "); // Zeigt immer den aktuellen Pfad an

            var input = Console.ReadLine() ?? " ";
            string[] args = input.Split(' ');

            command.commands(args);
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
            int rootCnt = users.Count(user => user.Permission == 2);
            if (rootCnt == 0)
            {
                User root = new User();
                root.Username = "root";
                root.Vorname = "root";
                root.Nachname = "root";
                Console.WriteLine("Please set a password for the root account");
                string password = UserControls.PromptForPassword();
                root.Password = password;
                password = UserControls.GenerateHash(password);
                string usrString = $"\n{root.Username}:{root.Vorname}:{root.Nachname}:{password}:{2}";
                File.AppendAllText(@"0:\Config\config.txt", usrString);
            }
        }
    }
}