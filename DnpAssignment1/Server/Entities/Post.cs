namespace Entities;

public class Post
{
    public string Title { get; set; }
    public string Body { get; set; }
    public int Id { get; set; }
    public int Likes { get; set; }
    public int Dislikes { get; set; }
    public DateTime Created { get; set; }
}