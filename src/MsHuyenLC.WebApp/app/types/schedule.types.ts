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

// Thời gian biểu
export interface ThoiGianBieuRequest {
  thu?: DayOfWeek;
  gioBatDau?: string;
  gioKetThuc?: string;
}

export interface ThoiGianBieu {
  id?: string;
  lichHocId?: string;
  thu?: DayOfWeek;
  gioBatDau?: string;
  gioKetThuc?: string;
}

// Lịch học
export interface LichHocRequest {
  lopHocId?: string;
  phongHocId?: string;
  thoiGianBieus?: ThoiGianBieuRequest[];
}

export interface LichHocUpdateRequest {
  phongHocId?: string;
  coHieuLuc?: boolean;
  thoiGianBieus?: ThoiGianBieuRequest[];
}

export interface LichHoc {
  id?: string;
  tuNgay?: string;
  denNgay?: string;
  coHieuLuc?: boolean;
  lopHocId?: string;
  phongHocId?: string;
  tenLop?: string;
  tenPhong?: string;
  lopHoc?: LopHoc;
  phongHoc?: PhongHoc;
  thoiGianBieu?: ThoiGianBieu[];
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
