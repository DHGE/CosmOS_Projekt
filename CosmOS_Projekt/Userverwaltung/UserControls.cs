using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CosmOS_Projekt.Userverwaltung
{
    internal class UserControls
    {

        // Definiert verbotene Zeichen im Passwort
        public static char[] forbiddenpwchars = new char[] { 'ü', 'Ü', 'ä', 'Ä', 'ö', 'Ö', ' ' };

        // konvertiert alle user in der Config-File in ein User-Objekt und legt es in eine Liste
        public static List<User> getAllUsers()
        {
            List<User> users = new List<User>();

            foreach (string line in File.ReadLines(@"0:\Config\config.txt"))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;  // Skip empty lines

                string[] info = line.Split(':');
                if (info.Length < 5)
                {
                    Console.WriteLine("Warning: Wrong format in file, skipping line.");
                    continue;
                }

                try
                {
                    short perm = short.Parse(info[4]);
                    users.Add(new User(info[0], info[1], info[2], info[3], perm));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error whilst reading user line: {ex.Message}");
                    // Continue to next line in case of an error
                }
            }

            return users;
        }

        // login der User bei Systemstart
        public static User LoginUser()
        {
            Console.Clear();

            Console.WriteLine("Please log in!\n");
            Console.WriteLine("Please enter your username!");
            string username = Console.ReadLine();
            Console.WriteLine("\nPlease enter your password!");
            string password = ReadPassword();
            foreach (User user in getAllUsers()) 
            {
                if(user.Username == username)
                {
                    if(user.GenerateHash(password) == user.Password)
                    {
                        Console.WriteLine($"\nWelcome back, {user.Username} ({user.Vorname})!\n");
                        return user;
                    }
                }
            }
            Console.WriteLine("\nIncorrect login, please try again!\n");
            Thread.Sleep(2000);

            return null;
        }

        // Fordert den Benutzer auf, einen eindeutigen Benutzernamen einzugeben
        public static string PromptForUniqueUsername()
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
        public static bool CheckUsernameExists(string username)
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
        public static string PromptForPassword()
        {
            string password;
            while (true)
            {
                Console.Write("Enter Password: ");
                password = ReadPassword();

                // Prüft, ob das Passwort gültig ist (Länge und keine verbotenen Zeichen)
                if (!IsValidPassword(password))
                {
                    continue;
                }

                Console.Write("Enter Password again: ");
                string passwordRetype = ReadPassword();

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
        public static string PromptForInput(string promptMessage)
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
        public static bool IsValidPassword(string password)
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

        /*
        Liest ein Passwort von der Konsole ein, ohne die tatsächlichen Zeichen anzuzeigen.
        Stattdessen werden Sternchen (*) angezeigt, um die Eingabe zu verschleiern.
        Das eingegebene Passwort als String.
        
        Hinweis: Die Grundidee dieser Methode wurde inspiriert von einer generierten Vorlage (ChatGPT),
        jedoch eigenständig angepasst und kommentiert.
        */
        public static string ReadPassword()
        {
            // Ein StringBuilder wird verwendet, um das Passwort zeichenweise sicher zusammenzusetzen.
            StringBuilder passwordBuilder = new StringBuilder();
            ConsoleKeyInfo keyInfo;

            while (true)
            {
                // Liest eine Taste von der Konsole, ohne sie direkt anzuzeigen (true = keine Ausgabe).
                keyInfo = Console.ReadKey(true);

                // Prüft, ob die Enter-Taste gedrückt wurde (Signal für das Ende der Eingabe).
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine(); // Springt nach Abschluss der Eingabe in eine neue Zeile.
                    break; // Bricht die Schleife ab und beendet die Eingabe.
                }
                // Prüft, ob die Backspace-Taste gedrückt wurde (zum Löschen des letzten Zeichens).
                else if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    // Wenn der Passwort-String nicht leer ist:
                    if (passwordBuilder.Length > 0)
                    {
                        // Entfernt das letzte Zeichen aus dem Passwort.
                        passwordBuilder.Remove(passwordBuilder.Length - 1, 1);

                        // Löscht das letzte Sternchen von der Konsole.
                        // Zeile 1 bewegt den Cursor zurück, " " überschreibt das Zeichen,
                        // und Zeile 3 bewegt den Cursor erneut zurück.
                        // Alternativ statt Zeile 1 und 3 geht ebenfalls Console.Write("\b \b");
                        // -> schreibt Sonderzeichen auf die Befehlszeile (nicht erwünscht)
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                        Console.Write(" ");
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                    }
                }
                else
                {
                    // Fügt das gedrückte Zeichen dem Passwort hinzu.
                    passwordBuilder.Append(keyInfo.KeyChar);

                    // Gibt ein Sternchen (*) auf der Konsole aus, um die Eingabe zu verschleiern.
                    Console.Write("*");
                }
            }

            // Gibt das vollständige Passwort als String zurück.
            return passwordBuilder.ToString();
        }
    }
}
