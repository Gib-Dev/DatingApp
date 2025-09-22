using System;

using API.Entities;
using API.Interfaces;
namespace API.Services;

public class TokenService : ITokenService
{
    public string CreateToken(AppUser user)
    {
        var tokenKey = config["TokenKey"] ?? throw Exception("Cannot find Token Key");
        if (tokenKey.Length < 64)
            throw Exception("Invalid token key - need at least 64 characters");
        var key = new SymmetricSecurityKey();
    }
}
