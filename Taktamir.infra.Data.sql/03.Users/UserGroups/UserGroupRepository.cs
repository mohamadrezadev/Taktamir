using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taktamir.Core.Domain._03.Users;
using Taktamir.Core.Domain._03.Users.UserGroups;
using Taktamir.infra.Data.sql._01.Common;

namespace Taktamir.infra.Data.sql._03.Users.UserGroups
{
    public class UserGroupRepository : Repository<UserGroup>, IUserGroupRepository
    {
        public UserGroupRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public Task<List<UserGroup>> GetUserGroups(long userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> GetUserIds(long groupId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsUserInGroup(long userId, long groupId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsUserInGroup(long userId, string token)
        {
            throw new NotImplementedException();
        }

        public Task JoinGroup(long userId, long groupId)
        {
            throw new NotImplementedException();
        }

        public Task JoinGroup(List<long> userIds, long groupId)
        {
            throw new NotImplementedException();
        }
    }
}
