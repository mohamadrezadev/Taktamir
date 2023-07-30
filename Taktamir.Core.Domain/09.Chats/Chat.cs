using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Taktamir.Core.Domain._03.Users;
using Taktamir.Core.Domain._01.Common;

namespace Taktamir.Core.Domain._09.Chats
{
    public class Chat:IEntity
    {
        public int Id { get; set; }

        public string ChatBody { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }


        #region Relations
        [ForeignKey("UserId")]
        public User User { get; set; }
        [ForeignKey("GroupId")]
        public ChatGroup CharGroup { get; set; }


        #endregion
    }}
