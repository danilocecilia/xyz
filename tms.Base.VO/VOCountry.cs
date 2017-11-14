using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tms.Base.VO
{
    public class VOCountry
    {
        public int ID { get; set; }
        public string Country { get; set; }

        public VOCountry()
        {

        }

        public VOCountry(int id, string country)
        {
            this.ID = id;
            this.Country = country;
        }
    }
}
