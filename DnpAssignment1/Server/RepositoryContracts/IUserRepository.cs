using Entities;

namespace RepositoryContracts;

public interface IUserRepository
{
    Task<User> AddAsync(User user); 
    Task<User> UpdateAsync(User user);
    Task DeleteAsync(int id);
    Task<User>getSingleAsync(int id);
    IQueryable<User> getManyAsync(); 
}