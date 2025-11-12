using MsHuyenLC.Domain.Entities.Learning;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Services.Learning;
using MsHuyenLC.Application.DTOs.Learning.DeThi;
using MsHuyenLC.Domain.Enums;
using MsHuyenLC.Domain.Entities.Learning.OnlineExam;
using FluentValidation;

namespace MsHuyenLC.Application.Services.Learning;

public class TestService : ITestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<DeThiRequest> _createValidator;
    private readonly IValidator<DeThiUpdateRequest> _updateValidator;

    public TestService(IUnitOfWork unitOfWork, IValidator<DeThiRequest> createValidator, IValidator<DeThiUpdateRequest> updateValidator)
    {
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

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
            TongCauHoi = request.TongCauHoi,
            ThoiLuongPhut = request.ThoiGianLamBai,
            LoaiDeThi = request.LoaiDeThi,
            NgayTao = DateTime.UtcNow,
            NguoiTaoId = request.NguoiTaoId ?? Guid.Empty,
        };

        if (!string.IsNullOrEmpty(request.KyThiId) && Guid.TryParse(request.KyThiId, out var kyThiId))
        {
            deThi.KyThiId = kyThiId;
        }

        var result = await _unitOfWork.DeThis.AddAsync(deThi);
        await _unitOfWork.SaveChangesAsync();

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
        var deThi = await _unitOfWork.DeThis.GetByIdAsync(id);
        if (deThi == null)
            return null;

        if (request.CauHoiIds != null && request.CauHoiIds.Count > 0)
        {
            if (request.TongCauHoi != request.CauHoiIds.Count)
            {
                throw new ArgumentException(
                    $"Số câu hỏi ({request.TongCauHoi}) không khớp với số lượng câu hỏi được chọn ({request.CauHoiIds.Count})"
                );
            }
        }

        deThi.TenDe = request.TenDe;
        deThi.TongCauHoi = request.TongCauHoi;
        deThi.ThoiLuongPhut = request.ThoiGianLamBai;
        deThi.LoaiDeThi = request.LoaiDeThi;
        deThi.KyThiId = request.KyThiId;

        if (request.CauHoiIds != null && request.CauHoiIds.Count > 0)
        {
            var oldCauHoiDeThis = await _unitOfWork.CauHoiDeThis.GetAllAsync(
                filter: cd => cd.DeThiId == deThi.Id
            );

            foreach (var old in oldCauHoiDeThis)
            {
                await _unitOfWork.CauHoiDeThis.DeleteAsync(old);
            }

            await _unitOfWork.SaveChangesAsync();

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
                    DeThiId = deThi.Id,
                    CauHoiId = cauHoiGuid,
                    ThuTuCauHoi = thuTu++
                };

                await _unitOfWork.CauHoiDeThis.AddAsync(cauHoiDeThi);
            }
        }

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

    public async Task<DeThi> AddQuestionToTestAsync(string testId, string questionId)
    {
        var deThi = await _unitOfWork.DeThis.GetByIdAsync(testId);
        if (deThi == null)
            throw new KeyNotFoundException($"Không tìm thấy đề thi với ID: {testId}");

        var cauHoi = await _unitOfWork.CauHois.GetByIdAsync(questionId);
        if (cauHoi == null)
            throw new KeyNotFoundException($"Không tìm thấy câu hỏi với ID: {questionId}");

        var cauHoiDeThi = new CauHoiDeThi
        {
            DeThiId = deThi.Id,
            CauHoiId = cauHoi.Id,
            ThuTuCauHoi = deThi.CacCauHoi.Count + 1
        };

        deThi.CacCauHoi.Add(cauHoiDeThi);
        await _unitOfWork.SaveChangesAsync();
        return deThi;
    }

    public async Task<bool> RemoveQuestionFromTestAsync(string testId, string questionId)
    {
        var deThi = await _unitOfWork.DeThis.GetByIdAsync(testId);
        if (deThi == null)
            return false;

        if (!Guid.TryParse(questionId, out var cauHoiId))
            return false;

        var cauHoiDeThi = deThi.CacCauHoi.FirstOrDefault(chdt => chdt.CauHoiId == cauHoiId);
        if (cauHoiDeThi == null)
            return false;

        deThi.CacCauHoi.Remove(cauHoiDeThi);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<DeThi>> GetTestsByCreatorAsync(string creatorId)
    {
        if (!Guid.TryParse(creatorId, out var id))
            return Enumerable.Empty<DeThi>();

        return await _unitOfWork.DeThis.GetAllAsync(filter: dt => dt.NguoiTaoId == id);
    }

    public async Task<IEnumerable<DeThi>> GetTestsByExamAsync(string examId)
    {
        if (!Guid.TryParse(examId, out var id))
            return Enumerable.Empty<DeThi>();

        return await _unitOfWork.DeThis.GetAllAsync(filter: dt => dt.KyThiId == id);
    }

    public async Task<DeThi> GenerateTestAsync(GenerateTestRequest request)
    {
        if (request.TongCauHoi <= 0)
            throw new ArgumentException("Số câu hỏi phải lớn hơn 0", nameof(request.TongCauHoi));

        if (request.ThoiGianLamBai <= 0)
            throw new ArgumentException("Thời gian làm bài phải lớn hơn 0", nameof(request.ThoiGianLamBai));

        if (string.IsNullOrWhiteSpace(request.TenDe))
            throw new ArgumentException("Tên đề không được để trống", nameof(request.TenDe));

        var deThi = new DeThi
        {
            Id = Guid.NewGuid(),
            TenDe = request.TenDe,
            MaDe = $"DT{DateTime.UtcNow:yyyyMMddHHmmss}",
            TongCauHoi = request.TongCauHoi,
            LoaiDeThi = request.LoaiDeThi,
            ThoiLuongPhut = request.ThoiGianLamBai,
            KyThiId = request.KyThiId,
            NgayTao = DateTime.UtcNow,
            NguoiTaoId = request.NguoiTaoId ?? Guid.Empty
        };

        var availableQuestions = await _unitOfWork.CauHois.GetAllAsync(
            filter: q => q.LoaiCauHoi == request.LoaiCauHoi
                      && q.KyNang == request.KyNang
                      && q.CapDo == request.CapDo
                      && q.DoKho == request.DoKho
                      && q.CacNhom.Count == 0
        );

        var questionList = availableQuestions.ToList();

        if (questionList.Count < request.TongCauHoi)
        {
            throw new InvalidOperationException(
                $"Không đủ câu hỏi trong ngân hàng. Yêu cầu: {request.TongCauHoi}, Có sẵn: {questionList.Count}. " +
                $"Tiêu chí: LoaiCauHoi={request.LoaiCauHoi}, KyNang={request.KyNang}, CapDo={request.CapDo}, DoKho={request.DoKho}"
            );
        }

        var selectedQuestions = SelectRandomQuestions(questionList, request.TongCauHoi);
        var thuTu = 1;
        foreach (var cauHoi in selectedQuestions)
        {
            var cauHoiDeThi = new CauHoiDeThi
            {
                Id = Guid.NewGuid(),
                DeThiId = deThi.Id,
                CauHoiId = cauHoi.Id,
                ThuTuCauHoi = thuTu++
            };
            
            deThi.CacCauHoi.Add(cauHoiDeThi);
        }

        await _unitOfWork.DeThis.AddAsync(deThi);
        await _unitOfWork.SaveChangesAsync();

        return deThi;
    }

    // Các method cũ - giữ lại để tương thích ngược
    public void AddQuestionToTest(DeThi deThi, NganHangCauHoi cauHoi)
    {
        deThi.CacCauHoi.Add(new CauHoiDeThi { DeThi = deThi, CauHoi = cauHoi });
        _unitOfWork.SaveChangesAsync();
    }

    public void RemoveQuestionFromTest(DeThi deThi, NganHangCauHoi cauHoi)
    {
        var cauHoiDeThi = deThi.CacCauHoi.FirstOrDefault(chdt => chdt.CauHoiId == cauHoi.Id);
        if (cauHoiDeThi != null)
        {
            deThi.CacCauHoi.Remove(cauHoiDeThi);
            _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<DeThi?> GetTestWithQuestionsAsync(string id)
    {
        var result = await _unitOfWork.DeThis.GetAllAsync(
            filter: dt => dt.Id.ToString() == id,
            includes: dt => dt.CacCauHoi
        );
        
        var deThi = result.FirstOrDefault();
        
        if (deThi != null)
        {
            foreach (var cauHoiDeThi in deThi.CacCauHoi)
            {
                var cauHoi = await _unitOfWork.CauHois.GetByIdAsync(cauHoiDeThi.CauHoiId.ToString());
                if (cauHoi != null)
                {
                    cauHoiDeThi.CauHoi = cauHoi;
                }
            }
        }
        
        return deThi;
    }

    public IEnumerable<NganHangCauHoi> GetQuestionsInTest(DeThi deThi)
    {
        return deThi.CacCauHoi
            .Where(chdt => chdt.CauHoi != null)
            .Select(chdt => chdt.CauHoi);
    }
    public async Task<object?> GetTestWithQuestionsGroupedAsync(string id)
    {
        // Lấy đề thi với câu hỏi
        var deThi = await GetTestWithQuestionsAsync(id);
        if (deThi == null) return null;

        var allQuestions = deThi.CacCauHoi
            .Where(chdt => chdt.CauHoi != null)
            .OrderBy(chdt => chdt.ThuTuCauHoi)
            .Select(chdt => chdt.CauHoi!)
            .ToList();

        var questionIds = allQuestions.Select(q => q.Id).ToList();

        var nhomChiTiets = await _unitOfWork.NhomCauHoiChiTiets.GetAllAsync(
            filter: nct => questionIds.Contains(nct.CauHoiId),
            includes: nct => nct.Nhom
        );

        var grouped = new Dictionary<string, List<NganHangCauHoi>>();
        var nhomDict = new Dictionary<string, NhomCauHoi>();

        foreach (var question in allQuestions)
        {
            var chiTiet = nhomChiTiets.FirstOrDefault(nct => nct.CauHoiId == question.Id);
            
            if (chiTiet != null && chiTiet.Nhom != null)
            {
                var nhomKey = chiTiet.NhomId.ToString();
                if (!grouped.ContainsKey(nhomKey))
                {
                    grouped[nhomKey] = new List<NganHangCauHoi>();
                    nhomDict[nhomKey] = chiTiet.Nhom;
                }
                grouped[nhomKey].Add(question);
            }
            else
            {
                const string independentKey = "independent";
                if (!grouped.ContainsKey(independentKey))
                {
                    grouped[independentKey] = new List<NganHangCauHoi>();
                }
                grouped[independentKey].Add(question);
            }
        }

        var result = new List<object>();

        foreach (var group in grouped)
        {
            if (group.Key == "independent")
            {
                result.Add(new
                {
                    id = "independent",
                    tieuDe = "Câu hỏi độc lập",
                    noiDung = (string?)null,
                    urlAmThanh = (string?)null,
                    urlHinhAnh = (string?)null,
                    cauHois = group.Value.Select(q => new
                    {
                        id = q.Id,
                        noiDungCauHoi = q.NoiDungCauHoi,
                        loaiCauHoi = (int)q.LoaiCauHoi,
                        urlHinhAnh = q.UrlHinhAnh,
                        urlAmThanh = q.UrlAmThanh,
                        loiThoai = q.LoiThoai,
                        cacDapAn = q.CacDapAn.Select(da => new
                        {
                            nhan = da.Nhan,
                            noiDung = da.NoiDung,
                            dung = da.Dung
                        }).ToList()
                    }).ToList()
                });
            }
            else
            {
                var nhom = nhomDict[group.Key];
                result.Add(new
                {
                    id = nhom.Id,
                    tieuDe = nhom.TieuDe,
                    noiDung = nhom.NoiDung,
                    urlAmThanh = nhom.UrlAmThanh,
                    urlHinhAnh = nhom.UrlHinhAnh,
                    doKho = (int)nhom.DoKho,
                    capDo = (int)nhom.CapDo,
                    cauHois = group.Value.Select(q => new
                    {
                        id = q.Id,
                        noiDungCauHoi = q.NoiDungCauHoi,
                        loaiCauHoi = (int)q.LoaiCauHoi,
                        urlHinhAnh = q.UrlHinhAnh,
                        urlAmThanh = q.UrlAmThanh,
                        loiThoai = q.LoiThoai,
                        cacDapAn = q.CacDapAn.Select(da => new
                        {
                            nhan = da.Nhan,
                            noiDung = da.NoiDung,
                            dung = da.Dung
                        }).ToList()
                    }).ToList()
                });
            }
        }

        return new
        {
            nhomCauHois = result,
            tongSoCauHoi = allQuestions.Count,
            tongSoNhom = result.Count
        };
    }

    public int GetNumberOfQuestions(DeThi deThi)
    {
        return deThi.CacCauHoi.Count;
    }

    public void ClearAllQuestions(DeThi deThi)
    {
        deThi.CacCauHoi.Clear();
        _unitOfWork.SaveChangesAsync();
    }

    public bool ContainsQuestion(DeThi deThi, NganHangCauHoi cauHoi)
    {
        return deThi.CacCauHoi.Any(chdt => chdt.CauHoiId == cauHoi.Id);
    }

    public async Task<DeThi> CreateMixedTestAsync(CreateMixedTestRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.TenDe))
            throw new ArgumentException("Tên đề không được để trống", nameof(request.TenDe));

        if (request.ThoiGianLamBai <= 0)
            throw new ArgumentException("Thời gian làm bài phải lớn hơn 0", nameof(request.ThoiGianLamBai));

        if (request.NhomCauHoiIds.Count == 0 && request.CauHoiDocLapIds.Count == 0)
            throw new ArgumentException("Phải chọn ít nhất một nhóm câu hỏi hoặc câu hỏi độc lập");

        Guid? kyThiId = null;
        if (!string.IsNullOrEmpty(request.KyThiId) && Guid.TryParse(request.KyThiId, out var parsedKyThiId))
        {
            kyThiId = parsedKyThiId;
        }

        var allCauHois = new List<NganHangCauHoi>();

        // 1. Lấy tất cả câu hỏi từ các nhóm được chọn
        foreach (var nhomId in request.NhomCauHoiIds)
        {
            var chiTiets = await _unitOfWork.NhomCauHoiChiTiets.GetAllAsync(
                filter: ct => ct.NhomId.ToString() == nhomId
            );

            if (!chiTiets.Any())
            {
                throw new InvalidOperationException($"Nhóm câu hỏi với ID {nhomId} không tồn tại hoặc không có câu hỏi");
            }

            foreach (var chiTiet in chiTiets.OrderBy(ct => ct.ThuTu))
            {
                var cauHoi = await _unitOfWork.CauHois.GetByIdAsync(chiTiet.CauHoiId.ToString());
                if (cauHoi != null)
                {
                    allCauHois.Add(cauHoi);
                }
            }
        }

        // 2. Lấy các câu hỏi độc lập
        foreach (var cauHoiId in request.CauHoiDocLapIds)
        {
            var cauHoi = await _unitOfWork.CauHois.GetByIdAsync(cauHoiId);
            if (cauHoi == null)
            {
                throw new InvalidOperationException($"Câu hỏi với ID {cauHoiId} không tồn tại");
            }

            // Kiểm tra câu hỏi có thuộc nhóm nào không
            var thuocNhom = await _unitOfWork.NhomCauHoiChiTiets.GetAllAsync(
                filter: ct => ct.CauHoiId == cauHoi.Id
            );

            if (thuocNhom.Any())
            {
                throw new InvalidOperationException(
                    $"Câu hỏi '{cauHoi.NoiDungCauHoi}' thuộc nhóm câu hỏi. " +
                    "Vui lòng chọn cả nhóm hoặc chỉ chọn câu hỏi độc lập."
                );
            }

            allCauHois.Add(cauHoi);
        }

        if (allCauHois.Count == 0)
        {
            throw new InvalidOperationException("Không tìm thấy câu hỏi nào để thêm vào đề thi");
        }

        // 3. Tạo đề thi
        var deThi = new DeThi
        {
            Id = Guid.NewGuid(),
            TenDe = request.TenDe,
            MaDe = $"DT{DateTime.UtcNow:yyyyMMddHHmmss}",
            TongCauHoi = allCauHois.Count,
            LoaiDeThi = request.LoaiDeThi,
            ThoiLuongPhut = request.ThoiGianLamBai,
            KyThiId = kyThiId,
            NgayTao = DateTime.UtcNow,
            NguoiTaoId = Guid.Empty
        };

        await _unitOfWork.DeThis.AddAsync(deThi);

        // 4. Thêm câu hỏi vào đề thi với thứ tự
        var thuTu = 1;
        foreach (var cauHoi in allCauHois)
        {
            var cauHoiDeThi = new CauHoiDeThi
            {
                Id = Guid.NewGuid(),
                DeThiId = deThi.Id,
                CauHoiId = cauHoi.Id,
                ThuTuCauHoi = thuTu++
            };

            await _unitOfWork.CauHoiDeThis.AddAsync(cauHoiDeThi);
        }

        await _unitOfWork.SaveChangesAsync();

        return deThi;
    }

    private List<NganHangCauHoi> SelectRandomQuestions(List<NganHangCauHoi> questions, int count)
    {
        var random = new Random();
        var shuffled = questions.OrderBy(x => random.Next()).ToList();
        return shuffled.Take(count).ToList();
    }

    public async Task<DeThi> GenerateTestWithDifficultyDistributionAsync(GenerateTestWithDifficultyRequest request)
    {
        int tongSoCau = request.SoCauDe + request.SoCauTrungBinh + request.SoCauKho;
        
        if (tongSoCau <= 0)
            throw new ArgumentException("Tổng số câu hỏi phải lớn hơn 0");

        if (request.ThoiGianLamBai <= 0)
            throw new ArgumentException("Thời gian làm bài phải lớn hơn 0");

        if (string.IsNullOrWhiteSpace(request.TenDe))
            throw new ArgumentException("Tên đề không được để trống");

        var deThi = new DeThi
        {
            Id = Guid.NewGuid(),
            TenDe = request.TenDe,
            MaDe = $"DT{DateTime.UtcNow:yyyyMMddHHmmss}",
            TongCauHoi = tongSoCau,
            LoaiDeThi = request.LoaiDeThi,
            ThoiLuongPhut = request.ThoiGianLamBai,
            KyThiId = request.KyThiId,
            NgayTao = DateTime.UtcNow,
            NguoiTaoId = request.NguoiTaoId ?? Guid.Empty
        };
        
        var thuTu = 1;

        if (request.SoCauDe > 0)
        {
            var cauDe = await GetQuestionsByDifficulty(
                request.LoaiCauHoi, 
                request.KyNang, 
                request.CapDo, 
                DoKho.de, 
                request.SoCauDe
            );
            foreach (var cauHoi in cauDe)
            {
                deThi.CacCauHoi.Add(new CauHoiDeThi
                {
                    Id = Guid.NewGuid(),
                    DeThiId = deThi.Id,
                    CauHoiId = cauHoi.Id,
                    ThuTuCauHoi = thuTu++
                });
            }
        }

        if (request.SoCauTrungBinh > 0)
        {
            var cauTrungBinh = await GetQuestionsByDifficulty(
                request.LoaiCauHoi, 
                request.KyNang, 
                request.CapDo, 
                DoKho.trungbinh, 
                request.SoCauTrungBinh
            );
            foreach (var cauHoi in cauTrungBinh)
            {
                deThi.CacCauHoi.Add(new CauHoiDeThi
                {
                    Id = Guid.NewGuid(),
                    DeThiId = deThi.Id,
                    CauHoiId = cauHoi.Id,
                    ThuTuCauHoi = thuTu++
                });
            }
        }
    
        if (request.SoCauKho > 0)
        {
            var cauKho = await GetQuestionsByDifficulty(
                request.LoaiCauHoi, 
                request.KyNang, 
                request.CapDo, 
                DoKho.kho, 
                request.SoCauKho
            );
            foreach (var cauHoi in cauKho)
            {
                deThi.CacCauHoi.Add(new CauHoiDeThi
                {
                    Id = Guid.NewGuid(),
                    DeThiId = deThi.Id,
                    CauHoiId = cauHoi.Id,
                    ThuTuCauHoi = thuTu++
                });
            }
        }

        await _unitOfWork.DeThis.AddAsync(deThi);
        await _unitOfWork.SaveChangesAsync();

        return deThi;
    }

    private async Task<List<NganHangCauHoi>> GetQuestionsByDifficulty(
        LoaiCauHoi loaiCauHoi,
        KyNang kyNang,
        CapDo capDo,
        DoKho doKho,
        int soCau)
    {
        var questions = await _unitOfWork.CauHois.GetAllAsync(
            filter: q => q.LoaiCauHoi == loaiCauHoi
                      && q.KyNang == kyNang
                      && q.CapDo == capDo
                      && q.DoKho == doKho
                      && q.CacNhom.Count == 0
        );

        var questionList = questions.ToList();

        if (questionList.Count < soCau)
        {
            throw new InvalidOperationException(
                $"Không đủ câu hỏi độ khó '{doKho}'. Yêu cầu: {soCau}, Có sẵn: {questionList.Count}"
            );
        }

        return SelectRandomQuestions(questionList, soCau);
    }

    public async Task<DeThi> CreateMixedTestAsync(
        string tenDe,
        int thoiGianLamBai,
        LoaiDeThi loaiDeThi,
        Guid? kyThiId,
        List<string> nhomCauHoiIds,
        List<string> cauHoiDocLapIds,
        Guid nguoiTaoId)
    {
        if (string.IsNullOrWhiteSpace(tenDe))
            throw new ArgumentException("Tên đề không được để trống", nameof(tenDe));

        if (thoiGianLamBai <= 0)
            throw new ArgumentException("Thời gian làm bài phải lớn hơn 0", nameof(thoiGianLamBai));

        if (nhomCauHoiIds.Count == 0 && cauHoiDocLapIds.Count == 0)
            throw new ArgumentException("Phải chọn ít nhất một nhóm câu hỏi hoặc câu hỏi độc lập");

        var allCauHois = new List<NganHangCauHoi>();

        // 1. Lấy tất cả câu hỏi từ các nhóm được chọn
        foreach (var nhomId in nhomCauHoiIds)
        {
            var chiTiets = await _unitOfWork.NhomCauHoiChiTiets.GetAllAsync(
                filter: ct => ct.NhomId.ToString() == nhomId,
                includes: ct => ct.CauHoi
            );

            if (!chiTiets.Any())
            {
                throw new InvalidOperationException($"Nhóm câu hỏi với ID {nhomId} không tồn tại hoặc không có câu hỏi");
            }

            var cauHoisTrongNhom = chiTiets
                .OrderBy(ct => ct.ThuTu)
                .Select(ct => ct.CauHoi)
                .Where(ch => ch != null)
                .ToList();

            allCauHois.AddRange(cauHoisTrongNhom!);
        }

        // 2. Lấy các câu hỏi độc lập
        foreach (var cauHoiId in cauHoiDocLapIds)
        {
            var cauHoi = await _unitOfWork.CauHois.GetByIdAsync(cauHoiId);
            if (cauHoi == null)
            {
                throw new InvalidOperationException($"Câu hỏi với ID {cauHoiId} không tồn tại");
            }

            // Kiểm tra câu hỏi có thuộc nhóm nào không
            var thuocNhom = await _unitOfWork.NhomCauHoiChiTiets.GetAllAsync(
                filter: ct => ct.CauHoiId == cauHoi.Id
            );

            if (thuocNhom.Any())
            {
                throw new InvalidOperationException(
                    $"Câu hỏi '{cauHoi.NoiDungCauHoi}' thuộc nhóm câu hỏi. " +
                    "Vui lòng chọn cả nhóm hoặc chỉ chọn câu hỏi độc lập."
                );
            }

            allCauHois.Add(cauHoi);
        }

        if (allCauHois.Count == 0)
        {
            throw new InvalidOperationException("Không tìm thấy câu hỏi nào để thêm vào đề thi");
        }

        // 3. Tạo đề thi
        var deThi = new DeThi
        {
            Id = Guid.NewGuid(),
            TenDe = tenDe,
            TongCauHoi = allCauHois.Count,
            LoaiDeThi = loaiDeThi,
            ThoiLuongPhut = thoiGianLamBai,
            KyThiId = kyThiId,
            NguoiTaoId = nguoiTaoId
        };

        await _unitOfWork.DeThis.AddAsync(deThi);

        // 4. Thêm câu hỏi vào đề thi với thứ tự
        var thuTu = 1;
        foreach (var cauHoi in allCauHois)
        {
            var cauHoiDeThi = new CauHoiDeThi
            {
                Id = Guid.NewGuid(),
                DeThiId = deThi.Id,
                CauHoiId = cauHoi.Id,
                ThuTuCauHoi = thuTu++
            };

            deThi.CacCauHoi.Add(cauHoiDeThi);
        }

        await _unitOfWork.SaveChangesAsync();

        return deThi;
    }
}

