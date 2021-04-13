using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace DataAccessLayer
{
    public class SQLDbManager : IDB
    {
        private string ConnectionString;
        public SQLDbManager(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public DataSet ExecuteQuery(string query)
        {
            DataSet dataSet = new DataSet();

            try
            {
                using (SqlConnection sqlConnection =
                    new SqlConnection(ConnectionString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(query, sqlConnection);
                    adapter.Fill(dataSet);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dataSet;
        }

        // Stored proc with no params
        public DataSet ExecuteStoredProcedure(string procedureName)
        {
            DataSet dataSet = new DataSet();

            try
            {
                using SqlConnection sqlConnection =
                   new SqlConnection(ConnectionString);
                {
                    SqlDataAdapter dataAdapter = new SqlDataAdapter();
                    SqlCommand command = new SqlCommand(procedureName, sqlConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    dataAdapter.SelectCommand = command;
                    dataAdapter.Fill(dataSet);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return dataSet;
        }

        // Stored proc with params
        public DataSet ExecuteStoredProcedure<T>(string procedureName, List<T> parameters) where T : DbParameter
        {
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
            DataSet dataSet = new DataSet();

            using SqlConnection sqlConnection =
                new SqlConnection(ConnectionString);
            {
                try
                {
                    SqlCommand command = new SqlCommand(procedureName, sqlConnection);

                    foreach (T param in parameters)
                    {
                        command.Parameters.Add(param);
                    }
                    command.CommandType = CommandType.StoredProcedure;
                    sqlDataAdapter.SelectCommand = command;
                    sqlDataAdapter.Fill(dataSet);
                }
                catch (Exception)
                {
                    throw;
                }
                return dataSet;
            }
        }
    }
}

