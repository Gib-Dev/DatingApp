using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(
    AppDbContext context,
    ITokenService tokenService,
    IPasswordService passwordService) : BaseApiController
{
    [HttpPost("register")] // POST: api/account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registrerDto)
    {
        if (await EmailExists(registrerDto.Email))
        {
            return BadRequest("Email is already in use");
        }

        passwordService.CreatePasswordHash(
            registrerDto.Password,
            out byte[] passwordHash,
            out byte[] passwordSalt
        );

        var user = new AppUser
        {
            Id = Guid.NewGuid().ToString(),
            DisplayName = registrerDto.DisplayName,
            Email = registrerDto.Email.ToLower(),
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user.ToDto(tokenService);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await context.Users
            .SingleOrDefaultAsync(x => x.Email.ToLower() == loginDto.Email.ToLower());

        if (user == null)
            return Unauthorized("Invalid email address");

        if (!passwordService.VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt))
            return Unauthorized("Invalid password");

        return user.ToDto(tokenService);
    }

    private async Task<bool> EmailExists(string email)
    {
        return await context.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());
    }
}
