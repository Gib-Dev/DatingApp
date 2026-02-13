namespace API.Entities;

public class UserLike
{
    public required string SourceUserId { get; set; }
    public AppUser SourceUser { get; set; } = null!;

    public required string LikedUserId { get; set; }
    public AppUser LikedUser { get; set; } = null!;
}
