using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class CommentFileRepository : ICommentRepository
{
    private readonly string filePath = "comments.json";

    public CommentFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    private async Task<List<Comment>> LoadCommentsAsync()
    {
        string commentsAsJson = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<Comment>>(commentsAsJson) ?? new List<Comment>();
    }

    private async Task SaveCommentsAsync(List<Comment> comments)
    {
        string commentsAsJson = JsonSerializer.Serialize(comments, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, commentsAsJson);
    }

    public async Task<Comment> AddAsync(Comment comment)
    {
        List<Comment> comments = await LoadCommentsAsync();

        int maxId = comments.Count > 0 ? comments.Max(c => c.Id) : 0;
        comment.Id = maxId + 1;

        comments.Add(comment);
        await SaveCommentsAsync(comments);

        return comment;
    }

    public async Task<Comment> UpdateAsync(Comment comment)
    {
        List<Comment> comments = await LoadCommentsAsync();

        Comment? commentToUpdate = comments.FirstOrDefault(c => c.Id == comment.Id);
        if (commentToUpdate == null)
        {
            throw new KeyNotFoundException($"Comment with ID '{comment.Id}' was not found.");
        }

        commentToUpdate.Body = comment.Body;
        commentToUpdate.UserId = comment.UserId;
        commentToUpdate.PostId = comment.PostId;

        await SaveCommentsAsync(comments);
        return commentToUpdate;
    }

    public async Task DeleteAsync(int id)
    {
        List<Comment> comments = await LoadCommentsAsync();

        Comment? commentToRemove = comments.FirstOrDefault(c => c.Id == id);
        if (commentToRemove == null)
        {
            throw new KeyNotFoundException($"Comment with ID '{id}' was not found.");
        }

        comments.Remove(commentToRemove);
        await SaveCommentsAsync(comments);
    }

    public async Task<Comment> GetSingleAsync(int id)
    {
        List<Comment> comments = await LoadCommentsAsync();

        Comment? comment = comments.FirstOrDefault(c => c.Id == id);
        if (comment == null)
        {
            throw new KeyNotFoundException($"Comment with ID '{id}' was not found.");
        }

        return comment;
    }

    public IQueryable<Comment> GetMany()
    {
        string commentAsJson = File.ReadAllTextAsync(filePath).Result;
        List<Comment> comments = JsonSerializer.Deserialize<List<Comment>>(commentAsJson) ?? new List<Comment>();
        return comments.AsQueryable();
    }
}