using SHBank.Entities;

namespace SHBank.Controllers
{
    public interface IAdminController
    {
        Admin Login();
        Admin CreateAdmin();
        Account CreateUser();
        void ShowListTransaction();
        void UpdateAdmin();
        void ShowListUser();
        void LockAndUnlockUser();
        void FindUserByAccountNumber();
        void SearchUserByPhone();
        void SearchUserByName();
        void SearchTransactionHistory();
        void ChangePassword(Admin admin);
        void ChangeInformation(Admin admin);
    }
}