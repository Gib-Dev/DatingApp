using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class LikesController(AppDbContext context) : BaseApiController
{
    [HttpPost("{likedUserId}")]
    public async Task<ActionResult> ToggleLike(string likedUserId)
    {
        var sourceUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (sourceUserId == null) return Unauthorized();

        if (sourceUserId == likedUserId)
            return BadRequest("You cannot like yourself");

        var existingLike = await context.UserLikes
            .FindAsync(sourceUserId, likedUserId);

        if (existingLike != null)
        {
            // Unlike
            context.UserLikes.Remove(existingLike);
        }
        else
        {
            // Like
            var like = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUserId
            };
            context.UserLikes.Add(like);
        }

        if (await context.SaveChangesAsync() > 0)
        {
            return Ok();
        }

        return BadRequest("Failed to update like");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUserLikes([FromQuery] string predicate)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        IQueryable<Member> query;

        switch (predicate)
        {
            case "liked":
                // Members that current user has liked
                query = context.UserLikes
                    .Where(like => like.SourceUserId == userId)
                    .Select(like => like.LikedUser.Member);
                break;

            case "likedBy":
                // Members who have liked the current user
                query = context.UserLikes
                    .Where(like => like.LikedUserId == userId)
                    .Select(like => like.SourceUser.Member);
                break;

            case "mutual":
                // Mutual likes (matches)
                var likedUserIds = context.UserLikes
                    .Where(like => like.SourceUserId == userId)
                    .Select(like => like.LikedUserId);

                query = context.UserLikes
                    .Where(like => like.LikedUserId == userId && likedUserIds.Contains(like.SourceUserId))
                    .Select(like => like.SourceUser.Member);
                break;

            default:
                return BadRequest("Invalid predicate");
        }

        var members = await query
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

        return Ok(members);
    }
}
