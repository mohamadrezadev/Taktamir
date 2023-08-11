
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Taktamir.Core.Domain._01.Common;

namespace Taktamir.Core.Domain._03.Users
{
    public class Role : IdentityRole<int>, IEntity
    {
        [MaxLength(255)]
        public string   namerol { get; set; }
    }
}
