using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taktamir.Core.Domain._01.Common;
using Taktamir.Core.Domain._03.Users;
using Taktamir.Core.Domain._4.Customers;
using Taktamir.framework.Common;

namespace Taktamir.Core.Domain._06.Wallets
{
    public interface IWalletRepository: IRepository<Wallet>
    {
        Task<bool> AddNewOrder(int walletid, Order order);
        Task<bool> CreateWallet(Wallet wallet);
        Task<List<Order>> GetAllOrders(int userid);
        Task<Order> GetOrderDetails(int orderid);
        Task<Tuple<List<User>, PaginationMetadata>> GetAll_Work_pending_Orders(int page = 1, int pageSize = 10);

    }
   
}
