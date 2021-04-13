using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace DataAccessLayer
{
    public class MySqlDbManager : IDB
    {
        string ConnectionString;

        public MySqlDbManager(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public DataSet ExecuteQuery(string query)
        {
            DataSet dataSet = new DataSet();

            try
            {
                using (MySqlConnection sqlConnection =
                    new MySqlConnection(ConnectionString))
                {
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, sqlConnection);
                    adapter.Fill(dataSet);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dataSet;
        }

        public DataSet ExecuteStoredProcedure(string procedureName)
        {
            DataSet dataSet = new DataSet();

            try
            {
                using MySqlConnection sqlConnection =
                   new MySqlConnection(ConnectionString);
                {
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter();
                    MySqlCommand command = new MySqlCommand(procedureName, sqlConnection)
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

        public DataSet ExecuteStoredProcedure<T>(string procedureName, List<T> parameters) where T : DbParameter
        {
            MySqlDataAdapter sqlDataAdapter = new MySqlDataAdapter();
            DataSet dataSet = new DataSet();

            using MySqlConnection sqlConnection =
                new MySqlConnection(ConnectionString);
            {
                try
                {
                    MySqlCommand command = new MySqlCommand(procedureName, sqlConnection);

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
