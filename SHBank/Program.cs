using System;
using SHBank.Controllers;
using SHBank.Views;

namespace SHBank
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("___Spring Hero Bank___");
                Console.WriteLine("Main Menu");
                Console.WriteLine("1. Register.");
                Console.WriteLine("2. Login.");
                Console.WriteLine("3. About SHBank.");
                Console.WriteLine("4. Exit application.");
                Console.WriteLine("_______________________");

                Console.WriteLine("Enter your choice: ");
                var choice = Convert.ToInt32(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        while (true)
                        {
                            Console.WriteLine("___Spring Hero Bank___");
                            Console.WriteLine("Register account as:");
                            Console.WriteLine("1. User.");
                            Console.WriteLine("2. Administrator.");
                            Console.WriteLine("3. Back");
                            Console.WriteLine("_______________________");

                            Console.WriteLine("Enter your choice: ");
                            var choiceRegister = Convert.ToInt32(Console.ReadLine());
                            switch (choiceRegister)
                            {
                                case 1:
                                    var userController = new UserController();
                                    userController.Register();
                                    break;
                                case 2:
                                    var adminController = new AdminController();
                                    adminController.CreateAdmin();
                                    break;
                            }

                            if (choiceRegister.Equals(3))
                            {
                                break;
                            }
                        }
                        break;
                    case 2:
                        while (true)
                        {
                            Console.WriteLine("___Spring Hero Bank___");
                            Console.WriteLine("Login as:");
                            Console.WriteLine("1. User.");
                            Console.WriteLine("2. Administrator.");
                            Console.WriteLine("3. Back");
                            Console.WriteLine("_______________________");
                            Console.WriteLine("Enter your choice: ");
                            var choiceRegister = Convert.ToInt32(Console.ReadLine());
                            switch (choiceRegister)
                            {
                                case 1:
                                    Console.WriteLine("____________________");
                                    var userController = new UserController();
                                    var userView = new UserView();
                                    userView.loggedInAccount = userController.Login();
                                    if (userView.loggedInAccount != null)
                                    {
                                        userView.GenerateUserMenu();
                                    }
                                    break;
                                case 2:
                                    Console.WriteLine("____________________");
                                    var adminController = new AdminController();
                                    var adminView = new AdminView();
                                    adminView.loggedInAdmin = adminController.Login();
                                    if (adminView.loggedInAdmin != null)
                                    {
                                        adminView.GenerateAdminMenu();
                                    }
                                    break;
                            }
                            if (choiceRegister.Equals(3))
                            {
                                break;
                            }
                        }
                        break;
                    case 3:
                        Console.WriteLine(".");
                        break;
                }

                if (choice.Equals(4))
                {
                    Console.WriteLine("Good bye!!");
                    break;
                }
            }
        }
    }
}