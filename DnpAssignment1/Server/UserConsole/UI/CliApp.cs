using Entities;
using RepositoryContracts;

namespace UserConsole.UI
{
    public class CliApp
    {
        private readonly IUserRepository userRepository;
        private readonly ICommentRepository commentRepository;
        private readonly IPostRepository postRepository;

        // The constructor that Program.cs is looking for
        public CliApp(IUserRepository userRepository, ICommentRepository commentRepository, IPostRepository postRepository)
        {
            this.userRepository = userRepository;
            this.commentRepository = commentRepository;
            this.postRepository = postRepository;
        }

        // The method that Program.cs is trying to call
        public async Task StartAsync()
        {
            Console.WriteLine(Path.GetFullPath("posts.json"));
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\n--- Forum Menu ---");
                Console.WriteLine("1. Create new user");
                Console.WriteLine("2. Create new post");
                Console.WriteLine("3. View posts overview");
                Console.WriteLine("0. Exit");
                Console.Write("Select an option: ");

                string? input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        await CreateUserAsync();
                        break;
                    case "2":
                        await CreatePostAsync();
                        break;
                    case "3":
                        ViewPostsOverview();
                        break;
                    case "4":
                        await ViewSinglePostAsync();
                        break;
                    case "5":
                        await AddCommentAsync();
                        break;
                    case "0":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option, please try again.");
                        break;
                }
            }
        }

        private async Task CreateUserAsync()
        {
            Console.Write("Enter username: ");
            string? username = Console.ReadLine();
            
            Console.Write("Enter password: ");
            string? password = Console.ReadLine();

            User newUser = new User { Username = username, Password = password };
            User createdUser = await userRepository.AddAsync(newUser);
            
            Console.WriteLine($"User '{createdUser.Username}' created successfully with ID: {createdUser.Id}");
        }
        
        
        
        private async Task CreatePostAsync()
        {
            Console.WriteLine("\n--- Create New Post ---");
    
            Console.Write("Enter post title: ");
            string? title = Console.ReadLine();
    
            Console.Write("Enter post body: ");
            string? body = Console.ReadLine();

            Console.Write("Enter your User ID: ");
            string? userIdInput = Console.ReadLine();

            // Basic validation to ensure the User ID is actually a number
            if (int.TryParse(userIdInput, out int userId))
            {
                // Check if the user actually exists before making the post
                try
                {
                    await userRepository.GetSingleAsync(userId);
            
                    Post newPost = new Post 
                    { 
                        Title = title, 
                        Body = body, 
                        UserId = userId 
                    };
            
                    Post createdPost = await postRepository.AddAsync(newPost);
                    Console.WriteLine($"Post '{createdPost.Title}' created successfully with ID: {createdPost.Id}");
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}. Cannot create post for a non-existent user.");
                }
            }
            else
            {
                Console.WriteLine("Invalid User ID format. Please enter a valid number.");
            }
        }
        
        private void ViewPostsOverview()
        {
            Console.WriteLine("\n--- Posts Overview ---");
    
            // Retrieve all posts from the repository
            var posts = postRepository.GetMany().ToList();

            if (!posts.Any())
            {
                Console.WriteLine("There are currently no posts to display.");
                return;
            }

            // Display just the ID and Title as required by the assignment
            foreach (var post in posts)
            {
                Console.WriteLine($"[{post.Id}] {post.Title}");
            }
        }
        
        
        private async Task ViewSinglePostAsync()
        {
            Console.Write("\nEnter the ID of the post you want to view: ");
            if (int.TryParse(Console.ReadLine(), out int postId))
            {
                try
                {
                    Post post = await postRepository.GetSingleAsync(postId);
                    Console.WriteLine($"\n--- {post.Title} ---");
                    Console.WriteLine(post.Body);
                    Console.WriteLine("-------------------");
                    Console.WriteLine("Comments:");
            
                    // Retrieve and filter comments for this specific post
                    var comments = commentRepository.GetMany().Where(c => c.PostId == postId).ToList();
            
                    if (!comments.Any())
                    {
                        Console.WriteLine("No comments yet.");
                    }
                    else
                    {
                        foreach (var comment in comments)
                        {
                            Console.WriteLine($"- {comment.Body} (User ID: {comment.UserId})");
                        }
                    }
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }
        
        private async Task AddCommentAsync()
        {
            Console.WriteLine("\n--- Add a Comment ---");
    
            Console.Write("Enter Post ID: ");
            string? postIdInput = Console.ReadLine();
    
            Console.Write("Enter your User ID: ");
            string? userIdInput = Console.ReadLine();
    
            Console.Write("Enter comment body: ");
            string? body = Console.ReadLine();

            if (int.TryParse(postIdInput, out int postId) && int.TryParse(userIdInput, out int userId))
            {
                try
                {
                    await userRepository.GetSingleAsync(userId);
                    await postRepository.GetSingleAsync(postId);
                    Comment newComment = new Comment 
                    { 
                        Body = body, 
                        UserId = userId, 
                        PostId = postId 
                    };
            
                    await commentRepository.AddAsync(newComment);
                    Console.WriteLine("Comment added successfully!");
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}. Make sure both the Post ID and User ID are correct.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format. Both Post ID and User ID must be numbers.");
            }
        }
        
        
        
        
        
    }
}