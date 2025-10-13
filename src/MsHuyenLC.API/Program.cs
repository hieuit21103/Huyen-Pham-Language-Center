using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using MsHuyenLC.Application.Interfaces;
using MsHuyenLC.Application.Interfaces.Auth;
using MsHuyenLC.Application.Services;
using MsHuyenLC.Infrastructure.Repositories;
using MsHuyenLC.Infrastructure.Services;
using MsHuyenLC.Infrastructure.Services.Email;
using MsHuyenLC.Infrastructure.Persistence.Seed;
using Microsoft.OpenApi.Models;
using MsHuyenLC.Application.Interfaces.Email;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "MsHuyenLC API", 
        Version = "v1",
        Description = "API for MsHuyenLC Course Management System",
        Contact = new OpenApiContact
        {
            Name = "MsHuyenLC Team",
            Email = "support@mshuyenlc.com"
        }
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token\n\nExample: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    var postgreSqlConnection = builder.Environment.IsDevelopment()
        ? builder.Configuration.GetConnectionString("DefaultConnection")
        : Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
    option.UseNpgsql(postgreSqlConnection);
});


builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisConnection = builder.Environment.IsDevelopment()
        ? builder.Configuration["Redis:ConnectionString"]
        : Environment.GetEnvironmentVariable("Redis__ConnectionString");

    var configuration = ConfigurationOptions.Parse(redisConnection ?? throw new InvalidOperationException("Redis ConnectionString is not set"), true);
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "MsHuyenLC",
        ValidAudience = "MsHuyenLC",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSuperSecretKeyMinimum32CharactersLong!@#")),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));

builder.Services.AddControllers();

var app = builder.Build();

// Enable Swagger in all environments
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MsHuyenLC API V1");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "MsHuyenLC API Documentation";
    c.DefaultModelsExpandDepth(-1);
    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
});

// Only use HTTPS redirection in production with HTTPS configured
// app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed default users
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        await new DefaultUserSeeder(scope.ServiceProvider.GetRequiredService<IPasswordHasher>()).SeedAsync(services);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while seeding default users: {ex.Message}");
    }
}

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    environment = builder.Environment.EnvironmentName
}))
.WithTags("Health")
.WithOpenApi();

// Root redirect to Swagger
app.MapGet("/", () => Results.Redirect("/swagger"))
    .ExcludeFromDescription();

app.Run();

