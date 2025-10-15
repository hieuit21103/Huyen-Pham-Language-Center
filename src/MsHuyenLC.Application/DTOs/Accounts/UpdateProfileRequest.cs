using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.DTOs.Accounts;

public class UpdateProfileRequest
{
    public string? Email { get; set; }
    public string? Sdt { get; set; }
    public string? Avatar { get; set; }
}




