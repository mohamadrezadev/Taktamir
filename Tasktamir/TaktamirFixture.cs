using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taktamir.Core.Domain._01.Jobs;
using Taktamir.infra.Data.sql._01.Common;
using Taktamir.infra.Data.sql._02.Jobs;

namespace Tasktamir
{
    public class TaktamirFixture
    {
        public  AppDbContext _DbContext { get; }
        public  IJobRepository _jobRepository { get; }
        public TaktamirFixture()
        {
            DbContextOptionsBuilder<AppDbContext> builder = new DbContextOptionsBuilder<AppDbContext>();
            builder.UseInMemoryDatabase("testDatabase");
           // _DbContext=new AppDbContext(builder.Options);
            _jobRepository = new JobRepository(_DbContext);
        }
    }
}
