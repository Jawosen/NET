using Entities;

namespace RepositoryContracts;

public interface IPost
{
    Task<Post> AddAsync(Post post); 
    Task<Post> UpdateAsync(Post post);
    Task DeleteAsync(int id);
    Task<Post>getSingleAsync(int id);
    IQueryable<Post> getManyAsync(); 
}