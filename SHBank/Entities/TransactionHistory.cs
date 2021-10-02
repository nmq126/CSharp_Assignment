using System;
using System.Collections.Generic;

namespace SHBank.Entities
{
    public class TransactionHistory
    {
        public string Id { get; set; }
        public int Type { get; set; }
        public double Amount { get; set; }
        public string Message { get; set; }
        public string SenderAccountNumber { get; set; }
        public string ReceiverAccountNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DeletedAt { get; set; }
        public int Status { get; set; } // ( pending) 1 success -1 fail

        public TransactionHistory()
        {
            GenerateTransactionId();
        }
        
        public Dictionary<string, string> CheckValid()
        {
            var errors = new Dictionary<string, string>();
            if (this.Amount <= 0)
            {
                errors.Add("amount", "Amount can not be less than or equal to zero");
            }
            return errors;
        }

        public override string ToString()
        {
            string typeWord;
            if (Type == 1)
            {
                typeWord = "Deposit";
            }
            else if (Type == 2)
            {
                typeWord = "Withdraw";
            }
            else
            {
                typeWord = "Transfer";
            }
            return $"{Id, 40} {"|", 5} {typeWord,10} {"|",5} {Amount,10} {"|",5} {SenderAccountNumber,40} {"|",5} {ReceiverAccountNumber,40} {"|",5} {CreatedAt.ToString("G"), 15} ";
        }

        public void GenerateTransactionId()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}