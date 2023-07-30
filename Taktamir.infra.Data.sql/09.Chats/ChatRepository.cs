using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taktamir.Core.Domain._09.Chats;
using Taktamir.infra.Data.sql._01.Common;

namespace Taktamir.infra.Data.sql._09.Chats
{
    public class ChatRepository : Repository<Chat>, IChatRepository
    {
        public ChatRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public Task<ChatGroup> GetGroupBy(long id)
        {
            throw new NotImplementedException();
        }

        public Task<ChatGroup> GetGroupBy(string token)
        {
            throw new NotImplementedException();
        }

        public Task<List<ChatGroup>> GetUserGroups(long userId)
        {
            throw new NotImplementedException();
        }

        public Task<ChatGroup> InsertGroup(ChatGroup model)
        {
            throw new NotImplementedException();
        }

        public Task<ChatGroup> InsertPrivateGroup(long userId, long receiverId)
        {
            throw new NotImplementedException();
        }

        public Task<List<ChatGroup>> Search(string title, long userId)
        {
            throw new NotImplementedException();
        }
    }
}
