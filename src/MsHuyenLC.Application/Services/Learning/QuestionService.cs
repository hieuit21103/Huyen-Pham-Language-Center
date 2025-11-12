using MsHuyenLC.Domain.Entities.Learning.OnlineExam;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Services.Learning;
using MsHuyenLC.Application.DTOs.Learning.CauHoi;
using MsHuyenLC.Domain.Enums;
using FluentValidation;

namespace MsHuyenLC.Application.Services.Learning;

public class QuestionService : IQuestionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CauHoiRequest> _createValidator;
    private readonly IValidator<CauHoiUpdateRequest> _updateValidator;

    public QuestionService(
        IUnitOfWork unitOfWork,
        IValidator<CauHoiRequest> createValidator,
        IValidator<CauHoiUpdateRequest> updateValidator
    )
    {
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<CauHoi?> GetByIdAsync(string id)
    {
        return await _unitOfWork.CauHois.GetByIdAsync(id);
    }

    public async Task<IEnumerable<CauHoi>> GetAllAsync()
    {
        return await _unitOfWork.CauHois.GetAllAsync();
    }

    public async Task<CauHoi> CreateAsync(CauHoiRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);
        var cauHoi = new CauHoi
        {
            NoiDungCauHoi = request.NoiDungCauHoi,
            LoaiCauHoi = request.LoaiCauHoi,
            KyNang = request.KyNang,
            DoKho = request.DoKho,
            CapDo = request.CapDo,
            UrlAmThanh = request.UrlAmThanh,
            UrlHinhAnh = request.UrlHinhAnh,
            LoiThoai = request.LoiThoai,
            CacDapAn = request.CacDapAn?.Select(dapAnRequest => new DapAnCauHoi
            {
                Nhan = dapAnRequest.Nhan,
                NoiDung = dapAnRequest.NoiDung,
                Dung = dapAnRequest.Dung,
                GiaiThich = dapAnRequest.GiaiThich
            }).ToList() ?? new List<DapAnCauHoi>()
        };

        var result = await _unitOfWork.CauHois.AddAsync(cauHoi);
        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<CauHoi?> UpdateAsync(string id, CauHoiUpdateRequest request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);
        var cauHoi = await _unitOfWork.CauHois.GetByIdAsync(id);
        if (cauHoi == null)
            return null;
        
        cauHoi.NoiDungCauHoi = request.NoiDungCauHoi ?? cauHoi.NoiDungCauHoi;
        cauHoi.LoaiCauHoi = request.LoaiCauHoi ?? cauHoi.LoaiCauHoi;
        cauHoi.KyNang = request.KyNang ?? cauHoi.KyNang;
        cauHoi.DoKho = request.DoKho ?? cauHoi.DoKho;
        cauHoi.CapDo = request.CapDo ?? cauHoi.CapDo;
        cauHoi.UrlAmThanh = request.UrlAmThanh ?? cauHoi.UrlAmThanh;
        cauHoi.UrlHinhAnh = request.UrlHinhAnh ?? cauHoi.UrlHinhAnh;
        cauHoi.LoiThoai = request.LoiThoai ?? cauHoi.LoiThoai;
        if (request.CacDapAn?.Count > 0)
        {
            cauHoi.CacDapAn.Clear();
            foreach (var dapAnRequest in request.CacDapAn)
            {
                cauHoi.CacDapAn.Add(new DapAnCauHoi
                {
                    Nhan = dapAnRequest.Nhan,
                    NoiDung = dapAnRequest.NoiDung,
                    Dung = dapAnRequest.Dung,
                    GiaiThich = dapAnRequest.GiaiThich
                });
            }
        }

        await _unitOfWork.CauHois.UpdateAsync(cauHoi);

        await _unitOfWork.SaveChangesAsync();
        return cauHoi;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var cauHoi = await _unitOfWork.CauHois.GetByIdAsync(id);
        if (cauHoi == null)
            return false;

        await _unitOfWork.CauHois.DeleteAsync(cauHoi);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<int> CountAsync()
    {
        return await _unitOfWork.CauHois.CountAsync();
    }

    public async Task<IEnumerable<CauHoi>> GetBySkillAsync(KyNang skill)
    {
        return await _unitOfWork.CauHois.GetAllAsync(filter: ch => ch.KyNang == skill);
    }

    public async Task<IEnumerable<CauHoi>> GetByDifficultyAsync(DoKho difficulty)
    {
        return await _unitOfWork.CauHois.GetAllAsync(filter: ch => ch.DoKho == difficulty);
    }

    public async Task<IEnumerable<CauHoi>> GetByLevelAsync(CapDo level)
    {
        return await _unitOfWork.CauHois.GetAllAsync(filter: ch => ch.CapDo == level);
    }

    public async Task<IEnumerable<CauHoi>> GetByTypeAsync(LoaiCauHoi type)
    {
        return await _unitOfWork.CauHois.GetAllAsync(filter: ch => ch.LoaiCauHoi == type);
    }

    public async Task<CauHoi> AddAnswerToQuestionAsync(string questionId, DapAnRequest answerRequest)
    {
        var cauHoi = await _unitOfWork.CauHois.GetByIdAsync(questionId);
        if (cauHoi == null)
            throw new KeyNotFoundException($"Không tìm thấy câu hỏi với ID: {questionId}");

        var dapAn = new DapAnCauHoi
        {
            Nhan = answerRequest.Nhan,
            NoiDung = answerRequest.NoiDung,
            Dung = answerRequest.Dung,
            GiaiThich = answerRequest.GiaiThich,
            CauHoiId = cauHoi.Id
        };

        cauHoi.CacDapAn.Add(dapAn);
        await _unitOfWork.SaveChangesAsync();
        return cauHoi;
    }

    public async Task<CauHoi?> GetQuestionWithAnswersAsync(string id)
    {
        var questions = await _unitOfWork.CauHois.GetAllAsync(
            filter: ch => ch.Id.ToString() == id,
            includes: ch => ch.CacDapAn
        );
        return questions.FirstOrDefault();
    }

    // Các method cũ - giữ lại để tương thích ngược
    public async Task<CauHoi> AddCorrectAnswerToQuestionAsync(string questionId, DapAnCauHoi dapAnCauHoi)
    {
        var cauHoi = await _unitOfWork.CauHois.GetByIdAsync(questionId);
        if (cauHoi == null)
        {
            throw new Exception("Câu hỏi không tồn tại");
        }

        cauHoi.CacDapAn.Add(dapAnCauHoi);
        await _unitOfWork.SaveChangesAsync();
        return cauHoi;
    }

    public async Task<CauHoi> AddGroupToQuestionAsync(string questionId, NhomCauHoiChiTiet nhomCauHoiChiTiet)
    {
        var cauHoi = await _unitOfWork.CauHois.GetByIdAsync(questionId);
        if (cauHoi == null)
        {
            throw new Exception("Câu hỏi không tồn tại");
        }

        cauHoi.CacNhom.Add(nhomCauHoiChiTiet);
        await _unitOfWork.SaveChangesAsync();
        return cauHoi;
    }
}

