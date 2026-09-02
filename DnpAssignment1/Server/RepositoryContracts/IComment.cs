using Entities;

namespace RepositoryContracts;

public interface IComment
{
    Task<Comment> AddAsync(Comment comment); 
    Task<Comment> UpdateAsync(Comment comment);
    Task DeleteAsync(int id);
    Task<Comment>getSingleAsync(int id);
    IQueryable<Comment> getManyAsync(); 
}