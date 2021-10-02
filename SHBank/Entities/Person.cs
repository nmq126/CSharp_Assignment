using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SHBank.Entities
{
    public class Person
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Gender { get; set; } //  1 male 0 female
        public DateTime Dob { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        
        public Dictionary<string, string> CheckValid()
        {
            var errors = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(FirstName))
            {
                errors.Add("firstname", "First name can not be null or empty!");
            }
            if (string.IsNullOrEmpty(LastName))
            {
                errors.Add("firstname", "Last name can not be null or empty!");
            }

            if (Gender != 1 && Gender != 0)
            {
                errors.Add("gender", "Gender must be 1 or 0!");
            }
            
            var pattern = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            if (!Regex.IsMatch(Email, pattern))
            {
                errors.Add("email", "Email is not valid!");
            }
            if (string.IsNullOrEmpty(Address))
            {
                errors.Add("address", "Address can not be null or empty!");
            }
            if (string.IsNullOrEmpty(Phone))
            {
                errors.Add("phone", "Phone number can not be null or empty!");
            }
            return errors;
        }
    }
}