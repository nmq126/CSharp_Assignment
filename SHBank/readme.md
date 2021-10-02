### Database
CREATE TABLE `accounts` (
  `account_number` varchar(100) NOT NULL,
  `username` varchar(100) NOT NULL,
  `password_hash` varchar(250) NOT NULL,
  `salt` varchar(7) NOT NULL,
  `balance` double DEFAULT 0,
  `createdAt` datetime NOT NULL DEFAULT current_timestamp(),
  `updatedAt` datetime NOT NULL DEFAULT current_timestamp(),
  `deletedAt` datetime NOT NULL DEFAULT current_timestamp(),
  `status` int(11) NOT NULL DEFAULT 1,
  PRIMARY KEY (`account_number`),
  UNIQUE KEY `username` (`username`),
  KEY `account_number` (`account_number`),
  KEY `account_number_2` (`account_number`),
  CONSTRAINT `FK_people_accounts` FOREIGN KEY (`username`) REFERENCES `people` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4
CREATE TABLE `admins` (
  `id` varchar(100) NOT NULL,
  `username` varchar(100) NOT NULL,
  `password_hash` varchar(250) NOT NULL,
  `salt` varchar(7) NOT NULL,
  `fullname` varchar(100) NOT NULL,
  `phone` varchar(100) NOT NULL,
  `status` int(11) NOT NULL,
  `createdAt` datetime NOT NULL DEFAULT current_timestamp(),
  `updatedAt` datetime NOT NULL DEFAULT current_timestamp(),
  `deletedAt` datetime NOT NULL DEFAULT current_timestamp(),
  PRIMARY KEY (`id`),
  UNIQUE KEY `username` (`username`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4
CREATE TABLE `people` (
  `id` varchar(100) NOT NULL,
  `first_name` varchar(50) NOT NULL,
  `last_name` varchar(50) NOT NULL,
  `gender` int(11) NOT NULL,
  `dob` datetime NOT NULL,
  `email` varchar(100) NOT NULL,
  `address` varchar(250) NOT NULL,
  `phone` varchar(100) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4
CREATE TABLE `transactions` (
  `id` varchar(100) NOT NULL,
  `type` int(11) NOT NULL,
  `amount` double NOT NULL,
  `message` varchar(100) NOT NULL,
  `sender_account_number` varchar(100) NOT NULL,
  `receiver_account_number` varchar(100) NOT NULL,
  `createdAt` datetime NOT NULL DEFAULT current_timestamp(),
  `updatedAt` datetime NOT NULL DEFAULT current_timestamp(),
  `deletedAt` datetime NOT NULL DEFAULT current_timestamp(),
  `status` int(11) NOT NULL DEFAULT 1,
  PRIMARY KEY (`id`),
  KEY `sender_account_number` (`sender_account_number`,`receiver_account_number`),
  KEY `sender_account_number_2` (`sender_account_number`,`receiver_account_number`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4

### Chức năng
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
