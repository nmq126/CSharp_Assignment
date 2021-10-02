using SHBank.Entities;

namespace SHBank.Controllers
{
    public interface IUserController
    {
        /*
         * - Ng dung nhap thong tin tk
         * - Validate
         * - Tao muoi, ma hoa password
         * - Luu database
         */
        Account Register();
        void ShowBankInformation();
        Account Login();
        void Withdraw(Account account);
        void Deposit(Account account);
        void Transfer(Account account);
        void ChangeInformation(Account account);
        void ChangePassword(Account account);
        void CheckTransactionHistory(Account account);
        void CheckBalance(Account account);
      
    }
}