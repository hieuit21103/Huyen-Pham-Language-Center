using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Services.Learning;
using MsHuyenLC.Application.DTOs.Learning.DeThi;
using MsHuyenLC.Domain.Entities.Learning.OnlineExam;
using MsHuyenLC.Domain.Enums;
using FluentValidation;

namespace MsHuyenLC.Application.Services.Learning;

public class ExamService : IExamService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<DeThiRequest> _createValidator;
    private readonly IValidator<DeThiUpdateRequest> _updateValidator;
    private readonly IValidator<GenerateExamRequest> _generateValidator;
    private readonly Random _random;

    public ExamService(
        IUnitOfWork unitOfWork,
        IValidator<DeThiRequest> createValidator,
        IValidator<DeThiUpdateRequest> updateValidator,
        IValidator<GenerateExamRequest> generateValidator)
    {
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _generateValidator = generateValidator;
        _random = new Random();
    }

    #region CRUD Operations

    public async Task<DeThi?> GetByIdAsync(string id)
    {
        return await _unitOfWork.DeThis.GetByIdAsync(id);
    }

    public async Task<IEnumerable<DeThi>> GetAllAsync()
    {
        return await _unitOfWork.DeThis.GetAllAsync();
    }

    public async Task<DeThi> CreateAsync(DeThiRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);

        var deThi = new DeThi
        {
            TenDe = request.TenDe,
            MaDe = $"DT{DateTime.UtcNow:yyyyMMddHHmmss}",
            ThoiLuongPhut = request.ThoiGianLamBai,
            NgayTao = DateTime.UtcNow,
            NguoiTaoId = request.NguoiTaoId ?? Guid.Empty
        };

        if (!string.IsNullOrEmpty(request.KyThiId) && Guid.TryParse(request.KyThiId, out var kyThiId))
        {
            deThi.KyThiId = kyThiId;
        }

        var result = await _unitOfWork.DeThis.AddAsync(deThi);

        var thuTu = 1;
        foreach (var cauHoiId in request.CauHoiIds)
        {
            if (!Guid.TryParse(cauHoiId, out var cauHoiGuid))
            {
                throw new ArgumentException($"ID câu hỏi không hợp lệ: {cauHoiId}");
            }

            var cauHoi = await _unitOfWork.CauHois.GetByIdAsync(cauHoiId);
            if (cauHoi == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy câu hỏi với ID: {cauHoiId}");
            }

            var cauHoiDeThi = new CauHoiDeThi
            {
                DeThiId = result.Id,
                CauHoiId = cauHoiGuid,
                ThuTuCauHoi = thuTu++
            };

            await _unitOfWork.CauHoiDeThis.AddAsync(cauHoiDeThi);
        }

        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<DeThi?> UpdateAsync(string id, DeThiUpdateRequest request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);

        var deThi = await _unitOfWork.DeThis.GetByIdAsync(id);
        if (deThi == null)
            return null;

        deThi.TenDe = request.TenDe;
        deThi.ThoiLuongPhut = request.ThoiGianLamBai;

        await _unitOfWork.DeThis.UpdateAsync(deThi);
        await _unitOfWork.SaveChangesAsync();

        return deThi;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var deThi = await _unitOfWork.DeThis.GetByIdAsync(id);
        if (deThi == null)
            return false;

        await _unitOfWork.DeThis.DeleteAsync(deThi);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<int> CountAsync()
    {
        return await _unitOfWork.DeThis.CountAsync();
    }

    public async Task<DeThi?> GetWithQuestionsAsync(string id)
    {
        var deThiList = await _unitOfWork.DeThis.GetAllAsync(
            filter: d => d.Id == Guid.Parse(id),
            includes: d => d.CacCauHoi
        );
        
        var deThi = deThiList.FirstOrDefault();
        if (deThi == null) return null;

        foreach (var cauHoiDeThi in deThi.CacCauHoi)
        {
            if (cauHoiDeThi.CauHoiId != Guid.Empty)
            {
                var cauHoi = await _unitOfWork.CauHois.GetByIdAsync(cauHoiDeThi.CauHoiId.ToString());
                if (cauHoi != null)
                {
                    cauHoiDeThi.CauHoi = cauHoi;
                    
                    var dapAns = await _unitOfWork.DapAnCauHois.GetAllAsync(
                        filter: da => da.CauHoiId == cauHoi.Id
                    );
                    cauHoi.CacDapAn = dapAns.ToList();
                }
            }

            if (cauHoiDeThi.NhomCauHoiId.HasValue)
            {
                var nhom = await _unitOfWork.NhomCauHois.GetByIdAsync(cauHoiDeThi.NhomCauHoiId.Value.ToString());
                if (nhom != null)
                {
                    cauHoiDeThi.NhomCauHoi = nhom;
                }
            }
        }

        return deThi;
    }

    public async Task<IEnumerable<DeThi>> GetByCreatorAsync(Guid creatorId)
    {
        return await _unitOfWork.DeThis.GetAllAsync(d => d.NguoiTaoId == creatorId);
    }

    public async Task<IEnumerable<DeThi>> GetByExamSessionAsync(Guid kyThiId)
    {
        return await _unitOfWork.DeThis.GetAllAsync(d => d.KyThiId == kyThiId);
    }

    #endregion

    #region Auto Generation

    public async Task<DeThi> GeneratePracticeTestAsync(
        CapDo capDo,
        DoKho doKho,
        KyNang kyNang,
        int soCauHoi,
        int thoiLuongPhut,
        Guid nguoiTaoId,
        CheDoCauHoi cheDoCauHoi = CheDoCauHoi.Don)
    {
        var tenDe = $"Đề luyện tập {kyNang} - {capDo} - {DateTime.UtcNow:yyyyMMddHHmmss}";

        var deThi = new DeThi
        {
            Id = Guid.NewGuid(),
            TenDe = tenDe,
            MaDe = $"LTAP{DateTime.UtcNow:yyyyMMddHHmmss}",
            ThoiLuongPhut = thoiLuongPhut,
            NgayTao = DateTime.UtcNow,
            NguoiTaoId = nguoiTaoId
        };

        List<CauHoi> selectedQuestions;
        var allNhomCauHoiIds = new HashSet<Guid>();

        if (cheDoCauHoi == CheDoCauHoi.Don)
        {
            var allQuestions = await _unitOfWork.CauHois.GetAllAsync(
                filter: q => q.CapDo == capDo
                          && q.DoKho == doKho
                          && q.KyNang == kyNang
            );

            var questionList = allQuestions.ToList();

            var questionsWithGroups = new HashSet<Guid>();
            var nhomChiTiets = await _unitOfWork.NhomCauHoiChiTiets.GetAllAsync();
            foreach (var nct in nhomChiTiets)
            {
                questionsWithGroups.Add(nct.CauHoiId);
            }

            var independentQuestions = questionList
                .Where(q => !questionsWithGroups.Contains(q.Id))
                .ToList();

            if (independentQuestions.Count < soCauHoi)
            {
                throw new InvalidOperationException(
                    $"Không đủ câu hỏi đơn lẻ. Yêu cầu: {soCauHoi}, Có sẵn: {independentQuestions.Count}. " +
                    $"Tiêu chí: CapDo={capDo}, DoKho={doKho}, KyNang={kyNang}"
                );
            }

            selectedQuestions = ShuffleQuestions(independentQuestions).Take(soCauHoi).ToList();
        }
        else
        {
            var allNhoms = await _unitOfWork.NhomCauHois.GetAllAsync(
                filter: n => n.CapDo == capDo
                          && n.DoKho == doKho
                          && n.KyNang == kyNang
            );

            var nhomList = allNhoms.ToList();

            if (nhomList.Count < soCauHoi)
            {
                throw new InvalidOperationException(
                    $"Không đủ nhóm câu hỏi. Yêu cầu: {soCauHoi} nhóm, Có sẵn: {nhomList.Count}. " +
                    $"Tiêu chí: CapDo={capDo}, DoKho={doKho}, KyNang={kyNang}"
                );
            }

            var shuffledNhoms = ShuffleNhoms(nhomList).Take(soCauHoi).ToList();
            selectedQuestions = new List<CauHoi>();

            foreach (var nhom in shuffledNhoms)
            {
                allNhomCauHoiIds.Add(nhom.Id);

                var chiTiets = await _unitOfWork.NhomCauHoiChiTiets.GetAllAsync(
                    filter: nct => nct.NhomId == nhom.Id
                );

                var orderedChiTiets = chiTiets.OrderBy(nct => nct.ThuTu).ToList();

                foreach (var chiTiet in orderedChiTiets)
                {
                    var cauHoi = await _unitOfWork.CauHois.GetByIdAsync(chiTiet.CauHoiId.ToString());
                    if (cauHoi != null)
                    {
                        selectedQuestions.Add(cauHoi);
                    }
                }
            }
        }

        if (!selectedQuestions.Any())
            throw new InvalidOperationException("Không tìm thấy câu hỏi nào phù hợp");

        var thuTu = 1;
        foreach (var cauHoi in selectedQuestions)
        {
            Guid? nhomCauHoiId = null;
            if (cheDoCauHoi == CheDoCauHoi.Nhom)
            {
                var nhomChiTiets = await _unitOfWork.NhomCauHoiChiTiets.GetAllAsync(
                    filter: nct => nct.CauHoiId == cauHoi.Id
                );
                var nhomChiTiet = nhomChiTiets.FirstOrDefault();
                if (nhomChiTiet != null && allNhomCauHoiIds.Contains(nhomChiTiet.NhomId))
                {
                    nhomCauHoiId = nhomChiTiet.NhomId;
                }
            }

            var cauHoiDeThi = new CauHoiDeThi
            {
                Id = Guid.NewGuid(),
                DeThiId = deThi.Id,
                CauHoiId = cauHoi.Id,
                ThuTuCauHoi = thuTu++,
                Diem = 1,
                NhomCauHoiId = nhomCauHoiId
            };

            deThi.CacCauHoi.Add(cauHoiDeThi);
        }

        await _unitOfWork.DeThis.AddAsync(deThi);
        await _unitOfWork.SaveChangesAsync();

        return deThi;
    }

    public async Task<DeThi> GenerateExamAsync(GenerateExamRequest request)
    {
        await _generateValidator.ValidateAndThrowAsync(request);

        var tenDe = $"Đề thi tự động - {DateTime.UtcNow:yyyyMMddHHmmss}";
        var kyThiList = await _unitOfWork.KyThis.GetAllAsync(
            filter: k => k.Id == request.KyThiId,
            includes: k => k.CauHinhKyThis
        );
        
        var kyThi = kyThiList.FirstOrDefault();
        if (kyThi == null)
            throw new KeyNotFoundException($"Không tìm thấy kỳ thi với ID: {request.KyThiId}");

        if (kyThi.CauHinhKyThis == null || !kyThi.CauHinhKyThis.Any())
            throw new InvalidOperationException($"Kỳ thi '{kyThi.TenKyThi}' chưa có cấu hình. Vui lòng thêm cấu hình trước.");

        var deThi = new DeThi
        {
            Id = Guid.NewGuid(),
            TenDe = tenDe,
            MaDe = $"DT{DateTime.UtcNow:yyyyMMddHHmmss}_{request.HocVienId.ToString().Substring(0, 8)}",
            ThoiLuongPhut = kyThi.ThoiLuong,
            KyThiId = request.KyThiId,
            NgayTao = DateTime.UtcNow,
            NguoiTaoId = request.NguoiTaoId
        };

        var allQuestions = new List<CauHoi>();
        var allNhomCauHoiIds = new HashSet<Guid>();

        foreach (var config in kyThi.CauHinhKyThis)
        {
            List<CauHoi> selectedQuestions;

            if (config.CheDoCauHoi == CheDoCauHoi.Don)
            {
                selectedQuestions = await RandomCauHoiDonAsync(config, request.HocVienId);
            }
            else
            {
                var (questions, nhomIds) = await RandomNhomCauHoiAsync(config, request.HocVienId);
                selectedQuestions = questions;
                
                foreach (var nhomId in nhomIds)
                {
                    allNhomCauHoiIds.Add(nhomId);
                }
            }

            allQuestions.AddRange(selectedQuestions);
        }

        if (!allQuestions.Any())
            throw new InvalidOperationException("Không tìm thấy câu hỏi nào phù hợp với cấu hình kỳ thi");

        var shuffledQuestions = allQuestions;

        var thuTu = 1;
        foreach (var cauHoi in shuffledQuestions)
        {
            Guid? nhomCauHoiId = null;
            var nhomChiTiets = await _unitOfWork.NhomCauHoiChiTiets.GetAllAsync(
                filter: nct => nct.CauHoiId == cauHoi.Id
            );
            var nhomChiTiet = nhomChiTiets.FirstOrDefault();
            if (nhomChiTiet != null && allNhomCauHoiIds.Contains(nhomChiTiet.NhomId))
            {
                nhomCauHoiId = nhomChiTiet.NhomId;
            }

            var cauHoiDeThi = new CauHoiDeThi
            {
                Id = Guid.NewGuid(),
                DeThiId = deThi.Id,
                CauHoiId = cauHoi.Id,
                ThuTuCauHoi = thuTu++,
                Diem = 1,
                NhomCauHoiId = nhomCauHoiId
            };

            deThi.CacCauHoi.Add(cauHoiDeThi);
        }

        await _unitOfWork.DeThis.AddAsync(deThi);
        await _unitOfWork.SaveChangesAsync();

        return deThi;
    }

    

    /// <summary>
    /// Random câu hỏi đơn lẻ (không thuộc nhóm)
    /// </summary>
    private async Task<List<CauHoi>> RandomCauHoiDonAsync(CauHinhKyThi config, Guid hocVienId)
    {
        var allQuestions = await _unitOfWork.CauHois.GetAllAsync(
            filter: q => q.CapDo == config.CapDo
                      && q.DoKho == config.DoKho
                      && q.KyNang == config.KyNang
        );

        var questionList = allQuestions.ToList();

        var questionsWithGroups = new HashSet<Guid>();
        var nhomChiTiets = await _unitOfWork.NhomCauHoiChiTiets.GetAllAsync();
        foreach (var nct in nhomChiTiets)
        {
            questionsWithGroups.Add(nct.CauHoiId);
        }

        var independentQuestions = questionList
            .Where(q => !questionsWithGroups.Contains(q.Id))
            .ToList();

        if (independentQuestions.Count < config.SoCauHoi)
        {
            throw new InvalidOperationException(
                $"Không đủ câu hỏi đơn lẻ. Yêu cầu: {config.SoCauHoi}, Có sẵn: {independentQuestions.Count}. " +
                $"Tiêu chí: CapDo={config.CapDo}, DoKho={config.DoKho}, KyNang={config.KyNang}"
            );
        }
        return independentQuestions.Take(config.SoCauHoi).ToList();
    }

    /// <summary>
    private async Task<(List<CauHoi> questions, List<Guid> nhomIds)> RandomNhomCauHoiAsync(
        CauHinhKyThi config, 
        Guid hocVienId)
    {
        var allNhoms = await _unitOfWork.NhomCauHois.GetAllAsync(
            filter: n => n.CapDo == config.CapDo
                      && n.DoKho == config.DoKho
                      && n.KyNang == config.KyNang
        );

        var nhomList = allNhoms.ToList();

        if (nhomList.Count < config.SoCauHoi)
        {
            throw new InvalidOperationException(
                $"Không đủ nhóm câu hỏi. Yêu cầu: {config.SoCauHoi} nhóm, Có sẵn: {nhomList.Count}. " +
                $"Tiêu chí: CapDo={config.CapDo}, DoKho={config.DoKho}, KyNang={config.KyNang}"
            );
        }

        var selectedNhoms = nhomList.Take(config.SoCauHoi).ToList();

        var allQuestions = new List<CauHoi>();
        var nhomIds = new List<Guid>();

        foreach (var nhom in selectedNhoms)
        {
            nhomIds.Add(nhom.Id);

            var chiTiets = await _unitOfWork.NhomCauHoiChiTiets.GetAllAsync(
                filter: nct => nct.NhomId == nhom.Id
            );

            var orderedChiTiets = chiTiets.OrderBy(nct => nct.ThuTu).ToList();

            foreach (var chiTiet in orderedChiTiets)
            {
                var cauHoi = await _unitOfWork.CauHois.GetByIdAsync(chiTiet.CauHoiId.ToString());
                if (cauHoi != null)
                {
                    allQuestions.Add(cauHoi);
                }
            }
        }

        return (allQuestions, nhomIds);
    }

    /// <summary>
    /// Shuffle danh sách câu hỏi sử dụng Fisher-Yates algorithm
    /// </summary>
    private List<CauHoi> ShuffleQuestions(List<CauHoi> questions)
    {
        var shuffled = new List<CauHoi>(questions);
        int n = shuffled.Count;
        
        for (int i = n - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            var temp = shuffled[i];
            shuffled[i] = shuffled[j];
            shuffled[j] = temp;
        }
        
        return shuffled;
    }

    /// <summary>
    /// Shuffle danh sách nhóm câu hỏi sử dụng Fisher-Yates algorithm
    /// </summary>
    private List<NhomCauHoi> ShuffleNhoms(List<NhomCauHoi> nhoms)
    {
        var shuffled = new List<NhomCauHoi>(nhoms);
        int n = shuffled.Count;
        
        for (int i = n - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            var temp = shuffled[i];
            shuffled[i] = shuffled[j];
            shuffled[j] = temp;
        }
        
        return shuffled;
    }

    #endregion
}
