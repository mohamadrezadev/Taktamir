using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taktamir.framework.Common
{
    
   public record PaginationMetadata(int TotalCount, int TotalPages, int CurrentPage, int PageSize);
}
