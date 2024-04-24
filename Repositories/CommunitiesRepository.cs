using Microsoft.EntityFrameworkCore;
using Reddit.Models;
using System.Linq.Expressions;

namespace Reddit.Repositories
{
    public class CommunitiesRepository : ICommunitiesRepository
    {
        private readonly ApplicationDbContext _context;

        public CommunitiesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedList<Community>> GetCommunities(int page, int pageSize, string? searchTerm, bool isAscending, string? sortKey)
        {
            var communities = _context.Communities.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                communities = communities.Where(c => c.Name.Contains(searchTerm) || c.Description.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(sortKey))
            {
                Func<IQueryable<Community>, Expression<Func<Community, dynamic>>, IQueryable<Community>> orderFunc;
                orderFunc = isAscending ? Queryable.OrderBy : Queryable.OrderByDescending;
                switch (sortKey.ToLower())
                {
                    case "id":
                        communities = orderFunc(communities, c => c.Id);
                        break;

                    case "createdat":
                        communities = orderFunc(communities, c => c.CreateAt);
                        break;

                    case "postscount":
                        communities = orderFunc(communities, c => c.Posts.Count);
                        break;

                    case "subscriberscount":
                        communities = orderFunc(communities, c => c.Subscribers.Count);
                        break;
                }
            }
            else
            {
                communities.OrderBy(c => c.Id);
            }

            return await PagedList<Community>.CreateAsync(communities, page, pageSize);
        }
    }
}
