namespace MsHuyenLC.Application.DTOs.Teachers;

/// <summary>
/// DTO trả về thông tin phân công giáo viên
/// </summary>
public class TeacherAssignmentResponse
{
    public Guid Id { get; set; }
    public Guid GiaoVienId { get; set; }
    public string TenGiaoVien { get; set; } = null!;
    public Guid LopHocId { get; set; }
    public string TenLop { get; set; } = null!;
    public string TenKhoaHoc { get; set; } = null!;
    public DateTime NgayPhanCong { get; set; }
}
