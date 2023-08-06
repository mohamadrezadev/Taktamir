using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Taktamir.framework.Common
{
    public static class AccountUtills
    {
        public static string SetConfermetionAccount(bool status)
        {
            switch (status)
            {
                case true:
                    return "Account is Verified";
                case false:
                    return "Account is not Verified";
            } 
        }
    }
}
