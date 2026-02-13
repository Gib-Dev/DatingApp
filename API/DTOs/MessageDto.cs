namespace API.DTOs;

public class MessageDto
{
    public int Id { get; set; }
    public required string SenderId { get; set; }
    public required string SenderName { get; set; }
    public string? SenderPhotoUrl { get; set; }
    public required string RecipientId { get; set; }
    public required string RecipientName { get; set; }
    public string? RecipientPhotoUrl { get; set; }
    public required string Content { get; set; }
    public DateTime DateSent { get; set; }
    public DateTime? DateRead { get; set; }
}
