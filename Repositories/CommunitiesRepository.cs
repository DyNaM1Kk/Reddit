using Microsoft.EntityFrameworkCore;
using Reddit.Models;

namespace Reddit.Repositories
{
    public class CommunitiesRepository : ICommunitiesRepository
    {
        private readonly ApplicationDbContext _context;

        public CommunitiesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedList<Community>> GetCommunities(int page, int pageSize, string? searchTerm)
        {
            var communities = _context.Communities.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                communities = communities.Where(c => c.Name.Contains(searchTerm) || c.Description.Contains(searchTerm));
            }

            return await PagedList<Community>.CreateAsync(communities, page, pageSize);
        }
    }
}
