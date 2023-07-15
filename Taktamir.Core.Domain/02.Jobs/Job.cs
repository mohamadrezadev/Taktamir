using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taktamir.Core.Domain._01.Common;
using Taktamir.Core.Domain._4.Customer.cs;

namespace Taktamir.Core.Domain._01.Jobs
{
    public class Job:IEntity
    {
        public int Id { get; set; }
        public string Name_Device { get; set; }
        public string Problems { get; set; }
        public virtual Customer Customer { get; set; }

        public StatusJob StatusJob { get; set; }
        public string Description { get; set; }
    }
}
