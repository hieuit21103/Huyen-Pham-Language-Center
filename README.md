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

### Domain Entities

| Module | Entity | Mô tả |
|--------|--------|-------|
| **Users** | `TaiKhoan` | Tài khoản người dùng (đăng nhập, vai trò, trạng thái) |
| | `GiaoVien` | Thông tin giáo viên (họ tên, chuyên môn, trình độ) |
| | `HocVien` | Thông tin học viên (họ tên, ngày sinh, địa chỉ) |
| | `GiaoVu` | Thông tin giáo vụ |
| **Courses** | `KhoaHoc` | Khóa học (tên, mô tả, học phí, thời lượng) |
| | `LopHoc` | Lớp học (tên lớp, sĩ số, khóa học) |
| | `LichHoc` | Lịch học (thứ, giờ, phòng học) |
| | `PhongHoc` | Phòng học (tên phòng, sức chứa) |
| | `PhanCong` | Phân công giảng dạy |
| **Learning** | `DangKyKhoaHoc` | Đăng ký khóa học từ học viên |
| | `DangKyTuVan` | Đăng ký tư vấn từ khách |
| | `ThongBao` | Thông báo |
| | `PhanHoi` | Phản hồi, đánh giá |
| **OnlineExam** | `CauHoi` | Câu hỏi thi |
| | `DapAnCauHoi` | Đáp án câu hỏi |
| | `NhomCauHoi` | Nhóm câu hỏi |
| | `NhomCauHoiChiTiet` | Chi tiết nhóm câu hỏi |
| | `DeThi` | Đề thi |
| | `CauHoiDeThi` | Câu hỏi trong đề thi |
| | `KyThi` | Kỳ thi |
| | `CauHinhKyThi` | Cấu hình kỳ thi |
| | `PhienLamBai` | Phiên làm bài |
| | `CauTraLoi` | Câu trả lời của học viên |
| **Finance** | `ThanhToan` | Thanh toán học phí (VNPay) |
| **System** | `CauHinhHeThong` | Cấu hình hệ thống |
| | `NhatKyHeThong` | Nhật ký hoạt động |
| | `SaoLuuDuLieu` | Sao lưu dữ liệu |

### Enums (Các trạng thái)

| Enum | Giá trị |
|------|---------|
| `VaiTro` | `admin`, `giaovu`, `giaovien`, `hocvien` |
| `TrangThaiTaiKhoan` | `hoatdong`, `tamdung`, `bikhoa` |
| `TrangThaiKhoaHoc` | `dangmo`, `dangdienra`, `ketthuc`, `huy` |
| `TrangThaiLopHoc` | `choxepgiaovien`, `danghoc`, `ketthuc`, `huy` |
| `TrangThaiHocVien` | `danghoc`, `tamngung`, `dahoanthanh` |
| `TrangThaiDangKy` | `choduyet`, `daduyet`, `daxeplop`, `danghoc`, `hoantat`, `huy` |
| `TrangThaiThanhToan` | `chuathanhtoan`, `dathanhtoan`, `thatbai` |
| `GioiTinh` | `nam`, `nu` |
| `LoaiCauHoi` | `TracNghiem`, `TuLuan` |
| `MucDo` | `de`, `trungbinh`, `kho` |

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
│   ├── MsHuyenLC.API/                     # 🎯 Presentation Layer
│   │   ├── Controller/
│   │   │   ├── Auth/
│   │   │   │   └── AuthController.cs      # Xác thực (login, logout, reset password)
│   │   │   ├── Courses/
│   │   │   │   ├── KhoaHocController.cs   # ✅ CRUD khóa học
│   │   │   │   ├── LopHocController.cs    # ✅ CRUD lớp học
│   │   │   │   ├── LichHocController.cs   # ✅ CRUD lịch học
│   │   │   │   ├── PhongHocController.cs  # ✅ CRUD phòng học
│   │   │   │   └── PhanCongController.cs  # ✅ Phân công giáo viên
│   │   │   ├── Finance/
│   │   │   │   └── ThanhToanController.cs # ✅ Thanh toán (VNPay)
│   │   │   ├── Learning/
│   │   │   │   ├── CauHoiController.cs    # ✅ Quản lý câu hỏi
│   │   │   │   ├── DangKyKhoaHocController.cs # ✅ Đăng ký khóa học
│   │   │   │   ├── DangKyTuVanController.cs # ✅ Đăng ký tư vấn
│   │   │   │   ├── DeThiController.cs     # ✅ Quản lý đề thi
│   │   │   │   ├── KyThiController.cs     # ✅ Quản lý kỳ thi
│   │   │   │   ├── NhomCauHoiController.cs # ✅ Nhóm câu hỏi
│   │   │   │   ├── PhanHoiController.cs   # ✅ Phản hồi, đánh giá
│   │   │   │   ├── PhienLamBaiController.cs # ✅ Phiên làm bài thi
│   │   │   │   └── ThongBaoController.cs  # ✅ Thông báo
│   │   │   ├── System/
│   │   │   │   ├── CauHinhHeThongController.cs # ✅ Cấu hình hệ thống
│   │   │   │   ├── SaoLuuDuLieuController.cs  # ✅ Sao lưu dữ liệu
│   │   │   │   └── SystemLoggerController.cs  # ✅ Nhật ký hệ thống
│   │   │   └── Users/
│   │   │       ├── TaiKhoanController.cs  # ✅ CRUD tài khoản (Admin)
│   │   │       ├── ProfileController.cs   # ✅ Quản lý profile cá nhân
│   │   │       ├── GiaoVienController.cs  # ✅ CRUD giáo viên
│   │   │       ├── HocVienController.cs   # ✅ CRUD học viên
│   │   │       └── GiaoVuController.cs    # ✅ CRUD giáo vụ
│   │   ├── UploadController.cs            # ✅ Upload file
│   │   ├── BaseController.cs              # Base controller với GetAll, GetById
│   │   ├── GlobalUsing.cs
│   │   ├── Program.cs                     # Entry point & DI configuration
│   │   ├── MsHuyenLC.API.csproj
│   │   ├── appsettings.json
│   │   └── appsettings.Development.json
│   │
│   ├── MsHuyenLC.Application/             # 💼 Business Layer
│   │   ├── DTOs/
│   │   │   ├── Auth/                      # Login, Register, Password DTOs
│   │   │   ├── Courses/                   # KhoaHoc, LopHoc, LichHoc, PhongHoc, PhanCong
│   │   │   ├── Finance/                   # ThanhToan DTOs
│   │   │   ├── Learning/
│   │   │   │   ├── CauHoi/                # Câu hỏi DTOs
│   │   │   │   ├── DangKyKhoaHoc/         # Đăng ký khóa học DTOs
│   │   │   │   ├── DangKyTuVan/           # Đăng ký tư vấn DTOs
│   │   │   │   ├── DeThi/                 # Đề thi DTOs
│   │   │   │   ├── KetQuaHocTap/          # Kết quả học tập DTOs
│   │   │   │   ├── KyThi/                 # Kỳ thi DTOs
│   │   │   │   ├── NhomCauHoi/            # Nhóm câu hỏi DTOs
│   │   │   │   ├── PhanHoi/               # Phản hồi DTOs
│   │   │   │   ├── PhienLamBai/           # Phiên làm bài DTOs
│   │   │   │   └── ThongBao/              # Thông báo DTOs
│   │   │   ├── System/                    # CauHinhHeThong, SaoLuuDuLieu DTOs
│   │   │   └── Users/                     # TaiKhoan, GiaoVien, HocVien, GiaoVu
│   │   ├── Exceptions/                    # Custom exceptions
│   │   ├── Interfaces/                    # Service & Repository interfaces
│   │   ├── Services/                      # Business logic services
│   │   └── MsHuyenLC.Application.csproj
│   │
│   ├── MsHuyenLC.Domain/                  # 🏛️ Domain Layer (Core)
│   │   ├── Entities/
│   │   │   ├── Courses/
│   │   │   │   ├── KhoaHoc.cs
│   │   │   │   ├── LopHoc.cs
│   │   │   │   ├── LichHoc.cs
│   │   │   │   ├── PhongHoc.cs
│   │   │   │   └── PhanCong.cs
│   │   │   ├── Finance/
│   │   │   │   └── ThanhToan.cs
│   │   │   ├── Learning/
│   │   │   │   ├── DangKyKhoaHoc.cs
│   │   │   │   ├── DangKyTuVan.cs
│   │   │   │   ├── PhanHoi.cs
│   │   │   │   ├── ThongBao.cs
│   │   │   │   └── OnlineExam/            # Module thi trực tuyến
│   │   │   │       ├── CauHinhKyThi.cs
│   │   │   │       ├── CauHoi.cs
│   │   │   │       ├── CauHoiDeThi.cs
│   │   │   │       ├── CauTraLoi.cs
│   │   │   │       ├── DapAnCauHoi.cs
│   │   │   │       ├── DeThi.cs
│   │   │   │       ├── KyThi.cs
│   │   │   │       ├── NhomCauHoi.cs
│   │   │   │       ├── NhomCauHoiChiTiet.cs
│   │   │   │       └── PhienLamBai.cs
│   │   │   ├── System/
│   │   │   │   ├── CauHinhHeThong.cs
│   │   │   │   ├── NhatKyHeThong.cs
│   │   │   │   └── SaoLuuDuLieu.cs
│   │   │   └── Users/
│   │   │       ├── TaiKhoan.cs
│   │   │       ├── GiaoVien.cs
│   │   │       ├── HocVien.cs
│   │   │       └── GiaoVu.cs
│   │   ├── Enums/
│   │   │   └── Enums.cs                   # VaiTro, TrangThai, etc.
│   │   ├── GlobalUsing.cs
│   │   └── MsHuyenLC.Domain.csproj
│   │
│   └── MsHuyenLC.Infrastructure/          # 🔧 Infrastructure Layer
│       ├── Persistence/
│       │   ├── ApplicationDbContext.cs    # EF Core DbContext
│       │   └── Seed/                      # Data seeding
│       ├── Repositories/                  # Generic & specific repositories
│       ├── Services/
│       │   ├── JwtService.cs              # JWT token generation
│       │   ├── TokenService.cs            # Password reset tokens
│       │   ├── PasswordHasher.cs          # BCrypt hashing
│       │   ├── VNPayService.cs            # VNPay payment integration
│       │   ├── Email/                     # SMTP email service
│       │   └── Excel/                     # Excel export service
│       ├── Templates/
│       │   ├── Email/                     # Email HTML templates
│       │   ├── EmailTemplateHelper.cs
│       │   └── README.md
│       ├── Migrations/                    # EF Core migrations
│       ├── GlobalUsing.cs
│       └── MsHuyenLC.Infrastructure.csproj
│
├── Dockerfile                             # Multi-stage Docker build
├── docker-compose.yml                     # Docker orchestration
├── .dockerignore
├── .env.example                           # Environment template
├── .gitignore
├── .gitattributes
├── MsHuyenLC.sln                          # Solution file
└── README.md                              # Tài liệu này
```

## 🎯 Chức năng chính

### ✅ Chức năng đã hoàn thành

#### 🔐 1. Xác thực & Phân quyền (Authentication & Authorization)

**Controllers**: `AuthController`, `ProfileController`, `TaiKhoanController`

| Tính năng | Endpoint | Trạng thái |
|-----------|----------|------------|
| Đăng nhập | `POST /api/Auth/login` | ✅ Hoàn thành |
| Đăng xuất | `POST /api/Auth/logout` | ✅ Hoàn thành |
| Đổi mật khẩu | `POST /api/Auth/change-password` | ✅ Hoàn thành |
| Quên mật khẩu | `POST /api/Auth/reset-password` | ✅ Hoàn thành |
| Xác nhận reset | `POST /api/Auth/reset-password/confirm` | ✅ Hoàn thành |
| Xem profile | `GET /api/profile` | ✅ Hoàn thành |
| Cập nhật profile | `PUT /api/profile` | ✅ Hoàn thành |
| CRUD Tài khoản (Admin) | `/api/TaiKhoan` | ✅ Hoàn thành |
| Tìm kiếm tài khoản | `GET /api/TaiKhoan/search` | ✅ Hoàn thành |

#### 📚 2. Quản lý Khóa học (Course Management)

**Controller**: `KhoaHocController`

| Tính năng | Endpoint | Trạng thái |
|-----------|----------|------------|
| Danh sách khóa học | `GET /api/KhoaHoc` | ✅ Hoàn thành |
| Chi tiết khóa học | `GET /api/KhoaHoc/{id}` | ✅ Hoàn thành |
| Tạo khóa học | `POST /api/KhoaHoc` | ✅ Hoàn thành |
| Cập nhật khóa học | `PUT /api/KhoaHoc/{id}` | ✅ Hoàn thành |
| Xóa khóa học | `DELETE /api/KhoaHoc/{id}` | ✅ Hoàn thành |
| Sắp xếp & phân trang | Query params | ✅ Hoàn thành |

#### 🏫 3. Quản lý Lớp học (Class Management)

**Controller**: `LopHocController`

| Tính năng | Endpoint | Trạng thái |
|-----------|----------|------------|
| Danh sách lớp học | `GET /api/LopHoc` | ✅ Hoàn thành |
| Chi tiết lớp học | `GET /api/LopHoc/{id}` | ✅ Hoàn thành |
| Tạo lớp học | `POST /api/LopHoc` | ✅ Hoàn thành |
| Cập nhật lớp học | `PUT /api/LopHoc/{id}` | ✅ Hoàn thành |
| Xóa lớp học | `DELETE /api/LopHoc/{id}` | ✅ Hoàn thành |
| Danh sách học viên trong lớp | `GET /api/LopHoc/{id}/students` | ✅ Hoàn thành |

#### 📅 4. Quản lý Lịch học (Schedule Management)

**Controller**: `LichHocController`

| Tính năng | Endpoint | Trạng thái |
|-----------|----------|------------|
| Danh sách lịch học | `GET /api/LichHoc` | ✅ Hoàn thành |
| Chi tiết lịch học | `GET /api/LichHoc/{id}` | ✅ Hoàn thành |
| Lịch theo lớp | `GET /api/LichHoc/class/{classId}` | ✅ Hoàn thành |
| Lịch theo giáo viên | `GET /api/LichHoc/teacher/{teacherId}` | ✅ Hoàn thành |
| Lịch theo học viên | `GET /api/LichHoc/student/{studentId}` | ✅ Hoàn thành |
| Phòng trống | `GET /api/LichHoc/available-rooms` | ✅ Hoàn thành |
| Tạo lịch học | `POST /api/LichHoc` | ✅ Hoàn thành |
| Cập nhật lịch học | `PUT /api/LichHoc/{id}` | ✅ Hoàn thành |
| Xóa lịch học | `DELETE /api/LichHoc/{id}` | ✅ Hoàn thành |

#### 🏢 5. Quản lý Phòng học (Room Management)

**Controller**: `PhongHocController`

| Tính năng | Endpoint | Trạng thái |
|-----------|----------|------------|
| Danh sách phòng học | `GET /api/PhongHoc` | ✅ Hoàn thành |
| Chi tiết phòng học | `GET /api/PhongHoc/{id}` | ✅ Hoàn thành |
| Tạo phòng học | `POST /api/PhongHoc` | ✅ Hoàn thành |
| Cập nhật phòng học | `PUT /api/PhongHoc/{id}` | ✅ Hoàn thành |
| Xóa phòng học | `DELETE /api/PhongHoc/{id}` | ✅ Hoàn thành |

#### 👨‍🏫 6. Quản lý Phân công Giảng dạy

**Controller**: `PhanCongController`

| Tính năng | Endpoint | Trạng thái |
|-----------|----------|------------|
| Danh sách phân công | `GET /api/PhanCong` | ✅ Hoàn thành |
| Phân công giáo viên | `POST /api/PhanCong` | ✅ Hoàn thành |
| Lớp theo giáo viên | `GET /api/PhanCong/giaovien/{id}` | ✅ Hoàn thành |
| Hủy phân công | `DELETE /api/PhanCong/{id}` | ✅ Hoàn thành |

#### 👥 7. Quản lý Người dùng (User Management)

**Controllers**: `GiaoVienController`, `HocVienController`, `GiaoVuController`

| Tính năng | Endpoint | Trạng thái |
|-----------|----------|------------|
| Danh sách giáo viên | `GET /api/GiaoVien` | ✅ Hoàn thành |
| Chi tiết giáo viên | `GET /api/GiaoVien/{id}` | ✅ Hoàn thành |
| Tạo giáo viên | `POST /api/GiaoVien` | ✅ Hoàn thành |
| Cập nhật giáo viên | `PUT /api/GiaoVien/{id}` | ✅ Hoàn thành |
| Vô hiệu hóa giáo viên | `DELETE /api/GiaoVien/{id}` | ✅ Hoàn thành |
| Danh sách học viên | `GET /api/HocVien` | ✅ Hoàn thành |
| Chi tiết học viên | `GET /api/HocVien/{id}` | ✅ Hoàn thành |
| Cập nhật học viên | `PUT /api/HocVien/{id}` | ✅ Hoàn thành |
| Xóa học viên | `DELETE /api/HocVien/{id}` | ✅ Hoàn thành |
| CRUD Giáo vụ | `/api/GiaoVu` | ✅ Hoàn thành |

#### 📝 8. Đăng ký & Tư vấn

**Controllers**: `DangKyKhoaHocController`, `DangKyTuVanController`

| Tính năng | Endpoint | Trạng thái |
|-----------|----------|------------|
| Đăng ký khóa học | `POST /api/DangKyKhoaHoc` | ✅ Hoàn thành |
| Danh sách đăng ký | `GET /api/DangKyKhoaHoc` | ✅ Hoàn thành |
| Duyệt đăng ký | `PUT /api/DangKyKhoaHoc/{id}` | ✅ Hoàn thành |
| Đăng ký tư vấn (khách) | `POST /api/DangKyTuVan` | ✅ Hoàn thành |
| Danh sách tư vấn | `GET /api/DangKyTuVan` | ✅ Hoàn thành |
| Xử lý yêu cầu tư vấn | `PUT /api/DangKyTuVan/{id}` | ✅ Hoàn thành |

#### 💳 9. Thanh toán (Payment)

**Controller**: `ThanhToanController`

| Tính năng | Endpoint | Trạng thái |
|-----------|----------|------------|
| Tạo giao dịch | `POST /api/ThanhToan` | ✅ Hoàn thành |
| Danh sách thanh toán | `GET /api/ThanhToan` | ✅ Hoàn thành |
| Chi tiết thanh toán | `GET /api/ThanhToan/{id}` | ✅ Hoàn thành |
| Tích hợp VNPay | VNPay Gateway | ✅ Hoàn thành |

#### 📋 10. Hệ thống Thi trực tuyến (Online Exam)

**Controllers**: `CauHoiController`, `NhomCauHoiController`, `DeThiController`, `KyThiController`, `PhienLamBaiController`

| Tính năng | Endpoint | Trạng thái |
|-----------|----------|------------|
| Quản lý câu hỏi | `/api/CauHoi` | ✅ Hoàn thành |
| Nhóm câu hỏi | `/api/NhomCauHoi` | ✅ Hoàn thành |
| Tạo đề thi | `/api/DeThi` | ✅ Hoàn thành |
| Quản lý kỳ thi | `/api/KyThi` | ✅ Hoàn thành |
| Phiên làm bài | `/api/PhienLamBai` | ✅ Hoàn thành |
| Nộp bài & chấm điểm | `/api/PhienLamBai/submit` | ✅ Hoàn thành |

#### 📢 11. Thông báo & Phản hồi

**Controllers**: `ThongBaoController`, `PhanHoiController`

| Tính năng | Endpoint | Trạng thái |
|-----------|----------|------------|
| Gửi thông báo | `POST /api/ThongBao` | ✅ Hoàn thành |
| Danh sách thông báo | `GET /api/ThongBao` | ✅ Hoàn thành |
| Tạo phản hồi | `POST /api/PhanHoi` | ✅ Hoàn thành |
| Danh sách phản hồi | `GET /api/PhanHoi` | ✅ Hoàn thành |

#### ⚙️ 12. Quản trị Hệ thống

**Controllers**: `CauHinhHeThongController`, `SaoLuuDuLieuController`, `SystemLoggerController`, `UploadController`

| Tính năng | Endpoint | Trạng thái |
|-----------|----------|------------|
| Cấu hình hệ thống | `/api/CauHinhHeThong` | ✅ Hoàn thành |
| Sao lưu dữ liệu | `/api/SaoLuuDuLieu` | ✅ Hoàn thành |
| Nhật ký hệ thống | `/api/SystemLogger` | ✅ Hoàn thành |
| Upload file | `/api/Upload` | ✅ Hoàn thành |

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
- `GET /api/LichHoc/available-rooms` - Phòng trống theo lịch
- `POST /api/LichHoc` - Tạo lịch học
- `PUT /api/LichHoc/{id}` - Cập nhật lịch học
- `DELETE /api/LichHoc/{id}` - Xóa lịch học

#### Room Management
- `GET /api/PhongHoc` - Danh sách phòng học
- `POST /api/PhongHoc` - Tạo phòng học
- `PUT /api/PhongHoc/{id}` - Cập nhật phòng học
- `DELETE /api/PhongHoc/{id}` - Xóa phòng học

#### Assignment Management
- `GET /api/PhanCong` - Danh sách phân công
- `POST /api/PhanCong` - Phân công giáo viên vào lớp
- `GET /api/PhanCong/giaovien/{id}` - Các lớp của giáo viên
- `DELETE /api/PhanCong/{id}` - Hủy phân công

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
- [ ] Dashboard thống kê
- [ ] Tích hợp thêm cổng thanh toán (MoMo, ZaloPay)
- [ ] Push notification
- [ ] SMS notification

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

