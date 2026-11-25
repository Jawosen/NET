namespace Entities;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
    public int Likes { get; set; }
    public int Dislikes { get; set; }
    public DateTime Created { get; set; }

    // Foreign key + navigation properties (EFC)
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
