using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taktamir.Core.Domain._09.Chats;
using Taktamir.infra.Data.sql._01.Common;

namespace Taktamir.infra.Data.sql._09.Chats
{
    public class ChatGroupRespository : Repository<ChatGroup>, IChatGroupRespository
    {
        public ChatGroupRespository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
