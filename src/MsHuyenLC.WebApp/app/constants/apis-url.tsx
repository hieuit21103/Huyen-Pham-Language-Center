
const getApiUrl = () => {
  if (typeof window !== 'undefined') {
    return window.ENV.CLIENT_API_URL || 'http://localhost:5000';
  }
  return process.env.SERVER_API_URL || 'http://localhost:5000';
};

export const BASE_API_URL = getApiUrl() + '/api';

// Auth URLs
export function AuthApiUrl(path: string) {
    return `${BASE_API_URL}/Auth/${path}`;
}

// PhienLamBai URLs
export function PhienLamBaiApiUrl(path: string = '') {
    return path ? `${BASE_API_URL}/PhienLamBai/${path}` : `${BASE_API_URL}/PhienLamBai`;
}

// CauHoi URLs
export function CauHoiApiUrl(path: string = '') {
    return path ? `${BASE_API_URL}/CauHoi/${path}` : `${BASE_API_URL}/CauHoi`;
}

// NhomCauHoi URLs (Reading Comprehension Group)
export function NhomCauHoiApiUrl(path: string = '') {
    return path ? `${BASE_API_URL}/NhomCauHoi/${path}` : `${BASE_API_URL}/NhomCauHoi`;
}

// DangKy URLs
export function DangKyApiUrl(path: string = '') {
    return path ? `${BASE_API_URL}/DangKy/${path}` : `${BASE_API_URL}/DangKy`;
}

// DangKyKhach URLs
export function DangKyKhachApiUrl(path: string = '') {
    return path ? `${BASE_API_URL}/DangKyKhach/${path}` : `${BASE_API_URL}/DangKyKhach`;
}

// DeThi URLs
export function DeThiApiUrl(path: string = '') {
    return path ? `${BASE_API_URL}/DeThi/${path}` : `${BASE_API_URL}/DeThi`;
}

// GiaoVien URLs
export function GiaoVienApiUrl(path: string = '') {
    return path ? `${BASE_API_URL}/GiaoVien/${path}` : `${BASE_API_URL}/GiaoVien`;
}

// HocVien URLs
export function HocVienApiUrl(path: string = '') {
    return path ? `${BASE_API_URL}/HocVien/${path}` : `${BASE_API_URL}/HocVien`;
}

// KhoaHoc URLs
export function KhoaHocApiUrl(path: string = '') {
    return path ? `${BASE_API_URL}/KhoaHoc/${path}` : `${BASE_API_URL}/KhoaHoc`;
}

// KyThi URLs
export function KyThiApiUrl(path: string = '') {
    return path ? `${BASE_API_URL}/KyThi/${path}` : `${BASE_API_URL}/KyThi`;
}

// LichHoc URLs
export function LichHocApiUrl(path: string = '') {
    return path ? `${BASE_API_URL}/LichHoc/${path}` : `${BASE_API_URL}/LichHoc`;
}

// LopHoc URLs
export function LopHocApiUrl(path: string = '') {
    return path ? `${BASE_API_URL}/LopHoc/${path}` : `${BASE_API_URL}/LopHoc`;
}

// PhanCong URLs
export function PhanCongApiUrl(path: string = '') {
    return path ? `${BASE_API_URL}/PhanCong/${path}` : `${BASE_API_URL}/PhanCong`;
}

// PhongHoc URLs
export function PhongHocApiUrl(path: string = '') {
    return path ? `${BASE_API_URL}/PhongHoc/${path}` : `${BASE_API_URL}/PhongHoc`;
}

// Profile URLs
export function ProfileApiUrl() {
    return `${BASE_API_URL}/profile`;
}

// TaiKhoan URLs
export function TaiKhoanApiUrl(path: string = '') {
    return path ? `${BASE_API_URL}/TaiKhoan/${path}` : `${BASE_API_URL}/TaiKhoan`;
}

// SystemLogger URLs
export function SystemLoggerApiUrl(path: string = '') {
    return path ? `${BASE_API_URL}/SystemLogger/${path}` : `${BASE_API_URL}/SystemLogger`;
}

// Report URLs
export function ReportApiUrl(path: string = '') {
    return path ? `${BASE_API_URL}/PhanHoi/${path}` : `${BASE_API_URL}/PhanHoi`;
}

// Notification URLs
export function NotificationApiUrl(path: string = '') {
    return path ? `${BASE_API_URL}/ThongBao/${path}` : `${BASE_API_URL}/ThongBao`;
}

// CauHinhHeThong URLs
export function CauHinhHeThongApiUrl(path: string = '') {
    return path ? `${BASE_API_URL}/CauHinhHeThong/${path}` : `${BASE_API_URL}/CauHinhHeThong`;
}

// GiaoVu URLs
export function GiaoVuApiUrl(path: string = '') {
    return path ? `${BASE_API_URL}/GiaoVu/${path}` : `${BASE_API_URL}/GiaoVu`;
}

// Health check
export function HealthApiUrl() {
    return `${BASE_API_URL.replace('/api', '')}/health`;
}

// Legacy support
export const BASE_COURSE_API_URL = KhoaHocApiUrl();

