using Entities;

namespace RepositoryContracts;

public interface IForumRepository
{
    Task<Forum> AddAsync(Forum forum); 
    Task<Forum> UpdateAsync(Forum forum);
    Task DeleteAsync(int id);
    Task<Forum>GetSingleAsync(int id);
    IQueryable<Forum> GetManyAsync(); 
}