using Taktamir.Core.Domain._05.Messages;
using Taktamir.infra.Data.sql._01.Common;

namespace Taktamir.infra.Data.sql._05.Messages
{
    public class RoomRepository : Repository<Room>, IRoomRepository
    {
        public RoomRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
