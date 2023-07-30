using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taktamir.Core.Domain._03.Users;
using Taktamir.Core.Domain._01.Common;

namespace Taktamir.Core.Domain._09.Chats
{
    public class ChatGroup:IEntity
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string GroupTitle { get; set; }
        [MaxLength(110)]
        public string GroupToken { get; set; }
        [MaxLength(110)]
        public string ImageName { get; set; }
        public int OwnerId { get; set; }
        public int? ReceiverId { get; set; }
        public bool IsPrivate { get; set; }


        #region MyRegion
        [ForeignKey("OwnerId")]
        public User User { get; set; }
        [ForeignKey("ReceiverId")]
        public User Receiver { get; set; }
        public ICollection<Chat> Chats { get; set; }
        public ICollection<UserGroup> UserGroups { get; set; }

        #endregion
    }
    
}
