using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tms.Base.Essentials
{
    public class Connection
    {
        #region Attributes

        private string _StrConnection;
        private SqlConnection connection;

        #endregion

        #region Constructor

        public Connection(string strConnection)
        {
            _StrConnection = strConnection;
        }

        #endregion

        #region Methods

        public decimal ExecuteIdenty(string SQL, List<SqlParameter> oParameter)
        {
            SqlConnection connection = new SqlConnection(_StrConnection);

            if (connection.State != ConnectionState.Open)
                connection.Open();

            SqlCommand objCommand = new SqlCommand(SQL, connection);
            objCommand.CommandType = CommandType.Text;

            foreach (SqlParameter oP in oParameter)
                objCommand.Parameters.Add(oP);

            decimal retorno = (decimal)objCommand.ExecuteScalar();

            connection.Close();
            connection.Dispose();

            return retorno;
        }

        public decimal ExecuteIdenty(string SQL, List<SqlParameter> oParameter, string strConnection)
        {
            SqlConnection connection = new SqlConnection(strConnection);

            if (connection.State != ConnectionState.Open)
                connection.Open();

            SqlCommand objCommand = new SqlCommand(SQL, connection);
            objCommand.CommandType = CommandType.Text;

            foreach (SqlParameter oP in oParameter)
                objCommand.Parameters.Add(oP);

            decimal retorno = (decimal)objCommand.ExecuteScalar();

            connection.Close();
            connection.Dispose();

            return retorno;
        }

        /// <summary>
        /// Método que executa uma query informada na base de dados. Usada para Insert, Delete e Update
        /// </summary>
        /// <param name="SQL"></param>
        public void ExecuteQuery(string SQL)
        {
            // Create the connection and transaction objects.
            SqlConnection connection = new SqlConnection(_StrConnection);
            SqlTransaction transaction;

            if (connection.State != ConnectionState.Open)
                connection.Open();

            using (transaction = connection.BeginTransaction())
            {
                try
                {
                    // Do some stuff here...
                    SqlCommand objCommand = new SqlCommand(SQL, connection);
                    objCommand.CommandType = CommandType.Text;
                    objCommand.Transaction = transaction;
                    objCommand.ExecuteNonQuery();

                    // Commit the transaction.
                    transaction.Commit();
                    transaction.Dispose();
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Dispose();
                }
            }

            connection.Close();
            connection.Dispose();
        }

        public void ExecuteQuery(string SQL, string strConnection)
        {
            // Create the connection and transaction objects.
            SqlConnection connection = new SqlConnection(strConnection);
            SqlTransaction transaction;

            if (connection.State != ConnectionState.Open)
                connection.Open();

            using (transaction = connection.BeginTransaction())
            {
                try
                {
                    // Do some stuff here...
                    SqlCommand objCommand = new SqlCommand(SQL, connection);
                    objCommand.CommandType = CommandType.Text;
                    objCommand.Transaction = transaction;
                    objCommand.ExecuteNonQuery();

                    // Commit the transaction.
                    transaction.Commit();
                    transaction.Dispose();
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Dispose();
                }
            }

            connection.Close();
            connection.Dispose();
        }

        public void ExecuteQuery(string SQL, List<SqlParameter> oParameter)
        {
            // Create the connection and transaction objects.
            SqlConnection connection = new SqlConnection(_StrConnection);
            SqlTransaction transaction;

            if (connection.State != ConnectionState.Open)
                connection.Open();


            using (transaction = connection.BeginTransaction())
            {
                try
                {
                    // Do some stuff here...
                    SqlCommand objCommand = new SqlCommand(SQL, connection);
                    objCommand.CommandType = CommandType.Text;
                    objCommand.Transaction = transaction;

                    foreach (SqlParameter oP in oParameter)
                        objCommand.Parameters.Add(oP);

                    objCommand.ExecuteNonQuery();

                    // Commit the transaction.
                    transaction.Commit();
                    transaction.Dispose();
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Dispose();
                }
            }

            connection.Close();
            connection.Dispose();
        }

        public void ExecuteQuery(string SQL, List<SqlParameter> oParameter, string strConnection)
        {
            // Create the connection and transaction objects.
            SqlConnection connection = new SqlConnection(strConnection);
            SqlTransaction transaction;

            if (connection.State != ConnectionState.Open)
                connection.Open();


            using (transaction = connection.BeginTransaction())
            {
                try
                {
                    // Do some stuff here...
                    SqlCommand objCommand = new SqlCommand(SQL, connection);
                    objCommand.CommandType = CommandType.Text;
                    objCommand.Transaction = transaction;

                    foreach (SqlParameter oP in oParameter)
                        objCommand.Parameters.Add(oP);

                    objCommand.ExecuteNonQuery();

                    // Commit the transaction.
                    transaction.Commit();
                    transaction.Dispose();
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Dispose();
                }
            }

            connection.Close();
            connection.Dispose();
        }

        /// <summary>
        /// Retorna SqlDataReader de uma query informada
        /// </summary>
        /// <param name="SQL">query a ser consultada na base de dados</param>
        /// <returns></returns>
        public DataTable GetItems(string SQL)
        {
            SqlDataReader retorno = null;

            SqlConnection connection = new SqlConnection(_StrConnection);
            SqlCommand command = new SqlCommand(SQL, connection);
            command.CommandTimeout = 600;
            connection.Open();
            retorno = command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(retorno);

            retorno.Close();
            retorno.Dispose();
            command.Dispose();
            connection.Close();
            connection.Dispose();

            return dt;
        }

        public DataTable GetItems(string SQL, string strConnection)
        {
            SqlDataReader retorno = null;

            SqlConnection connection = new SqlConnection(strConnection);
            SqlCommand command = new SqlCommand(SQL, connection);
            command.CommandTimeout = 600;
            connection.Open();
            retorno = command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(retorno);

            retorno.Close();
            retorno.Dispose();
            command.Dispose();
            connection.Close();
            connection.Dispose();

            return dt;
        }

        public DataTable GetItems(string SQL, List<SqlParameter> oParameter)
        {
            DataTable dt = null;

            try
            {
                SqlDataReader retorno = null;

                SqlConnection connection = new SqlConnection(_StrConnection);
                SqlCommand command = new SqlCommand(SQL, connection);
                command.CommandType = CommandType.Text;
                //command.CommandTimeout = 600;

                foreach (SqlParameter oP in oParameter)
                    command.Parameters.Add(oP);

                connection.Open();
                retorno = command.ExecuteReader();

                dt = new DataTable();
                dt.Load(retorno);

                retorno.Close();
                retorno.Dispose();
                command.Dispose();
                connection.Close();
                connection.Dispose();
            }
            catch
            {

            }

            return dt;
        }

        public DataTable GetItems(string SQL, List<SqlParameter> oParameter, string strConnection)
        {
            DataTable dt = null;

            try
            {
                SqlDataReader retorno = null;

                SqlConnection connection = new SqlConnection(strConnection);
                SqlCommand command = new SqlCommand(SQL, connection);
                command.CommandType = CommandType.Text;
                //command.CommandTimeout = 600;

                foreach (SqlParameter oP in oParameter)
                    command.Parameters.Add(oP);

                connection.Open();
                retorno = command.ExecuteReader();

                dt = new DataTable();
                dt.Load(retorno);

                retorno.Close();
                retorno.Dispose();
                command.Dispose();
                connection.Close();
                connection.Dispose();
            }
            catch
            {

            }

            return dt;
        }

        /// <summary>
        /// Retorna SqlDataReader de uma query informada
        /// </summary>
        /// <param name="proc"></param>
        /// <returns></returns>
        public DataTable GetItemsProc(string proc)
        {
            SqlDataReader retorno = null;

            SqlConnection connection = new SqlConnection(_StrConnection);
            SqlCommand command = new SqlCommand(proc, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 600;
            connection.Open();
            retorno = command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(retorno);

            retorno.Close();
            retorno.Dispose();
            command.Dispose();
            connection.Close();
            connection.Dispose();

            return dt;
        }

        public DataTable GetItemsProc(string proc, string strConnection)
        {
            SqlDataReader retorno = null;

            SqlConnection connection = new SqlConnection(strConnection);
            SqlCommand command = new SqlCommand(proc, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 600;
            connection.Open();
            retorno = command.ExecuteReader();

            DataTable dt = new DataTable();
            dt.Load(retorno);

            retorno.Close();
            retorno.Dispose();
            command.Dispose();
            connection.Close();
            connection.Dispose();

            return dt;
        }

        /// <summary>
        /// Método que fecha a conexão se estiver aberta.
        /// </summary>
        public void CloseConection()
        {

        }

        #endregion
    }
}
