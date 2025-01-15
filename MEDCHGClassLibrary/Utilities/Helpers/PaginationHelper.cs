using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MEDCHGClassLibrary.Utilities.Helpers
{
    public static class PaginationHelper
    {
        public static IEnumerable<T> Paginate<T>(this IEnumerable<T> source, int page, int pageSize)
        {
            return source.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}
