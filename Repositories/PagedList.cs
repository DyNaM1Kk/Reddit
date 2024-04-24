using Microsoft.EntityFrameworkCore;

namespace Reddit.Repositories
{
    public class PagedList<T>
    {
        public List<T> Items { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPrevPage { get; set; }

        private PagedList(List<T> items, int page, int pageSize, int count, bool hasNextPage, bool hasPrevPage)
        {
            Items = items;
            Page = page;
            PageSize = pageSize;
            Count = count;
            HasNextPage = hasNextPage;
            HasPrevPage = hasNextPage;
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> items, int page, int pageSize)
        {
            List<T> pagedItems = await items.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            int count = await items.CountAsync();
            bool hasNextPage = (page * pageSize) < count;
            bool hasPrevPage = page > 1;

            return new PagedList<T>(pagedItems, page, pageSize, count, hasNextPage, hasPrevPage);
        }
    }
}
