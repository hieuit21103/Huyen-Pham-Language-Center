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
  lopHocId?: string;
  phongHocId?: string;
  thu?: DayOfWeek;
  ngayHoc?: string;
  gioBatDau?: string;
  gioKetThuc?: string;
  tuNgay?: string;
  denNgay?: string;
  noiDung?: string;
  coHieuLuc?: boolean;
  lopHoc?: LopHoc;
  phongHoc?: PhongHoc;
  giaoVien?: GiaoVien;
}

// Phân công
export interface PhanCongRequest {
  giaoVienId?: string;
  lopHocId?: string;
}

export interface PhanCong {
  id?: string;
  giaoVienId?: string;
  lopHocId?: string;
  giaoVien?: GiaoVien;
  lopHoc?: LopHoc;
}
