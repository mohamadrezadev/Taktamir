using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taktamir.Core.Domain._03.Users;
using Taktamir.infra.Data.sql._01.Common;

namespace Taktamir.infra.Data.sql._03.Users
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public Task<User> CreateAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<User> Finduserbyphonenumber(string phonenumber, CancellationToken cancellationToken)
        {
           var result=await DbContext.Users.FirstOrDefaultAsync(p => p.PhoneNumber.Equals(phonenumber));
            if (result==null)return null;
            return result;

        }
        public Task<bool> TechnicianAccountVerification(int TechnicianId)
        {
            var user = DbContext.Users.FirstOrDefault(p => p.Id == TechnicianId);
            if (user == null) throw new Exception("Not Found Technician Account ....!");
            user.IsActive = true;
            DbContext.Update(user);
            DbContext.SaveChanges();
            return Task.FromResult(true);
        }
        
    }
}
