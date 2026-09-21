    using Entities;
    using RepositoryContracts;

    namespace InMemoryRepositories
    {
        public class UserInMemoryRepository : IUserRepository
        {
            private readonly List<User> users = new();
            
            public Task<User> AddAsync(User user)
            {
                user.Id = users.Any()
                    ? users.Max(u => u.Id) + 1
                    : 1;
                users.Add(user);
                return Task.FromResult(user);
            }

            private void SeedData()
            {
                new User { Username = "fool", Id = 12, Email = "fool@gmail.com", Password = "1234" };
            }

            public Task<User> UpdateAsync(User user)
            {
                User? existingUser = users.SingleOrDefault(u => u.Id == user.Id);
                if (existingUser is null)
                {
                    throw new InvalidOperationException($"User with ID '{user.Id}' not found");
                }
                users.Remove(existingUser);
                users.Add(user);
                return (Task<User>)Task.CompletedTask;
            }

            public Task DeleteAsync(int userId)
            {
                User? userToRemove = users.SingleOrDefault(u => u.Id == userId);
                if (userToRemove is null)
                {
                    throw new InvalidOperationException($"User with ID '{userId}' not found");
                }
                users.Remove(userToRemove);
                return Task.CompletedTask;
            }

            public Task<User> GetSingleAsync(int userId)
            {
                User? userToGet = users.SingleOrDefault(u => u.Id == userId);
                if (userToGet is null)
                {
                    throw new InvalidOperationException($"User with ID '{userId}' not found");
                }
                return Task.FromResult(userToGet);
            }

            public IQueryable<User> GetMany()
            {
                return users.AsQueryable();
            }
        }
    }