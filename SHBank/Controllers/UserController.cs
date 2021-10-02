using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SHBank.Entities;
using SHBank.Models;
using SHBank.Utils;

namespace SHBank.Controllers
{
    public class UserController: IUserController
    {
        private IAccountModel _model;

        public UserController()
        {
            _model = new AccountModel();
        }

        public Account CreateAccount()
        {
            bool isValid = false;
            Account account = null;
            do
            {
                account = GetAccountInformation();
                var errors = account.CheckValid();
                if (CheckExistUsername(account.Username))
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

                    Console.WriteLine("Please reenter your information");
                } 
            } while (!isValid);
            account.GenerateAccountNumber();
            while (CheckExistAccountNumber(account.AccountNumber))
            {
                account.GenerateAccountNumber();
            }
            Console.WriteLine(account.ToString());
            account.EncryptPassword();
            return account;
        }

        public Person CreatePersonalInformation(Account account)
        {
            bool isValid = false;
            Person person = null;
            do
            {
                person = GetPersonalInformation();
                var errors = person.CheckValid();
                isValid = errors.Count == 0;
                if (!isValid)
                {
                    foreach (var item in errors)
                    {
                        Console.WriteLine(item.Value);
                    }

                    Console.WriteLine("Please reenter your information");
                } 
            } while (!isValid);

            person.Id = account.Username;
            return person;
        }
        public Account Register()
        {
            var account = CreateAccount();
            var person = CreatePersonalInformation(account);
            var result = _model.Save(account, person);
            if (result == null)
            {
                Console.WriteLine("Register fails");
                return null;
            }
            Console.WriteLine("Account has been created!");
            return result;
        }

        private bool CheckExistAccountNumber(string accountNumber)
        {
            return _model.FindByAccountNumber(accountNumber) != null;
        }

        private bool CheckExistUsername(string username)
        {
            return _model.FindByUsername(username) != null;
        }

        private Account GetAccountInformation()
        {
            var account = new Account();
            Console.WriteLine("Please enter username: ");
            account.Username = Console.ReadLine();
            Console.WriteLine("Please enter password: ");
            account.Password = Console.ReadLine();
            Console.WriteLine("Please confirm your password: ");
            account.PasswordConfirm = Console.ReadLine();
            return account;
        }

        private Person GetPersonalInformation()
        {
            var person = new Person();
            Console.WriteLine("Please enter your first name: ");
            person.FirstName = Console.ReadLine();
            Console.WriteLine("Please enter your last name: ");
            person.LastName = Console.ReadLine();
            Console.WriteLine("Please enter 1 (male) or 0 (female): ");
            person.Gender = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Please enter your date of birth (e.g. 26/12/1987): ");
            person.Dob = DateTime.Parse(Console.ReadLine());
            Console.WriteLine("Please enter your email: ");
            person.Email = Console.ReadLine();
            Console.WriteLine("Please enter your address: ");
            person.Address = Console.ReadLine();
            Console.WriteLine("Please enter your phone number: ");
            person.Phone = Console.ReadLine();
            return person;
        }
        public void ShowBankInformation()
        {
            throw new System.NotImplementedException();
        }

        public Account Login()
        {
            bool isValid;
            Account account = null;
            do
            {
                account = GetLoginInformation();
                var errors = account.CheckValidLogin();
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

            var existingAccount = _model.FindByUsername(account.Username);
            if (existingAccount.Status == -1)
            {
                Console.WriteLine("Your account has been locked. Contact us to unlock it");
                return null;
            }
            if (existingAccount != null && Hash.CompareHashString(account.Password, existingAccount.Salt, existingAccount.PasswordHash))
            {
                Console.WriteLine("Login success");
                return existingAccount;
            }
            else
            {
                Console.WriteLine("Login fails");
            }
            return null;
        }

        private Account GetLoginInformation()
        {
            var account = new Account();
            Console.WriteLine("Please enter username: ");
            account.Username = Console.ReadLine();
            Console.WriteLine("Please enter password: ");
            account.Password = Console.ReadLine();
            return account;

        }

        public void Withdraw(Account inputAccount)
        {
            Account account = _model.FindByUsername(inputAccount.Username);
            bool isValid;
            var transactionHistory = new TransactionHistory();
            Console.WriteLine("Enter the amount you want to withdraw: ");
            transactionHistory.Amount = Convert.ToDouble(Console.ReadLine());
            var errors = transactionHistory.CheckValid();
            if (transactionHistory.Amount > account.Balance)
            {
                errors.Add("balance", "Your balance is not enough");
            }

            isValid = errors.Count == 0;
            if (!isValid)
            {
                foreach (var item in errors)
                {
                    Console.WriteLine(item.Value);
                }
                return;
            } 
                
            transactionHistory.Type = 2;
            transactionHistory.SenderAccountNumber = transactionHistory.ReceiverAccountNumber = account.AccountNumber;
            var newBalance = account.Balance - transactionHistory.Amount;
            transactionHistory.Message = $"Withdraw {transactionHistory.Amount}, account balance: {newBalance}";
            var result = _model.Withdraw(transactionHistory);
            if (result == null)
            {
                Console.WriteLine("Withdraw fails");
            }
            else
            {
                Console.WriteLine("Withdraw succeeds, here is your cash");
            }
        }
        

        public void Deposit(Account inputAccount)
        {
            var account = _model.FindByUsername(inputAccount.Username);
            bool isValid;
            var transactionHistory = new TransactionHistory();
            Console.WriteLine("Enter the amount you want to deposit: ");
            transactionHistory.Amount = Convert.ToDouble(Console.ReadLine());
            var errors = transactionHistory.CheckValid();
            isValid = errors.Count == 0;
            if (!isValid)
            {
                foreach (var item in errors)
                {
                    Console.WriteLine(item.Value);
                }
                return;
            } 

            transactionHistory.Type = 1;
            transactionHistory.SenderAccountNumber = transactionHistory.ReceiverAccountNumber = account.AccountNumber;
            var newBalance = account.Balance + transactionHistory.Amount;
            transactionHistory.Message = $"Deposit {transactionHistory.Amount}, account balance: {newBalance}";
            var result = _model.Deposit(transactionHistory);
            if (result == null)
            {
                Console.WriteLine("Deposit fails");
            }
            else
            {
                Console.WriteLine("Deposit succeeds");
            }
        }

        public void Transfer(Account inputAccount)
        {
            var account = _model.FindByUsername(inputAccount.Username);
            bool isValid;
            var transactionHistory = GetTransferInformation();
            if (transactionHistory == null)
            {
                return;
            }
            var errors = transactionHistory.CheckValid();
            if (transactionHistory.Amount > account.Balance)
            {
                errors.Add("balance", "Your balance is not enough");
            }

            isValid = errors.Count == 0;
            if (!isValid)
            {
                foreach (var item in errors)
                {
                    Console.WriteLine(item.Value);
                }
                return;
            } 
            transactionHistory.Type = 3;
            transactionHistory.SenderAccountNumber = account.AccountNumber;
            var result = _model.Transfer(transactionHistory);
            if (result == null)
            {
                Console.WriteLine("Transfer fails");
            }
            else
            {
                Console.WriteLine("Transfer succeeds");
            }
        }

        private TransactionHistory GetTransferInformation()
        {
            var transactionHistory = new TransactionHistory();
            Console.WriteLine("Please enter receiver's account number: ");
            var receiverAccountNumber = Console.ReadLine();
            if (!CheckExistAccountNumber(receiverAccountNumber))
            {
                Console.WriteLine("Receiver account number not found! ");
                return null;
            }

            var receiverAccount = _model.FindByAccountNumber(receiverAccountNumber);
            if (receiverAccount.Status == -1)
            {
                Console.WriteLine("This account has been locked and cannot receive money");
                return null;
            }
            transactionHistory.ReceiverAccountNumber = receiverAccountNumber;
            Console.WriteLine("Please enter amount you want to transfer: ");
            transactionHistory.Amount = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine("Please enter your message: ");
            transactionHistory.Message = Console.ReadLine();
            return transactionHistory;
        }

        public void CheckInformation(Account account)
        {
            throw new System.NotImplementedException();
        }

        public void ChangeInformation(Account account)
        {
            Console.WriteLine("Update information");
            bool isValid;
            var updatedPerson = new Person();
            do
            {
                updatedPerson.Id = account.Username;
                updatedPerson = GetPersonalInformation();
                var errors = updatedPerson.CheckValid();
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

            var result = _model.ChangeInformation(updatedPerson);
            if (result == null)
            {
                Console.WriteLine("Action fails");
            }
            else
            {
                Console.WriteLine("Your information has been changed successfully!");
            }
        }

        public void ChangePassword(Account account)
        {
            Console.WriteLine("Update password");
            bool isValid;
            Account updatedAccount;
            do
            {
                var errors = new Dictionary<string, string>(); 
                Console.WriteLine("Enter recent password:");
                var recentPassword = Console.ReadLine();
                updatedAccount = GetAccountUpdatePassword();
                if (!Hash.CompareHashString(recentPassword, account.Salt, account.PasswordHash))
                {
                    errors.Add("recentPassword", "You enter wrong recent password");
                }

                if (string.IsNullOrEmpty(updatedAccount.Password))
                {
                    errors.Add("password", "Password can not be null or empty!");
                }
                if (updatedAccount.Password.Length < 8)
                {
                    errors.Add("passwordLength", "Password must have at least 8 character");
                }

                if (!updatedAccount.Password.Equals(updatedAccount.PasswordConfirm))
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

            account.Password = updatedAccount.Password;
            account.EncryptPassword();
            account.UpdatedAt = DateTime.Now;
            var result = _model.ChangePassword(account);
            if (result == null)
            {
                Console.WriteLine("Action fails");
            }
            else
            {
                Console.WriteLine("Password has been changed successfully!");
            }
        }

        private Account GetAccountUpdatePassword()
        {
            var account = new Account();
            Console.WriteLine("Please enter new password: ");
            account.Password = Console.ReadLine();
            Console.WriteLine("Please confirm new password: ");
            account.PasswordConfirm = Console.ReadLine();
            return account;
        }

        public void CheckTransactionHistory(Account inputAccount)
        {
            // var account = _model.FindByUsername(inputAccount.Username);
            var list = _model.FindTransactionHistoryByAccountNumber(inputAccount.AccountNumber);
            if (list == null)
            {
                Console.WriteLine("No transaction found");
                return;
            }
            Console.WriteLine("Your transaction history: ");
            Console.WriteLine ($"{"Id", 40} {"|", 5} {"Type",10} {"|",5} {"Amount",10} {"|",5} {"SenderAccountNumber",40} {"|",5} {"ReceiverAccountNumber",40} {"|",5} {"Created At", 15}");
            foreach (var item in list)
            {
                Console.WriteLine(item.ToString());
            }
        }
        
        public void CheckBalance(Account inputAccount)
        {
            var account = _model.FindByUsername(inputAccount.Username);
            Console.WriteLine("Your balance: " + account.Balance);
        }

        public void LockTransaction()
        {
            throw new System.NotImplementedException();
        }

        public void UnlockTransaction()
        {
            throw new System.NotImplementedException();
        }
    }
}