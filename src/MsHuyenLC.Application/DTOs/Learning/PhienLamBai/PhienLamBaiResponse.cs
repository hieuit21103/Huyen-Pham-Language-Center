using MsHuyenLC.Application.DTOs.Learning.CauTraLoi;
using MsHuyenLC.Domain.Entities.Learning.OnlineExam;
using MsHuyenLC.Domain.Entities.Users;

namespace MsHuyenLC.Application.DTOs.Learning.PhienLamBai;

public class PhienLamBaiResponse
{
    public Guid Id { get; set; }
    public int? TongCauHoi { get; set; }
    public int? SoCauDung { get; set; }
    public decimal? Diem { get; set; }
    public TimeSpan? ThoiGianLam { get; set; }
    public DateOnly? NgayLam { get; set; }
    public Guid? HocVienId { get; set; }
    public HocVien? HocVien { get; set; }
    public Guid? DeThiId { get; set; }
    public Domain.Entities.Learning.OnlineExam.DeThi? DeThi { get; set; } 
    public IEnumerable<CauTraLoiResponse>? CauTraLoi { get; set; }
}
