using System;
using Sys = Cosmos.System;
using CosmOS_Projekt.Userverwaltung;
using System.Collections.Generic;
using System.IO;

namespace CosmOS_Projekt
{
    public class Kernel : Sys.Kernel
    {
        public static DateTime momentOfStart;
        public static Sys.FileSystem.CosmosVFS fs;  // Static variable for filesystem
        private List<User> users;
        User currentUser = null;

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
            Console.WriteLine("Please log in!\n");
            while(currentUser == null)
            {
                currentUser = UserControls.LoginUser();
            }
            
            Console.WriteLine("Enter your command:");

            var input = Console.ReadLine();
            string[] args = input.Split(' ');

            if (args.Length < 1) return;

            Commands command = new Commands();
            command.commands(args);
        }

        private void checkUsers()
        {
            if (!File.Exists(@"0:\Config\config.txt")) fs.CreateFile(@"0:\Config\config.txt");

            try
            {
                users = UserControls.getAllUsers();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return; }

            Console.WriteLine(users);

            int adminCnt = 0;
            foreach (User user in users)
            {
                if (user.Permission == 1) adminCnt++;
            }
            if (adminCnt == 0)
            {
                Console.WriteLine("There is no Admin account!\nPlease create one now!\n");
                UserCommands.createCommand(1);
            }
        }
    }
}