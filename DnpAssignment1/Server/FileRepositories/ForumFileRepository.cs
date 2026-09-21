using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class ForumFileRepository : IForumRepository
{
    private readonly string filePath = "forums.json";

    public ForumFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    public async Task<Forum> AddAsync(Forum forum)
    {
        string forumsAsJson = await File.ReadAllTextAsync(filePath);
        List<Forum> forums = JsonSerializer.Deserialize<List<Forum>>(forumsAsJson) ?? new List<Forum>();
        
        int maxId = forums.Count > 0 ? forums.Max(f => f.Id) : 0;
        forum.Id = maxId + 1;
        
        forums.Add(forum);
        forumsAsJson = JsonSerializer.Serialize(forums, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, forumsAsJson);
        
        return forum;
    }

    public async Task<Forum> UpdateAsync(Forum forum)
    {
        string forumsAsJson = await File.ReadAllTextAsync(filePath);
        List<Forum> forums = JsonSerializer.Deserialize<List<Forum>>(forumsAsJson) ?? new List<Forum>();
        
        Forum? forumToUpdate = forums.FirstOrDefault(f => f.Id == forum.Id);
        if (forumToUpdate == null)
        {
            throw new KeyNotFoundException($"Forum with ID '{forum.Id}' was not found.");
        }

        forumToUpdate.Name = forum.Name;

        forumsAsJson = JsonSerializer.Serialize(forums, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, forumsAsJson);
        
        return forumToUpdate;
    }

    public async Task DeleteAsync(int id)
    {
        string forumsAsJson = await File.ReadAllTextAsync(filePath);
        List<Forum> forums = JsonSerializer.Deserialize<List<Forum>>(forumsAsJson) ?? new List<Forum>();
        
        Forum? forumToRemove = forums.FirstOrDefault(f => f.Id == id);
        if (forumToRemove != null)
        {
            forums.Remove(forumToRemove);
            await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(forums, new JsonSerializerOptions { WriteIndented = true }));
        }
    }

    public async Task<Forum> GetSingleAsync(int id)
    {
        string forumsAsJson = await File.ReadAllTextAsync(filePath);
        List<Forum> forums = JsonSerializer.Deserialize<List<Forum>>(forumsAsJson) ?? new List<Forum>();
        
        Forum? forum = forums.FirstOrDefault(f => f.Id == id);
        if (forum == null)
        {
            throw new KeyNotFoundException($"Forum with ID '{id}' was not found.");
        }

        return forum;
    }
    

    public IQueryable<Forum> GetMany()
    {
        string forumsAsJson = File.ReadAllTextAsync(filePath).Result;
        List<Forum> forums = JsonSerializer.Deserialize<List<Forum>>(forumsAsJson) ?? new List<Forum>();
        return forums.AsQueryable();
    }
}