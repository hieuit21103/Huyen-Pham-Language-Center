using FluentValidation;
using MsHuyenLC.Application.DTOs.Learning.PhienLamBai;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Services.Learning;
using MsHuyenLC.Domain.Entities.Learning.OnlineExam;

namespace MsHuyenLC.Application.Services.Learning;

public class TestSessionService : ITestSessionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<SubmitRequest> _submitValidator;

    public TestSessionService(
        IUnitOfWork unitOfWork,
        IValidator<SubmitRequest> submitValidator)
    {
        _unitOfWork = unitOfWork;
        _submitValidator = submitValidator;
    }
    public async Task<PhienLamBai?> GetByIdAsync(string id)
    {
        return await _unitOfWork.PhienLamBais.GetByIdAsync(id);
    }
    public async Task<IEnumerable<PhienLamBai>> GetAllAsync()
    {
        return await _unitOfWork.PhienLamBais.GetAllAsync();
    }
    public async Task<IEnumerable<PhienLamBai>> GetByHocVienIdAsync(string hocVienId)
    {
        if (string.IsNullOrEmpty(hocVienId) || !Guid.TryParse(hocVienId, out var guid))
            return Enumerable.Empty<PhienLamBai>();

        return await _unitOfWork.PhienLamBais.GetAllAsync(
            filter: p => p.HocVienId == guid
        );
    }
    public async Task<IEnumerable<PhienLamBai>> GetByDeThiIdAsync(string deThiId)
    {
        return await _unitOfWork.PhienLamBais.GetAllAsync(
            filter: p => p.DeThiId == Guid.Parse(deThiId)
        );
    }
    public async Task<PhienLamBai?> SubmitAsync(SubmitRequest request, Guid taiKhoanId)
    {
        await _submitValidator.ValidateAndThrowAsync(request);

        var hocVien = await _unitOfWork.HocViens.GetAllAsync(filter: hv => hv.TaiKhoanId == taiKhoanId);
        if (hocVien == null || !hocVien.Any())
            return null;

        var hocVienId = hocVien.First().Id;
        
        if (request == null)
            return null;

        if (!Guid.TryParse(request.DeThiId, out var deThiGuid))
            return null;

        var deThiList = await _unitOfWork.DeThis.GetAllAsync(
            filter: d => d.Id == deThiGuid,
            includes: d => d.CacCauHoi
        );
        var deThi = deThiList.FirstOrDefault();
        if (deThi == null)
            return null;

        var existingPhienList = await _unitOfWork.PhienLamBais.GetAllAsync(
            filter: p => p.HocVienId == hocVienId && p.DeThiId == deThiGuid
        );
        var existingPhien = existingPhienList.FirstOrDefault();

        if (existingPhien != null)
        {
            var existingAnswers = await _unitOfWork.CauTraLois.GetAllAsync(
                filter: ct => ct.PhienId == existingPhien.Id
            );
            
            if (existingPhien.KyThiId.HasValue && existingAnswers.Any())
            {
                throw new InvalidOperationException("Bạn đã nộp bài thi chính thức này rồi. Chỉ được thi một lần.");
            }
            
            if (!existingPhien.KyThiId.HasValue && existingAnswers.Any())
            {
                existingPhien = null; // Force create new PhienLamBai for practice retry
            }
        }

        var allCauHoiIds = deThi.CacCauHoi.Select(ch => ch.CauHoiId).ToList();
        var allCauHois = await _unitOfWork.CauHois.GetAllAsync(
            filter: ch => allCauHoiIds.Contains(ch.Id)
        );

        int? soCauDung = null;
        int soCauTracNghiem = 0;
        int soCauTuLuan = 0;
        decimal? diem = null;

        if (request.TuDongCham)
        {
            soCauDung = 0;
            
            foreach (var answer in request.CacTraLoi)
            {
                var cauHoi = allCauHois.FirstOrDefault(ch => ch.Id == answer.Key);
                if (cauHoi == null) continue;

                // Essay questions (KyNang.Viet = 4) cannot be auto-graded
                if (cauHoi.KyNang == Domain.Enums.KyNang.Viet)
                {
                    soCauTuLuan++;
                    continue;
                }

                soCauTracNghiem++;

                // Check if answer is correct (multiple choice)
                var dapAns = await _unitOfWork.DapAnCauHois.GetAllAsync(
                    filter: da => da.CauHoiId == answer.Key && da.Dung == true
                );

                var dapAnDung = dapAns.FirstOrDefault();
                if (dapAnDung != null && 
                    dapAnDung.NoiDung?.Trim().Equals(answer.Value?.Trim(), StringComparison.OrdinalIgnoreCase) == true)
                {
                    soCauDung++;
                }
            }

            // Calculate score only for multiple choice questions
            if (soCauTracNghiem > 0)
            {
                // If there are essay questions, calculate partial score
                if (soCauTuLuan > 0)
                {
                    // Score for multiple choice part (weighted by ratio)
                    decimal tracNghiemRatio = (decimal)soCauTracNghiem / request.TongCauHoi;
                    diem = Math.Round((decimal)soCauDung.Value * 10 * tracNghiemRatio / soCauTracNghiem, 2);
                }
                else
                {
                    // All multiple choice - calculate normally
                    diem = Math.Round((decimal)soCauDung.Value * 10 / request.TongCauHoi, 2);
                }
            }
            else if (soCauTuLuan > 0)
            {
                // All essay - cannot auto-grade, needs teacher review
                diem = null;
            }
        }

        PhienLamBai result;

        if (existingPhien != null)
        {
            // Update existing PhienLamBai (from JoinExamAsync)
            existingPhien.TongCauHoi = request.TongCauHoi;
            existingPhien.SoCauDung = soCauDung;
            existingPhien.Diem = diem;
            existingPhien.ThoiGianLam = TimeSpan.FromSeconds(request.ThoiGianLamBai);
            existingPhien.NgayLam = DateOnly.FromDateTime(DateTime.UtcNow);
            
            result = existingPhien;
        }
        else
        {
            // Create new PhienLamBai (for practice tests without join)
            var phienLamBai = new PhienLamBai
            {
                TongCauHoi = request.TongCauHoi,
                SoCauDung = soCauDung,
                Diem = diem,
                ThoiGianLam = TimeSpan.FromSeconds(request.ThoiGianLamBai),
                NgayLam = DateOnly.FromDateTime(DateTime.UtcNow),
                HocVienId = hocVienId,
                DeThiId = deThiGuid,
                KyThiId = deThi.KyThiId
            };

            result = await _unitOfWork.PhienLamBais.AddAsync(phienLamBai);
        }

        // Save answers
        foreach (var answer in request.CacTraLoi)
        {
            var cauHoi = allCauHois.FirstOrDefault(ch => ch.Id == answer.Key);
            bool isDung = false;

            // Only mark as correct for multiple choice questions
            if (cauHoi != null && cauHoi.KyNang != Domain.Enums.KyNang.Viet)
            {
                var dapAns = await _unitOfWork.DapAnCauHois.GetAllAsync(
                    filter: da => da.CauHoiId == answer.Key && da.Dung == true
                );

                var dapAnDung = dapAns.FirstOrDefault();
                isDung = dapAnDung != null && 
                         dapAnDung.NoiDung?.Trim().Equals(answer.Value?.Trim(), StringComparison.OrdinalIgnoreCase) == true;
            }

            var cauTraLoi = new CauTraLoi
            {
                PhienId = result.Id,
                CauHoiId = answer.Key,
                CauTraLoiText = answer.Value,
                Dung = isDung
            };

            await _unitOfWork.CauTraLois.AddAsync(cauTraLoi);
        }

        await _unitOfWork.SaveChangesAsync();

        return result;
    }

    public async Task<PhienLamBai?> GradeAsync(string id, GradingRequest request)
    {
        if (string.IsNullOrEmpty(id))
            return null;

        var phienLamBai = await _unitOfWork.PhienLamBais.GetByIdAsync(id);
        if (phienLamBai == null)
            return null;

        if (request.SoCauDung.HasValue)
            phienLamBai.SoCauDung = request.SoCauDung.Value;

        if (request.Diem.HasValue)
            phienLamBai.Diem = request.Diem.Value;

        await _unitOfWork.SaveChangesAsync();

        return phienLamBai;
    }

    /// <summary>
    /// Lấy chi tiết phiên làm bài kèm câu trả lời
    /// </summary>
    public async Task<PhienLamBaiResponse?> GetDetailsAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;

        var phienLamBai = await _unitOfWork.PhienLamBais.GetByIdAsync(id);
        if (phienLamBai == null)
            return null;

        var hocVien = await _unitOfWork.HocViens.GetByIdAsync(phienLamBai.HocVienId.ToString());

        var deThi = await _unitOfWork.DeThis.GetByIdAsync(phienLamBai.DeThiId.ToString());

        var response = new PhienLamBaiResponse
        {
            Id = phienLamBai.Id,
            DeThiId = phienLamBai.DeThiId,
            DeThi = deThi,
            HocVienId = phienLamBai.HocVienId,
            HocVien = hocVien,
            Diem = phienLamBai.Diem,
            SoCauDung = phienLamBai.SoCauDung,
            TongCauHoi = phienLamBai.TongCauHoi,
            ThoiGianLam = phienLamBai.ThoiGianLam,
            NgayLam = phienLamBai.NgayLam
        };

        return response;
    }

    /// <summary>
    /// Xóa phiên làm bài
    /// </summary>
    public async Task<bool> DeleteAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
            return false;

        var phienLamBai = await _unitOfWork.PhienLamBais.GetByIdAsync(id);
        if (phienLamBai == null)
            return false;

        // Xóa các câu trả lời trước
        var cauTraLois = await _unitOfWork.CauTraLois.GetAllAsync(
            filter: ct => ct.PhienId == phienLamBai.Id
        );

        foreach (var cauTraLoi in cauTraLois)
        {
            await _unitOfWork.CauTraLois.DeleteAsync(cauTraLoi);
        }

        // Xóa phiên làm bài
        await _unitOfWork.PhienLamBais.DeleteAsync(phienLamBai);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    /// <summary>
    /// Đếm số lượng phiên làm bài
    /// </summary>
    public async Task<int> CountAsync()
    {
        return await _unitOfWork.PhienLamBais.CountAsync();
    }

    /// <summary>
    /// Kiểm tra học viên đã làm bài chưa
    /// </summary>
    public async Task<bool> HasSubmittedAsync(string hocVienId, string deThiId)
    {
        if (string.IsNullOrEmpty(hocVienId) || string.IsNullOrEmpty(deThiId))
            return false;

        if (!Guid.TryParse(hocVienId, out var hocVienGuid) || !Guid.TryParse(deThiId, out var deThiGuid))
            return false;

        return await _unitOfWork.PhienLamBais.ExistsAsync(
            p => p.HocVienId == hocVienGuid && p.DeThiId == deThiGuid
        );
    }
}
