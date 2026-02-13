using System.Security.Claims;
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

        [Authorize]
        [HttpPut]
        public async Task<ActionResult> UpdateMember(UpdateMemberDto updateDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var member = await context.Members
                .FirstOrDefaultAsync(m => m.Id == userId);

            if (member == null) return NotFound("Member not found");

            member.Description = updateDto.Description;
            member.City = updateDto.City;
            member.Country = updateDto.Country;
            member.LastActive = DateTime.UtcNow;

            if (await context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }

            return BadRequest("Failed to update member");
        }

        [Authorize]
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var member = await context.Members
                .Include(m => m.Photos)
                .FirstOrDefaultAsync(m => m.Id == userId);

            if (member == null) return NotFound("Member not found");

            // Validate file
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            if (file.Length > 10 * 1024 * 1024) // 10MB limit
                return BadRequest("File size cannot exceed 10MB");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                return BadRequest("Only image files (jpg, jpeg, png, gif) are allowed");

            // For now, save to wwwroot/images (later: integrate Cloudinary)
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var photoUrl = $"/images/{uniqueFileName}";

            var photo = new Entities.Photo
            {
                Url = photoUrl,
                PublicId = uniqueFileName
            };

            member.Photos.Add(photo);

            // If this is the first photo, set it as the main photo
            if (member.Photos.Count == 1)
            {
                member.ImageUrl = photoUrl;
            }

            if (await context.SaveChangesAsync() > 0)
            {
                return new PhotoDto
                {
                    Id = photo.Id,
                    Url = photo.Url,
                    PublicId = photo.PublicId
                };
            }

            return BadRequest("Failed to add photo");
        }

        [Authorize]
        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var member = await context.Members
                .Include(m => m.Photos)
                .FirstOrDefaultAsync(m => m.Id == userId);

            if (member == null) return NotFound("Member not found");

            var photo = member.Photos.FirstOrDefault(p => p.Id == photoId);
            if (photo == null) return NotFound("Photo not found");

            member.ImageUrl = photo.Url;

            if (await context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }

            return BadRequest("Failed to set main photo");
        }

        [Authorize]
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var member = await context.Members
                .Include(m => m.Photos)
                .FirstOrDefaultAsync(m => m.Id == userId);

            if (member == null) return NotFound("Member not found");

            var photo = member.Photos.FirstOrDefault(p => p.Id == photoId);
            if (photo == null) return NotFound("Photo not found");

            // Don't allow deleting the main photo if there are other photos
            if (member.ImageUrl == photo.Url && member.Photos.Count > 1)
            {
                return BadRequest("Cannot delete main photo. Set another photo as main first.");
            }

            // Delete physical file
            if (!string.IsNullOrEmpty(photo.PublicId))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", photo.PublicId);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            member.Photos.Remove(photo);

            // If this was the main photo, clear ImageUrl
            if (member.ImageUrl == photo.Url)
            {
                member.ImageUrl = member.Photos.FirstOrDefault()?.Url;
            }

            if (await context.SaveChangesAsync() > 0)
            {
                return NoContent();
            }

            return BadRequest("Failed to delete photo");
        }
    }
}
