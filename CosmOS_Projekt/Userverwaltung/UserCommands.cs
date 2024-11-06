using System;
using System.Collections.Generic;
using System.IO;

namespace CosmOS_Projekt.Userverwaltung
{
    internal class UserCommands
    {
        private Dictionary<string, Action<string[]>> commandMap;

        // Definiert verbotene Zeichen im Passwort
        public static char[] forbiddenpwchars = new char[] { 'ü', 'Ü', 'ä', 'Ä', 'ö', 'Ö', ' ' };

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
                { "create", args => createCommand(args) }
            };
        }

        // Verarbeitet Benutzerkommandos und prüft, ob ein gültiges Kommando eingegeben wurde
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
                    // Führt das Kommando aus, wenn es in der commandMap gefunden wurde
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

        // Erstellt einen neuen Benutzer und speichert diesen in der Konfigurationsdatei
        private void createCommand(string[] args)
        {
            // Fordert Benutzername, Vorname, Nachname und Passwort an
            string username = PromptForUniqueUsername();
            string vorname = PromptForInput("Enter Vorname: ");
            string nachname = PromptForInput("Enter Nachname: ");
            string password = PromptForPassword();

            try
            {
                // Erstellt ein neues User-Objekt und speichert es in der Konfigurationsdatei
                User usr = new User(username, vorname, nachname, password);
                string usrString = $"\n{usr.Username}:{usr.Vorname}:{usr.Nachname}:{usr.Password}";

                File.AppendAllText(@"0:\Config\config.txt", usrString);

                Console.WriteLine("User successfully created.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        // Fordert den Benutzer auf, einen eindeutigen Benutzernamen einzugeben
        private string PromptForUniqueUsername()
        {
            string username;
            do
            {
                username = PromptForInput("Enter Username: ");
                if (CheckUsernameExists(username))
                {
                    Console.WriteLine("Username already exists. Please enter a different username.");
                    username = null;
                }
            } while (string.IsNullOrEmpty(username));

            return username;
        }

        // Überprüft, ob der angegebene Benutzername bereits in der Konfigurationsdatei existiert
        private bool CheckUsernameExists(string username)
        {
            if (File.Exists(@"0:\Config\config.txt"))
            {
                var lines = File.ReadAllLines(@"0:\Config\config.txt");
                foreach (var line in lines)
                {
                    var split = line.Split(':');
                    if (split.Length > 0 && split[0] == username)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // Fordert das Passwort vom Benutzer an und prüft die Eingabebedingungen
        private string PromptForPassword()
        {
            string password;
            while (true)
            {
                Console.Write("Enter Password: ");
                password = Console.ReadLine();

                // Prüft, ob das Passwort gültig ist (Länge und keine verbotenen Zeichen)
                if (!IsValidPassword(password))
                {
                    continue;
                }

                Console.Write("Enter Password again: ");
                string passwordRetype = Console.ReadLine();

                // Stellt sicher, dass beide Passworteingaben übereinstimmen
                if (password != passwordRetype)
                {
                    Console.WriteLine("Passwords do not match, please try again.");
                    continue;
                }

                break;
            }

            return password;
        }

        // Fordert eine Benutzereingabe an und stellt sicher, dass das Eingabefeld nicht leer ist
        private string PromptForInput(string promptMessage)
        {
            string input;
            do
            {
                Console.Write(promptMessage);
                input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Input cannot be empty. Please try again.");
                }
            } while (string.IsNullOrWhiteSpace(input));

            return input;
        }

        // Überprüft, ob das Passwort gültig ist: keine leeren Felder, Mindestlänge und keine verbotenen Zeichen
        private bool IsValidPassword(string password)
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
