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
    chuathanhtoan,
    dathanhtoan,
    datuchoi,
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
    giaovien,
    hocvien,
    giaovu
}
#endregion

public enum DoiTuongNhan
{
    lophoc,
    tatca
}

public enum KetQuaDangKy
{
    chuaxuly,
    datuchoi,
    dahuy,
    daxuly
}

public enum HinhThucKyThi
{
    tructuyen,
    tructiep
}

public enum MucDo
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

public enum PhuongThucThanhToan
{
    tructuyen,
    tructiep,
    khac
}