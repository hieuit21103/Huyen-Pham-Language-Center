namespace MsHuyenLC.Domain.Entities.System;

public class SaoLuuDuLieu
{
    public Guid Id { get; set; }
    public DateTime NgaySaoLuu { get; set; } = DateTime.Now;
    public string DuongDan { get; set; } = null!;
}