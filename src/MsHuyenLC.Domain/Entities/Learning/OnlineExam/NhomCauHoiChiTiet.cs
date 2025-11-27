namespace MsHuyenLC.Domain.Entities.Learning.OnlineExam;

public class NhomCauHoiChiTiet
{
    public Guid Id { get; set; }
    public Guid NhomId { get; set; }
    public Guid CauHoiId { get; set; }
    public int ThuTu { get; set; }
    public virtual NhomCauHoi Nhom { get; set; } = null!;
    public virtual CauHoi CauHoi { get; set; } = null!;
}

