using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Runtime.InteropServices;

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
                { "list", args => listUsersCommand() },
                { "logout", args => Kernel.currentUser = null },
                { "del", args => deleteUserCommand(args) },
                { "delete", args => deleteUserCommand(args) }
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
                "create - creates a new user\n" +
                "setperm - change the permission of a user\n" +
                "list - lists all users");
        }

        // createCommand, welcher die Eingabe überprüft
        // keine Eingabe -> Berechtigung 0
        // gültige Eingabe -> Berechtigung welche einegegeben wurde (mit check das keine höhere eingegeben wurde)
        private void createCommand(string[] args)
        {
            if (args.Length < 3)
            {
                createCommand(0);
                return;
            }

            short userPerm = Kernel.currentUser.Permission;
            short perm = short.Parse(args[2]);

            if (perm > userPerm)
            {
                Console.WriteLine("You're not allowed to create a user with higher permissions!");
                return;
            }

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
                string pw = UserControls.GenerateHash(password);
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
            if (args.Length < 3)
            {
                Console.WriteLine("Missing arguments, try \"user help\" for a quick view of all file commands!");
                return;
            }
            short userPerm = Kernel.currentUser.Permission;
            short perm = short.Parse(args[3]);
            if (userPerm != 1)
            {
                Console.WriteLine("You're not allowed to change Permission levels");
                return;
            }
            if (perm != 1 && perm != 0)
            {
                Console.WriteLine("Please give valid Permissionlevel(0,1)");
                return;
            }
            string usr = args[2];
            List<User> users = UserControls.getAllUsers();
            foreach (var user in users)
            {
                if (user.Username == usr)
                {
                    user.Permission = perm;
                    UserControls.updateConfig(users);
                    Console.WriteLine("Permission succesfully changed");
                    return;
                }
            }
            Console.WriteLine("Please give existing Username");
            return;
        }

        // gibt alle vorhanden User auf der Befehlszeile aus
        private void listUsersCommand()
        {
            Console.WriteLine("Username\tVorname\tNachname\tPermission");
            foreach (var user in UserControls.getAllUsers())
            {
                Console.WriteLine($"{user.Username} : {user.Vorname} : {user.Nachname} : {user.Permission}");
            }

            Console.WriteLine($"\nTotal users: {UserControls.getAllUsers().Count}");
        }
        private void deleteUserCommand(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Missing arguments, try \"user help\" for a quick view of all file commands!");
                return;
            }
            string usr = args[2];
            if (!UserControls.CheckUsernameExists(usr))
            {
                Console.WriteLine("Please give existing Username");
                return;
            }
            List<User> allUsers = UserControls.getAllUsers();
            if (Kernel.currentUser.Username != usr && Kernel.currentUser.Permission == 0)
            {
                Console.WriteLine("You don't have the permission to delete this User");
                return;
            }
            foreach (var user in allUsers)
            {

                if (user.Username == usr)
                {
                    allUsers.Remove(user);
                    Console.WriteLine("Successfully deleted User");
                    if (Kernel.currentUser.Username == usr)
                    {
                        Kernel.currentUser = null;
                    }
                    UserControls.updateConfig(allUsers);
                    return;
                }
            }
        }

        private void editUserCommand(string[] args)
        {
            Console.WriteLine($"_________Editing User:{Kernel.currentUser.Username}__________");
            Console.WriteLine("What do you want to edit?: Password, Username, Name");
            string command = Console.ReadLine();
            command.ToLower();
            string oldusername = Kernel.currentUser.Username;
            switch (command)
            {
                case "password":
                    Console.WriteLine("Please enter your current password");
                    string pw = UserControls.ReadPassword();
                    pw = UserControls.GenerateHash(pw);
                    if (pw == Kernel.currentUser.Password)
                    {
                        string pw1 = UserControls.PromptForPassword();
                        Kernel.currentUser.Password = UserControls.GenerateHash(pw1);
                        Console.WriteLine("Password successfully changed");
                    }
                    break;
                case "username":
                    Console.WriteLine($"Old Username: {Kernel.currentUser.Username}");
                    Kernel.currentUser.Username = UserControls.PromptForUniqueUsername();
                    break;
                case "name":
                    break;
                default:
                    Console.WriteLine("Invalid input");
                    return;
            }
        }
    }
}
