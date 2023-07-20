using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taktamir.Core.Domain._01.Jobs;
using Tynamix.ObjectFiller;

namespace Tasktamir.MockData
{
    internal class JobMockData
    {
        List<Job> Jobs=new List<Job>();
        public List<Job> GetJobs() 
        {
            Jobs.AddRange(new Filler<Job>().Create(20));
            return Jobs;
        }
    }
}
