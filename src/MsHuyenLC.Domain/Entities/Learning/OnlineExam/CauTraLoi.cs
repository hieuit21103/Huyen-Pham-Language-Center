namespace MsHuyenLC.Domain.Entities.Learning.OnlineExam;

public class CauTraLoi
{
    public Guid Id { get; set; }
    public Guid PhienId { get; set; }
    public Guid CauHoiId { get; set; }
    public string? CauTraLoiText { get; set; }
    public bool? Dung { get; set; }
    public virtual PhienLamBai Phien { get; set; } = null!;
    public virtual NganHangCauHoi CauHoi { get; set; } = null!;
}