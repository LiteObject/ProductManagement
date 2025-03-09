using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Core.Models
{
    public class PaginationMetadata(int totalItemCount, int CurrentPage, int pageSize)
    {
        public int TotalItemCount { get; set; } = totalItemCount;
        public int PageSize { get; set; } = pageSize;
        public int CurrentPage { get; set; } = CurrentPage;
        public int TotalPages { get; private set; } = (int)Math.Ceiling(totalItemCount / (double)pageSize);
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
}
