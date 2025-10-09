namespace MsHuyenLC.Domain.Entities.System;

public class CauHinhHeThong
{
    public Guid Id { get; set; }
    public string TenCauHinh { get; set; } = null!;
    public string MaCauHinh { get; set; } = null!;
    public string GiaTri { get; set; } = null!;
    public string MoTa { get; set; } = null!;
    public DateTime NgayTao { get; set; } = DateTime.Now;
}