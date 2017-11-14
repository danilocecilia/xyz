using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tms.Base.BO
{
    public class BaseWrapper
    {
        #region Extended

        public string ExtendedQueryFields { get; set; }
        public string ExtendedQueryFrom { get; set; }
        public string ExtendedQueryJoins { get; set; }
        public string ExtendedBaseRecycle { get; set; }

        public List<SqlParameter> ExtendedParameters { get; set; }

        public string strConnection { get; set; }

        public object ExtClass { get; set; }
        
        public DataTable DataTable { get; set; }

        #endregion

        #region Constructor

        public BaseWrapper()
        {
            
        }

        #endregion
    }
}
