using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class CreateMessageDto
{
    [Required]
    public required string RecipientId { get; set; }

    [Required]
    [StringLength(500, MinimumLength = 1)]
    public required string Content { get; set; }
}
