using FileRepositories;
using UserConsole.UI;
using InMemoryRepositories;
using RepositoryContracts;

Console.WriteLine("Starting CLI App");

IUserRepository userRepository = new UserFileRepository();
ICommentRepository commentRepository = new CommentFileRepository();
IPostRepository postRepository = new PostFileRepository();

CliApp cliApp = new CliApp(userRepository, commentRepository, postRepository);
await cliApp.StartAsync();