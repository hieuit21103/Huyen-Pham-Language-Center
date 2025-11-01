// Exam Types - Câu hỏi, Đề thi, Bài thi

import type { 
  LoaiCauHoi, 
  KyNang, 
  CapDo, 
  DoKho, 
  LoaiDeThi,
  TrangThaiKyThi,
} from './enums';
import type { PaginationParams } from './common.types';
import type { LopHoc } from './course.types';

// Câu hỏi
export interface DapAnRequest {
  nhan?: string;
  noiDung?: string;
  dung?: boolean;
  giaiThich?: string;
}

export interface CauHoiRequest {
  noiDungCauHoi?: string;
  loaiCauHoi?: LoaiCauHoi;
  kyNang?: KyNang;
  urlHinhAnh?: string;
  urlAmThanh?: string;
  loiThoai?: string;
  doanVan?: string;
  capDo?: CapDo;
  doKho?: DoKho;
  dapAnCauHois?: DapAnRequest[];
  nhomCauHoiChiTiets?: any[];
}

export interface CauHoiUpdateRequest {
  noiDungCauHoi?: string;
  loaiCauHoi?: LoaiCauHoi;
  kyNang?: KyNang;
  urlHinhAnh?: string;
  urlAmThanh?: string;
  loiThoai?: string;
  doanVan?: string;
  capDo?: CapDo;
  doKho?: DoKho;
}

export interface CauHoiFilterParams extends PaginationParams {
  loaiCauHoi?: LoaiCauHoi;
  kyNang?: KyNang;
  capDo?: CapDo;
  doKho?: DoKho;
  keyword?: string;
}

export interface CauHoi {
  id?: string;
  noiDung?: string;
  loaiCauHoi?: LoaiCauHoi;
  kyNang?: KyNang;
  urlHinh?: string;
  urlAmThanh?: string;
  dapAnDung?: string;
  giaiThich?: string;
  capDo?: CapDo;
  doKho?: DoKho;
  docHieuId?: string;
  docHieu?: any;
}

// Nhóm câu hỏi (Reading Comprehension)
export interface NhomCauHoiRequest {
  urlAmThanh?: string;
  urlHinhAnh?: string;
  noiDung?: string;
  tieuDe?: string;
  soLuongCauHoi?: number;
  doKho?: DoKho;
  capDo?: CapDo;
}

export interface NhomCauHoiUpdateRequest {
  urlAmThanh?: string;
  urlHinhAnh?: string;
  noiDung?: string;
  tieuDe?: string;
  soLuongCauHoi?: number;
  doKho?: DoKho;
  capDo?: CapDo;
}

export interface NhomCauHoiChiTietRequest {
  nhomId?: string;
  cauHoiId?: string;
  thuTu?: number;
}

export interface NhomCauHoi {
  id?: string;
  urlAmThanh?: string;
  urlHinhAnh?: string;
  noiDung?: string;
  tieuDe?: string;
  soLuongCauHoi?: number;
  doKho?: DoKho;
  capDo?: CapDo;
  cacChiTiet?: NhomCauHoiChiTiet[];
  cacCauHoiDeThi?: any[];
}

export interface NhomCauHoiChiTiet {
  id?: string;
  nhomId?: string;
  cauHoiId?: string;
  thuTu?: number;
  nhom?: NhomCauHoi;
  cauHoi?: CauHoi;
}

export interface DocHieu {
  id?: string;
  noiDung?: string;
  capDo?: CapDo;
  doKho?: DoKho;
  cauHois?: CauHoi[];
}

// Đề thi
export interface DeThiRequest {
  id?: string;
  tenDe?: string;
  soCauHoi?: number;
  loaiDeThi?: LoaiDeThi;
  kyThiId?: string;
  kyThi?: any;
  thoiGianLamBai?: number;
  cauHoiIds?: string[];
}

export interface DeThiUpdateRequest {
  tenDe?: string;
  soCauHoi?: number;
  thoiGianLamBai?: number;
  loaiDeThi?: LoaiDeThi;
  kyThiId?: string;
  cauHoiIds?: string[];
}

export interface GenerateTestRequest {
  tenDe?: string;
  soCauHoi?: number;
  thoiGianLamBai?: number;
  loaiDeThi?: LoaiDeThi;
  loaiCauHoi?: LoaiCauHoi;
  kyNang?: KyNang;
  capDo?: CapDo;
  doKho?: DoKho;
  kyThiId?: string;
}

export interface GenerateTestWithDifficultyRequest {
  tenDe?: string;
  soCauDe?: number;
  soCauTrungBinh?: number;
  soCauKho?: number;
  thoiGianLamBai?: number;
  loaiDeThi?: LoaiDeThi;
  loaiCauHoi?: LoaiCauHoi;
  kyNang?: KyNang;
  capDo?: CapDo;
  kyThiId?: string;
}

export interface DeThi {
  id?: string;
  maDe?: string;
  tenDe?: string;
  tongCauHoi?: number;
  loaiDeThi?: LoaiDeThi;
  kyThiId?: string;
  kyThi?: KyThi;
  thoiLuongPhut?: number; 
  ngayTao?: string;
  nguoiTaoId?: string;
  nguoiTao?: any;
  cacCauHoi?: any[];
  cauHoiDeThis?: any[];
}

export interface CauHoiDeThi {
  id?: string;
  deThiId?: string;
  cauHoiId?: string;
  deThi?: DeThi;
  cauHoi?: CauHoi;
}

// Kỳ thi
export interface KyThiRequest {
  tenKyThi?: string;
  ngayThi?: string;
  gioBatDau?: string;
  gioKetThuc?: string;
  thoiLuong?: number;
  lopHocId?: string;
}

export interface KyThiUpdateRequest {
  tenKyThi?: string;
  ngayThi?: string;
  gioBatDau?: string;
  gioKetThuc?: string;
  thoiLuong?: number;
  lopHocId?: string;
  trangThai?: TrangThaiKyThi;
}

export interface KyThi {
  id?: string;
  tenKyThi?: string;
  ngayThi?: string;
  gioBatDau?: string;
  gioKetThuc?: string;
  thoiLuong?: number;
  trangThai?: TrangThaiKyThi;
  lopHocId?: string;
  lopHoc?: LopHoc;
}

// Làm bài thi
export interface CauTraLoiDto {
  cauHoiId?: string;
  cauTraLoi?: string;
}

export interface SubmitBaiThiRequest {
  deThiId?: string;
  hocVienId?: string;
  cauTraLois?: CauTraLoiDto[];
}

export interface SubmitRequest {
  deThiId?: string;
  tongCauHoi?: number;
  cacTraLoi?: Record<string, string>;
  thoiGianLamBai?: string;
  tuDongCham?: boolean;
}

export interface GradingRequest {
  soCauDung?: number;
  diem?: number;
}

export interface ChamCauHoiDto {
  baiThiChiTietId?: string;
  diem?: number;
  nhanXet?: string;
}

export interface ChamBaiRequest {
  cauHois?: ChamCauHoiDto[];
  nhanXetChung?: string;
}

export interface SaveAnswerRequest {
  cauHoiId?: string;
  cauTraLoi?: string;
}

export interface PhienLamBai {
  id?: string;
  tongCauHoi?: number;
  diem?: number;
  soCauDung?: number;
  thoiGianLam?: string;
  ngayLam?: string;
  hocVienId?: string;
  deThiId?: string;
  deThi?: DeThi;
  hocVien?: any;
}

export interface CauTraLoi {
  id?: string;
  cauHoiId?: string;
  cauTraLoiText?: string;
  dung?: boolean;
  noiDungCauHoi?: string;
}