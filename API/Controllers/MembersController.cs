using API.Data;
using API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    public class MembersController(AppDbContext context) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<MemberDto>>> GetMembers()
        {
            var members = await context.Members
                .Include(m => m.Photos)
                .Select(m => new MemberDto
                {
                    Id = m.Id,
                    DisplayName = m.DisplayName,
                    ImageUrl = m.ImageUrl,
                    DateOfBirth = m.DateOfBirth,
                    Created = m.Created,
                    LastActive = m.LastActive,
                    Gender = m.Gender,
                    Description = m.Description,
                    City = m.City,
                    Country = m.Country,
                    Photos = m.Photos.Select(p => new PhotoDto
                    {
                        Id = p.Id,
                        Url = p.Url,
                        PublicId = p.PublicId
                    }).ToList()
                })
                .ToListAsync();

            return members;
        }

        [Authorize]
        [HttpGet("{id}")] // localhost:5001/api/members/bob-id
        public async Task<ActionResult<MemberDto>> GetMember(string id)
        {
            var member = await context.Members
                .Include(m => m.Photos)
                .Where(m => m.Id == id)
                .Select(m => new MemberDto
                {
                    Id = m.Id,
                    DisplayName = m.DisplayName,
                    ImageUrl = m.ImageUrl,
                    DateOfBirth = m.DateOfBirth,
                    Created = m.Created,
                    LastActive = m.LastActive,
                    Gender = m.Gender,
                    Description = m.Description,
                    City = m.City,
                    Country = m.Country,
                    Photos = m.Photos.Select(p => new PhotoDto
                    {
                        Id = p.Id,
                        Url = p.Url,
                        PublicId = p.PublicId
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (member == null) return NotFound();
            return member;
        }
    }
}
