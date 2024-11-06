using Cosmos.System.Network.IPv4.TCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CosmOS_Projekt.Userverwaltung
{
    internal class UserCommands
    {

        public UserCommands() 
        { 
            InitializeCommands();
        }

        private Dictionary<string, Action<string[]>> commandMap;

        public void InitializeCommands()
        {
            commandMap = new Dictionary<string, Action<string[]>>
            {
                { "create", args => createCommand(args) }
            };
        }
        public void userCommands(string[] args)
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

        private void createCommand(string[] args)
        {
            Console.Write("Enter Username:");
            string username = Console.ReadLine();

            Console.Write("Enter Vorname:");
            string vorname = Console.ReadLine();

            Console.Write("Enter Nachname:");
            string nachname = Console.ReadLine();

            Console.Write("Enter Password:");
            string password = Console.ReadLine();

            try
            {
                User usr = new User(username, vorname, nachname, password);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
