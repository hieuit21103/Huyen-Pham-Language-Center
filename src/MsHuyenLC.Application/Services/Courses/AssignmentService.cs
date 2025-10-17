using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Domain.Entities.Courses;
using MsHuyenLC.Domain.Entities.Users;

namespace MsHuyenLC.Application.Services.Courses;

public class AssignmentService : GenericService<PhanCong>
{
    public AssignmentService(IGenericRepository<PhanCong> repository)
        : base(repository)
    {
    }

    public async Task<PhanCong?> GetByTeacherId(string teacherId)
    {
        var results = await _repository.GetAllAsync(
            1,
            int.MaxValue,
            Filter: p => p.GiaoVien.Id.ToString() == teacherId
        );
        return results.FirstOrDefault() ?? null;
    }

    public async Task<PhanCong?> GetByClassId(string classId)
    {
        var results = await _repository.GetAllAsync(
            1,
            int.MaxValue,
            Filter: p => p.LopHoc.Id.ToString() == classId
        );
        return results.FirstOrDefault() ?? null;
    }
}