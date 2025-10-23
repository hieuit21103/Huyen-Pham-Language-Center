using MsHuyenLC.Domain.Entities.Learning;
using MsHuyenLC.Application.Interfaces; 
using MsHuyenLC.Domain.Enums;
using MsHuyenLC.Domain.Entities.Learning.OnlineExam;

namespace MsHuyenLC.Application.Services.Learnings;

public class TestService : GenericService<DeThi>
{
    private readonly IGenericRepository<NganHangCauHoi> _cauHoiRepository;
    private readonly IGenericRepository<CauHoiDeThi> _cauHoiDeThiRepository;
    
    public TestService(
        IGenericRepository<DeThi> repository,
        IGenericRepository<NganHangCauHoi> cauHoiRepository,
        IGenericRepository<CauHoiDeThi> cauHoiDeThiRepository) : base(repository)
    {
        _cauHoiRepository = cauHoiRepository;
        _cauHoiDeThiRepository = cauHoiDeThiRepository;
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