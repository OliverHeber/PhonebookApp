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

        // Object cannot be initialized without a connection string - added security
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
                    // Adapter gets filled with whatever is returned from the query
                    adapter.Fill(dataSet);
                }
            }
            catch (Exception ex)
            {
                // If exceptions occur, throw the exception back to whoever called the function (doesn't throw entire stack trace)
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
                    // Use data adapter as offline access to SQL server
                    SqlDataAdapter dataAdapter = new SqlDataAdapter();
                    SqlCommand command = new SqlCommand(procedureName, sqlConnection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    // Connect command and adapter as adapter is currently blank (doesn't know what to fire)
                    dataAdapter.SelectCommand = command;
                    dataAdapter.Fill(dataSet);
                }
            }
            catch (Exception)
            {
                // Throws the entire stack trace 
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

