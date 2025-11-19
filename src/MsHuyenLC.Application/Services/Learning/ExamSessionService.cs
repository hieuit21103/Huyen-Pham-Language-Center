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
    
    private static readonly TimeZoneInfo VnTimeZone = GetVietnamTimeZone();

    private static TimeZoneInfo GetVietnamTimeZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        }
        catch
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Asia/Bangkok");
            }
            catch
            {
                return TimeZoneInfo.CreateCustomTimeZone(
                    "GMT+7",
                    TimeSpan.FromHours(7),
                    "GMT+7",
                    "GMT+7"
                );
            }
        }
    }

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

        var utcNow = DateTime.UtcNow;
        var vnNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, VnTimeZone);
        var today = DateOnly.FromDateTime(vnNow);
        var currentTime = TimeOnly.FromDateTime(vnNow);

        if (today != kyThi.NgayThi)
        {
            throw new InvalidOperationException($"Chưa đến ngày thi hoặc đã hết hạn làm bài. Ngày thi: {kyThi.NgayThi:dd/MM/yyyy}, Hôm nay: {today:dd/MM/yyyy}");
        }

        if (currentTime < kyThi.GioBatDau)
        {
            throw new InvalidOperationException($"Chưa đến giờ thi. Giờ bắt đầu: {kyThi.GioBatDau:HH\\:mm}, Giờ hiện tại: {currentTime:HH\\:mm}");
        }

        if (currentTime > kyThi.GioKetThuc)
        {
            throw new InvalidOperationException($"Đã hết giờ làm bài. Giờ kết thúc: {kyThi.GioKetThuc:HH\\:mm}, Giờ hiện tại: {currentTime:HH\\:mm}");
        }

        var existingPhienLamBai = await _unitOfWork.PhienLamBais.GetAllAsync(
            filter: p => p.KyThiId == request.KyThiId && p.HocVienId == request.HocVienId
        );

        if (existingPhienLamBai.Any())
        {
            var existingPhien = existingPhienLamBai.First();
            return existingPhien.DeThiId;
        }

        var hocVien = await _unitOfWork.HocViens.GetByIdAsync(request.HocVienId.ToString());
        if (hocVien == null)
            throw new KeyNotFoundException($"Không tìm thấy học viên với ID: {request.HocVienId}");
        var taiKhoan = await _unitOfWork.TaiKhoans.GetByIdAsync(hocVien.TaiKhoanId.ToString());
        if (taiKhoan == null)
        {
            throw new KeyNotFoundException($"Không tìm thấy tài khoản học viên với ID: {request.HocVienId}");
        }

        var generateExamRequest = new GenerateExamRequest
        {
            KyThiId = request.KyThiId,
            HocVienId = request.HocVienId,
            NguoiTaoId = taiKhoan.Id
        };

        var deThi = await _examService.GenerateExamAsync(generateExamRequest);
        
        var phienLamBai = new PhienLamBai
        {
            Id = Guid.NewGuid(),
            HocVienId = request.HocVienId,
            DeThiId = deThi.Id,
            KyThiId = request.KyThiId,
            NgayLam = today,
            ThoiGianLam = TimeSpan.Zero,
            TongCauHoi = deThi.CacCauHoi.Count
        };

        await _unitOfWork.PhienLamBais.AddAsync(phienLamBai);
        await _unitOfWork.SaveChangesAsync();

        return deThi.Id;
    }

    public async Task<IEnumerable<KyThi>> GetByStudentIdAsync(Guid studentId)
    {
        var hocVien = await _unitOfWork.HocViens.GetByIdAsync(studentId.ToString());
        if (hocVien == null)
            throw new KeyNotFoundException($"Không tìm thấy học viên với ID: {studentId}");

        var dangKyKhoaHocs = await _unitOfWork.DangKyKhoaHocs.GetAllAsync(
            filter: dk => dk.HocVienId == studentId && 
                         dk.LopHocId != null
        );

        var lopHocIds = dangKyKhoaHocs
            .Where(dk => dk.LopHocId.HasValue)
            .Select(dk => dk.LopHocId!.Value)
            .Distinct()
            .ToList();

        if (!lopHocIds.Any())
            return Enumerable.Empty<KyThi>();

        var kyThis = await _unitOfWork.KyThis.GetAllAsync(
            filter: k => lopHocIds.Contains(k.LopHocId)
        );

        return kyThis;
    }
}
