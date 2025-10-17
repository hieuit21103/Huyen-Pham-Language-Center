# 🎓 MsHuyenLC - Hệ Thống Quản Lý Đào Tạo Và Thi Trực Tuyến

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Latest-336791?style=flat-square&logo=postgresql)](https://www.postgresql.org/)
[![Redis](https://img.shields.io/badge/Redis-Latest-DC382D?style=flat-square&logo=redis)](https://redis.io/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?style=flat-square&logo=docker)](https://www.docker.com/)

## 📋 Mục lục

- [Giới thiệu](#-giới-thiệu)
- [Kiến trúc hệ thống](#-kiến-trúc-hệ-thống)
- [Công nghệ sử dụng](#️-công-nghệ-sử-dụng)
- [Cấu trúc dự án](#-cấu-trúc-dự-án)
- [Chức năng chính](#-chức-năng-chính)
- [Yêu cầu hệ thống](#-yêu-cầu-hệ-thống)
- [Hướng dẫn cài đặt](#-hướng-dẫn-cài-đặt)
- [Cấu hình môi trường](#️-cấu-hình-môi-trường)
- [Triển khai với Docker](#-triển-khai-với-docker)
- [API Documentation](#-api-documentation)
- [Phát triển trong tương lai](#-phát-triển-trong-tương-lai)

## 📖 Giới thiệu

**MsHuyenLC** là một hệ thống quản lý trung tâm đào tạo toàn diện được xây dựng trên nền tảng **.NET 9.0**, áp dụng kiến trúc **Clean Architecture**. Hệ thống cung cấp các tính năng quản lý khóa học, lớp học, giáo viên, học viên, lịch học, thanh toán và nhiều tính năng khác phục vụ cho việc vận hành một trung tâm đào tạo hiện đại.

### ✨ Điểm nổi bật

- ✅ **Kiến trúc Clean Architecture** - Dễ bảo trì, mở rộng và kiểm thử
- ✅ **RESTful API** - Thiết kế API chuẩn REST với Swagger/OpenAPI
- ✅ **Bảo mật cao** - JWT Authentication, phân quyền role-based
- ✅ **Hiệu năng tối ưu** - Sử dụng Redis cache, Entity Framework Core
- ✅ **Email Service** - Gửi email tự động (reset password, thông báo)
- ✅ **Docker Ready** - Triển khai nhanh chóng với Docker & Docker Compose
- ✅ **Database Migration** - Quản lý schema database với EF Core Migrations

## 🏗️ Kiến trúc hệ thống

Dự án áp dụng **Clean Architecture** (Onion Architecture) với 4 layer chính:

```
┌─────────────────────────────────────────────┐
│         MsHuyenLC.API (Presentation)        │
│  • Controllers                              │
│  • Middleware                               │
│  • Program.cs (DI Container)                │
└─────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────┐
│      MsHuyenLC.Application (Business)       │
│  • Services                                 │
│  • DTOs                                     │
│  • Interfaces                               │
│  • Exceptions                               │
└─────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────┐
│         MsHuyenLC.Domain (Core)             │
│  • Entities                                 │
│  • Enums                                    │
│  • Value Objects                            │
└─────────────────────────────────────────────┘
                    ↑
┌─────────────────────────────────────────────┐
│    MsHuyenLC.Infrastructure (External)      │
│  • DbContext (PostgreSQL)                   │
│  • Repositories                             │
│  • External Services (Email, JWT, Redis)    │
│  • Migrations                               │
└─────────────────────────────────────────────┘
```

### Nguyên tắc Clean Architecture

- **Domain Layer**: Chứa các entity và business logic core, không phụ thuộc vào layer nào
- **Application Layer**: Chứa business logic, DTOs, interfaces, phụ thuộc vào Domain
- **Infrastructure Layer**: Implement các interface từ Application, xử lý database, external services
- **API Layer**: Presentation layer, phụ thuộc vào tất cả các layer khác

## 🛠️ Công nghệ sử dụng

### Backend Framework & Language
- **.NET 9.0** - Framework chính
- **C# 12.0** - Ngôn ngữ lập trình
- **ASP.NET Core Web API** - Xây dựng RESTful API

### Database & Caching
- **PostgreSQL** - Database chính (RDBMS)
- **Entity Framework Core 9.0** - ORM
- **Redis** - Caching & Session management

### Authentication & Security
- **JWT (JSON Web Tokens)** - Authentication
- **BCrypt** - Password hashing
- **Role-based Authorization** - Phân quyền người dùng

### Documentation & Testing
- **Swagger/OpenAPI** - API Documentation
- **Swagger UI** - Interactive API testing

### Email Service
- **SMTP** - Gửi email (reset password, notifications)

### DevOps & Deployment
- **Docker** - Containerization
- **Docker Compose** - Multi-container orchestration

### Libraries & Packages
- **StackExchange.Redis** - Redis client for .NET
- **Npgsql.EntityFrameworkCore.PostgreSQL** - PostgreSQL provider for EF Core
- **Microsoft.AspNetCore.Authentication.JwtBearer** - JWT authentication

## 📁 Cấu trúc dự án

```
MsHuyenLC/
├── src/
│   ├── MsHuyenLC.API/                 # Presentation Layer
│   │   ├── Controller/
│   │   │   ├── Auth/                  # Authentication endpoints
│   │   │   ├── Courses/               # Course management
│   │   │   ├── Finance/               # Payment management
│   │   │   ├── Learning/              # Learning activities
│   │   │   ├── System/                # System management
│   │   │   └── Users/                 # User management
│   │   └── Program.cs                 # Application entry point
│   │
│   ├── MsHuyenLC.Application/         # Business Layer
│   │   ├── DTOs/                      # Data Transfer Objects
│   │   │   ├── Auth/                  # Login, Register, Password
│   │   │   ├── Courses/               # Course, Class, Schedule
│   │   │   └── Users/                 # User profiles
│   │   ├── Interfaces/                # Service interfaces
│   │   ├── Services/                  # Business logic services
│   │   └── Exceptions/                # Custom exceptions
│   │
│   ├── MsHuyenLC.Domain/              # Domain Layer
│   │   ├── Entities/                  # Domain entities
│   │   │   ├── Courses/               # KhoaHoc, LopHoc, LichHoc
│   │   │   ├── Finance/               # ThanhToan
│   │   │   ├── Learning/              # DangKy, BaiThi, KetQua
│   │   │   ├── System/                # NhatKy, SaoLuu
│   │   │   └── Users/                 # TaiKhoan, GiaoVien, HocVien
│   │   └── Enums/                     # Enumerations
│   │
│   └── MsHuyenLC.Infrastructure/      # Infrastructure Layer
│       ├── Persistence/               # Database context
│       │   └── Seed/                  # Initial data seeding
│       ├── Repositories/              # Data access
│       ├── Services/                  # External services
│       │   ├── Auth/                  # JWT, Password hashing
│       │   └── Email/                 # Email service
│       ├── Migrations/                # EF Core migrations
│       └── Templates/                 # Email templates
│
├── Dockerfile                         # Docker image definition
├── docker-compose.yml                 # Docker orchestration
├── .env.example                       # Environment variables template
├── MsHuyenLC.sln                      # Solution file
└── README.md                          # Documentation (file này)
```

## 🎯 Chức năng chính

### 🔐 1. Xác thực & Phân quyền (Authentication & Authorization)

**Controller**: `AuthController`, `ProfileController`, `TaiKhoanController`

- ✅ **Đăng nhập** - JWT token-based authentication
- ✅ **Đăng xuất** - Invalidate token/session
- ✅ **Đổi mật khẩu** - Change password cho user đã đăng nhập
- ✅ **Quên mật khẩu** - Reset password qua email
- ✅ **Xác nhận reset password** - Confirm token và đặt mật khẩu mới
- ✅ **Profile management** - Xem và cập nhật thông tin cá nhân
- ✅ **Phân quyền role-based** - Admin, GiaoVu, GiaoVien, HocVien

### 📚 2. Quản lý Khóa học (Course Management)

**Controller**: `KhoaHocController`

- ✅ **CRUD Khóa học** - Tạo, xem, sửa, xóa khóa học
- ✅ **Thông tin khóa học** - Tên, mô tả, học phí, thời lượng, ngày khai giảng
- ✅ **Tìm kiếm & lọc** - Sắp xếp theo tên, học phí, ngày khai giảng
- ✅ **Phân quyền** - Chỉ admin, giáo vụ được quản lý

### 🏫 3. Quản lý Lớp học (Class Management)

**Controller**: `LopHocController`

- ✅ **CRUD Lớp học** - Tạo, xem, sửa, xóa lớp học
- ✅ **Thông tin lớp học** - Tên lớp, khóa học, sĩ số, trạng thái
- ✅ **Danh sách học viên** - Xem học viên trong lớp
- ✅ **Quản lý lớp theo khóa học** - Liên kết với khóa học

### 📅 4. Quản lý Lịch học (Schedule Management)

**Controller**: `LichHocController`

- ✅ **CRUD Lịch học** - Tạo, xem, sửa, xóa lịch học
- ✅ **Lịch theo lớp** - Xem lịch học của một lớp
- ✅ **Lịch theo giáo viên** - Xem lịch dạy của giáo viên
- ✅ **Lịch theo học viên** - Xem lịch học của học viên
- ✅ **Thông tin chi tiết** - Ngày, giờ, phòng học, nội dung

### 🏢 5. Quản lý Phòng học (Room Management)

**Controller**: `PhongHocController`

- ✅ **CRUD Phòng học** - Tạo, xem, sửa, xóa phòng học
- ✅ **Thông tin phòng** - Tên phòng, sức chứa, thiết bị
- ✅ **Kiểm tra phòng trống** - API kiểm tra phòng khả dụng

### 👨‍🏫 6. Quản lý Phân công (Assignment Management)

**Controller**: `PhanCongController`

- ✅ **CRUD Phân công** - Phân công giáo viên dạy lớp
- ✅ **Xem phân công theo giáo viên** - Các lớp mà giáo viên đang dạy
- ✅ **Quản lý giảng dạy** - Thời gian bắt đầu, kết thúc

### 👥 7. Quản lý Người dùng (User Management)

**Controllers**: `GiaoVienController`, `HocVienController`, `GiaoVuController`

- ✅ **Quản lý Giáo viên** - CRUD thông tin giáo viên
- ✅ **Quản lý Học viên** - CRUD thông tin học viên
- ✅ **Tìm kiếm người dùng** - Tìm theo tên, email, số điện thoại
- 🚧 **Quản lý Giáo vụ** - Đang phát triển

### 📊 8. Chức năng đang phát triển

Các controller đã được định nghĩa nhưng chưa triển khai đầy đủ:

- 🚧 **Quản lý Đăng ký** (`DangKyController`) - Đăng ký học từ học viên
- 🚧 **Đăng ký từ khách** (`DangKyKhachController`) - Đăng ký từ người chưa có tài khoản
- 🚧 **Quản lý Thanh toán** (`ThanhToanController`) - Thanh toán học phí, hóa đơn
- 🚧 **Quản lý Đề thi** (`DeThiController`) - Tạo và quản lý đề thi
- 🚧 **Ngân hàng đề** (`NganHangDeController`) - Kho câu hỏi
- 🚧 **Quản lý Kỳ thi** (`KyThiController`) - Lên lịch và tổ chức kỳ thi
- 🚧 **Bài thi** (`BaiThiController`) - Nộp bài, chấm bài
- 🚧 **Kết quả học tập** (`KetQuaHocTapController`) - Quản lý điểm số, kết quả
- 🚧 **Thông báo** (`ThongBaoController`) - Gửi thông báo đến người dùng
- 🚧 **Phản hồi** (`PhanHoiController`) - Phản hồi, đánh giá khóa học
- 🚧 **Nhật ký hệ thống** (`NhatKyHeThongController`) - Logging, audit trail
- 🚧 **Sao lưu dữ liệu** (`SaoLuuDuLieuController`) - Backup & restore

## 💻 Yêu cầu hệ thống

### Phát triển (Development)

- **.NET SDK 9.0** hoặc cao hơn
- **PostgreSQL 14+**
- **Redis 7+**
- **Docker Desktop** (optional, cho môi trường container)
- **Visual Studio 2022** / **VS Code** / **Rider**

### Triển khai (Production)

- **Docker & Docker Compose** (recommended)
- Hoặc máy chủ có cài:
  - .NET Runtime 9.0
  - PostgreSQL 14+
  - Redis 7+

## 🚀 Hướng dẫn cài đặt

### Option 1: Chạy trực tiếp với .NET CLI

#### 1. Clone repository

```bash
git clone https://github.com/hieuit21103/MsHuyenLC.git
cd MsHuyenLC
```

#### 2. Cài đặt dependencies

```bash
dotnet restore
```

#### 3. Cấu hình môi trường

Tạo file `.env` từ template:

```bash
cp .env.example .env
```

Hoặc cấu hình trong `appsettings.json` / `appsettings.Development.json`

#### 4. Cấu hình Database

Đảm bảo PostgreSQL đang chạy và cập nhật connection string trong `.env` hoặc `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=MsHuyenLCDb;Username=postgres;Password=your_password"
  }
}
```

#### 5. Chạy Migration

```bash
cd src/MsHuyenLC.API
dotnet ef database update
```

#### 6. Khởi động ứng dụng

```bash
dotnet run
```

API sẽ chạy tại: `http://localhost:5000` (hoặc cổng được cấu hình)

Swagger UI: `http://localhost:5000/swagger`

### Option 2: Chạy với Visual Studio / Rider

1. Mở file `MsHuyenLC.sln` bằng IDE
2. Cấu hình connection string trong `appsettings.Development.json`
3. Set **MsHuyenLC.API** làm startup project
4. Chạy migration: Open Package Manager Console và chạy `Update-Database`
5. Nhấn **F5** để chạy

## ⚙️ Cấu hình môi trường

### File `.env` (Recommended)

```bash
# Database
DB_CONNECTION_STRING=Host=localhost;Port=5432;Database=MsHuyenLCDb;Username=postgres;Password=YourSecurePassword123!

# Redis
REDIS_CONNECTION_STRING=localhost:6379,user=default,password=YourRedisPassword123!
REDIS_INSTANCE_NAME=MsHuyenLC:

# JWT Authentication
JWT_SECRET_KEY=YourSuperSecretKeyMinimum32CharactersLongForJWT!@#
JWT_ISSUER=MsHuyenLC
JWT_AUDIENCE=MsHuyenLC
JWT_EXPIRATION_MINUTES=60

# Token Settings
PASSWORD_RESET_EXPIRATION_MINUTES=15
EMAIL_CONFIRMATION_EXPIRATION_HOURS=24

# Email Service (SMTP)
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=your-email@gmail.com
SMTP_PASSWORD=your-app-password
SMTP_FROM_EMAIL=noreply@your-domain.com
SMTP_FROM_NAME=MsHuyenLC

# Application
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:5000
```

### Các biến môi trường quan trọng

| Biến | Mô tả | Giá trị mặc định |
|------|-------|------------------|
| `DB_CONNECTION_STRING` | PostgreSQL connection string | - |
| `REDIS_CONNECTION_STRING` | Redis connection string | - |
| `JWT_SECRET_KEY` | Secret key cho JWT (tối thiểu 32 ký tự) | - |
| `JWT_EXPIRATION_MINUTES` | Thời gian hết hạn token (phút) | 60 |
| `SMTP_HOST` | SMTP server host | smtp.gmail.com |
| `SMTP_PORT` | SMTP server port | 587 |
| `ASPNETCORE_ENVIRONMENT` | Environment (Development/Production) | Development |

## 🐳 Triển khai với Docker

### Chuẩn bị

1. **Cài đặt Docker & Docker Compose**
   - [Docker Desktop](https://www.docker.com/products/docker-desktop) (Windows/Mac)
   - Docker Engine (Linux)

2. **Tạo file `.env`** từ template:

```bash
cp .env.example .env
```

3. **Cấu hình external database** trong `.env`:

```bash
# External PostgreSQL
DB_CONNECTION_STRING=Host=your-postgres-host;Port=5432;Database=MsHuyenLCDb;Username=postgres;Password=YourPassword

# External Redis
REDIS_CONNECTION_STRING=your-redis-host:6379,password=YourRedisPassword
```

### Chạy với Docker Compose

```bash
# Build và chạy container
docker-compose up -d

# Xem logs
docker-compose logs -f api

# Dừng container
docker-compose down

# Rebuild image
docker-compose up -d --build
```

### API Endpoint sau khi chạy

- **API**: `http://localhost:5000`
- **Swagger**: `http://localhost:5000/swagger`
- **Health Check**: `http://localhost:5000/health`

### Build Docker Image riêng

```bash
# Build image
docker build -t mshuyenlc-api:latest .

# Run container
docker run -d \
  --name mshuyenlc-api \
  -p 5000:8080 \
  --env-file .env \
  mshuyenlc-api:latest
```

### Lưu ý khi triển khai

- ✅ Đảm bảo PostgreSQL và Redis đang chạy và có thể kết nối được
- ✅ Chạy database migration trước khi khởi động API
- ✅ Sử dụng mật khẩu mạnh cho production
- ✅ Cấu hình HTTPS cho production environment
- ✅ Backup database định kỳ

## 📚 API Documentation

### Swagger UI

Sau khi chạy ứng dụng, truy cập Swagger UI để xem và test API:

```
http://localhost:5000/swagger
```

### Authentication

Hầu hết các API endpoint đều yêu cầu authentication. Để sử dụng:

1. **Login** qua endpoint `/api/Auth/login`:
```json
POST /api/Auth/login
{
  "tenDangNhap": "your_username",
  "matKhau": "your_password"
}
```

2. **Copy JWT token** từ response

3. Trong Swagger UI:
   - Click nút **"Authorize"** (ở góc trên)
   - Nhập: `Bearer {your_token}`
   - Click **"Authorize"** để lưu

4. Giờ bạn có thể gọi các protected endpoints

### Các API Endpoint chính

#### Authentication
- `POST /api/Auth/login` - Đăng nhập
- `POST /api/Auth/logout` - Đăng xuất
- `POST /api/Auth/change-password` - Đổi mật khẩu
- `POST /api/Auth/reset-password` - Quên mật khẩu (gửi email)
- `POST /api/Auth/reset-password/confirm` - Xác nhận reset password

#### Courses Management
- `GET /api/KhoaHoc` - Danh sách khóa học
- `GET /api/KhoaHoc/{id}` - Chi tiết khóa học
- `POST /api/KhoaHoc` - Tạo khóa học mới (Admin/GiaoVu)
- `PUT /api/KhoaHoc/{id}` - Cập nhật khóa học (Admin/GiaoVu)
- `DELETE /api/KhoaHoc/{id}` - Xóa khóa học (Admin/GiaoVu)

#### Class Management
- `GET /api/LopHoc` - Danh sách lớp học
- `GET /api/LopHoc/{id}` - Chi tiết lớp học
- `GET /api/LopHoc/{id}/students` - Danh sách học viên trong lớp
- `POST /api/LopHoc` - Tạo lớp học (Admin/GiaoVu)
- `PUT /api/LopHoc/{id}` - Cập nhật lớp học
- `DELETE /api/LopHoc/{id}` - Xóa lớp học

#### Schedule Management
- `GET /api/LichHoc` - Danh sách lịch học
- `GET /api/LichHoc/class/{classId}` - Lịch học theo lớp
- `GET /api/LichHoc/teacher/{teacherId}` - Lịch dạy theo giáo viên
- `GET /api/LichHoc/student/{studentId}` - Lịch học theo học viên
- `POST /api/LichHoc` - Tạo lịch học
- `PUT /api/LichHoc/{id}` - Cập nhật lịch học
- `DELETE /api/LichHoc/{id}` - Xóa lịch học

#### Room Management
- `GET /api/PhongHoc` - Danh sách phòng học
- `GET /api/PhongHoc/available-rooms` - Phòng trống
- `POST /api/PhongHoc` - Tạo phòng học
- `PUT /api/PhongHoc/{id}` - Cập nhật phòng học
- `DELETE /api/PhongHoc/{id}` - Xóa phòng học

#### User Management
- `GET /api/Profile` - Thông tin cá nhân
- `PUT /api/Profile` - Cập nhật profile
- `GET /api/GiaoVien` - Danh sách giáo viên
- `GET /api/GiaoVien/{id}` - Chi tiết giáo viên
- `GET /api/HocVien` - Danh sách học viên
- `GET /api/HocVien/search` - Tìm kiếm học viên

### Phân quyền (Roles)

| Role | Mô tả | Quyền |
|------|-------|-------|
| **Admin** | Quản trị viên hệ thống | Full quyền truy cập tất cả |
| **GiaoVu** | Giáo vụ | Quản lý khóa học, lớp học, lịch học, phân công |
| **GiaoVien** | Giáo viên | Xem lịch dạy, cập nhật nội dung giảng dạy |
| **HocVien** | Học viên | Xem lịch học, đăng ký khóa học, xem điểm |

## 🔮 Phát triển trong tương lai

### Tính năng sắp triển khai

1. **Hệ thống Thi & Đánh giá**
   - Ngân hàng câu hỏi
   - Tạo đề thi tự động
   - Thi online
   - Chấm tự động
   - Quản lý kết quả học tập

2. **Quản lý Thanh toán**
   - Thanh toán học phí online
   - Quản lý hóa đơn
   - Báo cáo doanh thu
   - Tích hợp payment gateway (VNPay, MoMo)

3. **Đăng ký Khóa học**
   - Đăng ký từ website (cho khách)
   - Đăng ký từ học viên
   - Approval workflow
   - Email xác nhận tự động

4. **Hệ thống Thông báo**
   - Push notification
   - Email notification
   - SMS notification (optional)
   - Lịch sử thông báo

5. **Phản hồi & Đánh giá**
   - Đánh giá khóa học
   - Đánh giá giáo viên
   - Feedback từ học viên
   - Rating system

6. **System Management**
   - Nhật ký hoạt động (Audit Log)
   - Sao lưu & khôi phục dữ liệu
   - Dashboard thống kê
   - Báo cáo hệ thống

### Cải tiến kỹ thuật

- [ ] Unit Tests & Integration Tests
- [ ] CI/CD Pipeline (GitHub Actions)
- [ ] Logging nâng cao (Serilog/ELK Stack)
- [ ] Rate Limiting
- [ ] API Versioning
- [ ] GraphQL API (optional)
- [ ] Background Jobs (Hangfire)
- [ ] Real-time features (SignalR)
- [ ] Monitoring & Alerting (Prometheus, Grafana)

## 🤝 Đóng góp

Mọi đóng góp đều được chào đón! Vui lòng:

1. Fork repository
2. Tạo branch mới (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Mở Pull Request

## 📝 License

Dự án này thuộc về Lê Minh Hiếu.

## 📧 Liên hệ

- **GitHub**: https://github.com/hieuit21103/MsHuyenLC
- **Issues**: https://github.com/hieuit21103/MsHuyenLC/issues

---

**Được phát triển bởi Lê Minh Hiếu**

