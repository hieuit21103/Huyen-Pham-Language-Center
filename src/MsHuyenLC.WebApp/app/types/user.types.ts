// User Types - Giáo viên và Học viên

import type { GioiTinh, TrangThaiHocVien } from './enums';
import type { TaiKhoan } from './auth.types';

// Giáo viên
export interface GiaoVienRequest {
  hoTen?: string;
  chuyenMon?: string;
  trinhDo?: string;
  kinhNghiem?: string;
  taiKhoanId?: string;
}

export interface GiaoVienUpdateRequest {
  hoTen?: string;
  chuyenMon?: string;
  trinhDo?: string;
  kinhNghiem?: string;
}

export interface GiaoVien {
  id?: string;
  hoTen?: string;
  chuyenMon?: string;
  trinhDo?: string;
  kinhNghiem?: number;
  taiKhoanId?: string;
  taiKhoan?: TaiKhoan;
}

// Học viên
export interface HocVienUpdateRequest {
  hoTen?: string;
  ngaySinh?: string;
  gioiTinh?: GioiTinh;
  diaChi?: string;
  trangThai?: TrangThaiHocVien;
}

export interface HocVien {
  id?: string;
  hoTen?: string;
  ngaySinh?: string;
  gioiTinh?: GioiTinh;
  diaChi?: string;
  taiKhoanId?: string;
  taiKhoan?: TaiKhoan;
  dangKys?: any[];
}

/**
 * Interface cho request tạo/cập nhật giáo vụ
 */
export interface GiaoVuRequest {
    hoTen: string;
    boPhan?: string;
    taiKhoanId?: string;
}

/**
 * Interface cho giáo vụ
 */
export interface GiaoVu {
    id: string;
    hoTen: string;
    boPhan?: string;
    taiKhoanId?: string;
    taiKhoan?: any;
}