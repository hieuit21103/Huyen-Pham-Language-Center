namespace MsHuyenLC.Domain.Entities.Learning.OnlineExam;

public class DapAnCauHoi
{
    public Guid Id { get; set; }
    public Guid CauHoiId { get; set; }
    public string Nhan { get; set; } = string.Empty; // A, B, C, D
    public string NoiDung { get; set; } = string.Empty;
    public bool Dung { get; set; }
    public string? GiaiThich { get; set; }
    public virtual NganHangCauHoi CauHoi { get; set; } = null!;
}

