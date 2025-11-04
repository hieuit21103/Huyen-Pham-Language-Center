using MsHuyenLC.Domain.Entities.Learning.OnlineExam;
using MsHuyenLC.Application.Interfaces;

namespace MsHuyenLC.Application.Services.Learnings;

public class GroupQuestionService : GenericService<NhomCauHoi>
{
    private readonly IGenericRepository<NhomCauHoiChiTiet> _detailRepository;
    public GroupQuestionService(IGenericRepository<NhomCauHoi> repository, IGenericRepository<NhomCauHoiChiTiet> detailRepository) : base(repository)
    {
        _detailRepository = detailRepository;
    }

    public async Task<NhomCauHoi> AddQuestionToGroup(string groupId, string questionId, int? thuTu = null)
    {
        var nhomCauHoi = await _repository.GetByIdAsync(groupId);
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
        await _detailRepository.AddAsync(new NhomCauHoiChiTiet
        {
            NhomId = Guid.Parse(groupId),
            CauHoiId = Guid.Parse(questionId),
            ThuTu = thuTu.Value
        });
        await _detailRepository.SaveChangesAsync();
        return nhomCauHoi;
    }
    
    public async Task<List<NganHangCauHoi>> GetQuestionsByGroupId(string groupId)
    {
        var chiTiets = await _detailRepository.GetAllAsync(
            PageNumber: 1,
            PageSize: 1000,
            Filter: ct => ct.NhomId.ToString() == groupId,
            Includes: ct => ct.CauHoi
        );
        
        if (!chiTiets.Any())
        {
            return new List<NganHangCauHoi>();
        }
        
        var questions = chiTiets
            .OrderBy(ct => ct.ThuTu)
            .Select(ct => ct.CauHoi)
            .Where(ch => ch != null)
            .ToList();
            
        return questions!;
    }
}
