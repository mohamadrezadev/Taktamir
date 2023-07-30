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
        public string username { get; set; }
        public string  Text { get; set; }
        public DateTime Timestamp { get; set; }

        public int UserId { get; set; }
        public virtual User Sender { get; set; }
    }

}
