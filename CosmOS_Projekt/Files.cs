using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CosmOS_Projekt
{
    public class Files
    {
        // 000 - "no" permission
        // 001 - read permission
        // 011 - read and write permission
        // 111 - all permissions
        private string name;
        private string path;
        private string owner;

        public Files(string name, string path, string owner)
        {
            this.name = name;
            this.path = path;
            this.owner = owner;
            Kernel.fs.CreateFile(path + name);
            CreateConfig();
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public string Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        private void CreateConfig()
        {
            string Config_path = @"0:\Config\" + name + "_Permission.txt";
            Kernel.fs.CreateFile(Config_path);
            Userverwaltung.UserControls.getAllUsers().ForEach(u =>
            {
                if (u.Username == owner)
                {
                    File.AppendAllText(Config_path, u.Username + ":" + "111");
                    
                }
                else if (u.Permission == 2)
                {
                    File.AppendAllText(Config_path, u.Username + ":" + "111");
                }
                else if (u.Permission == 1)
                {
                    File.AppendAllText(Config_path, u.Username + ":" + "001");
                }
                else
                {
                    File.AppendAllText(Config_path, u.Username + ":" + "000");
                }
            });
            return;
        }
    }
}
