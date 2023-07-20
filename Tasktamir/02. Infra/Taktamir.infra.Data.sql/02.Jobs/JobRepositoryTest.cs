using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Taktamir.Core.Domain._01.Jobs;
using Taktamir.Core.Domain._4.Customers;
using Taktamir.infra.Data.sql._01.Common;
using Taktamir.infra.Data.sql._02.Jobs;
using Tasktamir.MockData;
using Tynamix.ObjectFiller;

namespace Tasktamir._02._Infra.Taktamir.infra.Data.sql._02.Jobs
{
    public class JobRepositoryTest:IClassFixture<TaktamirFixture>
    {

        private readonly JobMockData _JobMockData;
        private readonly TaktamirFixture fixture;
        public JobRepositoryTest(TaktamirFixture fixture)
        {
            _JobMockData = new JobMockData();
            this.fixture = fixture;

        }
        [Fact]
        public async void Return_List_jobs_for_admin()
        {
            //Arenge
            var dbContextMock = new Mock<AppDbContext>();
            var jobRepositoryMock = new Mock<JobRepository>(dbContextMock.Object);
            jobRepositoryMock.Setup(repo => repo.GetAllJobs()).ReturnsAsync(_JobMockData.GetJobs());


            //Act
            // jobRepositoryMock.Object.GetAllJobsByAdmin();
            var result = jobRepositoryMock.Object.GetAllJobsByAdmin();
            //Assert
            Assert.NotNull(result);
            Assert.IsType<List<Job>>(result);


        }
        [Fact]
        public  async void Add_New_Job_in_Database()
        {
            //Arenge
            var newjob =new Job {Id=1,Description="jbu" }; //new Filler<Job>().Create();
            var customer = new Filler<Customer>().Create();
            newjob.Customer = customer;
            JobRepository jobRepository = new JobRepository(fixture._DbContext);

            //Act
            await jobRepository.AddAsync(newjob,CancellationToken.None );
            //Assert
            var lastjob=fixture._DbContext.Jobs.FirstOrDefault(p=>p.Id==newjob.Id);
            Assert.NotNull(lastjob);

        }
    }
}
