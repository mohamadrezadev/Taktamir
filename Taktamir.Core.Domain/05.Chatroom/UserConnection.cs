﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taktamir.Core.Domain._01.Common;
using Taktamir.Core.Domain._03.Users;

namespace Taktamir.Core.Domain._05.Messages
{
    public class UserConnection:IEntity
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string Nameroom { get; set; }
      
    } 
   
}
