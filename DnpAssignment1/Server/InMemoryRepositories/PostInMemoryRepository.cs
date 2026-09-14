using Entities;
using RepositoryContracts;

namespace InMemoryRepositories
{
    public class PostInMemoryRepository : IPostRepository
    {
        private readonly List<Post> posts = new();

        public PostInMemoryRepository()
        {
            seedData();
        }

        private void seedData()
        {
            posts.Add(new Post { Id = 1, Title = "Welcome to the forum!", Body = "This is the first post." });
            posts.Add(new Post { Id = 2, Title = "C# tips", Body = "Use var when the type is obvious." });
            posts.Add(new Post { Id = 3, Title = "Async/await gotchas", Body = "Don't block on async code." });
        }
        
        public Task<Post> AddAsync(Post post)
        {
            post.Id = posts.Any()
                ? posts.Max(p => p.Id) + 1
                : 1;
            posts.Add(post);
            return Task.FromResult(post);
        }

    

        public Task UpdateAsync(Post post)
        {
            Post? existingPost = posts.SingleOrDefault(p => p.Id == post.Id);
            if (existingPost is null)
            {
                throw new InvalidOperationException($"Post with ID '{post.Id}' not found");
            }
            posts.Remove(existingPost);
            posts.Add(post);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int postId)
        {
            Post ? postToRemove  = posts.SingleOrDefault(p=> p.Id == postId);
            if (postToRemove is null)
            {
                throw new InvalidOperationException($"Post with ID '{postId}' not found");
            }
            posts.Remove(postToRemove);
            return Task.CompletedTask;
        }

     

        public Task<Post> GetSingleAsync(int postId)
        {
            Post ? postToGet = posts.SingleOrDefault(p => p.Id == postId);
            if (postToGet is null)
            {
                throw new InvalidOperationException($"Post with ID '{postId}' not found");
            }
            
            
            return Task.FromResult(postToGet);
        }

        public IQueryable<Post> GetManyAsync()
        {
            return posts.AsQueryable();
        }
    }
}