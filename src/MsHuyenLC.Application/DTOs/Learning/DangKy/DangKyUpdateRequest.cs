using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Learning.DangKy;

public class DangKyUpdateRequest
{
    public Guid? LopHocId { get; set; }
    public TrangThaiDangKy TrangThai { get; set; }
}
