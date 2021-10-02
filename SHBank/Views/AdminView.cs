using System;
using SHBank.Controllers;
using SHBank.Entities;

namespace SHBank.Views
{
    public class AdminView: IAdminView
    {
        public Admin loggedInAdmin;
        private IAdminController AdminController = new AdminController();
        public void GenerateAdminMenu()
        {
            while (true)
            {
                Console.WriteLine("___Spring Hero Bank___");
                Console.WriteLine($"Welcome {loggedInAdmin.Username} back. Please choose:");
                Console.WriteLine("1. User list.");
                Console.WriteLine("2. Transaction history list.");
                Console.WriteLine("3. Find user by name.");
                Console.WriteLine("4. Find user by account number.");
                Console.WriteLine("5. Find user by phone number.");
                Console.WriteLine("6. Add new user.");
                Console.WriteLine("7. Lock, unlock user.");
                Console.WriteLine("8. Transaction history by account number.");
                Console.WriteLine("9. Change information.");
                Console.WriteLine("10. Change password.");
                Console.WriteLine("11. Log out.");
                Console.WriteLine("_______________________");
                Console.WriteLine("Enter your choice:");
                var choiceAdmin = Convert.ToInt32(Console.ReadLine());
                switch (choiceAdmin)
                {
                    case 1:
                        AdminController.ShowListUser();
                        break;
                    case 2:
                        AdminController.ShowListTransaction();
                        break;
                    case 3:
                        AdminController.SearchUserByName();
                        break;
                    case 4:
                        AdminController.FindUserByAccountNumber();
                        break;
                    case 5:
                        AdminController.SearchUserByPhone();
                        break;
                    case 6:
                        AdminController.CreateUser();
                        break;
                    case 7:
                        AdminController.LockAndUnlockUser();
                        break;
                    case 8:
                        AdminController.SearchTransactionHistory();
                        break;
                    case 9:
                        AdminController.ChangeInformation(loggedInAdmin);
                        break;
                    case 10:
                        AdminController.ChangePassword(loggedInAdmin);
                        break;
                }

                if (choiceAdmin == 11)
                {
                    break;
                }
            }
        }
        
    }
}