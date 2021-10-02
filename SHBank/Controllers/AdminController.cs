using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SHBank.Entities;
using SHBank.Models;
using SHBank.Utils;

namespace SHBank.Controllers
{
    public class AdminController : IAdminController
    {
        private IAdminModel _model;
        private IAccountModel _modelAccount;
        private IUserController _userController;

        public AdminController()
        {
            _model = new AdminModel();
            _modelAccount = new AccountModel();
            _userController = new UserController();
        }

        public Admin CreateAdmin()
        {
            Admin admin;
            bool isValid;
            do
            {
                admin = GetAdminInformation();
                var errors = admin.CheckValid();

                if (CheckExistUser(admin.Username))
                {
                    errors.Add("username_duplicate", "This username has been taken, please choose another");
                }

                isValid = errors.Count == 0;
                if (!isValid)
                {
                    foreach (var item in errors)
                    {
                        Console.WriteLine(item.Value);
                    }

                    Console.WriteLine("Please reenter account information:");
                }
            } while (!isValid);

            if (CheckExistId(admin.Id))
            {
                admin.GenerateId();
            }

            admin.EncryptPassword();
            var result = _model.Save(admin);
            if (result == null)
            {
                Console.WriteLine("Register fails");
                return null;
            }

            Console.WriteLine("Register succeeds!");
            return result;
        }

        public Account CreateUser()
        {
            return _userController.Register();
        }

        public Admin Login()
        {
            bool isValid;
            Admin admin = null;
            do
            {
                admin = GetLoginInformation();
                var errors = admin.CheckValidLogin();
                isValid = errors.Count == 0;
                if (!isValid)
                {
                    foreach (var item in errors)
                    {
                        Console.WriteLine(item.Value);
                    }

                    Console.WriteLine("Please reenter your login information");
                }
            } while (!isValid);

            var existingAdmin = _model.FindByUsername(admin.Username);
            if (existingAdmin != null &&
                Hash.CompareHashString(admin.Password, existingAdmin.Salt, existingAdmin.PasswordHash))
            {
                Console.WriteLine("Login success");
                return existingAdmin;
            }
            else
            {
                Console.WriteLine("Login fails");
            }

            return null;
        }

        private Admin GetLoginInformation()
        {
            var admin = new Admin();
            Console.WriteLine("Please enter username: ");
            admin.Username = Console.ReadLine();
            Console.WriteLine("Please enter password: ");
            admin.Password = Console.ReadLine();
            return admin;
        }

        private bool CheckExistId(string id)
        {
            return _model.FindById(id) != null;
        }

        private bool CheckExistUser(string username)
        {
            return _model.FindByUsername(username) != null;
        }

        private Admin GetAdminInformation()
        {
            var admin = new Admin();
            Console.WriteLine("Please enter username: ");
            admin.Username = Console.ReadLine();
            Console.WriteLine("Please enter password: ");
            admin.Password = Console.ReadLine();
            Console.WriteLine("Please enter password confirm: ");
            admin.PasswordConfirm = Console.ReadLine();
            Console.WriteLine("Please enter your fullname: ");
            admin.Fullname = Console.ReadLine();
            Console.WriteLine("Please enter your phone number: ");
            admin.Phone = Console.ReadLine();
            return admin;
        }

        public void ShowListTransaction()
        {
            var transactions = _model.FindAllTransaction();
            if (transactions == null)
            {
                Console.WriteLine("No transaction found");
            }

            Console.WriteLine("All Transaction history: ");
            Console.WriteLine(
                $"{"Id",40} {"|",5} {"Type",10} {"|",5} {"Amount",10} {"|",5} {"SenderAccountNumber",40} {"|",5} {"ReceiverAccountNumber",40} {"|",5} {"Created At",15}");
            foreach (var item in transactions)
            {
                Console.WriteLine(item.ToString());
            }
        }

        public void UpdateAdmin()
        {
            throw new System.NotImplementedException();
        }

        public void ShowListUser()
        {
            var accounts = _model.FindAll();
            if (accounts == null)
            {
                Console.WriteLine("No user found");
            }

            Console.WriteLine("Show user list");
            Console.WriteLine($"{"Username",10} {"|",5} {"Status",8} {"|",5} {"Created At",25} {"|",5}" +
                              $" {"First name",12} {"|",5} {"Last name",12} {"|",5} {"Gender",6} {"|",5} {"DOB",10} {"|",5}" +
                              $"{"Email",25} {"|",5} {"Address",30} {"|",5} {"Phone number",12} {"|",5}");
            foreach (var item in accounts)
            {
                Console.WriteLine(item.ToString());
            }
        }


        public void LockAndUnlockUser()
        {
            Console.WriteLine("Enter account number");
            var accountNumber = Console.ReadLine();
            var account = _modelAccount.FindByAccountNumber(accountNumber);
            string choice;
            if (account == null)
            {
                Console.WriteLine("This account number is not existed");
                return;
            }

            bool result;
            switch (account.Status)
            {
                case -1:
                    result = _model.LockAndUnlockAccount(accountNumber, 1);
                    Console.WriteLine(result ? "Account has been unlocked successfully!" : "Action fails");
                    break;
                case 1:
                    result = _model.LockAndUnlockAccount(accountNumber, -1);
                    Console.WriteLine(result ? "Account has been locked successfully!" : "Action fails");
                    break;
            }
        }

        public void FindUserByAccountNumber()
        {
            Console.WriteLine("Enter User's account number");
            var accountNumber = Console.ReadLine();
            var user = _model.FinByUserAccountNumber(accountNumber);
            if (user == null)
            {
                Console.WriteLine("No user found!");
                return;
            }

            Console.WriteLine($"{"Username",10} {"|",5} {"Status",6} {"|",5} {"Created At",25} {"|",5}" +
                              $" {"First name",12} {"|",5} {"Last name",12} {"|",5} {"Gender",6} {"|",5} {"DOB",10} {"|",5}" +
                              $"{"Email",25} {"|",5} {"Address",30} {"|",5} {"Phone number",12} {"|",5}");
            Console.WriteLine(user.ToString());
        }

        public void SearchUserByPhone()
        {
            Console.WriteLine("Please enter user's phone number");
            var phone = Console.ReadLine();
            var users = _model.SearchByPhone(phone);
            if (users == null)
            {
                Console.WriteLine("No user found");
                return;
            }

            Console.WriteLine($"{"Username",10} {"|",5} {"Status",6} {"|",5} {"Created At",25} {"|",5}" +
                              $" {"First name",12} {"|",5} {"Last name",12} {"|",5} {"Gender",6} {"|",5} {"DOB",10} {"|",5}" +
                              $"{"Email",25} {"|",5} {"Address",30} {"|",5} {"Phone number",12} {"|",5}");
            foreach (var item in users)
            {
                Console.WriteLine(item.ToString());
            }
        }

        public void SearchUserByName()
        {
            Console.WriteLine("Enter User's first name or last name ");
            var name = Console.ReadLine();
            var users = _model.SearchByName(name);
            if (users == null)
            {
                Console.WriteLine("No user found");
                return;
            }

            Console.WriteLine($"{"Username",10} {"|",5} {"Status",6} {"|",5} {"Created At",25} {"|",5}" +
                              $" {"First name",12} {"|",5} {"Last name",12} {"|",5} {"Gender",6} {"|",5} {"DOB",10} {"|",5}" +
                              $"{"Email",25} {"|",5} {"Address",30} {"|",5} {"Phone number",12} {"|",5}");
            foreach (var item in users)
            {
                Console.WriteLine(item.ToString());
            }
        }

        public void SearchTransactionHistory()
        {
            Console.WriteLine("Please enter user's account number");
            var accountNumber = Console.ReadLine();
            var account = _modelAccount.FindByAccountNumber(accountNumber);
            if (account == null)
            {
                Console.WriteLine("No user found!");
                return;
            }

            _userController.CheckTransactionHistory(account);
        }

        public void ChangePassword(Admin admin)
        {
            Console.WriteLine("Update password");
            bool isValid;
            Admin updatedAdmin;
            do
            {
                var errors = new Dictionary<string, string>();
                Console.WriteLine("Enter recent password:");
                var recentPassword = Console.ReadLine();
                updatedAdmin = GetAdminUpdatePassword();
                if (!Hash.CompareHashString(recentPassword, admin.Salt, admin.PasswordHash))
                {
                    errors.Add("recentPassword", "You enter wrong recent password");
                }

                if (string.IsNullOrEmpty(updatedAdmin.Password))
                {
                    errors.Add("password", "Password can not be null or empty!");
                }

                if (updatedAdmin.Password.Length < 8)
                {
                    errors.Add("passwordLength", "Password must have at least 8 character");
                }

                if (!updatedAdmin.Password.Equals(updatedAdmin.PasswordConfirm))
                {
                    errors.Add("passwordConfirm", "Password confirm not match!");
                }

                isValid = errors.Count == 0;
                if (!isValid)
                {
                    foreach (var item in errors)
                    {
                        Console.WriteLine(item.Value);
                    }

                    Console.WriteLine("Please reenter information!");

                }
            } while (!isValid);

            admin.Password = updatedAdmin.Password;
            admin.EncryptPassword();
            admin.UpdatedAt = DateTime.Now;
            var result = _model.ChangePassword(admin);
            if (result == null)
            {
                Console.WriteLine("Action fails");
            }
            else
            {
                Console.WriteLine("Password has been changed successfully!");
            }
        }

        public void ChangeInformation(Admin admin)
        {
            Console.WriteLine("Update information");
            bool isValid;
            Admin updatedAdmin;
            do
            {
                updatedAdmin = new Admin();
                var errors = new Dictionary<string, string>();
                Console.WriteLine("Please enter your full name: ");
                updatedAdmin.Fullname = Console.ReadLine();
                Console.WriteLine("Please enter your phone number ");
                updatedAdmin.Phone = Console.ReadLine();
                if (string.IsNullOrEmpty(updatedAdmin.Fullname))
                {
                    errors.Add("fullname", "Fullname can not be null or empty!");
                }
                if (string.IsNullOrEmpty(updatedAdmin.Phone))
                {
                    errors.Add("phone", "Phone number can not be null or empty!");
                }
                isValid = errors.Count == 0;
                if (!isValid)
                {
                    foreach (var item in errors)
                    {
                        Console.WriteLine(item.Value);
                    }

                    Console.WriteLine("Please reenter information!");
                }
            } while (!isValid);

            updatedAdmin.Id = admin.Id;
            var result = _model.ChangeInformation(updatedAdmin);
            if (result == null)
            {
                Console.WriteLine("Action fails");
            }
            else
            {
                Console.WriteLine("Information has been changed successfully!");
            }
            
        }

        private Admin GetAdminUpdatePassword()
        {
            var admin = new Admin();
            Console.WriteLine("Please enter new password: ");
            admin.Password = Console.ReadLine();
            Console.WriteLine("Please confirm new password: ");
            admin.PasswordConfirm = Console.ReadLine();
            return admin;

        }
    }
}