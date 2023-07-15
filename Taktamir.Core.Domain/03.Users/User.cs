
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

namespace Taktamir.Core.Domain._03.Users
{
    public class User : IdentityUser, IEntity
    {
        public User()
        {
            IsActive = false;
            IsLocked = false;
            IsCompleteprofile = false;
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

        public string JobId { get; set; }

        [ForeignKey("JobId")]
        public virtual Job Job { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }

        public virtual ICollection<Job> Jobs { get; set; }

        
    }
    public enum UserRole 
    {
        Admin = 0,
        Technician= 1,
        Employee= 2,
    }
}
