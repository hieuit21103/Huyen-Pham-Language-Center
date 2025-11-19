using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.KyThi;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Services.Learning;
using MsHuyenLC.Application.DTOs.Learning.DeThi;
using MsHuyenLC.Domain.Entities.Learning.OnlineExam;
using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.Services.Learning;

public class ExamSessionService : IExamSessionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<KyThiRequest> _createValidator;
    private readonly IValidator<KyThiUpdateRequest> _updateValidator;
    private readonly IValidator<JoinExamRequest> _joinExamValidator;
    private readonly IExamService _examService;

    public ExamSessionService(
        IUnitOfWork unitOfWork,
        IValidator<KyThiRequest> createValidator,
        IValidator<KyThiUpdateRequest> updateValidator,
        IValidator<JoinExamRequest> joinExamValidator,
        IExamService examService)
    {
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _joinExamValidator = joinExamValidator;
        _examService = examService;
    }

    public async Task<KyThi?> GetByIdAsync(string id)
    {
        return await _unitOfWork.KyThis.GetByIdAsync(id);
    }

    public async Task<IEnumerable<KyThi>> GetAllAsync()
    {
        return await _unitOfWork.KyThis.GetAllAsync();
    }

    public async Task<KyThi> CreateAsync(KyThiRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);

        var kyThi = new KyThi
        {
            TenKyThi = request.TenKyThi,
            NgayThi = request.NgayThi,
            GioBatDau = request.GioBatDau,
            GioKetThuc = request.GioKetThuc,
            ThoiLuong = request.ThoiLuong,
            LopHocId = request.LopHocId,
            TrangThai = TrangThaiKyThi.sapdienra
        };

        var result = await _unitOfWork.KyThis.AddAsync(kyThi);

        var cauHinhKyThi = new List<CauHinhKyThi>();
        foreach (var config in request.CauHinhKyThis)
        {
            cauHinhKyThi.Add(new CauHinhKyThi
            {
                CapDo = config.CapDo,
                SoCauHoi = config.SoCauHoi,
                CheDoCauHoi = config.CheDoCauHoi,
                DoKho = config.DoKho,
                KyNang = config.KyNang,
                KyThiId = result.Id
            });
        }
        
        result.CauHinhKyThis = cauHinhKyThi;
        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<KyThi?> UpdateAsync(string id, KyThiUpdateRequest request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);

        // Load KyThi với CauHinhKyThis collection
        var kyThiList = await _unitOfWork.KyThis.GetAllAsync(
            filter: k => k.Id == Guid.Parse(id),
            includes: k => k.CauHinhKyThis
        );
        var kyThi = kyThiList.FirstOrDefault();
        if (kyThi == null)
            throw new KeyNotFoundException($"Không tìm thấy kỳ thi với ID: {id}");

        // Update basic fields
        kyThi.TenKyThi = request.TenKyThi ?? kyThi.TenKyThi;
        kyThi.NgayThi = request.NgayThi ?? kyThi.NgayThi;
        kyThi.GioBatDau = request.GioBatDau ?? kyThi.GioBatDau;
        kyThi.GioKetThuc = request.GioKetThuc ?? kyThi.GioKetThuc;
        kyThi.ThoiLuong = request.ThoiLuong ?? kyThi.ThoiLuong;
        kyThi.TrangThai = request.TrangThai ?? kyThi.TrangThai;
        kyThi.LopHocId = request.LopHocId ?? kyThi.LopHocId;

        // Update CauHinhKyThis nếu có
        if (request.CauHinhKyThis != null && request.CauHinhKyThis.Any())
        {
            // Clear existing configs
            kyThi.CauHinhKyThis.Clear();
            
            // Add new configs
            foreach (var configRequest in request.CauHinhKyThis)
            {
                kyThi.CauHinhKyThis.Add(new CauHinhKyThi
                {
                    CapDo = configRequest.CapDo ?? default,
                    DoKho = configRequest.DoKho ?? default,
                    KyNang = configRequest.KyNang ?? default,
                    CheDoCauHoi = configRequest.CheDoCauHoi ?? default,
                    SoCauHoi = configRequest.SoCauHoi ?? 0
                });
            }
        }

        await _unitOfWork.SaveChangesAsync();
        return kyThi;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var kyThi = await _unitOfWork.KyThis.GetByIdAsync(id);
        if (kyThi == null)
            return false;

        await _unitOfWork.KyThis.DeleteAsync(kyThi);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<KyThi>> GetByClassIdAsync(string classId)
    {
        if (!Guid.TryParse(classId, out var id))
            return Enumerable.Empty<KyThi>();

        return await _unitOfWork.KyThis.GetAllAsync(k => k.LopHocId == id);
    }

    public async Task<int> CountAsync()
    {
        return await _unitOfWork.KyThis.CountAsync();
    }

    public async Task<Guid> JoinExamAsync(JoinExamRequest request)
    {
        await _joinExamValidator.ValidateAndThrowAsync(request);

        var kyThiList = await _unitOfWork.KyThis.GetAllAsync(
            filter: k => k.Id == request.KyThiId,
            includes: k => k.CauHinhKyThis
        );
        var kyThi = kyThiList.FirstOrDefault();
        
        if (kyThi == null)
            throw new KeyNotFoundException($"Không tìm thấy kỳ thi với ID: {request.KyThiId}");

        if (kyThi.TrangThai != TrangThaiKyThi.dangdienra)
        {
            throw new InvalidOperationException($"Kỳ thi chưa bắt đầu hoặc đã kết thúc. Trạng thái: {kyThi.TrangThai}");
        }

        // Validate exam timing (date and time)
        var now = DateTime.Now;
        var today = DateOnly.FromDateTime(now);
        var currentTime = TimeOnly.FromDateTime(now);

        if (today != kyThi.NgayThi)
        {
            throw new InvalidOperationException("Chưa đến ngày thi hoặc đã hết hạn làm bài");
        }

        if (currentTime < kyThi.GioBatDau)
        {
            throw new InvalidOperationException($"Chưa đến giờ thi. Giờ bắt đầu: {kyThi.GioBatDau:HH\\:mm}");
        }

        if (currentTime > kyThi.GioKetThuc)
        {
            throw new InvalidOperationException($"Đã hết giờ làm bài. Giờ kết thúc: {kyThi.GioKetThuc:HH\\:mm}");
        }

        // Check if student already joined this exam
        var existingPhienLamBai = await _unitOfWork.PhienLamBais.GetAllAsync(
            filter: p => p.KyThiId == request.KyThiId && p.HocVienId == request.HocVienId
        );

        if (existingPhienLamBai.Any())
        {
            var existingPhien = existingPhienLamBai.First();
            return existingPhien.DeThiId;
        }

        var generateExamRequest = new GenerateExamRequest
        {
            KyThiId = request.KyThiId,
            HocVienId = request.HocVienId,
            NguoiTaoId = Guid.Empty // System generated
        };

        var deThi = await _examService.GenerateExamAsync(generateExamRequest);
        var phienLamBai = new PhienLamBai
        {
            Id = Guid.NewGuid(),
            HocVienId = request.HocVienId,
            DeThiId = deThi.Id,
            KyThiId = request.KyThiId,
            NgayLam = DateOnly.FromDateTime(DateTime.UtcNow),
            ThoiGianLam = TimeSpan.Zero,
            TongCauHoi = deThi.CacCauHoi.Count
        };

        await _unitOfWork.PhienLamBais.AddAsync(phienLamBai);
        await _unitOfWork.SaveChangesAsync();

        return deThi.Id;
    }
}
