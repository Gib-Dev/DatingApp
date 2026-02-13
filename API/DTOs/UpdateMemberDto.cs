using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class UpdateMemberDto
{
    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public required string City { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public required string Country { get; set; }
}
