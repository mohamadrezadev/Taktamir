using Taktamir.Core.Domain._06.Wallets;
using Taktamir.infra.Data.sql._01.Common;

namespace Taktamir.infra.Data.sql._06.Wallets
{
    public class OrdersRepository : Repository<Order>, IOrderRepository
    {
        public OrdersRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
