









# Validation trong MsHuyenLC

## Công nghệ sử dụng
- **FluentValidation 12.0** - Library mạnh mẽ cho validation trong .NET

## Cấu trúc Validation

### 1. **Validators** (Application Layer)
- Đặt tại: `src/MsHuyenLC.Application/Validators/`
- Chứa các business rules và validation logic
- Kế thừa `AbstractValidator<T>` từ FluentValidation

### 2. **Exceptions** (Application Layer)
- Đặt tại: `src/MsHuyenLC.Application/Exceptions/`
- `ValidationException` - throw khi validation fail

### 3. **Services** (Application Layer)
- Gọi validators trước khi thực hiện Add/Update
- Tự động throw `ValidationException` nếu validation fail

## Cách sử dụng

### Tạo Validator mới với FluentValidation

```csharp
using FluentValidation;
using MsHuyenLC.Domain.Entities.Courses;

namespace MsHuyenLC.Application.Validators;

public class KhoaHocValidator : AbstractValidator<KhoaHoc>
{
    public KhoaHocValidator()
    {
        // Kiểm tra tên khóa học
        RuleFor(x => x.TenKhoaHoc)
            .NotEmpty().WithMessage("Tên khóa học không được để trống")
            .MinimumLength(3).WithMessage("Tên khóa học phải có ít nhất 3 ký tự")
            .MaximumLength(200).WithMessage("Tên khóa học không được vượt quá 200 ký tự");

        // Kiểm tra học phí
        RuleFor(x => x.HocPhi)
            .GreaterThanOrEqualTo(0).WithMessage("Học phí không được âm");

        // Kiểm tra thời lượng
        RuleFor(x => x.ThoiLuong)
            .GreaterThan(0).WithMessage("Thời lượng khóa học phải lớn hơn 0");

        // Kiểm tra ngày khai giảng
        RuleFor(x => x.NgayKhaiGiang)
            .GreaterThanOrEqualTo(DateTime.Now.Date)
            .WithMessage("Ngày khai giảng không được trong quá khứ");

        // Kiểm tra có điều kiện
        RuleFor(x => x.MoTa)
            .MaximumLength(1000).WithMessage("Mô tả không được vượt quá 1000 ký tự")
            .When(x => !string.IsNullOrWhiteSpace(x.MoTa));
    }
}
```

### Đăng ký Service với Validator (trong Program.cs)

```csharp
using FluentValidation;

// Generic Repository
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Validators - Đăng ký tất cả validators từ Assembly
builder.Services.AddValidatorsFromAssemblyContaining<KhoaHocValidator>();

// Hoặc đăng ký từng validator
builder.Services.AddScoped<IValidator<KhoaHoc>, KhoaHocValidator>();

// Services với validator
builder.Services.AddScoped<IGenericService<KhoaHoc>, KhoaHocService>();
```

### Sử dụng trong Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class KhoaHocController : ControllerBase
{
    private readonly IGenericService<KhoaHoc> _khoaHocService;

    public KhoaHocController(IGenericService<KhoaHoc> khoaHocService)
    {
        _khoaHocService = khoaHocService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateKhoaHoc([FromBody] KhoaHoc khoaHoc)
    {
        try
        {
            var result = await _khoaHocService.AddAsync(khoaHoc);
            return CreatedAtAction(nameof(GetKhoaHoc), new { id = result.Id }, result);
        }
        catch (MsHuyenLC.Application.Exceptions.ValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateKhoaHoc(Guid id, [FromBody] KhoaHoc khoaHoc)
    {
        try
        {
            khoaHoc.Id = id;
            await _khoaHocService.UpdateAsync(khoaHoc);
            return NoContent();
        }
        catch (MsHuyenLC.Application.Exceptions.ValidationException ex)
        {
            return BadRequest(new { errors = ex.Errors });
        }
    }
}
```

## FluentValidation Rules phổ biến

```csharp
// Not Empty/Null
RuleFor(x => x.Name).NotEmpty();
RuleFor(x => x.Name).NotNull();

// Length
RuleFor(x => x.Name).Length(5, 50);
RuleFor(x => x.Name).MinimumLength(3);
RuleFor(x => x.Name).MaximumLength(100);

// Comparison
RuleFor(x => x.Age).GreaterThan(18);
RuleFor(x => x.Age).GreaterThanOrEqualTo(18);
RuleFor(x => x.Age).LessThan(100);
RuleFor(x => x.Age).LessThanOrEqualTo(100);

// Range
RuleFor(x => x.Price).InclusiveBetween(0, 1000);
RuleFor(x => x.Price).ExclusiveBetween(0, 1000);

// Email & Format
RuleFor(x => x.Email).EmailAddress();
RuleFor(x => x.Phone).Matches(@"^\d{10}$");

// Custom validation
RuleFor(x => x.Password)
    .Must(BeValidPassword)
    .WithMessage("Password must contain at least one uppercase, lowercase, and digit");

// Conditional validation
RuleFor(x => x.MoTa)
    .MaximumLength(500)
    .When(x => !string.IsNullOrEmpty(x.MoTa));

// Async validation
RuleFor(x => x.Email)
    .MustAsync(async (email, cancellation) => 
    {
        return await IsEmailUnique(email);
    })
    .WithMessage("Email already exists");
```

## Validation Flow

```
Controller → Service → FluentValidator → Repository → Database
                ↓
         ValidationException
                ↓
         Catch in Controller
                ↓
         Return BadRequest
```

## Lợi ích của FluentValidation

1. ✅ **Declarative** - Validation rules rõ ràng, dễ đọc
2. ✅ **Reusable** - Validators có thể tái sử dụng
3. ✅ **Testable** - Dễ dàng unit test
4. ✅ **Clean Architecture** - Tách biệt validation logic
5. ✅ **Powerful** - Hỗ trợ async validation, custom rules, conditional validation
6. ✅ **Localization** - Hỗ trợ đa ngôn ngữ cho error messages
7. ✅ **Community** - Library phổ biến với cộng đồng lớn

