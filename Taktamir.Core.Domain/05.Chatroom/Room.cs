using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Taktamir.Core.Domain._01.Common;
using Taktamir.Core.Domain._03.Users;

namespace Taktamir.Core.Domain._05.Messages
{
    public class Room: IEntity
    {
        public Room()
        {
            
            this.Messages = new List<Message>();
          //  Admin = new User();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoomId { get; set; }
        public string NameRoom { get; set; }
     
        public virtual ICollection<string> UsersId { get; set; }=new List<string>();
        public virtual ICollection<Message> Messages { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }

}
