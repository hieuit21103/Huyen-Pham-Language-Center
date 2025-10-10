using FluentValidation;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Validators;
using MsHuyenLC.Domain.Entities.Courses;

namespace MsHuyenLC.Application.Services;

public class KhoaHocService : GenericService<KhoaHoc>
{
    public KhoaHocService(
        IGenericRepository<KhoaHoc> repository, 
        KhoaHocValidator validator) 
        : base(repository, validator)
    {
    }

    // Có thể thêm các business logic đặc thù cho KhoaHoc ở đây
}
