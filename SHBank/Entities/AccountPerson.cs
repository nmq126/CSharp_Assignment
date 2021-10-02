namespace SHBank.Entities
{
    public class AccountPerson  
    {
        public Account Account { get; set; }
        public Person Person { get; set; }

        public AccountPerson()
        {
            Account = new Account();
            Person = new Person();
        }

        public override string ToString()
        {
            string GenderWord = Person.Gender == 1 ? "Male" : "Female";
            string StatusWord = Account.Status == 1 ? "Active" : "Inactive";
            return $"{Account.Username, 10} {"|", 5} {StatusWord,8} {"|",5} {Account.CreatedAt.ToString("G"),25} {"|",5}" +
                   $" {Person.FirstName,12} {"|",5} {Person.LastName,12} {"|",5} {GenderWord, 6} {"|",5} {Person.Dob.ToString("dd/MM/yyyy"), 10} {"|",5}"+
                   $"{Person.Email, 25} {"|",5} {Person.Address, 30} {"|",5} {Person.Phone, 12} {"|",5}";

        }
    }
}