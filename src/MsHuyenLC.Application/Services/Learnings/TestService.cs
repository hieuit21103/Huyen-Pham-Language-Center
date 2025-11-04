using MsHuyenLC.Domain.Entities.Learning;
using MsHuyenLC.Application.Interfaces; 
using MsHuyenLC.Domain.Enums;
using MsHuyenLC.Domain.Entities.Learning.OnlineExam;

namespace MsHuyenLC.Application.Services.Learnings;

public class TestService : GenericService<DeThi>
{
    private readonly IGenericRepository<NganHangCauHoi> _cauHoiRepository;
    private readonly IGenericRepository<CauHoiDeThi> _cauHoiDeThiRepository;
    private readonly IGenericRepository<NhomCauHoi> _nhomCauHoiRepository;
    private readonly IGenericRepository<NhomCauHoiChiTiet> _nhomCauHoiChiTietRepository;
    
    public TestService(
        IGenericRepository<DeThi> repository,
        IGenericRepository<NganHangCauHoi> cauHoiRepository,
        IGenericRepository<CauHoiDeThi> cauHoiDeThiRepository,
        IGenericRepository<NhomCauHoi> nhomCauHoiRepository,
        IGenericRepository<NhomCauHoiChiTiet> nhomCauHoiChiTietRepository) : base(repository)
    {
        _cauHoiRepository = cauHoiRepository;
        _cauHoiDeThiRepository = cauHoiDeThiRepository;
        _nhomCauHoiRepository = nhomCauHoiRepository;
        _nhomCauHoiChiTietRepository = nhomCauHoiChiTietRepository;
    }

    public void AddQuestionToTest(DeThi deThi, NganHangCauHoi cauHoi)
    {
        deThi.CacCauHoi.Add(new CauHoiDeThi { DeThi = deThi, CauHoi = cauHoi });
        _repository.SaveChangesAsync();
    }

    public void RemoveQuestionFromTest(DeThi deThi, NganHangCauHoi cauHoi)
    {
        var cauHoiDeThi = deThi.CacCauHoi.FirstOrDefault(chdt => chdt.CauHoiId == cauHoi.Id);
        if (cauHoiDeThi != null)
        {
            deThi.CacCauHoi.Remove(cauHoiDeThi);
            _repository.SaveChangesAsync();
        }
    }

    public async Task<DeThi?> GetTestWithQuestionsAsync(string id)
    {
        var result = await _repository.GetAllAsync(
            PageNumber: 1,
            PageSize: 1,
            Filter: dt => dt.Id.ToString() == id,
            Includes: dt => dt.CacCauHoi
        );
        
        var deThi = result.FirstOrDefault();
        
        if (deThi != null)
        {
            foreach (var cauHoiDeThi in deThi.CacCauHoi)
            {
                var cauHoi = await _cauHoiRepository.GetByIdAsync(cauHoiDeThi.CauHoiId.ToString());
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
        
        var nhomChiTiets = await _nhomCauHoiChiTietRepository.GetAllAsync(
            PageNumber: 1,
            PageSize: 10000,
            Filter: nct => questionIds.Contains(nct.CauHoiId),
            Includes: nct => nct.Nhom
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
        _repository.SaveChangesAsync();
    }

    public bool ContainsQuestion(DeThi deThi, NganHangCauHoi cauHoi)
    {
        return deThi.CacCauHoi.Any(chdt => chdt.CauHoiId == cauHoi.Id);
    }

    public async Task<DeThi?> GenerateTestAsync(
        string tenDe, 
        int tongCauHoi, 
        int thoiLuongPhut, 
        LoaiDeThi loaiDeThi, 
        LoaiCauHoi loaiCauHoi, 
        KyNang kyNang, 
        CapDo capDo, 
        DoKho doKho,
        Guid nguoiTaoId)
    {
        if (tongCauHoi <= 0)
            throw new ArgumentException("Số câu hỏi phải lớn hơn 0", nameof(tongCauHoi));

        if (thoiLuongPhut <= 0)
            throw new ArgumentException("Thời gian làm bài phải lớn hơn 0", nameof(thoiLuongPhut));

        if (string.IsNullOrWhiteSpace(tenDe))
            throw new ArgumentException("Tên đề không được để trống", nameof(tenDe));

        var deThi = new DeThi
        {
            Id = Guid.NewGuid(),
            TenDe = tenDe,
            TongCauHoi = tongCauHoi,
            LoaiDeThi = loaiDeThi,
            ThoiLuongPhut = thoiLuongPhut,
            NguoiTaoId = nguoiTaoId
        };

        var availableQuestions = await _cauHoiRepository.GetAllAsync(
            PageNumber: 1,
            PageSize: tongCauHoi * 3,
            Filter: q => q.LoaiCauHoi == loaiCauHoi
                      && q.KyNang == kyNang
                      && q.CapDo == capDo
                      && q.DoKho == doKho
                      && q.CacNhom.Count == 0
        );

        var questionList = availableQuestions.ToList();

        if (questionList.Count < tongCauHoi)
        {
            throw new InvalidOperationException(
                $"Không đủ câu hỏi trong ngân hàng. Yêu cầu: {tongCauHoi}, Có sẵn: {questionList.Count}. " +
                $"Tiêu chí: LoaiCauHoi={loaiCauHoi}, KyNang={kyNang}, CapDo={capDo}, DoKho={doKho}"
            );
        }

        var selectedQuestions = SelectRandomQuestions(questionList, tongCauHoi);
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

        await _repository.AddAsync(deThi);
        await _repository.SaveChangesAsync();

        return deThi;
    }

    private List<NganHangCauHoi> SelectRandomQuestions(List<NganHangCauHoi> questions, int count)
    {
        var random = new Random();
        var shuffled = questions.OrderBy(x => random.Next()).ToList();
        return shuffled.Take(count).ToList();
    }

    public async Task<DeThi> GenerateTestWithDifficultyDistributionAsync(
        string tenDe,
        int soCauDe,
        int soCauTrungBinh,
        int soCauKho,
        int thoiLuongPhut,
        LoaiDeThi loaiDeThi,
        LoaiCauHoi loaiCauHoi,
        KyNang kyNang,
        CapDo capDo,
        Guid nguoiTaoId)
    {
        int tongSoCau = soCauDe + soCauTrungBinh + soCauKho;
        
        if (tongSoCau <= 0)
            throw new ArgumentException("Tổng số câu hỏi phải lớn hơn 0");

        var deThi = new DeThi
        {
            Id = Guid.NewGuid(),
            TenDe = tenDe,
            TongCauHoi = tongSoCau,
            LoaiDeThi = loaiDeThi,
            ThoiLuongPhut = thoiLuongPhut,
            NguoiTaoId = nguoiTaoId
        };
        
        if (soCauDe > 0)
        {
            var cauDe = await GetQuestionsByDifficulty(loaiCauHoi, kyNang, capDo, DoKho.de, soCauDe);
            foreach (var cauHoi in cauDe)
            {
                deThi.CacCauHoi.Add(new CauHoiDeThi
                {
                    Id = Guid.NewGuid(),
                    DeThiId = deThi.Id,
                    CauHoiId = cauHoi.Id
                });
            }
        }

        if (soCauTrungBinh > 0)
        {
            var cauTrungBinh = await GetQuestionsByDifficulty(loaiCauHoi, kyNang, capDo, DoKho.trungbinh, soCauTrungBinh);
            foreach (var cauHoi in cauTrungBinh)
            {
                deThi.CacCauHoi.Add(new CauHoiDeThi
                {
                    Id = Guid.NewGuid(),
                    DeThiId = deThi.Id,
                    CauHoiId = cauHoi.Id
                });
            }
        }
    
        if (soCauKho > 0)
        {
            var cauKho = await GetQuestionsByDifficulty(loaiCauHoi, kyNang, capDo, DoKho.kho, soCauKho);
            foreach (var cauHoi in cauKho)
            {
                deThi.CacCauHoi.Add(new CauHoiDeThi
                {
                    Id = Guid.NewGuid(),
                    DeThiId = deThi.Id,
                    CauHoiId = cauHoi.Id
                });
            }
        }

        await _repository.AddAsync(deThi);
        await _repository.SaveChangesAsync();

        return deThi;
    }

    private async Task<List<NganHangCauHoi>> GetQuestionsByDifficulty(
        LoaiCauHoi loaiCauHoi,
        KyNang kyNang,
        CapDo capDo,
        DoKho doKho,
        int soCau)
    {
        var questions = await _cauHoiRepository.GetAllAsync(
            PageNumber: 1,
            PageSize: soCau * 2,
            Filter: q => q.LoaiCauHoi == loaiCauHoi
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
            var chiTiets = await _nhomCauHoiChiTietRepository.GetAllAsync(
                PageNumber: 1,
                PageSize: 1000,
                Filter: ct => ct.NhomId.ToString() == nhomId,
                Includes: ct => ct.CauHoi
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
            var cauHoi = await _cauHoiRepository.GetByIdAsync(cauHoiId);
            if (cauHoi == null)
            {
                throw new InvalidOperationException($"Câu hỏi với ID {cauHoiId} không tồn tại");
            }

            // Kiểm tra câu hỏi có thuộc nhóm nào không
            var thuocNhom = await _nhomCauHoiChiTietRepository.GetAllAsync(
                PageNumber: 1,
                PageSize: 1,
                Filter: ct => ct.CauHoiId == cauHoi.Id
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

        await _repository.AddAsync(deThi);

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

        await _repository.SaveChangesAsync();

        return deThi;
    }
}