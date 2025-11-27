using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MsHuyenLC.Domain.Entities.System;

public class CauHinhHeThong
{
    public Guid Id { get; set; }
    
    [MaxLength(100)]
    [Column(TypeName = "varchar(100)")]
    public string Ten { get; set; } = null!;
    
    [MaxLength(500)]
    [Column(TypeName = "varchar(500)")]
    public string GiaTri { get; set; } = null!;
}
