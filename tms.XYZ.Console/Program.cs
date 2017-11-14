using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tms.XYZ.BO;
using tms.XYZ.VO;

namespace tms.XYZ.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            VOCountry i = new VOCountry();

            i.Country = "SPAIN";
            i.CountryCode = "ES";

            i = BOCountry.AddWithReturn(i);

            List<VOCountry> countries = BOCountry.Get();
        }
    }
}
