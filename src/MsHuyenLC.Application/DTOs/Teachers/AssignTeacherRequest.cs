using System.ComponentModel.DataAnnotations;

namespace MsHuyenLC.Application.DTOs.Teachers;

/// <summary>
/// DTO cho việc phân công giáo viên vào lớp học
/// </summary>
public class AssignTeacherRequest
{
    [Required(ErrorMessage = "Mã giáo viên là bắt buộc")]
    public string GiaoVienId { get; set; }

    [Required(ErrorMessage = "Mã lớp học là bắt buộc")]
    public string LopHocId { get; set; }
}
