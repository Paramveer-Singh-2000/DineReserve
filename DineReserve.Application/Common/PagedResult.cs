using DineReserve.Application.Restaurants.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace DineReserve.Application.Common
{
    public class PagedResult<T>
    {
        public PagedResult(IEnumerable<T> items, int totalCount, int pageSize, int pageNumber)
        {
            Items = items;
            TotalItemCount = totalCount;
            TotalPages = pageSize > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0;
            ItemFrom = pageSize * (pageNumber - 1);
            ItemTo = ItemFrom  + pageSize - 1;
        }
        public IEnumerable<T> Items { get; set; }
        public int TotalPages { get; set; }
        public int TotalItemCount { get; set; }
        public int ItemFrom {  get; set; }
        public int ItemTo { get; set; }
}
}
