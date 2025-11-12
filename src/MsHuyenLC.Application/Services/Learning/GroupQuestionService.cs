using MsHuyenLC.Domain.Entities.Learning.OnlineExam;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Services.Learning;
using MsHuyenLC.Application.DTOs.Learning.NhomCauHoi;
using MsHuyenLC.Application.DTOs.Learning.CauHoi;

namespace MsHuyenLC.Application.Services.Learning;

public class GroupQuestionService : IGroupQuestionService
{
    private readonly IUnitOfWork _unitOfWork;

    public GroupQuestionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<NhomCauHoiResponse>> GetAllAsync()
    {
        var entities = await _unitOfWork.NhomCauHois.GetAllAsync();
        return entities.Select(MapToResponse);
    }

    public async Task<NhomCauHoiResponse?> GetByIdAsync(string id)
    {
        var entity = await _unitOfWork.NhomCauHois.GetByIdAsync(id);
        return entity != null ? MapToResponse(entity) : null;
    }

    public async Task<NhomCauHoiResponse> CreateAsync(NhomCauHoiRequest request)
    {
        var entity = new NhomCauHoi
        {
            UrlHinhAnh = request.UrlHinhAnh,
            UrlAmThanh = request.UrlAmThanh,
            NoiDung = request.NoiDung,
            TieuDe = request.TieuDe,
            SoLuongCauHoi = request.SoLuongCauHoi,
            CapDo = request.CapDo,
            DoKho = request.DoKho,
        };

        await _unitOfWork.NhomCauHois.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        
        return MapToResponse(entity);
    }

    public async Task<NhomCauHoiResponse> UpdateAsync(string id, NhomCauHoiUpdateRequest request)
    {
        var entity = await _unitOfWork.NhomCauHois.GetByIdAsync(id);
        if (entity == null)
        {
            throw new Exception("Không tìm thấy nhóm câu hỏi");
        }

        if (!string.IsNullOrWhiteSpace(request.NoiDung))
            entity.NoiDung = request.NoiDung;

        if (!string.IsNullOrWhiteSpace(request.TieuDe))
            entity.TieuDe = request.TieuDe;

        if (request.SoLuongCauHoi.HasValue)
            entity.SoLuongCauHoi = request.SoLuongCauHoi.Value;

        if (!string.IsNullOrWhiteSpace(request.UrlHinhAnh))
            entity.UrlHinhAnh = request.UrlHinhAnh;

        if (!string.IsNullOrWhiteSpace(request.UrlAmThanh))
            entity.UrlAmThanh = request.UrlAmThanh;

        if (request.CapDo.HasValue)
            entity.CapDo = request.CapDo.Value;

        if (request.DoKho.HasValue)
            entity.DoKho = request.DoKho.Value;

        await _unitOfWork.NhomCauHois.UpdateAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        
        return MapToResponse(entity);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var entity = await _unitOfWork.NhomCauHois.GetByIdAsync(id);
        if (entity == null)
        {
            throw new Exception("Không tìm thấy nhóm câu hỏi");
        }

        await _unitOfWork.NhomCauHois.DeleteAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<NhomCauHoiResponse> AddQuestionToGroupAsync(string groupId, string questionId, int? thuTu = null)
    {
        var nhomCauHoi = await _unitOfWork.NhomCauHois.GetByIdAsync(groupId);
        if (nhomCauHoi == null)
        {
            throw new Exception("Nhóm câu hỏi không tồn tại");
        }
        if (nhomCauHoi.CacChiTiet.Count >= nhomCauHoi.SoLuongCauHoi)
        {
            throw new Exception("Nhóm câu hỏi đã đạt số lượng câu hỏi tối đa");
        }
        if (thuTu == null)
        {
            thuTu = nhomCauHoi.CacChiTiet.Count + 1;
        }
        await _unitOfWork.NhomCauHoiChiTiets.AddAsync(new NhomCauHoiChiTiet
        {
            NhomId = Guid.Parse(groupId),
            CauHoiId = Guid.Parse(questionId),
            ThuTu = thuTu.Value
        });
        await _unitOfWork.SaveChangesAsync();
        
        return MapToResponse(nhomCauHoi);
    }
    
    public async Task<List<CauHoiResponse>> GetQuestionsByGroupIdAsync(string groupId)
    {
        var chiTiets = await _unitOfWork.NhomCauHoiChiTiets.GetAllAsync(
            filter: ct => ct.NhomId.ToString() == groupId,
            includes: ct => ct.CauHoi
        );
        
        if (!chiTiets.Any())
        {
            return new List<CauHoiResponse>();
        }
        
        var questions = chiTiets
            .OrderBy(ct => ct.ThuTu)
            .Select(ct => ct.CauHoi)
            .Where(ch => ch != null)
            .Select(MapCauHoiToResponse)
            .ToList();
            
        return questions;
    }

    // Private mapping methods
    private static NhomCauHoiResponse MapToResponse(NhomCauHoi entity)
    {
        return new NhomCauHoiResponse
        {
            Id = entity.Id.ToString(),
            UrlAmThanh = entity.UrlAmThanh,
            UrlHinhAnh = entity.UrlHinhAnh,
            NoiDung = entity.NoiDung,
            TieuDe = entity.TieuDe,
            SoLuongCauHoi = entity.SoLuongCauHoi,
            DoKho = entity.DoKho,
            CapDo = entity.CapDo,
        };
    }

    private static CauHoiResponse MapCauHoiToResponse(NganHangCauHoi entity)
    {
        return new CauHoiResponse
        {
            NoiDungCauHoi = entity.NoiDungCauHoi,
            LoaiCauHoi = entity.LoaiCauHoi,
            KyNang = entity.KyNang,
            UrlHinhAnh = entity.UrlHinhAnh,
            UrlAmThanh = entity.UrlAmThanh,
            LoiThoai = entity.LoiThoai,
            CapDo = entity.CapDo,
            DoKho = entity.DoKho,
            DapAnCauHois = entity.CacDapAn?.Select(da => new DapAnResponse
            {
                Nhan = da.Nhan,
                NoiDung = da.NoiDung
            }).ToList()
        };
    }
}

