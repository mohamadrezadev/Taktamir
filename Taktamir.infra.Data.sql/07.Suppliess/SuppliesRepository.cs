using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taktamir.Core.Domain._07.Suppliess;
using Taktamir.infra.Data.sql._01.Common;

namespace Taktamir.infra.Data.sql._07.Suppliess
{
    public class SuppliesRepository : Repository<Supplies>, ISuppliesRepository
    {
        public SuppliesRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
