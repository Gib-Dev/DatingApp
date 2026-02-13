using System.Text.Json;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{
    public static async Task SeedUsers(AppDbContext context, IPasswordService passwordService)
    {
        if (await context.Users.AnyAsync()) return;

        var memberData = await File.ReadAllTextAsync("Data/UserSeedData.json");
        var members = JsonSerializer.Deserialize<List<SeedUserDto>>(memberData);

        if (members == null)
        {
            Console.WriteLine("No member data found to seed.");
            return;
        }

        // Use a consistent password for all seeded users (for development/testing only)
        const string seedPassword = "Pa$$w0rd";

        foreach (var member in members)
        {
            passwordService.CreatePasswordHash(
                seedPassword,
                out byte[] passwordHash,
                out byte[] passwordSalt
            );

            var user = new AppUser
            {
                Id = member.Id,
                Email = member.Email,
                DisplayName = member.DisplayName,
                ImageUrl = member.ImageUrl,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Member = new Member
                {
                    Id = member.Id,
                    DisplayName = member.DisplayName,
                    Description = member.Description,
                    DateOfBirth = member.DateOfBirth,
                    ImageUrl = member.ImageUrl,
                    Gender = member.Gender,
                    City = member.City,
                    Country = member.Country,
                    LastActive = member.LastActive,
                    Created = member.Created,
                }
            };

            context.Users.Add(user);
        }

        await context.SaveChangesAsync();
    }
}
