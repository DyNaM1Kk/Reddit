using Reddit.Models;

namespace Reddit.Repositories
{
    public class PostsRepository : IPostsRepository
    {
        private readonly ApplicationDbContext _context;

        public PostsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedList<Post>> GetPosts(int page, int pageSize, string? searchTerm)
        {
            var posts = _context.Posts.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                posts = posts.Where(post => post.Title.Contains(searchTerm) || post.Content.Contains(searchTerm));
            }

            return await PagedList<Post>.CreateAsync(posts, page, pageSize);
        }
    }
}
