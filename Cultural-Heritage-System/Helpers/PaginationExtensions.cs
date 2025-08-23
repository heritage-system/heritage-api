using AutoMapper;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Models;
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
    }
}

