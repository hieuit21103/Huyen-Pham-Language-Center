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
        // Validate request
        await _submitValidator.ValidateAndThrowAsync(request);

        var hocVien = await _unitOfWork.HocViens.GetAllAsync(filter: hv => hv.TaiKhoanId == taiKhoanId);
        if (hocVien == null || !hocVien.Any())
            return null;

        var hocVienId = hocVien.First().Id;
        {
            if (request == null)
                return null;

            // Validate đề thi tồn tại
            if (!Guid.TryParse(request.DeThiId, out var deThiGuid))
                return null;

            var deThi = await _unitOfWork.DeThis.GetByIdAsync(request.DeThiId);
            if (deThi == null)
                return null;

            // Kiểm tra học viên đã làm bài chưa
            var existed = await HasSubmittedAsync(hocVienId.ToString(), request.DeThiId);
            if (existed)
                return null;

            // Tính điểm nếu tự động chấm
            int? soCauDung = null;
            decimal? diem = null;

            if (request.TuDongCham)
            {
                soCauDung = 0;
                foreach (var answer in request.CacTraLoi)
                {
                    var dapAn = await _unitOfWork.DapAnCauHois.GetAllAsync(
                        filter: da => da.CauHoiId == answer.Key && da.Dung == true
                    );

                    var dapAnDung = dapAn.FirstOrDefault();
                    if (dapAnDung != null && dapAnDung.NoiDung?.Trim().ToLower() == answer.Value?.Trim().ToLower())
                    {
                        soCauDung++;
                    }
                }

                if (request.TongCauHoi > 0)
                {
                    diem = Math.Round((decimal)soCauDung.Value * 10 / request.TongCauHoi, 2);
                }
            }

            var phienLamBai = new PhienLamBai
            {
                TongCauHoi = request.TongCauHoi,
                SoCauDung = soCauDung,
                Diem = diem,
                ThoiGianLam = TimeSpan.FromSeconds(request.ThoiGianLamBai),
                NgayLam = DateOnly.FromDateTime(DateTime.UtcNow),
                HocVienId = hocVienId,
                DeThiId = deThiGuid
            };

            var result = await _unitOfWork.PhienLamBais.AddAsync(phienLamBai);

            foreach (var answer in request.CacTraLoi)
            {
                var dapAn = await _unitOfWork.DapAnCauHois.GetAllAsync(
                    filter: da => da.CauHoiId == answer.Key && da.Dung == true
                );

                var dapAnDung = dapAn.FirstOrDefault();
                bool isDung = dapAnDung != null && dapAnDung.NoiDung?.Trim().ToLower() == answer.Value?.Trim().ToLower();

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
