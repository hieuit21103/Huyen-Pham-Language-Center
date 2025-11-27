using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MsHuyenLC.Domain.Entities.Courses;

public class LopHoc
{
    public Guid Id { get; set; }
    
    [MaxLength(100)]
    [Column(TypeName = "varchar(100)")]
    public string TenLop { get; set; } = null!;
    
    public int SiSoHienTai { get; set; }
    public int SiSoToiDa { get; set; }
    public TrangThaiLopHoc TrangThai { get; set; } = TrangThaiLopHoc.danghoc;
    public Guid KhoaHocId { get; set; }
    public KhoaHoc KhoaHoc { get; set; } = null!;
    public ICollection<DangKyKhoaHoc> CacDangKy { get; set; } = new List<DangKyKhoaHoc>();
    public void CapNhatSiSo(int soLuongThayDoi)
    {
        SiSoHienTai += soLuongThayDoi;
        if (SiSoHienTai < 0)
        {
            SiSoHienTai = 0;
        }
    }
}
