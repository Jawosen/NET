using Entities;

namespace RepositoryContracts;

public interface IForum
{
    Task<Forum> AddAsync(Forum forum); 
    Task<Forum> UpdateAsync(Forum forum);
    Task DeleteAsync(int id);
    Task<Forum>getSingleAsync(int id);
    IQueryable<Forum> getManyAsync(); 
}