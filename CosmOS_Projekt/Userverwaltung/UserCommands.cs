using System;
using System.Collections.Generic;

namespace CosmOS_Projekt.Userverwaltung
{
    internal class UserCommands
    {
        private Dictionary<string, Action<string[]>> commandMap;

        public UserCommands()
        {
            InitializeCommands();
        }

        public static char[] forbiddenpwchars = new char[] { 'ü', 'Ü', 'ä', 'Ä', 'ö', 'Ö', ' ' };

        public void InitializeCommands()
        {
            commandMap = new Dictionary<string, Action<string[]>>
            {
                { "help", args => helpCommand() },
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

        private void helpCommand()
        {
            Console.WriteLine(
                    "User Commands:\n" +
                    "To access the specific commands for users use the following format\n" +
                    "user [OPTIONS]..\n" +
                    "[OPTIONS] - specifies the specific commands\n" +
                    "create - creates a new user");
        }

        private void createCommand(string[] args)
        {
            string username = PromptForInput("Enter Username: ");
            if (username == null) return;

            string vorname = PromptForInput("Enter Vorname:");
            string nachname = PromptForInput("Enter Nachname:");

            string password = PromptForPassword();
            if (password == null) return;

            try
            {
                User usr = new User(username, vorname, nachname, password);
                Console.WriteLine("User successfully created.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private string PromptForPassword()
        {
            string password;
            while (true)
            {
                Console.Write("Enter Password: ");
                password = Console.ReadLine();

                if (!isvalidpw(password))
                {
                    continue;
                }

                Console.Write("Enter Password again: ");
                string passwordretype = Console.ReadLine();

                if (password != passwordretype)
                {
                    Console.WriteLine("Passwords do not match, please try again.");
                    continue;
                }

                break;
            }

            return password;
        }

        private string PromptForInput(string promptMessage)
        {
            Console.Write(promptMessage);
            return Console.ReadLine();
        }

        public bool isvalidpw(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                Console.WriteLine("Password is too short - min. 6 characters");
                return false;
            }

            foreach (char c in forbiddenpwchars)
            {
                if (password.Contains(c))
                {
                    Console.WriteLine($"Invalid character '{c}' in password.");
                    return false;
                }
            }

            return true;
        }
    }
}