using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Transactions;
using MySql.Data.MySqlClient;
using SHBank.Entities;

namespace SHBank.Models
{
    public class AccountModel : IAccountModel
    {
        private readonly string _insertCommand =
            $"INSERT INTO accounts (account_number, username, password_hash, salt) VALUES (@account_number, @username, @password_hash, @salt)";
        private readonly string _insertPersonCommand =
            $"INSERT INTO people (id, first_name, last_name, gender, dob, email, address, phone)" +
            $" VALUES (@id, @first_name, @last_name, @gender, @dob, @email, @address, @phone)";
        private readonly string _insertTransactionCommand =
            $"INSERT INTO transactions (id, type, amount, message, sender_account_number, receiver_account_number)" +
                        $" VALUES (@id, @type, @amount, @message, @sender_account_number, @receiver_account_number)";
        private readonly string _selectByIdCommand = $"SELECT * FROM accounts WHERE account_number = @account_number";
        private readonly string _selectByUsernameCommand = $"SELECT * FROM accounts WHERE username = @username";
        private readonly string _selectTransactionByAccountNumberCommand = 
            $"SELECT * FROM transactions WHERE sender_account_number = @account_number OR receiver_account_number = @account_number ORDER BY createdAt ASC";
        private readonly string _selectAccountCommand = 
            $"SELECT * FROM accounts ORDER BY username ASC";
        private readonly string _withdrawCommand =
            $"UPDATE accounts SET balance = balance - @amount where account_number = @account_number";
        private readonly string _depositCommand =
            $"UPDATE accounts SET balance = balance + @amount where account_number = @account_number";
        private readonly string _changePasswordCommand =
            $"UPDATE accounts SET password_hash = @password_hash, salt = @salt, updatedAt = @updatedAt where account_number = @account_number";
        private readonly string _changeInformationCommand =
                $"UPDATE people SET first_name = @first_name, last_name = @last_name, gender = @gender, dob = @dob, email = @email, address = @address, phone = @phone" +
                $" where id = @id";
                


        public Account Save(Account account, Person person)
        {
            using (var cnn = ConnectionHelper.GetInstance())
            {
                cnn.Open();
                var command = cnn.CreateCommand();
                MySqlTransaction transaction;
                transaction = cnn.BeginTransaction();
                command.Connection = cnn;
                command.Transaction = transaction;
                try
                {
                    command.CommandText = _insertPersonCommand;
                    command.Parameters.AddWithValue("@id", account.Username);
                    command.Parameters.AddWithValue("@first_name", person.FirstName);
                    command.Parameters.AddWithValue("@last_name", person.LastName);
                    command.Parameters.AddWithValue("@gender", person.Gender);
                    command.Parameters.AddWithValue("@dob", person.Dob);
                    command.Parameters.AddWithValue("@email", person.Email);
                    command.Parameters.AddWithValue("@address", person.Address);
                    command.Parameters.AddWithValue("@phone", person.Phone);
                    command.Prepare();
                    command.ExecuteNonQuery();
                    command.CommandText = _insertCommand;
                    command.Parameters.AddWithValue("@account_number", account.AccountNumber);
                    command.Parameters.AddWithValue("@username", account.Username);
                    command.Parameters.AddWithValue("@password_hash", account.PasswordHash);
                    command.Parameters.AddWithValue("@salt", account.Salt);
                    command.Prepare();
                    var result = command.ExecuteNonQuery();
                    transaction.Commit();
                    if (result > 0)
                    {
                        return account;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                    Console.WriteLine("  Message: {0}", ex.Message);
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                        Console.WriteLine("  Message: {0}", ex2.Message);
                    }
                }
            }

            return null;
            // try
            // {
            //     using (var cnn = ConnectionHelper.GetInstance())
            //     {
            //         cnn.Open();
            //         var transaction = cnn.BeginTransaction();
            //         var mySqlCommand1 = new MySqlCommand(_insertCommand, cnn, transaction);
            //         mySqlCommand1.Parameters.AddWithValue("@account_number", account.AccountNumber);
            //         mySqlCommand1.Parameters.AddWithValue("@username", account.Username);
            //         mySqlCommand1.Parameters.AddWithValue("@password_hash", account.PasswordHash);
            //         mySqlCommand1.Parameters.AddWithValue("@salt", account.Salt);
            //         mySqlCommand1.Prepare();
            //         var mySqlCommand2 = new MySqlCommand(_insertPersonCommand, cnn, mySqlCommand1.Transaction);
            //         mySqlCommand2.Parameters.AddWithValue("@id", account.Username);
            //         mySqlCommand2.Parameters.AddWithValue("@first_name", person.FirstName);
            //         mySqlCommand2.Parameters.AddWithValue("@last_name", person.LastName);
            //         mySqlCommand2.Parameters.AddWithValue("@gender", person.Gender);
            //         mySqlCommand2.Parameters.AddWithValue("@dob", person.Dob);
            //         mySqlCommand2.Parameters.AddWithValue("@email", person.Email);
            //         mySqlCommand2.Parameters.AddWithValue("@address", person.Address);
            //         mySqlCommand2.Prepare();
            //
            //         try
            //         {
            //             mySqlCommand1.ExecuteNonQuery();
            //             mySqlCommand2.ExecuteNonQuery();
            //             transaction.Commit();
            //         }
            //         catch (Exception e)
            //         {
            //             Console.WriteLine("roll backed");
            //             transaction.Rollback();
            //         }
            //     }
            // }
            // catch (Exception e)
            // {
            //     Console.WriteLine(e);
            // }
            //
            // return null;
        }

        public Person SavePersonalInformation(Person person)
        {
            try
            {
                using (var cnn = ConnectionHelper.GetInstance())
                {
                    cnn.Open();
                    var mySqlCommand = new MySqlCommand(_insertPersonCommand, cnn);
                    mySqlCommand.Parameters.AddWithValue("@id", person.Id);
                    mySqlCommand.Parameters.AddWithValue("@first_name", person.FirstName);
                    mySqlCommand.Parameters.AddWithValue("@last_name", person.LastName);
                    mySqlCommand.Parameters.AddWithValue("@gender", person.Gender);
                    mySqlCommand.Parameters.AddWithValue("@dob", person.Dob);
                    mySqlCommand.Parameters.AddWithValue("@email", person.Email);
                    mySqlCommand.Parameters.AddWithValue("@address", person.Address);
                    mySqlCommand.Parameters.AddWithValue("@phone", person.Phone);
                    mySqlCommand.Prepare();
                    var result = mySqlCommand.ExecuteNonQuery();
                    if (result > 0)
                    {
                        return person;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        

        public Account FindByAccountNumber(string accountNumber)
        {
            try
            {
                using (var cnn = ConnectionHelper.GetInstance())
                {
                    cnn.Open();
                    var mySqlCommand = new MySqlCommand(_selectByIdCommand, cnn);
                    mySqlCommand.Parameters.AddWithValue("@account_number", accountNumber);
                    using (var reader = mySqlCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var account = new Account()
                            {
                                AccountNumber = reader.GetString("account_number"),
                                Username = reader.GetString("username"),
                                PasswordHash = reader.GetString("password_hash"),
                                Salt = reader.GetString("salt"),
                                Status = reader.GetInt32("status")
                            };
                            return account;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return null;
        }

        public Account FindByUsername(string username)
        {
            try
            {
                using (var cnn = ConnectionHelper.GetInstance())
                {
                    cnn.Open();
                    var mySqlCommand = new MySqlCommand(_selectByUsernameCommand, cnn);
                    mySqlCommand.Parameters.AddWithValue("@username", username);
                    using (var reader = mySqlCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var account = new Account()
                            {
                                AccountNumber = reader.GetString("account_number"),
                                Username = reader.GetString("username"),
                                PasswordHash = reader.GetString("password_hash"),
                                Salt = reader.GetString("salt"),
                                Balance = reader.GetDouble("balance"),
                                Status = reader.GetInt32("status")
                            };
                            return account;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return null;
        }
        
        public List<Account> SearchByPhone(string keyword)
        {
            throw new NotImplementedException();
        }

        public List<Account> SearchByIdentityNumber(string keyword)
        {
            throw new NotImplementedException();
        }

        public List<TransactionHistory> FindTransactionHistoryByAccountNumber(string accountNumber)
            // , DateTime startTime,
            // DateTime endTime, int page, int limit)
        {
            var transactionHistories = new List<TransactionHistory>();
            try
            {
                using (var cnn = ConnectionHelper.GetInstance())
                {
                    cnn.Open();
                    var mySqlCommand = new MySqlCommand(_selectTransactionByAccountNumberCommand, cnn);
                    mySqlCommand.Parameters.AddWithValue("@account_number", accountNumber);
                    mySqlCommand.Prepare();
                    using (var reader = mySqlCommand.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            return null;
                        }
                        while (reader.Read())
                        {
                            var transaction = new TransactionHistory()
                            {
                                Id = reader.GetString("id"),
                                Type = reader.GetInt32("type"),
                                Amount = reader.GetDouble("amount"),
                                Message = reader.GetString("message"),
                                SenderAccountNumber = reader.GetString("sender_account_number"),
                                ReceiverAccountNumber = reader.GetString("receiver_account_number"),
                                CreatedAt = reader.GetDateTime("createdAt"),
                                UpdatedAt = reader.GetDateTime("updatedAt"),
                                DeletedAt = reader.GetDateTime("deletedAt"),
                                Status = reader.GetInt32("status")
                            };
                            transactionHistories.Add(transaction);
                        }
                        return transactionHistories;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public TransactionHistory Deposit(TransactionHistory deposit)
        {
            try
            {
                using (var cnn = ConnectionHelper.GetInstance())
                {
                    cnn.Open();
                    var mySqlCommand = new MySqlCommand(_insertTransactionCommand, cnn);
                    mySqlCommand.Parameters.AddWithValue("@id", deposit.Id);
                    mySqlCommand.Parameters.AddWithValue("@type", deposit.Type);
                    mySqlCommand.Parameters.AddWithValue("@amount", deposit.Amount);
                    mySqlCommand.Parameters.AddWithValue("@message", deposit.Message);
                    mySqlCommand.Parameters.AddWithValue("@sender_account_number", deposit.SenderAccountNumber);
                    mySqlCommand.Parameters.AddWithValue("@receiver_account_number", deposit.ReceiverAccountNumber);
                    mySqlCommand.Prepare();
                    var result1 = mySqlCommand.ExecuteNonQuery();
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.CommandText = _depositCommand;
                    mySqlCommand.Parameters.AddWithValue("@amount", deposit.Amount);
                    mySqlCommand.Parameters.AddWithValue("@account_number", deposit.SenderAccountNumber);
                    mySqlCommand.Prepare();
                    var result2 = mySqlCommand.ExecuteNonQuery();
                    if (result1 > 0 && result2 > 0)
                    {
                        return deposit;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }



        public TransactionHistory Transfer(TransactionHistory transfer)
        {
            try
            {
                using (var cnn = ConnectionHelper.GetInstance())
                {
                    cnn.Open();
                    var mySqlCommand = new MySqlCommand(_insertTransactionCommand, cnn);
                    mySqlCommand.Parameters.AddWithValue("@id", transfer.Id);
                    mySqlCommand.Parameters.AddWithValue("@type", transfer.Type);
                    mySqlCommand.Parameters.AddWithValue("@amount", transfer.Amount);
                    mySqlCommand.Parameters.AddWithValue("@message", transfer.Message);
                    mySqlCommand.Parameters.AddWithValue("@sender_account_number", transfer.SenderAccountNumber);
                    mySqlCommand.Parameters.AddWithValue("@receiver_account_number", transfer.ReceiverAccountNumber);
                    mySqlCommand.Prepare();
                    var result1 = mySqlCommand.ExecuteNonQuery();
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.CommandText = _withdrawCommand;
                    mySqlCommand.Parameters.AddWithValue("@amount", transfer.Amount);
                    mySqlCommand.Parameters.AddWithValue("@account_number", transfer.SenderAccountNumber);
                    mySqlCommand.Prepare();
                    var result2 = mySqlCommand.ExecuteNonQuery();
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.CommandText = _depositCommand;
                    mySqlCommand.Parameters.AddWithValue("@amount", transfer.Amount);
                    mySqlCommand.Parameters.AddWithValue("@account_number", transfer.ReceiverAccountNumber);
                    mySqlCommand.Prepare();
                    var result3 = mySqlCommand.ExecuteNonQuery();
                    if (result1 > 0 && result2 > 0 && result3 > 0)
                    {
                        return transfer;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }

        public Account ChangePassword(Account account)
        {
            try
            {
                using (var cnn = ConnectionHelper.GetInstance())
                {
                    cnn.Open();
                    var mySqlCommand = new MySqlCommand(_changePasswordCommand, cnn);
                    mySqlCommand.Parameters.AddWithValue("@password_hash", account.PasswordHash);
                    mySqlCommand.Parameters.AddWithValue("@salt", account.Salt);
                    mySqlCommand.Parameters.AddWithValue("@updatedAt", account.UpdatedAt);
                    mySqlCommand.Parameters.AddWithValue("@account_number", account.AccountNumber);
                    mySqlCommand.Prepare();
                    var result = mySqlCommand.ExecuteNonQuery();
                    if (result > 0)
                    {
                        return account;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return null;
        }

        public Person ChangeInformation(Person person)
        {
            try
            {
                using (var cnn = ConnectionHelper.GetInstance())
                {
                    cnn.Open();
                    var mySqlCommand = new MySqlCommand(_changeInformationCommand, cnn);
                    mySqlCommand.Parameters.AddWithValue("@first_name", person.FirstName);
                    mySqlCommand.Parameters.AddWithValue("@last_name", person.LastName);
                    mySqlCommand.Parameters.AddWithValue("@gender", person.Gender);
                    mySqlCommand.Parameters.AddWithValue("@dob", person.Dob);
                    mySqlCommand.Parameters.AddWithValue("@email", person.Email);
                    mySqlCommand.Parameters.AddWithValue("@address", person.Address);
                    mySqlCommand.Parameters.AddWithValue("@id", person.Id);
                    mySqlCommand.Prepare();
                    var result = mySqlCommand.ExecuteNonQuery();
                    if (result > 0)
                    {
                        return person;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return null;
        }


        public TransactionHistory Withdraw(TransactionHistory withdraw)
        {
            try
            {
                using (var cnn = ConnectionHelper.GetInstance())
                {
                    cnn.Open();
                    var mySqlCommand = new MySqlCommand(_insertTransactionCommand, cnn);
                    mySqlCommand.Parameters.AddWithValue("@id", withdraw.Id);
                    mySqlCommand.Parameters.AddWithValue("@type", withdraw.Type);
                    mySqlCommand.Parameters.AddWithValue("@amount", withdraw.Amount);
                    mySqlCommand.Parameters.AddWithValue("@message", withdraw.Message);
                    mySqlCommand.Parameters.AddWithValue("@sender_account_number", withdraw.SenderAccountNumber);
                    mySqlCommand.Parameters.AddWithValue("@receiver_account_number", withdraw.ReceiverAccountNumber);
                    mySqlCommand.Prepare();
                    var result1 = mySqlCommand.ExecuteNonQuery();
                    mySqlCommand.Parameters.Clear();
                    mySqlCommand.CommandText = _withdrawCommand;
                    mySqlCommand.Parameters.AddWithValue("@amount", withdraw.Amount);
                    mySqlCommand.Parameters.AddWithValue("@account_number", withdraw.SenderAccountNumber);
                    mySqlCommand.Prepare();
                    var result2 = mySqlCommand.ExecuteNonQuery();
                    if (result1 > 0 && result2 > 0)
                    {
                        return withdraw;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }
    }
}