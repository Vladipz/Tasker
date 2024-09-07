using Microsoft.EntityFrameworkCore;

namespace Tasker.BLL.Models
{
    public class PagedList<T>
    {
        private PagedList(List<T> items, int page, int pageSize, int count)
        {
            Items = items;
            TotalCount = count;
            PageSize = pageSize;
            Page = page;
        }

        public IEnumerable<T> Items { get; set; }

        public int Page { get; }

        public int PageSize { get; }

        public int TotalCount { get; }

        public bool HasPrevious => Page > 1;

        public bool HasNext => (Page * PageSize) < TotalCount;

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> query, int page, int pageSize)
        {
            int TotalCount = await query.CountAsync();

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return new(items, page, pageSize, TotalCount);
        }
    }
}