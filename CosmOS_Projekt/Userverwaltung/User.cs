using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CosmOS_Projekt.Userverwaltung
{
    public class User
    {
        private string username;
        private string password;
        private string vorname;
        private string nachname;
        // 0 - "no" permission (user)
        // 1 - all permissions (admin)
        // 2 - root
        private short permission;

        public User() { }

        public User(string username, string vorname, string nachname, string password, short permission)
        {
            this.username = username;
            this.vorname = vorname;
            this.nachname = nachname;
            this.password = password;
            this.permission = permission;
        }

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public string Vorname
        {
            get { return vorname; }
            set { vorname = value; }
        }

        public string Nachname
        {
            get { return nachname; }
            set { nachname = value; }
        }

        public short Permission
        {
            get { return permission; }
            set { permission = value; }
        }
    }
}
