using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taktamir.Core.Domain._01.Common;
using Taktamir.Core.Domain._08.Verifycodes;
using Taktamir.infra.Data.sql._01.Common;

namespace Taktamir.infra.Data.sql._08.Verifycodes
{

    public class VerifycodeRepository : Repository<Verifycode>, IVerifycodeRepository
    {
        public VerifycodeRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public Task add_or_update_verifycode(Verifycode verifycode)
        {
            var isexist=DbContext.Verifycodes.Any(p=>p.phone_number==verifycode.phone_number);
            if (isexist)
            {
                var updateverifycode = DbContext.Verifycodes.FirstOrDefault(p => p.phone_number == verifycode.phone_number);
                updateverifycode.code = verifycode.code;
                verifycode.DateTimeSendeCode = verifycode.DateTimeSendeCode;
                DbContext.SaveChanges();
                return Task.CompletedTask;
            }
            DbContext.Verifycodes.Add(verifycode);
            DbContext.SaveChanges();
            return Task.CompletedTask;
        }

      
        public async Task<Tuple<bool,int>> Isvalidcode(string phone_number, string code)
        {
            var IsSende = await DbContext.Verifycodes
                    .SingleOrDefaultAsync(p => p.phone_number == phone_number && p.code.Equals(code));
           // var IsSende =await DbContext.Verifycodes.Where(p=>p.phone_number==phone_number&&p.code.Equals(code)).SingleAsync();
            if (IsSende!=null) return Tuple.Create(true,IsSende.Id);
            return Tuple.Create(false,0);
        }
    }

}
