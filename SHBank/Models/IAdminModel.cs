using System.Collections.Generic;
using SHBank.Entities;

namespace SHBank.Models
{
    public interface IAdminModel
    {
        Admin Save(Admin admin);
        bool Update(string id, Admin updateAdmin);
        Admin ChangePassword(Admin admin);
        Admin ChangeInformation(Admin admin);
        Admin FindById(string id);
        List<AccountPerson> FindAll();
        List<TransactionHistory> FindAllTransaction();
        Admin FindByUsername(string username);
        List<AccountPerson> SearchByName(string name);
        List<AccountPerson> SearchByPhone(string phone);
        AccountPerson FinByUserAccountNumber(string name);
        bool LockAndUnlockAccount(string accountNumber, int a);
    }
}