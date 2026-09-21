using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class UserFileRepository : IUserRepository
{
    private readonly string filePath = "users.json";

    public UserFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    public async Task<User> AddAsync(User user)
    {
        string usersAsJson = await File.ReadAllTextAsync(filePath);
        List<User> users = JsonSerializer.Deserialize<List<User>>(usersAsJson) ?? new List<User>();
        
        int maxId = users.Count > 0 ? users.Max(u => u.Id) : 0;
        user.Id = maxId + 1;

        users.Add(user);
        usersAsJson = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, usersAsJson);
        
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        string usersAsJson = await File.ReadAllTextAsync(filePath);
        List<User> users = JsonSerializer.Deserialize<List<User>>(usersAsJson) ?? new List<User>();
        
        User? userToUpdate = users.FirstOrDefault(u => u.Id == user.Id);
        if (userToUpdate == null)
        {
            throw new KeyNotFoundException($"User with ID '{user.Id}' was not found.");
        }

        userToUpdate.Username = user.Username;
        userToUpdate.Password = user.Password;
        userToUpdate.Email = user.Email;

        usersAsJson = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, usersAsJson);
        
        return userToUpdate;
    }

    public async Task DeleteAsync(int id)
    {
        string usersAsJson = await File.ReadAllTextAsync(filePath);
        List<User> users = JsonSerializer.Deserialize<List<User>>(usersAsJson) ?? new List<User>();
        
        User? userToRemove = users.FirstOrDefault(u => u.Id == id);
        if (userToRemove != null)
        {
            users.Remove(userToRemove);
            await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true }));
        }
    }

    public async Task<User> GetSingleAsync(int id)
    {
        string usersAsJson = await File.ReadAllTextAsync(filePath);
        List<User> users = JsonSerializer.Deserialize<List<User>>(usersAsJson) ?? new List<User>();
        
        User? user = users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID '{id}' was not found.");
        }

        return user;
    }

    public IQueryable<User> GetMany()
    {
        string usersAsJson = File.ReadAllTextAsync(filePath).Result;
        List<User> users = JsonSerializer.Deserialize<List<User>>(usersAsJson) ?? new List<User>();
        return users.AsQueryable();
    }
}