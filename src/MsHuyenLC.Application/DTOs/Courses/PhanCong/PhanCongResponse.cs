namespace MsHuyenLC.Application.DTOs.Courses.PhanCong;

public class PhanCongResponse
{
    public Guid Id { get; set; }
    public Guid GiaoVienId { get; set; }
    public string? TenGiaoVien { get; set; }
    public Guid LopHocId { get; set; }
    public string? TenLop { get; set; }
    public DateTime NgayPhanCong { get; set; }
}
