// Schedule Types - Lịch học, Phòng học, Phân công

import type { DayOfWeek } from './enums';
import type { LopHoc } from './course.types';
import type { GiaoVien } from './user.types';

// Phòng học
export interface PhongHocRequest {
  tenPhong?: string;
  soGhe?: number;
}

export interface PhongHoc {
  id?: string;
  tenPhong?: string;
  soGhe?: number;
}

// Lịch học
export interface LichHocRequest {
  lopHocId?: string;
  phongHocId?: string;
  thu?: DayOfWeek;
  gioBatDau?: string;
  gioKetThuc?: string;
  tuNgay?: string;
  denNgay?: string;
}

export interface LichHocUpdateRequest {
  phongHocId?: string;
  thu?: DayOfWeek;
  gioBatDau?: string;
  gioKetThuc?: string;
  tuNgay?: string;
  denNgay?: string;
  coHieuLuc?: boolean;
}

export interface LichHoc {
  id?: string;
  ngayHoc?: string;
  tuNgay?: string;
  denNgay?: string;
  thu?: number; 
  gioBatDau?: string;
  gioKetThuc?: string;
  coHieuLuc?: boolean;
  lopHocId?: string;
  phongHocId?: string;
  lopHoc?: LopHoc;
  phongHoc?: PhongHoc;
}

// Phân công
export interface PhanCongRequest {
  giaoVienId?: string;
  lopHocId?: string;
}

export interface PhanCongResponse {
  id?: string;
  giaoVienId?: string;
  lopHocId?: string;
  tenGiaoVien?: string;
  tenLop?: string;
  ngayPhanCong?: string;
}

export interface PhanCong {
  id?: string;
  giaoVienId?: string;
  lopHocId?: string;
  giaoVien?: GiaoVien;
  lopHoc?: LopHoc;
}
