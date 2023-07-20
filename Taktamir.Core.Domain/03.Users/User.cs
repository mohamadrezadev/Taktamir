
using Microsoft.AspNetCore.Identity;
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
            this.Specialties = new HashSet<string>();
            
        }

        [Required]
        public string Firstname { get; set; }

        [Required]
        public string LastName { get; set; }

        public string Profile_url { get; set; }

        public DateTime Create_at { get; set; }

        public DateTime Update_at { get; set; }

        public bool IsActive { get; set; }

        public bool IsLocked { get; set; }

        public string SerialNumber { get; set; }

        public bool IsCompleteprofile { get; set; }

        public string Refresh_Token { get; set; }

        public string Access_Token { get; set; }

        public virtual ICollection<string> Specialties { get; set; }
        public virtual Wallet Wallet { get; set; }

        public ICollection<Message> Messages { get; set; }

    }
    public enum UserRole 
    {
        Admin = 0,
        Technician= 1,
        Employee= 2,
    }
}
