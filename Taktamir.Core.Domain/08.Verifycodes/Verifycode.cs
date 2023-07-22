using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taktamir.Core.Domain._01.Common;

namespace Taktamir.Core.Domain._08.Verifycodes
{
    public class Verifycode : IEntity<int>
    {
        public Verifycode()
        {
            this.DateTimeSendeCode = DateTime.Now;
            this.IsUsed = false;
        }
        public int Id { get; set; }
        public string code { get; set; }
        public string phone_number { get; set; }
        public DateTime DateTimeSendeCode { get; set; }
        public bool IsUsed { get; private set; }
        public void setIsUsed(bool isUsed)
        {
            this.IsUsed = isUsed;
        }
    }
}
