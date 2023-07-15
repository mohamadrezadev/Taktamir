using Taktamir.Core.Domain._01.Common;

namespace Taktamir.Core.Domain._03.Users
{
    public interface IUserRepository : IRepository<User>
    {
        public Task<User> Finduserbyphonenumber(string phonenumberId, CancellationToken cancellationToken);
        public Task<User> CreateAsync(User user, CancellationToken cancellationToken);
        public Task<User> UpdateUser(User user, CancellationToken cancellationToken);
    }
}
