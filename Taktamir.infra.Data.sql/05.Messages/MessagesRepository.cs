using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taktamir.Core.Domain._05.Messages;
using Taktamir.infra.Data.sql._01.Common;

namespace Taktamir.infra.Data.sql._05.Messages
{
    public class MessagesRepository : Repository<Message>, IMessagesRepository
    {
        public MessagesRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
