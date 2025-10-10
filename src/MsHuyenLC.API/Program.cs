using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseNpgsql(Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection"));
});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse(Environment.GetEnvironmentVariable("Redis__ConnectionString") ?? throw new InvalidOperationException("Redis__ConnectionString is not set"), true);
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

var app = builder.Build();

// Test DB connection and Redis connection
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.CanConnectAsync();
        Console.WriteLine("✓ Database connection successful");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"✗ Database connection failed: {ex.Message}");
    }
    
    try
    {
        var redis = services.GetRequiredService<IConnectionMultiplexer>();
        var db = redis.GetDatabase();
        await db.PingAsync();
        Console.WriteLine("✓ Redis connection successful");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"✗ Redis connection failed: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.Run();

