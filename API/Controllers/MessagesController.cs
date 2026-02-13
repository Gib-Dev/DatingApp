using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class MessagesController(AppDbContext context) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
    {
        var senderId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (senderId == null) return Unauthorized();

        if (senderId == createMessageDto.RecipientId)
            return BadRequest("You cannot send messages to yourself");

        var sender = await context.Users
            .Include(u => u.Member)
            .FirstOrDefaultAsync(u => u.Id == senderId);

        var recipient = await context.Users
            .Include(u => u.Member)
            .FirstOrDefaultAsync(u => u.Id == createMessageDto.RecipientId);

        if (recipient == null)
            return NotFound("Recipient not found");

        var message = new Message
        {
            SenderId = senderId,
            RecipientId = createMessageDto.RecipientId,
            Content = createMessageDto.Content
        };

        context.Messages.Add(message);

        if (await context.SaveChangesAsync() > 0)
        {
            return new MessageDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                SenderName = sender?.Member?.DisplayName ?? sender?.DisplayName ?? "Unknown",
                SenderPhotoUrl = sender?.Member?.ImageUrl,
                RecipientId = message.RecipientId,
                RecipientName = recipient.Member?.DisplayName ?? recipient.DisplayName,
                RecipientPhotoUrl = recipient.Member?.ImageUrl,
                Content = message.Content,
                DateSent = message.DateSent,
                DateRead = message.DateRead
            };
        }

        return BadRequest("Failed to send message");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessages([FromQuery] string container = "inbox")
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        IQueryable<Message> query = container switch
        {
            "inbox" => context.Messages
                .Where(m => m.RecipientId == userId && !m.RecipientDeleted),
            "outbox" => context.Messages
                .Where(m => m.SenderId == userId && !m.SenderDeleted),
            "unread" => context.Messages
                .Where(m => m.RecipientId == userId && !m.RecipientDeleted && m.DateRead == null),
            _ => context.Messages.Where(m => false)
        };

        var messages = await query
            .Include(m => m.Sender).ThenInclude(u => u.Member)
            .Include(m => m.Recipient).ThenInclude(u => u.Member)
            .OrderByDescending(m => m.DateSent)
            .Select(m => new MessageDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                SenderName = m.Sender.Member!.DisplayName,
                SenderPhotoUrl = m.Sender.Member.ImageUrl,
                RecipientId = m.RecipientId,
                RecipientName = m.Recipient.Member!.DisplayName,
                RecipientPhotoUrl = m.Recipient.Member.ImageUrl,
                Content = m.Content,
                DateSent = m.DateSent,
                DateRead = m.DateRead
            })
            .ToListAsync();

        return Ok(messages);
    }

    [HttpGet("thread/{userId}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string userId)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (currentUserId == null) return Unauthorized();

        var messages = await context.Messages
            .Include(m => m.Sender).ThenInclude(u => u.Member)
            .Include(m => m.Recipient).ThenInclude(u => u.Member)
            .Where(m =>
                (m.SenderId == currentUserId && m.RecipientId == userId && !m.SenderDeleted) ||
                (m.SenderId == userId && m.RecipientId == currentUserId && !m.RecipientDeleted)
            )
            .OrderBy(m => m.DateSent)
            .ToListAsync();

        // Mark unread messages as read
        var unreadMessages = messages
            .Where(m => m.RecipientId == currentUserId && m.DateRead == null)
            .ToList();

        if (unreadMessages.Any())
        {
            foreach (var message in unreadMessages)
            {
                message.DateRead = DateTime.UtcNow;
            }
            await context.SaveChangesAsync();
        }

        var messageDtos = messages.Select(m => new MessageDto
        {
            Id = m.Id,
            SenderId = m.SenderId,
            SenderName = m.Sender.Member!.DisplayName,
            SenderPhotoUrl = m.Sender.Member.ImageUrl,
            RecipientId = m.RecipientId,
            RecipientName = m.Recipient.Member!.DisplayName,
            RecipientPhotoUrl = m.Recipient.Member.ImageUrl,
            Content = m.Content,
            DateSent = m.DateSent,
            DateRead = m.DateRead
        }).ToList();

        return Ok(messageDtos);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var message = await context.Messages.FindAsync(id);
        if (message == null) return NotFound();

        if (message.SenderId == userId)
            message.SenderDeleted = true;

        if (message.RecipientId == userId)
            message.RecipientDeleted = true;

        if (message.SenderDeleted && message.RecipientDeleted)
            context.Messages.Remove(message);

        if (await context.SaveChangesAsync() > 0)
            return NoContent();

        return BadRequest("Failed to delete message");
    }
}
