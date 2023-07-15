using Taktamir.Core.Domain._01.Common;
using Taktamir.Core.Domain._03.Users;

namespace Taktamir.Core.Domain._05.Messages
{
    public class Room: IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public User Admin { get; set; }
        public ICollection<Message> Messages { get; set; }
    }

}
