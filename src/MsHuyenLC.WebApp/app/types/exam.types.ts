// Exam Types - Câu hỏi, Đề thi, Bài thi

import type { 
  KyNang, 
  CapDo, 
  DoKho,
  CheDoCauHoi,
  TrangThaiKyThi,
} from './enums';
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
  kyNang?: KyNang;
  urlHinhAnh?: string;
  urlAmThanh?: string;
  loiThoai?: string;
  capDo?: CapDo;
  doKho?: DoKho;
  dapAnCauHois?: DapAnRequest[];
  nhomCauHoiChiTiets?: any[];
}

export interface CauHoiUpdateRequest {
  noiDungCauHoi?: string;
  kyNang?: KyNang;
  urlHinhAnh?: string;
  urlAmThanh?: string;
  loiThoai?: string;
  capDo?: CapDo;
  doKho?: DoKho;
}

export interface CauHoiFilterParams {
  kyNang?: KyNang;
  capDo?: CapDo;
  doKho?: DoKho;
  keyword?: string;
}

export interface CauHoi {
  id?: string;
  noiDungCauHoi?: string;
  kyNang?: KyNang;
  urlHinh?: string;
  urlAmThanh?: string;
  capDo?: CapDo;
  doKho?: DoKho;
  loiThoai?: string;
  cacDapAn?: DapAnCauHoi[];
  cacNhom?: NhomCauHoiChiTiet[];
}

export interface DapAnCauHoi {
  id?: string;
  cauHoiId?: string; 
  nhan?: string;
  noiDung?: string;
  dung?: boolean;
  giaiThich?: string;
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

// Đề thi
export interface DeThiRequest {
  id?: string;
  tenDe?: string;
  soCauHoi?: number;
  kyThiId?: string;
  kyThi?: any;
  thoiGianLamBai?: number;
  cauHoiIds?: string[];
}

export interface DeThiUpdateRequest {
  tenDe?: string;
  soCauHoi?: number;
  thoiGianLamBai?: number;
  kyThiId?: string;
  cauHoiIds?: string[];
}

// New dynamic exam generation request
export interface GenerateExamRequest {
  kyThiId: string;
  hocVienId: string;
  nguoiTaoId: string;
}

// Join exam request
export interface JoinExamRequest {
  kyThiId: string;
  hocVienId?: string; // Optional, auto-filled by backend from token
}

// Grouped questions response
export interface QuestionGroupedItem {
  nhomCauHoiId?: string;
  cauHoi: CauHoi;
  thuTu: number;
}

export interface StandaloneQuestionItem {
  cauHoi: CauHoi;
  thuTu: number;
}

export interface QuestionsGroupedResponse {
  deThi: DeThi;
  groupedQuestions: QuestionGroupedItem[];
  standaloneQuestions: StandaloneQuestionItem[];
  totalQuestions: number;
}

export interface DeThi {
  id?: string;
  maDe?: string;
  tenDe?: string;
  tongCauHoi?: number;
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

// Cấu hình kỳ thi
export interface CauHinhKyThiRequest {
  capDo: CapDo;
  doKho: DoKho;
  kyNang: KyNang;
  cheDoCauHoi: number;
  soCauHoi: number;
}

// Kỳ thi
export interface KyThiRequest {
  tenKyThi: string;
  ngayThi: string;
  gioBatDau: string;
  gioKetThuc: string;
  thoiLuong: number;
  lopHocId: string;
  cauHinhKyThis: CauHinhKyThiRequest[];
}

export interface KyThiUpdateRequest {
  tenKyThi?: string;
  ngayThi?: string;
  gioBatDau?: string;
  gioKetThuc?: string;
  thoiLuong?: number;
  lopHocId?: string;
  trangThai?: TrangThaiKyThi;
  cauHinhKyThis?: CauHinhKyThiRequest[];
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
  thoiGianLamBai?: number;
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
  kyThiId?: string;
  kyThi?: KyThi;
  cacCauTraLoi?: CauTraLoi[];
  hocVien?: any;
}

export interface CauTraLoi {
  id?: string;
  phienId?: string;
  cauHoiId?: string;
  cauTraLoiText?: string;
  dung?: boolean;
  noiDungCauHoi?: string;
  phien?: any;
  cauHoi?: CauHoi;
}