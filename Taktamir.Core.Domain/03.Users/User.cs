
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taktamir.Core.Domain._01.Common;
using Taktamir.Core.Domain._01.Jobs;
using Taktamir.Core.Domain._05.Messages;
using Taktamir.Core.Domain._06.Wallets;
using Taktamir.Core.Domain._09.Chats;

namespace Taktamir.Core.Domain._03.Users
{
    public class User : IdentityUser<int>, IEntity
    {
        public User()
        {
            IsActive = false;
            IsLocked = false;
            IsCompleteprofile = false;
            this.Wallet=new Wallet();
            IsConfirmedAccount = false;
            Messages=new HashSet<Message>();
            Chats=new HashSet<Chat>();
            UserGroups = new HashSet<UserGroup>();
            
            
            
        }

        public string Firstname { get; set; }

        public string LastName { get; set; }

        public string Profile_url { get; set; }

        public DateTime Create_at { get; set; }

        public DateTime Update_at { get; set; }

        public bool IsActive { get; set; }

        public bool IsLocked { get; set; }

        public string SerialNumber { get; set; }

        public bool IsCompleteprofile { get; set; }

        public string Access_Token { get; set; }

        public string? RefreshToken { get; set; }

        public bool IsConfirmedAccount { get; private set; }



        public DateTime RefreshTokenExpiryTime { get; set; }
        public virtual ICollection<Specialty> Specialties { get; set; }
      

        [ForeignKey("walletId")]
        public int walletId { get; set; }
        public virtual Wallet Wallet { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Chat> Chats { get; set; }
        public virtual ICollection<UserGroup> UserGroups { get; set; }

        public void confirme(bool confirme)
        {
            this.IsConfirmedAccount = confirme;
        }


    }
    public enum Roleuser
    {
        Admin=1,
        Technician=2,
        Employee=3
    }
    public static class UserRoleApp 
    {
       public const string Admin = "Admin";
       public const string Technician = "Technician";
       public const string Employee = "Employee";
    }
}
