using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using tms.Base.Essentials;
using tms.Base.VO;

namespace tms.Base.BO
{
    public class BOCountry
    {
        #region Attributes

        private static Connection connection;
        private static BaseWrapper extended;

        #endregion

        #region Properties

        #region Base Queries

        private static string QueryFields
        {
            get
            {
                StringBuilder query = new StringBuilder();

                query.Append("    vCountry.ID_Country, ");
                query.Append("    vCountry.Country ");

                return query.ToString();
            }
        }

        private static string QueryFrom
        {
            get
            {
                StringBuilder query = new StringBuilder();
                query.Append("FROM vCountry ");

                return query.ToString();
            }
        }

        private static string QueryJoins
        {
            get
            {
                StringBuilder query = new StringBuilder();

                return query.ToString();
            }
        }

        private static string BaseRecycle
        {
            get
            {
                StringBuilder query = new StringBuilder();

                query.Append("SELECT ");
                query.Append("    Country.ID_Country, ");
                query.Append("    Country.Country ");
                query.Append("FROM Country ");

                return query.ToString();
            }
        }

        #endregion

        #endregion

        #region Lists

        public static T GetObject<T>(DataTable dt, DataRow row, BaseWrapper wrapper)
        {
            VOCountry i = null;
            
            if (dt != null && row != null)
            {
                if (dt.Columns.Contains("ID_Country"))
                {
                    if (!string.IsNullOrEmpty(row["ID_Country"].ToString()))
                    {
                        i = new VOCountry();

                        i.ID = Tools.GetInt(row["ID_Country"].ToString());
                        i.Country = dt.Columns.Contains("Country") ? !string.IsNullOrEmpty(row["Country"].ToString()) ? row["Country"].ToString() : string.Empty : string.Empty;
                    }
                }
            }

            return Tools.ConvertExamp1<T>(i);
        }

        public static T GetObject<T>(string query, ref BaseWrapper wrapper)
        {
            VOCountry i = null;

            Connection connection = new Connection(wrapper.strConnection);
            DataTable dt = connection.GetItems(query, wrapper.strConnection);
            foreach (DataRow row in dt.Rows)
            {
                i = GetObject<VOCountry>(dt, row, wrapper);
            }

            wrapper.DataTable = dt; // Returning DataTable by Reference

            return Tools.BaseExtensionConverter<T>(i);
        }

        public static T GetObject<T>(string query, List<SqlParameter> parameters, ref BaseWrapper wrapper)
        {
            VOCountry i = null;

            Connection connection = new Connection(wrapper.strConnection);
            DataTable dt = connection.GetItems(query, parameters, wrapper.strConnection);
            foreach (DataRow row in dt.Rows)
            {
                i = GetObject<VOCountry>(dt, row, wrapper);
            }

            wrapper.DataTable = dt; // Returning DataTable by Reference

            return Tools.BaseExtensionConverter<T>(i);
        }

        public static List<T> GetList<T>(string query, ref BaseWrapper wrapper)
        {
            List<VOCountry> items = null;

            Connection connection = new Connection(wrapper.strConnection);
            DataTable dt = connection.GetItems(query, wrapper.strConnection);
            if (dt.Rows.Count > 0)
            {
                items = new List<VOCountry>();

                foreach (DataRow row in dt.Rows)
                {
                    items.Add(GetObject<VOCountry>(dt, row, wrapper));
                }
            }

            wrapper.DataTable = dt; // Returning DataTable by Reference

            return Tools.BaseExtensionConverter<List<T>>(items);
        }

        public static List<T> GetList<T>(string query, List<SqlParameter> parameters, ref BaseWrapper wrapper)
        {
            List<VOCountry> items = null;

            Connection connection = new Connection(wrapper.strConnection);
            DataTable dt = connection.GetItems(query, parameters, wrapper.strConnection);
            if (dt.Rows.Count > 0)
            {
                items = new List<VOCountry>();

                foreach (DataRow row in dt.Rows)
                {
                    items.Add(GetObject<VOCountry>(dt, row, wrapper));
                }
            }

            wrapper.DataTable = dt; // Returning DataTable by Reference

            return Tools.BaseExtensionConverter<List<T>>(items);
        }

        public static DataTable GetDataTable(string query, ref BaseWrapper wrapper)
        {
            Connection connection = new Connection(wrapper.strConnection);
            DataTable dt = connection.GetItems(query, wrapper.strConnection);
            return dt;
        }

        public static DataTable GetDataTable(string query, List<SqlParameter> parameters, ref BaseWrapper wrapper)
        {
            Connection connection = new Connection(wrapper.strConnection);
            DataTable dt = connection.GetItems(query, parameters, wrapper.strConnection);
            return dt;
        }

        #endregion

        #region Get

        public static DataTable Get(int id, BaseWrapper wrapper)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@ID_Country", id));

            StringBuilder query = new StringBuilder();
            query.Append("SELECT ");
            query.Append(QueryFields);
            query.Append(wrapper.ExtendedQueryFields);
            query.Append(QueryFrom);
            query.Append(QueryJoins);
            query.Append(wrapper.ExtendedQueryJoins);
            query.Append("WHERE ");
            query.Append("    vCountry.ID_Country = @ID_Country ");

            return GetDataTable(query.ToString(), parameters, ref wrapper);
        }

        public static DataTable Get(BaseWrapper wrapper)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT ");
            query.Append(QueryFields);
            query.Append(wrapper.ExtendedQueryFields);
            query.Append(QueryFrom);
            query.Append(QueryJoins);
            query.Append(wrapper.ExtendedQueryJoins);

            return GetDataTable(query.ToString(), ref wrapper);
        }

        public static T GetLast<T>(BaseWrapper wrapper)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT TOP 1 ");
            query.Append(QueryFields);
            query.Append(wrapper.ExtendedQueryFields);
            query.Append(QueryFrom);
            query.Append(QueryJoins);
            query.Append(wrapper.ExtendedQueryJoins);
            query.Append("ORDER BY ");
            query.Append("    vCountry.ID_Country DESC ");

            return GetObject<T>(query.ToString(), ref wrapper);
        }

        #endregion

        #region SQL

        public static bool Add(VOCountry i, List<SqlParameter> extendedParameters, BaseWrapper wrapper)
        {
            bool status = false;

            if (i != null)
            {
                try
                {
                    #region Parameters

                    List<SqlParameter> parameters = new List<SqlParameter>();
                    parameters.Add(new SqlParameter("@Country", !string.IsNullOrEmpty(i.Country) ? i.Country : string.Empty));

                    #region Extended

                    if (extendedParameters != null && extendedParameters.Count > 0 && parameters != null)
                        parameters.AddRange(extendedParameters);

                    #endregion

                    #endregion

                    #region Query

                    StringBuilder query = new StringBuilder();
                    query.Append("INSERT INTO Country ");
                    query.Append("( ");
                    query.Append("    Country ");

                    #region Extended

                    if (extendedParameters != null && extendedParameters.Count > 0)
                    {
                        foreach (SqlParameter p in extendedParameters)
                        {
                            query.Append(String.Format("    ,{0} ", p.ParameterName));
                        }
                    }

                    #endregion

                    query.Append(") ");
                    query.Append("VALUES ");
                    query.Append("( ");
                    query.Append("    @Country ");

                    #region Extended

                    if (extendedParameters != null && extendedParameters.Count > 0)
                    {
                        foreach (SqlParameter p in extendedParameters)
                        {
                            query.Append(String.Format("    ,@{0} ", p.ParameterName));
                        }
                    }

                    #endregion

                    query.Append(") ");

                    #endregion

                    Connection connection = new Connection(wrapper.strConnection);
                    connection.ExecuteQuery(query.ToString(), parameters, wrapper.strConnection);
                    status = true;
                }
                catch
                {
                }

            }

            return status;
        }

        public static VOCountry AddWithReturn(VOCountry i, List<SqlParameter> extendedParameters, BaseWrapper wrapper)
        {
            VOCountry item = null;

            if (i != null)
            {
                if (Add(i, extendedParameters, wrapper))
                {
                    item = GetLast<VOCountry>(wrapper);
                }
            }

            return item;
        }

        public static bool Update(VOCountry i, List<SqlParameter> extendedParameters, BaseWrapper wrapper)
        {
            bool status = false;

            if (i != null && i.ID != 0)
            {
                try
                {
                    #region Parameters

                    List<SqlParameter> parameters = new List<SqlParameter>();
                    parameters.Add(new SqlParameter("@ID", i.ID));
                    parameters.Add(new SqlParameter("@Country", !string.IsNullOrEmpty(i.Country) ? i.Country : string.Empty));

                    #region Extended

                    if (extendedParameters != null && extendedParameters.Count > 0 && parameters != null)
                        parameters.AddRange(extendedParameters);

                    #endregion

                    #endregion

                    #region Query

                    StringBuilder query = new StringBuilder();
                    query.Append("UPDATE Country SET ");
                    query.Append("    Country = @Country ");

                    #region Extended

                    if (extendedParameters != null && extendedParameters.Count > 0)
                    {
                        foreach (SqlParameter p in extendedParameters)
                        {
                            query.Append(String.Format("    , {0} = @{1} ", p.ParameterName, p.Value));
                        }
                    }

                    #endregion

                    query.Append("WHERE ");
                    query.Append("    ID_Country = @ID ");

                    #endregion

                    Connection connection = new Connection(wrapper.strConnection);
                    connection.ExecuteQuery(query.ToString(), parameters, wrapper.strConnection);
                    status = true;
                }
                catch
                {
                }
            }

            return status;
        }

        #endregion
    }
}
