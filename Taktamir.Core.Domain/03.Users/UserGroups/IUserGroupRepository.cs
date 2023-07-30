using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taktamir.Core.Domain._01.Common;

namespace Taktamir.Core.Domain._03.Users.UserGroups
{
    public interface IUserGroupRepository:IRepository<UserGroup>
    {
        Task<List<UserGroup>> GetUserGroups(long userId);
        Task<List<string>> GetUserIds(long groupId);
        Task JoinGroup(long userId, long groupId);
        Task JoinGroup(List<long> userIds, long groupId);
        Task<bool> IsUserInGroup(long userId, long groupId);
        Task<bool> IsUserInGroup(long userId, string token);
    }
}
