namespace ApiContracts;

public class PostDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
    public DateTime Created { get; set; }
    public int UserId { get; set; }

    // Only filled when requested with ?includeAuthor=true / ?includeComments=true
    public UserDto? Author { get; set; }
    public List<CommentDto> Comments { get; set; } = new();
}
