using MsHuyenLC.Domain.Entities.Learning.OnlineExam;
using MsHuyenLC.Application.Interfaces;

namespace MsHuyenLC.Application.Services.Learnings;

public class QuestionService : GenericService<NganHangCauHoi>
{
    public QuestionService(IGenericRepository<NganHangCauHoi> repository) : base(repository)
    {
    }

    public async Task<NganHangCauHoi> AddCorrectAnswerToQuestionAsync(string questionId, DapAnCauHoi dapAnCauHoi)
    {
        var cauHoi = await _repository.GetByIdAsync(questionId);
        if (cauHoi == null)
        {
            throw new Exception("Câu hỏi không tồn tại");
        }

        cauHoi.CacDapAn.Add(dapAnCauHoi);
        await _repository.SaveChangesAsync();
        return cauHoi;
    }

    public async Task<NganHangCauHoi> AddGroupToQuestionAsync(string questionId, NhomCauHoiChiTiet nhomCauHoiChiTiet)
    {
        var cauHoi = await _repository.GetByIdAsync(questionId);
        if (cauHoi == null)
        {
            throw new Exception("Câu hỏi không tồn tại");
        }

        cauHoi.CacNhom.Add(nhomCauHoiChiTiet);
        await _repository.SaveChangesAsync();
        return cauHoi;
    }
}
