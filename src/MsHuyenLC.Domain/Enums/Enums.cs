namespace MsHuyenLC.Domain.Enums;

#region Status Enums
public enum TrangThaiTaiKhoan
{
    hoatdong,
    tamdung,
    bikhoa
}

public enum TrangThaiLopHoc
{
    choxepgiaovien,
    danghoc,
    ketthuc,
    huy
}

public enum TrangThaiHocVien
{
    danghoc,
    tamngung,
    dahoanthanh
}

public enum TrangThaiKhoaHoc
{
    dangmo,
    dangdienra,
    ketthuc,
    huy
}

public enum TrangThaiDangKy
{
    choduyet,
    daduyet,
    choxeplop,
    daxeplop,
    huy
}

public enum TrangThaiKyThi
{
    sapdienra,
    dangdienra,
    ketthuc,
    huy
}

public enum TrangThaiThanhToan
{
    chuathanhtoan,
    dathanhtoan,
    thatbai
}
#endregion

#region User Enums
public enum GioiTinh
{
    nam,
    nu
}

public enum VaiTro
{
    admin,
    giaovu,
    giaovien,
    hocvien
}
#endregion

public enum KetQuaDangKy
{
    chuaxuly,
    datuchoi,
    dahuy,
    daxuly
}

public enum CapDo
{
    A1,
    A2,
    B1,
    B2,
    C1,
    C2
}

public enum DoKho
{
    de,
    trungbinh,
    kho
}

public enum LoaiCauHoi
{
    TracNghiem,
    TuLuan,
}

public enum KyNang
{
    Nghe,
    Doc,
    Viet
}

public enum LoaiDeThi
{
    LuyenTap,
    ChinhThuc
}

public enum PhuongThucThanhToan
{
    tructuyen,
    tructiep,
    khac
}
