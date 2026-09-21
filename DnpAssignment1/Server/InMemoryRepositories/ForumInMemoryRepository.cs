using Entities;
using RepositoryContracts;

namespace InMemoryRepositories
{
    public class ForumInMemoryRepository : IForumRepository
    {
        private readonly List<Forum> forums = new();

        public Task<Forum> AddAsync(Forum forum)
        {
            forum.Id = forums.Any()
                ? forums.Max(f => f.Id) + 1
                : 1;
            forums.Add(forum);
            return Task.FromResult(forum);
        }

        public Task<Forum> UpdateAsync(Forum forum)
        {
            Forum? existingForum = forums.SingleOrDefault(f => f.Id == forum.Id);
            if (existingForum is null)
            {
                throw new InvalidOperationException($"Forum with ID '{forum.Id}' not found");
            }
            forums.Remove(existingForum);
            forums.Add(forum);
            return (Task<Forum>)Task.CompletedTask;
        }

        public Task DeleteAsync(int forumId)
        {
            Forum? forumToRemove = forums.SingleOrDefault(f => f.Id == forumId);
            if (forumToRemove is null)
            {
                throw new InvalidOperationException($"Forum with ID '{forumId}' not found");
            }
            forums.Remove(forumToRemove);
            return Task.CompletedTask;
        }

        public Task<Forum> GetSingleAsync(int forumId)
        {
            Forum? forumToGet = forums.SingleOrDefault(f => f.Id == forumId);
            if (forumToGet is null)
            {
                throw new InvalidOperationException($"Forum with ID '{forumId}' not found");
            }
            return Task.FromResult(forumToGet);
        }

        public IQueryable<Forum> GetMany()
        {
            return forums.AsQueryable();
        }
    }
}