using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using MsHuyenLC.Application.Interfaces.Auth;

namespace MsHuyenLC.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IDistributedCache _cache;
    public TokenService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public string GeneratePasswordResetToken(string userId)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        
        var cacheKey = $"reset_token:{userId}";
        var expirationMinutes = int.Parse("15");
        
        _cache.SetString(cacheKey, token, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expirationMinutes)
        });

        return token;
    }

    public bool ValidatePasswordResetToken(string userId, string token)
    {
        var cacheKey = $"reset_token:{userId}";
        var storedToken = _cache.GetString(cacheKey);

        if (storedToken == token)
        {
            _cache.Remove(cacheKey);
            return true;
        }

        return false;
    }

    public string GenerateEmailConfirmationToken(string userId)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        
        var cacheKey = $"email_token:{userId}";
        var expirationHours = 24;
        
        _cache.SetString(cacheKey, token, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(expirationHours)
        });

        return token;
    }

    public bool ValidateEmailConfirmationToken(string userId, string token)
    {
        var cacheKey = $"email_token:{userId}";
        var storedToken = _cache.GetString(cacheKey);

        if (storedToken == token)
        {
            _cache.Remove(cacheKey);
            return true;
        }

        return false;
    }
}