using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taktamir.Core.Domain._01.Common;
using Taktamir.Core.Domain._01.Jobs;
using System.Diagnostics.CodeAnalysis;

namespace Taktamir.Core.Domain._07.Suppliess
{
    public class Supplies:IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }

        public int jobid { get; set; }
        [AllowNull]
        public Job Job { get; set; }

    }
}
