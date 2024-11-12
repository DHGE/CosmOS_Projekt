using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
                if (line == "" || String.IsNullOrEmpty(line))
                {
                    return new List<User>();
                }
                string[] info = line.Split(":");
                if (info.Length == 0)
                {
                    Console.WriteLine("Wrong format in file");
                    return new List<User>();
                }
                else
                {
                    short perm = short.Parse(info[4]);
                    users.Add(new User(info[0], info[1], info[2], info[3], perm));
                }
            }
            return users;
        }

        // login der User bei Systemstart
        public static User LoginUser()
        {
            Console.WriteLine("Please enter your username!");
            string username = Console.ReadLine();
            Console.WriteLine("Please enter your password!");
            string password = Console.ReadLine();
            foreach (User user in getAllUsers()) 
            {
                if(user.Username == username)
                {
                    if(user.GenerateHash(password) == user.Password)
                    {
                        Console.WriteLine($"Welcome back {user.Username}!");
                        return user;
                    }
                    Console.WriteLine("Wrong password!");
                }
                Console.WriteLine("Wrong username!");
            }
            Console.WriteLine("\nIncorrect login, please try again!\n");
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
    }

}
