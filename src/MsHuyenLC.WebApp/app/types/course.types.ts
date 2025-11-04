// Course Types - Khóa học, Lớp học, Đăng ký

import type { 
  TrangThaiKhoaHoc, 
  TrangThaiLopHoc, 
  TrangThaiDangKy, 
  KetQuaDangKy 
} from './enums';
import type { HocVien } from './user.types';

// Khóa học
export interface KhoaHocRequest {
  tenKhoaHoc?: string;
  moTa?: string;
  hocPhi?: number;
  thoiLuong?: number;
  ngayKhaiGiang?: string;
}

export interface KhoaHocUpdateRequest {
  tenKhoaHoc?: string;
  moTa?: string;
  hocPhi?: number;
  thoiLuong?: number;
  ngayKhaiGiang?: string;
  trangThai?: TrangThaiKhoaHoc;
}

export interface KhoaHoc {
  id?: string;
  tenKhoaHoc?: string;
  moTa?: string;
  hocPhi?: number;
  thoiLuong?: number;
  ngayKhaiGiang?: string;
  trangThai?: TrangThaiKhoaHoc;
}

// Lớp học
export interface LopHocRequest {
  tenLop?: string;
  khoaHocId?: string;
  siSoToiDa?: number;
}

export interface LopHocUpdateRequest {
  tenLop?: string;
  siSoToiDa?: number;
  trangThai?: TrangThaiLopHoc;
}

export interface LopHoc {
  id?: string;
  tenLop?: string;
  siSoHienTai?: number;
  siSoToiDa?: number;
  trangThai?: TrangThaiLopHoc;
  khoaHocId?: string;
  khoaHoc?: KhoaHoc;
}

// Đăng ký
export interface DangKyRequest {
  hocVienId?: string;
  khoaHocId?: string;
}

export interface DangKyCreateRequest {
  khoaHocId?: string;
  hocVienId?: string;
  trangThai?: TrangThaiDangKy;
  ngayDangKy?: string;
  lopHocId?: string;
  ngayXepLop?: string;
}

export interface DangKyUpdateRequest {
  khoaHocId?: string;
  hocVienId?: string;
  ngayDangKy?: string;
  lopHocId?: string;
  ngayXepLop?: string;
  trangThai?: TrangThaiDangKy;
}

export interface DangKy {
  id?: string;
  hocVienId?: string;
  hocVien?: HocVien;
  khoaHocId?: string;
  khoaHoc?: KhoaHoc;
  lopHocId?: string;
  lopHoc?: LopHoc;
  ngayDangKy?: string;
  ngayXepLop?: string;
  trangThai?: TrangThaiDangKy;
}

// Đăng ký khách
export interface DangKyKhachCreateRequest {
  hoTen?: string;
  email?: string;
  soDienThoai?: string;
  noiDung?: string;
  khoaHocId?: string;
}

export interface DangKyKhachUpdateRequest {
  hoTen?: string;
  email?: string;
  soDienThoai?: string;
  noiDung?: string;
  khoaHocId?: string;
  trangThai?: TrangThaiDangKy;
  ketQua?: KetQuaDangKy;
}

export interface DangKyKhachRequest {
  hoTen?: string;
  soDienThoai?: string;
  email?: string;
  khoaHocId?: string;
  noiDung?: string;
  gioiTinh?: number;
}

export interface DangKyKhach {
    id?: string;
    hoTen?: string;
    email?: string;
    soDienThoai?: string;
    khoaHocId?: string;
    ngayDangKy?: string;
    trangThai?: TrangThaiDangKy;
    ghiChu?: string;
    khoaHoc?: {
        id?: string;
        tenKhoaHoc?: string;
    };
}
