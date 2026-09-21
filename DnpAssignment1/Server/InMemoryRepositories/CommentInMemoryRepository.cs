using Entities;
using RepositoryContracts;

namespace InMemoryRepositories
{
    public class CommentInMemoryRepository : ICommentRepository
    {
        private readonly List<Comment> comments = new();

        public Task<Comment> AddAsync(Comment comment)
        {
            comment.Id = comments.Any()
                ? comments.Max(c => c.Id) + 1
                : 1;
            comments.Add(comment);
            return Task.FromResult(comment);
        }

        public Task<Comment> UpdateAsync(Comment comment)
        {
            Comment? existingComment = comments.SingleOrDefault(c => c.Id == comment.Id);
            if (existingComment is null)
            {
                throw new InvalidOperationException($"Comment with ID '{comment.Id}' not found");
            }
            comments.Remove(existingComment);
            comments.Add(comment);
            return (Task<Comment>)Task.CompletedTask;
        }

        public Task DeleteAsync(int commentId)
        {
            Comment? commentToRemove = comments.SingleOrDefault(c => c.Id == commentId);
            if (commentToRemove is null)
            {
                throw new InvalidOperationException($"Comment with ID '{commentId}' not found");
            }
            comments.Remove(commentToRemove);
            return Task.CompletedTask;
        }
        

        public Task<Comment> GetSingleAsync(int commentId)
        {
            Comment? commentToGet = comments.SingleOrDefault(c => c.Id == commentId);
            if (commentToGet is null)
            {
                throw new InvalidOperationException($"Comment with ID '{commentId}' not found");
            }
            return Task.FromResult(commentToGet);
        }

        public IQueryable<Comment> GetMany()
        {
            return comments.AsQueryable();
        }
    }
}