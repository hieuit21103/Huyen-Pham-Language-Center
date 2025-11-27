using System.ComponentModel.DataAnnotations;

namespace MsHuyenLC.Application.DTOs.System.CauHinhHeThong;

public class CauHinhHeThongRequest
{
    public string Ten { get; set; } = null!;
    
    public string GiaTri { get; set; } = null!;
}
