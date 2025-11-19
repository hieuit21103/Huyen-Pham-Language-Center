import type { TaiKhoan } from "./auth.types";

export interface ThongBaoRequest {
  nguoiNhanId?: string;
  tieuDe: string;
  noiDung: string;
}

export interface ThongBaoResponse {
  id?: string;
  nguoiGuiId?: string;
  nguoiGui?: TaiKhoan;
  nguoiNhanId?: string;
  tieuDe?: string;
  noiDung?: string;
  ngayTao?: string;
}
