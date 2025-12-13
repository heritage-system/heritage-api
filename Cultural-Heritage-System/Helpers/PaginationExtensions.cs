using AutoMapper;
using Cultural_Heritage_System.Dtos.Response;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Helpers
{
    public static class PaginationExtensions
    {
        public static async Task<PageResponse<T>> ToPagedResponseAsync<T>(
            this IQueryable<T> query,
            int page,
            int pageSize)
        {
            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PageResponse<T>
            {
                CurrentPages = page,
                PageSizes = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                TotalElements = totalCount,
                Items = items
            };
        }

        // ========== Sync version ==========
        public static PageResponse<T> ToPagedResponse<T>(
            this IQueryable<T> query,
            int page,
            int pageSize)
        {
            var totalCount = query.Count();
            var items = query;
            if(page > 0)
            {
                items = items
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
           
            }          
            return new PageResponse<T>
            {
                CurrentPages = page,
                PageSizes = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                TotalElements = totalCount,
                Items = items.ToList()
            };
        }
    }
}
