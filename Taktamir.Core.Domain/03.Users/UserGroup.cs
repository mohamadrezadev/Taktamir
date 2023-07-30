using System.ComponentModel.DataAnnotations.Schema;
using Taktamir.Core.Domain._01.Common;
using Taktamir.Core.Domain._09.Chats;

namespace Taktamir.Core.Domain._03.Users
{
    public class UserGroup:IEntity
    {

        public int Id { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }

        #region Relations
        [ForeignKey("UserId")]
        public User User { get; set; }
        [ForeignKey("GroupId")]
        public ChatGroup ChatGrop { get; set; }

        #endregion
    }
}
