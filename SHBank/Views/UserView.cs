using System;
using System.Runtime.InteropServices;
using SHBank.Controllers;
using SHBank.Entities;

namespace SHBank.Views
{
    public class UserView: IUserView
    {
        public Account loggedInAccount;
        private IUserController UserController = new UserController();
        public void GenerateUserMenu()
        {
            while (true)
            {
                Console.WriteLine("___Spring Hero Bank___");
                Console.WriteLine($"Welcome {loggedInAccount.Username} back. Please choose:");
                Console.WriteLine("1. Deposit.");
                Console.WriteLine("2. Withdraw.");
                Console.WriteLine("3. Transfer.");
                Console.WriteLine("4. Check balance.");
                Console.WriteLine("5. Change information.");
                Console.WriteLine("6. Change password.");
                Console.WriteLine("7. Check Transaction History.");
                Console.WriteLine("8. Log out.");
                Console.WriteLine("_______________________");
                Console.WriteLine("Enter your choice:");
                var choiceUser = Convert.ToInt32(Console.ReadLine());
                switch (choiceUser)
                {
                    case 1:
                        UserController.Deposit(loggedInAccount);
                        break;
                    case 2:
                        UserController.Withdraw(loggedInAccount);
                        break;
                    case 3:
                        UserController.Transfer(loggedInAccount);
                        break;
                    case 4:
                        UserController.CheckBalance(loggedInAccount);
                        break;
                    case 5:
                        UserController.ChangeInformation(loggedInAccount);
                        break;
                    case 6:
                        UserController.ChangePassword(loggedInAccount);
                        break;
                    case 7:
                        UserController.CheckTransactionHistory(loggedInAccount);
                        break;
                }

                if (choiceUser == 8)
                {
                    return;
                }
            }
        }
    }
}