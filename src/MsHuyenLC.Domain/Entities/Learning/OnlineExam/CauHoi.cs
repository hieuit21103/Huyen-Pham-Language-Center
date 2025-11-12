namespace MsHuyenLC.Domain.Entities.Learning.OnlineExam;

public class CauHoi
{
    public Guid Id { get; set; }
    public string NoiDungCauHoi { get; set; } = string.Empty;
    public LoaiCauHoi LoaiCauHoi { get; set; } = LoaiCauHoi.TracNghiem;
    public KyNang KyNang { get; set; } = KyNang.Doc;
    public DoKho DoKho { get; set; } = DoKho.trungbinh;
    public CapDo CapDo { get; set; } = CapDo.A1;
    public string? UrlAmThanh { get; set; }
    public string? UrlHinhAnh { get; set; }
    public string? LoiThoai { get; set; }
    public virtual ICollection<DapAnCauHoi> CacDapAn { get; set; } = new List<DapAnCauHoi>();
    public virtual ICollection<NhomCauHoiChiTiet> CacNhom { get; set; } = new List<NhomCauHoiChiTiet>();
}
