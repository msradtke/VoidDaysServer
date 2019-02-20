using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidDaysConsoleClient.VoidDaysLoginService;

namespace VoidDaysConsoleClient
{
    class Program
    {

        static VoidDaysLoginServiceClient client;
        static void Main(string[] args)
        {
            string exit = "";
            while (exit != "exit")
            {
                //Step 1: Create an instance of the WCF proxy.  
                client = new VoidDaysLoginServiceClient();
                Console.WriteLine("1. Create User");
                Console.WriteLine("2. Login");
                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        CreateUser();
                        continue;
                    case "2":
                        Login();
                        continue;
                    default:
                        break;
                }

                //Step 3: Closing the client gracefully closes the connection and cleans up resources.  

                break;
            }

            client.Close();
        }

        static void CreateUser()
        {
            Console.Write("Username: ");
            var username = Console.ReadLine();
            Console.Write("password: ");
            var password = Console.ReadLine();
            client.CreateUser(username, password);
        }
        static void Login()
        {
            Console.Write("Username: ");
            var username = Console.ReadLine();
            Console.Write("password: ");
            var password = Console.ReadLine();
            var schema = client.LoginUser(username, password);
            Console.WriteLine(schema);
        }
    }
}
