using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class PostFileRepository : IPostRepository
{
    private readonly string filePath = "posts.json";

    public PostFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    public async Task<Post> AddAsync(Post post)
    {
        string postsAsJson = await File.ReadAllTextAsync(filePath);
        List<Post> posts = JsonSerializer.Deserialize<List<Post>>(postsAsJson) ?? new List<Post>();
        
        int maxId = posts.Count > 0 ? posts.Max(p => p.Id) : 0;
        post.Id = maxId + 1;
        post.Created = DateTime.UtcNow;

        posts.Add(post);
        postsAsJson = JsonSerializer.Serialize(posts, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, postsAsJson);
        
        return post;
    }

    public async Task UpdateAsync(Post post)
    {
        string postsAsJson = await File.ReadAllTextAsync(filePath);
        List<Post> posts = JsonSerializer.Deserialize<List<Post>>(postsAsJson) ?? new List<Post>();
    
        Post? postToUpdate = posts.FirstOrDefault(p => p.Id == post.Id);
        if (postToUpdate == null)
        {
            throw new KeyNotFoundException($"Post with ID '{post.Id}' was not found.");
        }

        postToUpdate.Title = post.Title;
        postToUpdate.Body = post.Body;
        postToUpdate.Likes = post.Likes;
        postToUpdate.Dislikes = post.Dislikes;
        postToUpdate.UserId = post.UserId;

        postsAsJson = JsonSerializer.Serialize(posts, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, postsAsJson);
    }

    public async Task DeleteAsync(int id)
    {
        string postsAsJson = await File.ReadAllTextAsync(filePath);
        List<Post> posts = JsonSerializer.Deserialize<List<Post>>(postsAsJson) ?? new List<Post>();
        
        Post? postToRemove = posts.FirstOrDefault(p => p.Id == id);
        if (postToRemove != null)
        {
            posts.Remove(postToRemove);
            await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(posts, new JsonSerializerOptions { WriteIndented = true }));
        }
    }

    public async Task<Post> GetSingleAsync(int id)
    {
        string postsAsJson = await File.ReadAllTextAsync(filePath);
        List<Post> posts = JsonSerializer.Deserialize<List<Post>>(postsAsJson) ?? new List<Post>();
        
        Post? post = posts.FirstOrDefault(p => p.Id == id);
        if (post == null)
        {
            throw new KeyNotFoundException($"Post with ID '{id}' was not found.");
        }

        return post;
    }

    public IQueryable<Post> GetMany()
    {
        string postsAsJson = File.ReadAllTextAsync(filePath).Result;
        List<Post> posts = JsonSerializer.Deserialize<List<Post>>(postsAsJson) ?? new List<Post>();
        return posts.AsQueryable();
    }
}