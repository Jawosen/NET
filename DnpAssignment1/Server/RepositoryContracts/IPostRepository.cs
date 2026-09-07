using Entities;

namespace RepositoryContracts;

public interface IPostRepository
{
    Task<Post> AddAsync(Post post); 
    Task<Post> UpdateAsync(Post post);
    Task DeleteAsync(int id);
    Task<Post>GetSingleAsync(int id);
    IQueryable<Post> GetManyAsync(); 
}