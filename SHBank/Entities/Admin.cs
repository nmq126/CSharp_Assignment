using System;
using System.Collections.Generic;
using SHBank.Utils;

namespace SHBank.Entities
{
    public class Admin
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public string Fullname { get; set; }
        public string Phone { get; set; }
        public int Status { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DeletedAt { get; set; }
        
        public Admin()
        {
            GenerateId();
        }
        
        public Dictionary<string, string> CheckValid()
        {
            var errors = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(this.Username))
            {
                errors.Add("username", "Username can not be null or empty!");
            }

            if (this.Username.Length < 6)
            {
                errors.Add("usernameLength", "Username must have at least 6 character");
            }
            if (string.IsNullOrEmpty(this.Password))
            {
                errors.Add("password", "Password can not be null or empty!");
            }
            if (this.Password.Length < 8)
            {
                errors.Add("passwordLength", "Password must have at least 8 character");
            }
            if (!this.Password.Equals(this.PasswordConfirm))
            {
                errors.Add("passwordConfirm", "Password confirm not match!");
            }
            if (string.IsNullOrEmpty(Fullname))
            {
                errors.Add("fullname", "Fullname can not be null or empty!");
            }
            if (string.IsNullOrEmpty(Phone))
            {
                errors.Add("phone", "Phone number can not be null or empty!");
            }
            return errors;
        }
        public void EncryptPassword()
        {
            Salt = Hash.RandomString(7);
            PasswordHash = Hash.GenerateSaltedSHA1(Password, Salt);
        }
        public void GenerateId()
        {
            Id = Guid.NewGuid().ToString();
        }
        
        public Dictionary<string, string> CheckValidLogin()
        {
            var errors = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(this.Username))
            {
                errors.Add("username", "Username can not be null or empty!");
            }
            if (string.IsNullOrEmpty(this.Password))
            {
                errors.Add("password", "Password can not be null or empty!");
            }
            return errors;
        }
    }
}