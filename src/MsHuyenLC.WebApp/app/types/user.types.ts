import type { GioiTinh, TrangThaiHocVien, TrangThaiTaiKhoan, VaiTro } from './enums';
import type { TaiKhoan } from './auth.types';

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
  ngayDangKy?: string;
  taiKhoanId?: string;
  taiKhoan?: TaiKhoan;
  trangThai?: TrangThaiTaiKhoan;
}

export interface GiaoVuRequest {
    hoTen: string;
    boPhan?: string;
    taiKhoanId?: string;
}

export interface GiaoVu {
    id: string;
    hoTen: string;
    boPhan?: string;
    taiKhoanId?: string;
    taiKhoan?: any;
}

export interface Profile {
  id?: string;
  tenDangNhap?: string;
  matkhau?: string;
  email?: string;
  sdt?: string;
  vaiTro?: VaiTro;
  trangThai?: TrangThaiTaiKhoan;
  avatar?: string;
  ngaytao?: string;
}