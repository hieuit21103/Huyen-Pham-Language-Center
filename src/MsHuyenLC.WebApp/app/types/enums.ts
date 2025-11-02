// #region Status Enums
export enum TrangThaiTaiKhoan {
  HoatDong = 0,    // hoatdong
  TamDung = 1,     // tamdung
  BiKhoa = 2       // bikhoa
}

export enum TrangThaiLopHoc {
  ChoXepGiaoVien = 0,  // choxepgiaovien
  DangHoc = 1,         // danghoc
  KetThuc = 2,         // ketthuc
  Huy = 3              // huy
}

export enum TrangThaiHocVien {
  DangHoc = 0,       // danghoc
  TamNgung = 1,      // tamngung
  DaHoanThanh = 2    // dahoanthanh
}

export enum TrangThaiKhoaHoc {
  DangMo = 0,        // dangmo
  DangDienRa = 1,    // dangdienra
  KetThuc = 2,       // ketthuc
  Huy = 3            // huy
}

export enum TrangThaiDangKy {
  ChoDuyet = 0,      // choduyet
  DaDuyet = 1,       // daduyet
  DaThanhToan = 2,   // dathanhtoan
  DaXepLop = 3,      // daxeplop
  Huy = 4            // huy
}

export enum TrangThaiKyThi {
  SapDienRa = 0,     // sapdienra
  DangDienRa = 1,    // dangdienra
  KetThuc = 2,       // ketthuc
  Huy = 3            // huy
}

export enum TrangThaiThanhToan {
  ChuaThanhToan = 0,  // chuathanhtoan
  DaThanhToan = 1,    // dathanhtoan
  ThatBai = 2         // thatbai
}
// #endregion

// #region User Enums
export enum GioiTinh {
  Nam = 0,  // nam
  Nu = 1    // nu
}

export enum VaiTro {
  Admin = 0,      // admin
  GiaoVu = 1,     // giaovu
  GiaoVien = 2,   // giaovien
  HocVien = 3     // hocvien
}
// #endregion

export enum KetQuaDangKy {
  ChuaXuLy = 0,    // chuaxuly
  DaTuChoi = 1,    // datuchoi
  DaHuy = 2,       // dahuy
  DaXuLy = 3       // daxuly
}

export enum CapDo {
  A1 = 0,
  A2 = 1,
  B1 = 2,
  B2 = 3,
  C1 = 4,
  C2 = 5
}

export enum DoKho {
  De = 0,           // de
  TrungBinh = 1,    // trungbinh
  Kho = 2           // kho
}

export enum LoaiCauHoi {
  TracNghiem = 0,   // TracNghiem
  TuLuan = 1        // TuLuan
}

export enum KyNang {
  Nghe = 0,   // Nghe
  Doc = 1,    // Doc
  Viet = 2    // Viet
}

export enum LoaiDeThi {
  LuyenTap = 0,      // LuyenTap
  ChinhThuc = 1      // ChinhThuc
}

export enum PhuongThucThanhToan {
  TrucTuyen = 0,   // tructuyen
  TrucTiep = 1,    // tructiep
  Khac = 2         // khac
}

export enum DayOfWeek {
  Sunday = 0,
  Monday = 1,
  Tuesday = 2,
  Wednesday = 3,
  Thursday = 4,
  Friday = 5,
  Saturday = 6
}
