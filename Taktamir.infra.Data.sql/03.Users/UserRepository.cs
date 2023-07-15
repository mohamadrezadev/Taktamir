using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taktamir.Core.Domain._03.Users;
using Taktamir.infra.Data.sql._01.Common;

namespace Taktamir.infra.Data.sql._03.Users
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public Task<User> CreateAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<User> Finduserbyphonenumber(string phonenumberId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<User> UpdateUser(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
