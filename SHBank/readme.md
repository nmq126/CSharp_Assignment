###Chức năng
- Khách hàng:
  - Đăng ký
  - Đăng nhập
  - Xem thông tin ngân hàng
  - Gửi phản hồi ngân hàng
  - Sau khi đăng nhập kh gửi tiền vào nh
  - Sau khi đăng nhập kh rút tiền khỏi nh
  - Sau khi đăng nhập kh thực hiện chuyển tiền trong nh
  - Sau khi đăng nhập kh tra cứu lịch sử gd
  - Sau khi đăng nhập kh tra cứu thông tin cá nhân
  - Sau khi đăng nhập kh chỉnh sửa thông tin cá nhân
  - Option: đóng tài khoản
- Administrator:
  - Đăng nhập
  - Duyệt/từ chối tài khoản vừa đăng ký
  - Tạo tài khoản admin mới
  - Thay đổi trạng thái tài khoản kh
  - Tìm kiếm thông tin khách hàng theo stk
  - Tìm kiếm thông tin khách hàng theo sđt
  - Tìm kiếm thông tin khách hàng theo cmnd, cccd
  - Kiểm tra lịch sử giao dịch theo stk
###Usecase 
  - Actors: guest, user, admin
###Entities
  - Admin
    - Id
    - Username (String)
    - PasswordHash (string)
    - Salt (String)
    - Fullname
    - Phone
    - CreatedAt (DateTime)
    - UpdatedAt (DateTime)
    - DeletedAt (DateTime)
    - Status (int) 0 active 1 lock -1 deleted

  - Account
    - AccountNumber (string)
    - Type (int)
    - Balance (double)
    - Username (string)
    - PasswordHash (string)
    - Salt (string)
    
    - First name (string)
    - Last name (string)
    - Gender (int)
    - Dob (DateTime)
    - Email (string)
    - Address (string)
    
    - CreatedAt (DateTime)
    - UpdatedAt (DateTime)
    - DeletedAt (DateTime)
    - Status (int) 1 unlock -1 lock
  - TransactionHistory
    - Id (string)
    - Type (int): withdraw 1, deposit 2, transfer 3
    - Amount (double)
    - Message (string)
    - SenderAccountNumber (FK)
    - ReceiverAccountNumber (FK)
    - CreatedAt (DateTime)
    - UpdatedAt (DateTime)
    - DeletedAt (DateTime)
    - Status (int) 1 thành công 2 pending 3 thất bại
  - BankInformation (thay = file txt)
  - Admin

https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqltransaction.commit?view=dotnet-plat-ext-5.0