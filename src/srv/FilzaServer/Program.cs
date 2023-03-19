using System;
using System.IO;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.Dynamic;

namespace FilzaServer
{
    internal class Program
    {
        static bool isRoot = false;
        public static setup? srvSetup;

        public enum logTypes
        {
            WARN = 0,
            ERROR = 1,
            LOG = 2,
        }

        public static void log(string message, logTypes type)
        {
            switch(type)
            {
                case logTypes.WARN:
                    Console.WriteLine($"(ci) [WARING] {message}");
                    break;
                case logTypes.ERROR:
                    Console.WriteLine($"(ci) [ERROR!] {message}!");
                    break;
                case logTypes.LOG:
                    Console.WriteLine($"(ci) {message}");
                    break;
            }
        }

        static bool checkValidity()
        {
            if (!File.Exists("./serverSetup.json"))
            {
                log("serverSetup.json not found", logTypes.ERROR);
                return false;
            }

            string json = File.ReadAllText("./serverSetup.json");

            srvSetup = JsonConvert.DeserializeObject<setup>(json);

            if (!File.Exists(srvSetup.usersPath))
            {
                log("users file not found", logTypes.ERROR);
                return false;
            }

            return true;
        }

        static void Main(string[] args)
        {
            if (args.Contains("-help"))
            {
                Console.WriteLine("View the protecol here: https://github.com/lucasammer/filza/blob/main/protecol.md");
                Console.WriteLine();
                Console.WriteLine("-addUser | --a : add a user to the server");
                Console.WriteLine("-removeUser | --r : add a user to the server");
                Environment.Exit(0);
            }

            if(!checkValidity())
            {
                Environment.Exit(1);
            }

            
            string json = File.ReadAllText(srvSetup.usersPath);
            var users = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            if (args.Contains("-addUser") || args.Contains("--a"))
            {
                isRoot = true;
                Console.WriteLine("what username?");
                string uname = Console.ReadLine();
                Console.WriteLine("what password?");
                string pword = Console.ReadLine();
                pword = ComputeSha256Hash(pword);
                if (!users.ContainsKey(uname))
                {
                    users.Add(uname, pword);
                    log($"{uname} created succesfully", logTypes.LOG);
                }
                else
                {
                    log($"{uname} already exists", logTypes.WARN);
                }
                
            }
            if (args.Contains("-removeUser") || args.Contains("--r"))
            {
                isRoot = true;
                Console.WriteLine("what username?");
                string uname = Console.ReadLine();

                if (users.ContainsKey(uname))
                {
                    users.Remove(uname);
                    log($"{uname} Removed succesfully", logTypes.LOG);
                }
                else
                {
                    log($"{uname} doesn't exist", logTypes.WARN);
                }
            }
            json = JsonConvert.SerializeObject(users);
            File.WriteAllText(srvSetup.usersPath, json);

            if (isRoot)
            {
                log("you are running as root", logTypes.WARN);
            }

            Console.WriteLine("idy");
            while(true)
            {
                string cmd = Console.ReadLine();
                switch(cmd)
                {

                }
            }
        }

        static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}