using MsHuyenLC.Domain.Entities.Learning;
using MsHuyenLC.Application.Interfaces; 
using MsHuyenLC.Domain.Enums;

namespace MsHuyenLC.Application.Services.Learnings;

public class TestService : GenericService<DeThi>
{
    private readonly IGenericRepository<CauHoi> _cauHoiRepository;
    private readonly IGenericRepository<CauHoiDeThi> _cauHoiDeThiRepository;
    
    public TestService(
        IGenericRepository<DeThi> repository,
        IGenericRepository<CauHoi> cauHoiRepository,
        IGenericRepository<CauHoiDeThi> cauHoiDeThiRepository) : base(repository)
    {
        _cauHoiRepository = cauHoiRepository;
        _cauHoiDeThiRepository = cauHoiDeThiRepository;
    }

    public void AddQuestionToTest(DeThi deThi, CauHoi cauHoi)
    {
        deThi.CauHoiDeThis.Add(new CauHoiDeThi { DeThi = deThi, CauHoi = cauHoi });
        _repository.SaveChangesAsync();
    }

    public void RemoveQuestionFromTest(DeThi deThi, CauHoi cauHoi)
    {
        var cauHoiDeThi = deThi.CauHoiDeThis.FirstOrDefault(chdt => chdt.CauHoiId == cauHoi.Id);
        if (cauHoiDeThi != null)
        {
            deThi.CauHoiDeThis.Remove(cauHoiDeThi);
            _repository.SaveChangesAsync();
        }
    }

    public IEnumerable<CauHoi> GetQuestionsInTest(DeThi deThi)
    {
        return deThi.CauHoiDeThis.Select(chdt => chdt.CauHoi);
    }

    public int GetNumberOfQuestions(DeThi deThi)
    {
        return deThi.CauHoiDeThis.Count;
    }

    public void ClearAllQuestions(DeThi deThi)
    {
        deThi.CauHoiDeThis.Clear();
        _repository.SaveChangesAsync();
    }

    public bool ContainsQuestion(DeThi deThi, CauHoi cauHoi)
    {
        return deThi.CauHoiDeThis.Any(chdt => chdt.CauHoiId == cauHoi.Id);
    }

    public async Task<DeThi?> GenerateTestAsync(
        string tenDe, 
        int soCauHoi, 
        int thoiGianLamBai, 
        LoaiDeThi loaiDeThi, 
        LoaiCauHoi loaiCauHoi, 
        KyNang kyNang, 
        CapDo capDo, 
        DoKho doKho)
    {
        if (soCauHoi <= 0)
            throw new ArgumentException("Số câu hỏi phải lớn hơn 0", nameof(soCauHoi));
        
        if (thoiGianLamBai <= 0)
            throw new ArgumentException("Thời gian làm bài phải lớn hơn 0", nameof(thoiGianLamBai));
        
        if (string.IsNullOrWhiteSpace(tenDe))
            throw new ArgumentException("Tên đề không được để trống", nameof(tenDe));

        // Bước 2: Tạo đề thi mới
        var deThi = new DeThi
        {
            Id = Guid.NewGuid(),
            TenDe = tenDe,
            SoCauHoi = soCauHoi,
            LoaiDeThi = loaiDeThi,
            ThoiGianLamBai = thoiGianLamBai
        };

        // Bước 3: Lấy câu hỏi từ ngân hàng dựa theo tiêu chí
        var availableQuestions = await _cauHoiRepository.GetAllAsync(
            PageNumber: 1,
            PageSize: soCauHoi * 3, // Lấy nhiều hơn để có đủ dự phòng
            Filter: q => q.LoaiCauHoi == loaiCauHoi 
                      && q.KyNang == kyNang 
                      && q.CapDo == capDo 
                      && q.DoKho == doKho
        );

        var questionList = availableQuestions.ToList();

        // Bước 4: Kiểm tra xem có đủ câu hỏi không
        if (questionList.Count < soCauHoi)
        {
            throw new InvalidOperationException(
                $"Không đủ câu hỏi trong ngân hàng. Yêu cầu: {soCauHoi}, Có sẵn: {questionList.Count}. " +
                $"Tiêu chí: LoaiCauHoi={loaiCauHoi}, KyNang={kyNang}, CapDo={capDo}, DoKho={doKho}"
            );
        }

        // Bước 5: Chọn ngẫu nhiên câu hỏi
        var selectedQuestions = SelectRandomQuestions(questionList, soCauHoi);

        // Bước 6: Thêm câu hỏi vào đề thi
        foreach (var cauHoi in selectedQuestions)
        {
            var cauHoiDeThi = new CauHoiDeThi
            {
                Id = Guid.NewGuid(),
                DeThiId = deThi.Id,
                CauHoiId = cauHoi.Id
            };
            
            deThi.CauHoiDeThis.Add(cauHoiDeThi);
        }

        // Bước 7: Lưu đề thi vào database
        await _repository.AddAsync(deThi);
        await _repository.SaveChangesAsync();

        return deThi;
    }

    private List<CauHoi> SelectRandomQuestions(List<CauHoi> questions, int count)
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
        int thoiGianLamBai,
        LoaiDeThi loaiDeThi,
        LoaiCauHoi loaiCauHoi,
        KyNang kyNang,
        CapDo capDo)
    {
        int tongSoCau = soCauDe + soCauTrungBinh + soCauKho;
        
        if (tongSoCau <= 0)
            throw new ArgumentException("Tổng số câu hỏi phải lớn hơn 0");

        // Tạo đề thi
        var deThi = new DeThi
        {
            Id = Guid.NewGuid(),
            TenDe = tenDe,
            SoCauHoi = tongSoCau,
            LoaiDeThi = loaiDeThi,
            ThoiGianLamBai = thoiGianLamBai
        };
        
        // Lấy câu dễ
        if (soCauDe > 0)
        {
            var cauDe = await GetQuestionsByDifficulty(loaiCauHoi, kyNang, capDo, DoKho.de, soCauDe);
            foreach (var cauHoi in cauDe)
            {
                deThi.CauHoiDeThis.Add(new CauHoiDeThi
                {
                    Id = Guid.NewGuid(),
                    DeThiId = deThi.Id,
                    CauHoiId = cauHoi.Id
                });
            }
        }

        // Lấy câu trung bình
        if (soCauTrungBinh > 0)
        {
            var cauTrungBinh = await GetQuestionsByDifficulty(loaiCauHoi, kyNang, capDo, DoKho.trungbinh, soCauTrungBinh);
            foreach (var cauHoi in cauTrungBinh)
            {
                deThi.CauHoiDeThis.Add(new CauHoiDeThi
                {
                    Id = Guid.NewGuid(),
                    DeThiId = deThi.Id,
                    CauHoiId = cauHoi.Id
                });
            }
        }

        // Lấy câu khó
        if (soCauKho > 0)
        {
            var cauKho = await GetQuestionsByDifficulty(loaiCauHoi, kyNang, capDo, DoKho.kho, soCauKho);
            foreach (var cauHoi in cauKho)
            {
                deThi.CauHoiDeThis.Add(new CauHoiDeThi
                {
                    Id = Guid.NewGuid(),
                    DeThiId = deThi.Id,
                    CauHoiId = cauHoi.Id
                });
            }
        }

        // Lưu đề thi
        await _repository.AddAsync(deThi);
        await _repository.SaveChangesAsync();

        return deThi;
    }

    private async Task<List<CauHoi>> GetQuestionsByDifficulty(
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
}