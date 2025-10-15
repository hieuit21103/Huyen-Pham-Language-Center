using System.ComponentModel.DataAnnotations;

namespace MsHuyenLC.Application.DTOs.Teachers;

public class TeacherCreateRequest
{
    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
    public string HoTen { get; set; } = null!;

    [StringLength(200, ErrorMessage = "Chuyên môn không được vượt quá 200 ký tự")]
    public string? ChuyenMon { get; set; }

    [StringLength(100, ErrorMessage = "Trình độ không được vượt quá 100 ký tự")]
    public string? TrinhDo { get; set; }

    [StringLength(500, ErrorMessage = "Kinh nghiệm không được vượt quá 500 ký tự")]
    public string? KinhNghiem { get; set; }

    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string SoDienThoai { get; set; } = null!;
}
