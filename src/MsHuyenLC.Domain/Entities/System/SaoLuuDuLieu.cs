using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MsHuyenLC.Domain.Entities.System;

public class SaoLuuDuLieu
{
    public Guid Id { get; set; }
    public DateOnly NgaySaoLuu { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    
    [MaxLength(500)]
    [Column(TypeName = "varchar(500)")]
    public string DuongDan { get; set; } = null!;
}
