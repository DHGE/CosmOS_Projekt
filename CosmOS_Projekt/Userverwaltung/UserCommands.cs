using System;
using System.Collections.Generic;
using System.IO;

namespace CosmOS_Projekt.Userverwaltung
{
    internal class UserCommands
    {
        private Dictionary<string, Action<string[]>> commandMap;

        // Konstruktor: Initialisiert das Command Mapping
        public UserCommands()
        {
            InitializeCommands();
        }

        // Initialisiert die verfügbaren Benutzerkommandos
        public void InitializeCommands()
        {
            commandMap = new Dictionary<string, Action<string[]>>
            {
                { "help", args => helpCommand() },
                { "create", args => createCommand(args) },
                { "setperm", args => setPermissionCommand(args) },
                { "list", args => Console.WriteLine(UserControls.getAllUsers()) }
            };
        }

        // Verarbeitet Benutzerkommandos und prüft, ob ein gültiges Kommando eingegeben wurde
        public void userCommands(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Missing arguments, try \"user help\" for a quick view of all commands!");
                return;
            }

            string command = args[1].ToLower();

            try
            {
                if (commandMap.TryGetValue(command, out var commandAction))
                {
                    // Führt das Kommando aus, wenn es in der commandMap gefunden wurde
                    commandAction(args);
                }
                else
                {
                    Console.WriteLine("Unknown command, try \" user help\" for a quick view of all commands!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing command '{command}': {ex.Message}");
            }
        }

        // Zeigt die Hilfenachricht für verfügbare Benutzerkommandos an
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
            short perm = 0;

            /*
            logic for setting perm manually
            */

            createCommand(perm);
        }

        // Erstellt einen neuen Benutzer und speichert diesen in der Konfigurationsdatei
        public static void createCommand(short perm)
        {
            // Fordert Benutzername, Vorname, Nachname und Passwort an
            string username = UserControls.PromptForUniqueUsername();
            string vorname = UserControls.PromptForInput("Enter Vorname: ");
            string nachname = UserControls.PromptForInput("Enter Nachname: ");
            string password = UserControls.PromptForPassword();

            try
            {
                // Erstellt ein neues User-Objekt und speichert es in der Konfigurationsdatei
                User usr = new User(username, vorname, nachname, password, perm);
                string pw = usr.GenerateHash(password);
                string usrString = $"\n{usr.Username}:{usr.Vorname}:{usr.Nachname}:{pw}:{perm}";

                File.AppendAllText(@"0:\Config\config.txt", usrString);

                Console.WriteLine("User successfully created.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        // setzt Permissions eines Nutzers (nur für Admins -> Permission = 1)
        private void setPermissionCommand(string[] args)
        {

        }
    }
}
