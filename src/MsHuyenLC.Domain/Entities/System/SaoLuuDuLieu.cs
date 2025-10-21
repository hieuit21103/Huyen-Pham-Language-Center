namespace MsHuyenLC.Domain.Entities.System;

public class SaoLuuDuLieu
{
    public Guid Id { get; set; }
    public DateOnly NgaySaoLuu { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public string DuongDan { get; set; } = null!;
}