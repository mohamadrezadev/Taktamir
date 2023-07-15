using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    public static class Generatecode
    {
        public static string Genraete_Code()
        {
            var code = new Random((int)DateTime.Now.Ticks).Next(10000, 99999);
            return code.ToString();
        }
    }

