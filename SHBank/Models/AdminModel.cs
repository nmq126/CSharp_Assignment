using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using SHBank.Entities;

namespace SHBank.Models
{
    public class AdminModel: IAdminModel
    {
        private readonly string _selectAdminByUsernameCommand = $"SELECT * FROM admins WHERE username = @username";
        private readonly string _selectAdminByIdCommand = $"SELECT * FROM admins WHERE id = @id";
        private readonly string _selectTransactionCommand = $"SELECT * FROM transactions";
        private readonly string _insertAdminCommand =
            $"INSERT INTO admins (id, username, password_hash, salt, fullname, phone, status)" +
            $" VALUES (@id, @username, @password_hash, @salt, @fullname, @phone, @status)";
        private readonly string _selectByUserAccountNumberCommand = $"SELECT a.username, a.status, a.createdAt, p.first_name, p.last_name, p.gender, p.dob, p.email, p.address, p.phone"
                                                                    + $" FROM accounts AS a INNER JOIN people AS p ON a.username = p.id WHERE a.account_number = @account_number" ;
        private readonly string _selectUserCommand = $"SELECT a.username, a.status, a.createdAt, p.first_name, p.last_name, p.gender, p.dob, p.email, p.address, p.phone"
                                                     + $" FROM accounts AS a INNER JOIN people AS p ON a.username = p.id ORDER BY p.first_name" ;
        private readonly string _selectUserByNameCommand = $"SELECT a.username, a.status, a.createdAt, p.first_name, p.last_name, p.gender, p.dob, p.email, p.address, p.phone"
                                                     + $" FROM accounts AS a INNER JOIN people AS p ON a.username = p.id"
                                                     + $" WHERE p.first_name = @name OR p.last_name = @name";
        private readonly string _selectUserByPhoneCommand = $"SELECT a.username, a.status, a.createdAt, p.first_name, p.last_name, p.gender, p.dob, p.email, p.address, p.phone"
                                                           + $" FROM accounts AS a INNER JOIN people AS p ON a.username = p.id"
                                                           + $" WHERE p.phone = @phone";
        private readonly string _updateAccountStatusCommand = $"UPDATE accounts SET status = @status WHERE account_number = @account_number";
        private readonly string _changePasswordCommand =
            $"UPDATE admins SET password_hash = @password_hash, salt = @salt, updatedAt = @updatedAt where id = @id";
        private readonly string  _changeInformationCommand =
            $"UPDATE admins SET fullname = @fullname, phone = @phone, updatedAt = @updatedAt where id = @id";
        
        public Admin Save(Admin admin)
        {
            using (var cnn = ConnectionHelper.GetInstance())
            {
                cnn.Open();
                var mySqlCommand = new MySqlCommand(_insertAdminCommand, cnn);
                mySqlCommand.Parameters.AddWithValue("@id", admin.Id);
                mySqlCommand.Parameters.AddWithValue("@username", admin.Username);
                mySqlCommand.Parameters.AddWithValue("@password_hash", admin.PasswordHash);
                mySqlCommand.Parameters.AddWithValue("@salt", admin.Salt);
                mySqlCommand.Parameters.AddWithValue("@fullname", admin.Fullname);
                mySqlCommand.Parameters.AddWithValue("@phone", admin.Phone);
                mySqlCommand.Parameters.AddWithValue("@status", admin.Status); 
                mySqlCommand.Prepare();
                var result = mySqlCommand.ExecuteNonQuery();
                if (result > 0)
                {
                    return admin;
                }    
            }
            return null;
        }

        public bool Update(string id, Admin updateAdmin)
        {
            throw new System.NotImplementedException();
        }

        public Admin ChangePassword(Admin admin)
        {
            try
            {
                using (var cnn = ConnectionHelper.GetInstance())
                {
                    cnn.Open();
                    var mySqlCommand = new MySqlCommand(_changePasswordCommand, cnn);
                    mySqlCommand.Parameters.AddWithValue("@password_hash", admin.PasswordHash);
                    mySqlCommand.Parameters.AddWithValue("@salt", admin.Salt);
                    mySqlCommand.Parameters.AddWithValue("@updatedAt", admin.UpdatedAt);
                    mySqlCommand.Parameters.AddWithValue("@id", admin.Id);
                    mySqlCommand.Prepare();
                    var result = mySqlCommand.ExecuteNonQuery();
                    if (result > 0)
                    {
                        return admin;
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

        public Admin ChangeInformation(Admin admin)
        {
            try
            {
                using (var cnn = ConnectionHelper.GetInstance())
                {
                    cnn.Open();
                    var mySqlCommand = new MySqlCommand(_changeInformationCommand, cnn);
                    mySqlCommand.Parameters.AddWithValue("@fullname", admin.Fullname);
                    mySqlCommand.Parameters.AddWithValue("@phone", admin.Phone);
                    mySqlCommand.Parameters.AddWithValue("@updatedAt", DateTime.Now);
                    mySqlCommand.Parameters.AddWithValue("@id", admin.Id);
                    mySqlCommand.Prepare();
                    var result = mySqlCommand.ExecuteNonQuery();
                    if (result > 0)
                    {
                        return admin;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return null;        }


        public Admin FindById(string id)
        {
            try
            {
                using (var cnn = ConnectionHelper.GetInstance())
                {
                    cnn.Open();
                    var mySqlCommand = new MySqlCommand(_selectAdminByIdCommand, cnn);
                    mySqlCommand.Parameters.AddWithValue("@id", id);
                    using (var reader = mySqlCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var admin = new Admin()
                            {
                                Id = reader.GetString("account_number"),
                                Username = reader.GetString("username"),
                                PasswordHash = reader.GetString("password_hash"),
                                Salt = reader.GetString("salt")
                            };
                            return admin;
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

        public List<AccountPerson> FindAll()
        {
            var accountPersons = new List<AccountPerson>();
            try
            {
                using (var cnn = ConnectionHelper.GetInstance())
                {
                    cnn.Open();
                    var mySqlCommand = new MySqlCommand(_selectUserCommand, cnn);
                    using (var reader = mySqlCommand.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            return null;
                        }
                        while (reader.Read())
                        {
                            var accountPerson = new AccountPerson();
                            var account = new Account()
                            {
                                Username = reader.GetString("username"),
                                CreatedAt = reader.GetDateTime("createdAt"),
                                Status = reader.GetInt32("status")
                            };
                            var person = new Person()
                            {
                                FirstName = reader.GetString("first_name"),
                                LastName = reader.GetString("last_name"),
                                Gender = reader.GetInt32("gender"),
                                Dob = reader.GetDateTime("dob"),
                                Email = reader.GetString("email"),
                                Address = reader.GetString("address"),
                                Phone = reader.GetString("phone"),
                            };
                            accountPerson.Account = account;
                            accountPerson.Person = person;
                            accountPersons.Add(accountPerson);
                        }

                        return accountPersons;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public List<TransactionHistory> FindAllTransaction()
        {
            var transactionHistories = new List<TransactionHistory>();
            try
            {
                using (var cnn = ConnectionHelper.GetInstance())
                {
                    cnn.Open();
                    var mySqlCommand = new MySqlCommand(_selectTransactionCommand, cnn);
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

        public Admin FindByUsername(string username)
        {
            try
            {
                using (var cnn = ConnectionHelper.GetInstance())
                {
                    cnn.Open();
                    var mySqlCommand = new MySqlCommand(_selectAdminByUsernameCommand, cnn);
                    mySqlCommand.Parameters.AddWithValue("@username", username);
                    using (var reader = mySqlCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var admin = new Admin()
                            {
                                Id = reader.GetString("id"),
                                Username = reader.GetString("username"),
                                PasswordHash = reader.GetString("password_hash"),
                                Salt = reader.GetString("salt"),
                            };
                            return admin;
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

        public List<AccountPerson> SearchByName(string name)
        {
            var accountPersons = new List<AccountPerson>();
            try
            {
                using (var cnn = ConnectionHelper.GetInstance())
                {
                    cnn.Open();
                    var mySqlCommand = new MySqlCommand(_selectUserByNameCommand, cnn);
                    mySqlCommand.Parameters.AddWithValue("@name", name);
                    using (var reader = mySqlCommand.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            return null;
                        }
                        while (reader.Read())
                        {
                            var accountPerson = new AccountPerson();
                            var account = new Account()
                            {
                                Username = reader.GetString("username"),
                                CreatedAt = reader.GetDateTime("createdAt"),
                                Status = reader.GetInt32("status")
                            };
                            var person = new Person()
                            {
                                FirstName = reader.GetString("first_name"),
                                LastName = reader.GetString("last_name"),
                                Gender = reader.GetInt32("gender"),
                                Dob = reader.GetDateTime("dob"),
                                Email = reader.GetString("email"),
                                Address = reader.GetString("address"),
                                Phone = reader.GetString("phone"),
                            };
                            accountPerson.Account = account;
                            accountPerson.Person = person;
                            accountPersons.Add(accountPerson);
                        }

                        return accountPersons;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public List<AccountPerson> SearchByPhone(string phone)
        {
            var accountPersons = new List<AccountPerson>();
            try
            {
                using (var cnn = ConnectionHelper.GetInstance())
                {
                    cnn.Open();
                    var mySqlCommand = new MySqlCommand(_selectUserByPhoneCommand, cnn);
                    mySqlCommand.Parameters.AddWithValue("@phone", phone);
                    using (var reader = mySqlCommand.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            return null;
                        }
                        while (reader.Read())
                        {
                            var accountPerson = new AccountPerson();
                            var account = new Account()
                            {
                                Username = reader.GetString("username"),
                                CreatedAt = reader.GetDateTime("createdAt"),
                                Status = reader.GetInt32("status")
                            };
                            var person = new Person()
                            {
                                FirstName = reader.GetString("first_name"),
                                LastName = reader.GetString("last_name"),
                                Gender = reader.GetInt32("gender"),
                                Dob = reader.GetDateTime("dob"),
                                Email = reader.GetString("email"),
                                Address = reader.GetString("address"),
                                Phone = reader.GetString("phone"),
                            };
                            accountPerson.Account = account;
                            accountPerson.Person = person;
                            accountPersons.Add(accountPerson);
                        }

                        return accountPersons;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public AccountPerson FinByUserAccountNumber(string accountNumber)
        {
            try
            {
                using (var cnn = ConnectionHelper.GetInstance())
                {
                    cnn.Open();
                    var mySqlCommand = new MySqlCommand(_selectByUserAccountNumberCommand, cnn);
                    mySqlCommand.Parameters.AddWithValue("@account_number", accountNumber);
                    using (var reader = mySqlCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var accountPerson = new AccountPerson();
                            var account = new Account()
                            {
                                Username = reader.GetString("username"),
                                CreatedAt = reader.GetDateTime("createdAt"),
                                Status = reader.GetInt32("status")
                            };
                            var person = new Person()
                            {
                                FirstName = reader.GetString("first_name"),
                                LastName = reader.GetString("last_name"),
                                Gender = reader.GetInt32("gender"),
                                Dob = reader.GetDateTime("dob"),
                                Email = reader.GetString("email"),
                                Address = reader.GetString("address"),
                                Phone = reader.GetString("phone"),
                            };
                            accountPerson.Account = account;
                            accountPerson.Person = person;
                            return accountPerson;
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

        public bool LockAndUnlockAccount(string accountNumber, int a)
        {
            using (var cnn = ConnectionHelper.GetInstance())
            {
                cnn.Open();
                var mySqlCommand = new MySqlCommand(_updateAccountStatusCommand, cnn);
                mySqlCommand.Parameters.AddWithValue("@status", a);
                mySqlCommand.Parameters.AddWithValue("@account_number", accountNumber);
                mySqlCommand.Parameters.AddWithValue("@updatedAt", DateTime.Now);
                mySqlCommand.Prepare();
                var result = mySqlCommand.ExecuteNonQuery();
                return result > 0;
            }
        }

    }
}