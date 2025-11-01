// Auth Types - Các types liên quan đến xác thực và tài khoản

import type { VaiTro, TrangThaiTaiKhoan } from './enums';

export interface LoginRequest {
  tenDangNhap: string;
  matKhau: string;
}

export interface ChangePasswordRequest {
  matKhauCu: string;
  matKhauMoi: string;
}

export interface ResetPasswordRequest {
  email: string;
  returnUrl: string;
}

export interface ConfirmResetPasswordRequest {
  email: string;
  token: string;
  matKhauMoi: string;
}

export interface TaiKhoanRequest {
  tenDangNhap?: string;
  matKhau?: string;
  email?: string;
  sdt?: string;
  vaiTro?: VaiTro;
  avatar?: string;
}

export interface TaiKhoanUpdateRequest {
  email?: string;
  sdt?: string;
  avatar?: string;
  vaiTro?: VaiTro;
  trangThai?: TrangThaiTaiKhoan;
}

export interface TaiKhoan {
  id?: string;
  tenDangNhap?: string;
  email?: string;
  sdt?: string;
  matKhau?: string;
  vaiTro?: VaiTro;
  trangThai?: TrangThaiTaiKhoan;
}
