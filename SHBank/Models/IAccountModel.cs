using System;
using System.Collections.Generic;
using SHBank.Entities;

namespace SHBank.Models
{
    public interface IAccountModel
    {
        Account Save(Account account, Person person);
        Person SavePersonalInformation(Person person);
        
        Account FindByAccountNumber(string accountNumber);
        Account FindByUsername(string username);
        
        List<Account> SearchByPhone(string keyword);

        // , int page, int limit);
        List<Account> SearchByIdentityNumber(string keyword);
        // , int page, int limit);

        List<TransactionHistory> FindTransactionHistoryByAccountNumber(
            string accountNumber);
        // DateTime startTime,
        // DateTime endTime,
        // int page, int limit);

        TransactionHistory Deposit(TransactionHistory deposit);
        TransactionHistory Withdraw(TransactionHistory withdraw);
        TransactionHistory Transfer(TransactionHistory transfer);
        Account ChangePassword(Account account);
        Person ChangeInformation(Person person);
    }
}