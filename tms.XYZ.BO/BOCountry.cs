using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tms.Base.BO;
using tms.XYZ.VO;

namespace tms.XYZ.BO
{
    public class BOCountry : tms.Base.BO.BOCountry
    {
        #region Constructors

        public BOCountry()
            : base()
        {
            InstantiateWrapper();   
        }

        #endregion

        #region Properties

        #region Extensions

        public static BaseWrapper Wrapper { get; set; }

        private static string ExtQueryFields
        {
            get
            {
                StringBuilder query = new StringBuilder();
                query.Append("    ,vCountry.CountryCode ");

                return query.ToString();
            }
        }

        private static string ExtQueryJoins
        {
            get
            {
                StringBuilder query = new StringBuilder();

                return query.ToString();
            }
        }

        private static string ExtBaseRecycle
        {
            get
            {
                StringBuilder query = new StringBuilder();
                query.Append(" ");

                return query.ToString();
            }
        }

        private static string ExtQueryFrom
        {
            get
            {
                StringBuilder query = new StringBuilder();
                query.Append(" ");

                return query.ToString();
            }
        }        

        #endregion

        #endregion

        #region Wrapper

        public static void InstantiateWrapper()
        {
            if (Wrapper == null)
            {
                Wrapper = new BaseWrapper();

                Wrapper.ExtendedQueryFields = ExtQueryFields;
                Wrapper.ExtendedQueryJoins = ExtQueryJoins;
                Wrapper.ExtendedBaseRecycle = ExtBaseRecycle;
                Wrapper.ExtendedQueryFrom = ExtQueryFrom;

                Wrapper.ExtendedParameters = null;
                Wrapper.strConnection = "server=localhost;User ID=sa;Password=$y$t3m4dm1n1$tr4t0r;Initial Catalog=XYZ_Content;Max Pool Size=1000;Pooling=true;";
            }
        }

        #endregion

        #region Lists

        public static VOCountry GetObject(DataTable dt, DataRow row)
        {
            InstantiateWrapper();

            VOCountry i = GetObject<VOCountry>(dt, row, Wrapper);

            #region Extended

            if (i != null && i.ID != 0)
            {
                i.CountryCode = dt.Columns.Contains("CountryCode") ? !string.IsNullOrEmpty(row["CountryCode"].ToString()) ? row["CountryCode"].ToString() : string.Empty : string.Empty;
            }

            #endregion

            return i;
        }

        public static VOCountry GetObject(DataTable dt)
        {
            InstantiateWrapper();

            VOCountry i = null;

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    i = GetObject(dt, row);
                }
            }

            return i;
        }

        public static List<VOCountry> GetList(DataTable dt)
        {
            InstantiateWrapper();

            List<VOCountry> items = null;

            if (dt.Rows.Count > 0)
            {
                items = new List<VOCountry>();
                foreach (DataRow row in dt.Rows)
                {
                    items.Add(GetObject(dt, row));
                }
            }

            return items;
        }

        #endregion

        #region Get

        public static VOCountry Get(int id)
        {
            InstantiateWrapper();
            return GetObject(Get(id, Wrapper));
        }

        public static List<VOCountry> Get()
        {
            InstantiateWrapper();
            return GetList(Get(Wrapper));
        }

        public static VOCountry GetLast()
        {
            InstantiateWrapper();
            return GetLast<VOCountry>(Wrapper);
        }

        #endregion

        #region SQL

        public static bool Add(VOCountry i)
        {
            bool status = false;

            if (i != null)
            {
                InstantiateWrapper();

                #region Extended Parameters

                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("CountryCode", i.CountryCode));

                #endregion

                status = Add(i, parameters, Wrapper);
            }

            return status;
        }

        public static VOCountry AddWithReturn(VOCountry i)
        {
            VOCountry item = null;

            if (i != null)
            {
                InstantiateWrapper();

                #region Extended Parameters

                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("CountryCode", i.CountryCode));

                #endregion



                var teste = tms.Base.Essentials.Tools.BaseExtensionConverter<tms.Base.VO.VOCountry>(i);

               //item = tms.Base.Essentials.Tools.BaseExtensionConverter<VOCountry>(AddWithReturn(tms.Base.Essentials.Tools.BaseExtensionConverter<tms.Base.VO.VOCountry>(i), parameters, Wrapper));
            }

            return item;
        }

        public static bool Update(VOCountry i)
        {
            bool status = false;

            if (i != null && i.ID != 0)
            {
                InstantiateWrapper();

                #region Extended Parameters

                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("CountryCode", i.CountryCode));

                #endregion

                var teste = tms.Base.Essentials.Tools.BaseExtensionConverter<VOCountry>(i);
                //status = Update(tms.Base.Essentials.Tools.BaseExtensionConverter<tms.Base.VO.VOCountry>(i), parameters, Wrapper);
            }

            return status;
        }

        #endregion
    }
}
