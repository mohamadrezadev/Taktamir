using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Taktamir.Core.Domain._01.Common;
using Taktamir.Core.Domain._06.Wallets;
using Taktamir.framework.Common;

namespace Taktamir.Core.Domain._03.Users
{
    public interface IUserRepository : IRepository<User>
    {
        public Task<User> Finduserbyphonenumber(string phonenumberId, CancellationToken cancellationToken);
        public Task<User> CreateAsync(User user, CancellationToken cancellationToken);
        Task<Tuple<List<User>, PaginationMetadata>> GetAllUsersAsync(int page = 1, int pageSize = 10);
        Task<Tuple<List<User>, PaginationMetadata>> Unverified_users(int page = 1, int pageSize = 10);
        Task<Tuple<List<User>, PaginationMetadata>> Verified_user_account(int page = 1, int pageSize = 10);
        Task<Tuple<Wallet, PaginationMetadata>> JobsUser(int walletId, int page = 1, int pageSize = 10);
        /// <summary>
        /// فعال کردن حساب تکنسین 
        /// </summary>
        /// <param name="TechnicianId"></param>
        /// <returns></returns>
        Task<bool> TechnicianAccountVerification(int TechnicianId);
    }
}
