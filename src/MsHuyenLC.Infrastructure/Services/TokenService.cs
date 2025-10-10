using System.Security.Cryptography;
using MsHuyenLC.Application.Interfaces.Auth;
using StackExchange.Redis;

namespace MsHuyenLC.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IConnectionMultiplexer _cache;
    public TokenService(IConnectionMultiplexer cache)
    {
        _cache = cache;
    }

    public string GeneratePasswordResetToken(string userId)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        
        var cacheKey = $"reset_token:{userId}";
        var expirationMinutes = int.Parse("15");

        _cache.GetDatabase().StringSet(cacheKey, token, TimeSpan.FromMinutes(expirationMinutes));

        return token;
    }

    public bool ValidatePasswordResetToken(string userId, string token)
    {
        var cacheKey = $"reset_token:{userId}";
        var storedToken = _cache.GetDatabase().StringGet(cacheKey);

        if (storedToken == token)
        {
            _cache.GetDatabase().KeyDelete(cacheKey);
            return true;
        }

        return false;
    }

    public string GenerateEmailConfirmationToken(string userId)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        
        var cacheKey = $"email_token:{userId}";
        var expirationHours = 24;

        _cache.GetDatabase().StringSet(cacheKey, token, TimeSpan.FromHours(expirationHours));

        return token;
    }

    public bool ValidateEmailConfirmationToken(string userId, string token)
    {
        var cacheKey = $"email_token:{userId}";
        var storedToken = _cache.GetDatabase().StringGet(cacheKey);

        if (storedToken == token)
        {
            _cache.GetDatabase().KeyDelete(cacheKey);
            return true;
        }

        return false;
    }
}