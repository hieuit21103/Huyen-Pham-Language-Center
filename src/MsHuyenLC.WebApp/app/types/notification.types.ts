export interface ThongBaoRequest {
  nguoiNhanId?: string;
  tieuDe: string;
  noiDung: string;
}

export interface ThongBaoResponse {
  id?: string;
  nguoiGuiId?: string;
  nguoiNhanId?: string;
  tieuDe?: string;
  noiDung?: string;
  ngayTao?: string;
}
