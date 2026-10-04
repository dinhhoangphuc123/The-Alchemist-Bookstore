<div align="center">

# THE ALCHEMIST BOOKSTORE

### Hệ thống Website bán sách trực tuyến

**ASP.NET Core MVC · Entity Framework Core · PostgreSQL**

</div>

---

## 1. Giới thiệu

**The Alchemist Bookstore** là website bán sách trực tuyến được xây dựng bằng **ASP.NET Core MVC**, kết hợp **Entity Framework Core** và **PostgreSQL**.

Hệ thống được phát triển nhằm mô phỏng quy trình bán sách trực tuyến, từ việc quản lý tài khoản, thông tin sách, tồn kho đến đặt hàng, theo dõi đơn hàng và tiếp nhận phản hồi từ khách hàng.

### Mục tiêu của hệ thống

- Xây dựng giao diện website bán sách trực tuyến.
- Quản lý tài khoản và phân quyền người dùng.
- Quản lý thông tin sách và số lượng tồn kho.
- Hỗ trợ khách hàng tạo và theo dõi đơn hàng.
- Cung cấp khu vực quản trị cho Admin.
- Đảm bảo dữ liệu được lưu trữ và xử lý tập trung trên PostgreSQL.

---

## 2. Tính năng nổi bật

### 2.1. Đối với khách hàng

| Chức năng | Mô tả |
|---|---|
| Đăng ký tài khoản | Tạo tài khoản mới và lưu thông tin người dùng |
| Đăng nhập | Xác thực tài khoản bằng Cookie Authentication |
| Xem sách | Xem thông tin, giá bán, giá khuyến mãi và tồn kho |
| Đặt hàng | Tạo đơn hàng với số lượng sách mong muốn |
| Theo dõi đơn hàng | Xem danh sách và trạng thái các đơn hàng của tài khoản |
| Phản hồi | Gửi đánh giá và phản hồi về sản phẩm |

### 2.2. Đối với Admin

| Chức năng | Mô tả |
|---|---|
| Quản lý đơn hàng | Xem và cập nhật trạng thái đơn hàng |
| Quản lý tồn kho | Kiểm soát số lượng sách trong kho |
| Quản lý phản hồi | Theo dõi và xử lý phản hồi của khách hàng |
| Quản lý hệ thống | Truy cập các chức năng dành riêng cho Admin |

### 2.3. Quy trình đơn hàng

```text
Chờ xác nhận
      ↓
Đã xác nhận
      ↓
Đang giao hàng
      ↓
Hoàn thành
```

Đơn hàng có thể được hủy theo trạng thái cho phép. Khi đơn hàng bị hủy, số lượng sách đã giữ trong kho được hoàn trả.

---

## 3. Cấu trúc thư mục

Project được tổ chức theo mô hình **ASP.NET Core MVC**, tách riêng Controller, Model, Service, View và các tài nguyên phía Client.

```text
The-Alchemist-Bookstore/
│
├── Controllers/
│   ├── AccountController.cs
│   ├── AdminController.cs
│   ├── FeedbackController.cs
│   ├── HomeController.cs
│   └── OrderController.cs
│
├── Models/
│   ├── EF/
│   │   ├── ApplicationDbContext.cs
│   │   └── DbSeeder.cs
│   │
│   ├── Entity/
│   │   ├── Account.cs
│   │   ├── Book.cs
│   │   ├── Feedback.cs
│   │   ├── Order.cs
│   │   ├── OrderStatus.cs
│   │   └── Setting.cs
│   │
│   └── ViewModels/
│       ├── AccountViewModels.cs
│       ├── AdminViewModels.cs
│       ├── FeedbackViewModels.cs
│       └── OrderViewModels.cs
│
├── Services/
│   ├── AccountService.cs
│   ├── AdminOrderService.cs
│   ├── BookService.cs
│   ├── FeedbackService.cs
│   ├── InventoryService.cs
│   ├── OrderService.cs
│   └── SiteSettingsService.cs
│
├── Views/
│   ├── Account/
│   ├── Admin/
│   ├── Feedback/
│   ├── Home/
│   ├── Order/
│   └── Shared/
│
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── lib/
│
├── Tests/
│   └── NhaGiaKim.Tests/
│
├── Database/
│   └── the_alchemist_bookstore.sql
│
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
└── The-Alchemist-Bookstore.csproj
```

---

## 4. Công nghệ sử dụng

| Công nghệ | Vai trò |
|---|---|
| **C#** | Ngôn ngữ lập trình chính |
| **ASP.NET Core MVC** | Framework xây dựng Web Application |
| **Entity Framework Core** | ORM và thao tác với cơ sở dữ liệu |
| **PostgreSQL** | Hệ quản trị cơ sở dữ liệu |
| **Npgsql** | Provider kết nối EF Core với PostgreSQL |
| **Razor View** | Xây dựng giao diện phía Server |
| **HTML / CSS** | Xây dựng và định dạng giao diện |
| **JavaScript** | Xử lý tương tác phía Client |
| **Cookie Authentication** | Xác thực và phân quyền người dùng |
| **xUnit** | Framework kiểm thử |

---

## 5. Cơ sở dữ liệu

Hệ thống sử dụng **PostgreSQL** để lưu trữ dữ liệu.

Database mặc định:

```text
the_alchemist_bookstore
```

Một số nhóm dữ liệu chính:

| Nhóm dữ liệu | Nội dung |
|---|---|
| Account | Tài khoản, thông tin người dùng và quyền |
| Book | Thông tin sách, giá và tồn kho |
| Order | Thông tin đơn hàng |
| Feedback | Đánh giá và phản hồi |
| Setting | Cấu hình của website |

File cơ sở dữ liệu được cung cấp tại:

```text
Database/the_alchemist_bookstore.sql
```

`DbSeeder` hỗ trợ khởi tạo dữ liệu mẫu cho hệ thống sau khi database đã được thiết lập.

---

## 6. Cài đặt và chạy Project

### 6.1. Yêu cầu môi trường

- .NET SDK
- PostgreSQL
- Visual Studio hoặc Visual Studio Code
- Git

### 6.2. Clone Project

```powershell
git clone <repository-url>
cd The-Alchemist-Bookstore
```

### 6.3. Khôi phục Package

```powershell
dotnet restore
```

### 6.4. Thiết lập Database

Tạo database PostgreSQL:

```text
the_alchemist_bookstore
```

Sau đó chạy file SQL:

```text
Database/the_alchemist_bookstore.sql
```

Kiểm tra lại chuỗi kết nối trong:

```text
appsettings.json
appsettings.Development.json
```

### 6.5. Build và chạy

```powershell
dotnet build
dotnet run
```

Sau khi ứng dụng khởi động thành công, truy cập địa chỉ được hiển thị trong Terminal.

---

## 7. Triển khai

Project có thể được triển khai lên môi trường Server/Cloud có hỗ trợ **ASP.NET Core** và **PostgreSQL**.

Quy trình triển khai cơ bản:

```text
GitHub Repository
       ↓
Build Project
       ↓
Configure Environment
       ↓
Configure PostgreSQL
       ↓
Publish ASP.NET Core
       ↓
Run Application
```

### Các thông tin cần cấu hình

| Thành phần | Nội dung |
|---|---|
| Application | ASP.NET Core Web Application |
| Database | PostgreSQL |
| Connection String | Cấu hình theo môi trường triển khai |
| Environment | Development / Production |
| Port | Cấu hình theo Server hoặc Cloud Provider |

Khi triển khai Production, thông tin kết nối database và các dữ liệu nhạy cảm nên được cấu hình bằng **Environment Variables** thay vì lưu trực tiếp trong source code.

---

## 8. Kiểm thử

Project có thư mục kiểm thử riêng:

```text
Tests/
└── NhaGiaKim.Tests/
```

Các nhóm kiểm thử chính:

| Test | Nội dung |
|---|---|
| `BookTests.cs` | Kiểm tra logic liên quan đến giá sách và giảm giá |
| `OrderFormValidationTests.cs` | Kiểm tra dữ liệu nhập của đơn hàng |
| `OrderStatusTests.cs` | Kiểm tra chuyển đổi trạng thái đơn hàng |
| `VndTests.cs` | Kiểm tra định dạng tiền Việt Nam |

Chạy toàn bộ test bằng lệnh:

```powershell
dotnet test
```

Nếu tất cả test đều thành công, Terminal sẽ hiển thị kết quả kiểm thử tương ứng.

---

<div align="center">

**The Alchemist Bookstore**

ASP.NET Core MVC Web Application

</div>
