export interface DoanhThuStats {
  tongDoanhThu: number;
  tongDaThanhToan: number;
  tongChuaThanhToan: number;
  soLuongGiaoDich: number;
  soLuongDaThanhToan: number;
  soLuongChuaThanhToan: number;
}

export interface DoanhThuTheoNgay {
  ngay: string;
  soLuong: number;
  tongTien: number;
  daThanhToan: number;
  chuaThanhToan: number;
}

export interface DoanhThuTheoKhoaHoc {
  khoaHocId: string;
  tenKhoaHoc: string;
  soLuong: number;
  tongTien: number;
  daThanhToan: number;
  chuaThanhToan: number;
}