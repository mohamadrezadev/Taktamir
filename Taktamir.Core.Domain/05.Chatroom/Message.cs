 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taktamir.Core.Domain._01.Common;
using Taktamir.Core.Domain._03.Users;

namespace Taktamir.Core.Domain._05.Messages
{
    public class Message: IEntity
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public string Reciver { get; set; }
        public string  Text { get; set; }
        public DateTime Timestamp { get; set; }

        public int RoomId { get; set; }
        public Room Room { get; set; }

        
    }

}
