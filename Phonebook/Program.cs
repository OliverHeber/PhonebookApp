using DataAccessLayer;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;


namespace Phonebook
{
    class Program
    {
        // Enums
        public enum DatabaseServer
        {
            SqlServer = 1,
            MySql = 2,
            Exit = 3
        }
        public enum Options
        {
            Create = 1,
            Read = 2,
            Update = 3,
            Delete = 4,
            Exit = 5
        }
        public enum QueryOptions
        {
            ReturnAll = 1,
            ReturnOne = 2
        }

        // I have my connection strings hard coded right now, but they should be ideally taken from a cfg file when users want to plug their own connection strings in
        static string SqlConnectionString = "Data Source = localhost; Initial Catalog = PhonebookDB; Integrated Security = True";
        static string mySqlConnectionString = "server=localhost;user id=root;database=phonebook;pwd=root;";

        // Implemented delegates here to understand how delegates work in C# so decided to implement them
        public delegate List<SqlParameter> UpdateSqlParamatersDelegate(int id, string name, string email, string contact);
        public delegate List<MySqlParameter> UpdateMySqlParamatersDelegate(int id, string name, string email, string contact);

        static void Main(string[] args)
        {
            IDB dbManager = null;

            // Two local functions to create a list of mySQL/SQL parameters (can only be accessed using delegates) which will be plugged into functions using delegates
            // Can be avoided by placing functions outside of void main but wanted to learn how delegates work so used this approach
            List<SqlParameter> AddSQLUpdateParamaters(int id, string name, string email, string contact)
            {
                List<SqlParameter> list = new List<SqlParameter>
                                {
                                    new SqlParameter("@Id", id),
                                    new SqlParameter("@Name", name),
                                    new SqlParameter("@Email", email),
                                    new SqlParameter("@Contact", contact)
                                };
                return list;
            }
            
            List<MySqlParameter> AddMySQLUpdateParamaters(int id, string name, string email, string contact)
            {
                List<MySqlParameter> list = new List<MySqlParameter>
                                {
                                    new MySqlParameter("IdVal", id),
                                    new MySqlParameter("NameVal", name),
                                    new MySqlParameter("EmailVal", email),
                                    new MySqlParameter("ContactVal", contact)
                                };
                return list;
            }

            Console.WriteLine("Which database server would you like to choose? \n 1 = SqlServer \n 2 = MySql \n 3 = Exit");
            int server = Convert.ToInt32(Console.ReadLine());
            DatabaseServer db = (DatabaseServer)server;

            switch (db)
            {
                case DatabaseServer.SqlServer:
                    // SQLDbManager derives from IDB
                    dbManager = new SQLDbManager(SqlConnectionString);
                    break;
                case DatabaseServer.MySql:
                    dbManager = new MySqlDbManager(mySqlConnectionString);
                    break;
                case DatabaseServer.Exit:
                    Environment.Exit(0);
                    break;
                default:
                    break;
            }

            while (true)
            {
                Console.WriteLine("Choose from these options: \n 1 = Create \n 2 = Read \n 3 = Update \n 4 = Delete \n 5 = Exit");
                int answer = Convert.ToInt32(Console.ReadLine());
                //Cast answer (an int) to Options (an enum)
                Options o = (Options)answer;

                switch (o)
                {
                    case Options.Create:
                        InsertRecord(dbManager);
                        break;

                    case Options.Read:
                        ReadFunctionality(dbManager);
                        break;

                    case Options.Update:
                          UpdateRecord(dbManager, AddSQLUpdateParamaters, AddMySQLUpdateParamaters);
                        break;

                    case Options.Delete:
                        DeleteRecord(dbManager);
                        break;

                    case Options.Exit:
                        Environment.Exit(0);
                        break;

                    default:
                        break;
                }
                // Wait for signal to clear console and re-start while loop
                Console.WriteLine("\nPress enter to continue...");
                Console.ReadLine();
                Console.Clear();
            }
        }

        public static void InsertRecord(IDB dbManager)
        {
            Console.WriteLine("Please enter a name");
            string name = Console.ReadLine();
            Console.WriteLine("Please enter an email");
            string email = Console.ReadLine();
            Console.WriteLine("Please enter a contact");
            string contact = Console.ReadLine();

            if (dbManager is SQLDbManager)
            {
                // Bind our parameters to our list 
                List<SqlParameter> list = new List<SqlParameter>
                                {
                                    new SqlParameter("@Name", name),
                                    new SqlParameter("@Email", email),
                                    new SqlParameter("@Contact", contact)
                                };
                // Also a generic function that works for both SQL and MySQL
                InsertOneRecord<SqlParameter>(dbManager, list);
            }
            else if (dbManager is MySqlDbManager)
            {
                List<MySqlParameter> list = new List<MySqlParameter>
                                {
                                    new MySqlParameter("NameVal", name),
                                    new MySqlParameter("EmailVal", email),
                                    new MySqlParameter("ContactVal", contact)
                                };
                InsertOneRecord<MySqlParameter>(dbManager, list);
            }
        }

        public static void InsertOneRecord<T>(IDB dbManager, List<T> list) where T : DbParameter
        {
            try
            {                
                dbManager.ExecuteStoredProcedure("InsertRecord", list);
                Console.WriteLine("Data inserted succesfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //READ FUNCTIONS
        public static void ReadFunctionality(IDB dbManager)
        {
            Console.WriteLine("Choose from these options: \n 1 = Return all records \n 2 = Return records for a given id");
            int answer2 = Convert.ToInt32(Console.ReadLine());
            QueryOptions queryOptions = (QueryOptions)answer2;


            switch (queryOptions)
            {
                case QueryOptions.ReturnAll:
                    ReadAllRecords(dbManager);
                    break;

                case QueryOptions.ReturnOne:
                    Console.WriteLine("Enter record Id");
                    int id = Convert.ToInt32(Console.ReadLine());

                    if (dbManager is SQLDbManager)
                    {
                        List<SqlParameter> list = new List<SqlParameter>
                                {
                                    new SqlParameter("@RecordId", id)
                                };
                        ReadOneRecord(dbManager, list);
                    }
                    else if (dbManager is MySqlDbManager)
                    {
                        List<MySqlParameter> list = new List<MySqlParameter>
                                {
                                    new MySqlParameter("RecordId", id)
                                };
                        ReadOneRecord(dbManager, list);
                    }
                    break;
                default:
                    break;
            }
        }

        public static void ReadAllRecords(IDB dbManager)
        {
            try
            {
                DataSet data = dbManager.ExecuteStoredProcedure("GetAllRecords");
                ShowRecords(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
            }
        }

        public static void ReadOneRecord<T>(IDB dbManager, List<T> parameters) where T : DbParameter
        {
            try
            {
                DataSet dataSet = dbManager.ExecuteStoredProcedure("GetRecord", parameters);
                ShowRecords(dataSet);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void ShowRecords(DataSet data)
        {
            foreach (DataTable table in data.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    foreach (DataColumn column in table.Columns)
                    {
                        Console.WriteLine($"{column.ColumnName}:{row[column]}");
                    }
                }
            }
        }

        //UPDATE FUNCTIONS
        // Any function that has the same signature as the delegates can be passed as a parameter
        public static void UpdateRecord(IDB dbManager, UpdateSqlParamatersDelegate sqlDel, UpdateMySqlParamatersDelegate mySqlDel)
        {            
            Console.WriteLine("Please enter an Id");
            int id = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Please enter a name");
            string name = Console.ReadLine();
            Console.WriteLine("Please enter an email");
            string email = Console.ReadLine();
            Console.WriteLine("Please enter a contact");
            string contact = Console.ReadLine();

            try
            {
                if (dbManager is SQLDbManager)
                {
                    // Where connection between delegate and local function is shown
                    List<SqlParameter> list = sqlDel(id, name, email, contact);
                    UpdateOneRecord<SqlParameter>(dbManager, list);
                }
                else if (dbManager is MySqlDbManager)
                {
                    List<MySqlParameter> list = mySqlDel(id, name, email, contact);
                    UpdateOneRecord<MySqlParameter>(dbManager, list);
                }
            }
            catch (Exception ex)
            {
                // Exception is handled 
                Console.WriteLine(ex.Message);
            }
        }

        public static void UpdateOneRecord<T>(IDB dbManager, List<T> list) where T : DbParameter
        {
            dbManager.ExecuteStoredProcedure("UpdateRecord", list);
            Console.WriteLine("Data updated succesfully");
        }

        //DELETE FUNCTIONS
        public static void DeleteRecord(IDB dbManager)
        {
            Console.WriteLine("Please enter a Id");
            int id = Convert.ToInt32(Console.ReadLine());
            try
            {
                if (dbManager is SQLDbManager)
                {
                    // Create a list of SQL parameter objects, here just with id
                    List<SqlParameter> list = new List<SqlParameter>
                                {
                                    new SqlParameter("@Id", id)
                                };
                    DeleteOneRecord<SqlParameter>(dbManager, list);
                }

                else if (dbManager is MySqlDbManager)
                {
                    List<MySqlParameter> list = new List<MySqlParameter>
                                {
                                    new MySqlParameter("IdVal", id)
                                };
                    DeleteOneRecord<MySqlParameter>(dbManager, list);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void DeleteOneRecord<T>(IDB dbManager, List<T> list) where T : DbParameter 
        {
            try
            {
                dbManager.ExecuteStoredProcedure("DeleteRecord", list);
                Console.WriteLine("Data deleted succesfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // ADO.NET
        public static void SqlFunctions()
        {

            //anywhere using sql need a connection string   

            //string query = "SELECT * FROM Records";


            // ONLINE MODE
            //SqlConnection sqlConnection = new SqlConnection(connectionString);

            //try
            //{
            //    sqlConnection.Open();


            //    Console.WriteLine("Connection opened");

            //    sqlConnection.Close();
            //    sqlConnection.Dispose();
            //}
            //catch (Exception ex)
            //{
            //    //using throw will throw the exception out and the app will crash
            //    //throw;
            //    //here we are handling the exception
            //    Console.WriteLine($"{ex.Message}");
            //}

            //ONLINE MODE 

            //using (SqlConnection sqlConnection =
            //    new SqlConnection(connectionString)) 
            //{
            //    try
            //    {
            //        sqlConnection.Open();
            //        Console.WriteLine("Connection opened");
            //        SqlCommand command = new SqlCommand(query, sqlConnection);

            //        //Online connection to the query
            //        SqlDataReader reader = command.ExecuteReader();

            //        //Reader reads line by line - while loop checks a new line is being read
            //        while (reader.Read())
            //        {
            //            Console.WriteLine($"{reader[0]},{reader[1]},{reader[2]},{reader[3]}");
            //        }
            //        reader.Close();
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.Message);                    
            //    }
            //}


            //OFFLINE MODE

            //DataTable dataTable = new DataTable();

            //using (SqlConnection sqlConnection =
            //    new SqlConnection(connectionString))
            //{
            //    try
            //    {
            //        SqlDataAdapter adapter = new SqlDataAdapter(query, sqlConnection);
            //        sqlConnection.Open();

            //        adapter.Fill(dataTable);

            //        // always try to use correct type with foreach
            //        // dataTable.Rows[0] -> DataRow
            //        foreach (DataRow row in dataTable.Rows)
            //        {
            //            Console.WriteLine($"{row["Id"]},{row["Name"]},{row["Email"]},{row["Contact"]}");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.Message);
            //    }
            //}

            //try
            //{
            //    DbManager dbManager = new DbManager(connectionString);
            //    DataSet dataSet1 = dbManager.ExecuteQuery(query);
            //    foreach (DataTable dataTable in dataSet1.Tables)
            //    {
            //        foreach (DataRow dataRow in dataTable.Rows)
            //        {
            //            foreach (DataColumn dataColumn in dataTable.Columns)
            //            {
            //                Console.WriteLine($"{dataRow[dataColumn]}");
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    //throw;
            //    Console.WriteLine(ex.StackTrace + ex.Message);
            //}

            //using (SqlConnection sqlConnection = 
            //        new SqlConnection(connectionString))
            //{
            //    try
            //    {
            //        SqlDataAdapter dataAdapter = new SqlDataAdapter();
            //        SqlCommand command = new SqlCommand("GetAllRecords",sqlConnection);
            //        command.CommandType = CommandType.StoredProcedure;
            //        dataAdapter.SelectCommand = command;
            //        DataTable dataTable = new DataTable();
            //        // Connection is opened here because it is needed then will close (offline mode)
            //        // After data has been retrieved can play with it but will never affect SQL tables (unlike DbSets)
            //        dataAdapter.Fill(dataTable);

            //        foreach (DataRow row in dataTable.Rows)
            //        {
            //            foreach (DataColumn col in dataTable.Columns)
            //            {
            //                Console.WriteLine(row[col.ColumnName]);
            //            }
            //        }
            //        // Equivalent to Select * from record where Id = 1 in SQL
            //        // DataTable does not inherit from IEnumerable so must convert with .AsEnumerable to allow LINQ call
            //        // DataTable is equivalent to tables in SQL -> DbSet runs on-top of DataTable and allows for tracking (can update SQL tables from c#) -> cannot update SQL tables with DataTables 
            //        DataTable newTable = dataTable.AsEnumerable().Where(s => s.Field<int>("Id") == 1).CopyToDataTable();
            //        var newTable1 = dataTable.AsEnumerable().Where(s => s.Field<int>("Id") == 1).Select(s => s);
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.Message);
            //    }
            //}

            //DbManager dbManager1 = new DbManager(connectionString);
            //try
            //{
            //    DataSet data = dbManager1.ExecuteStoredProcedure("GetAllRecords");
            //    foreach (DataTable table in data.Tables)
            //    {
            //        foreach (DataRow row in table.Rows)
            //        {
            //            foreach (DataColumn column in table.Columns)
            //            {
            //                Console.WriteLine($"{column.ColumnName}:{row[column]}");
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    // Will print the error message and stack trace
            //    Console.WriteLine($"{ex}");
            //}


            //using (SqlConnection sqlConnection = 
            //        new SqlConnection(connectionString))
            //{
            //    try
            //    {
            //        // 1. Create adapter
            //        SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
            //        Console.WriteLine("Enter record Id");
            //        int id = Convert.ToInt32(Console.ReadLine());

            //        // 2. Create Sql command (parameter is stored proc name)
            //        SqlCommand command = new SqlCommand("GetRecord", sqlConnection);

            //        // 3. Add paramteres to stored proc (if stored proc takes params)
            //        command.Parameters.Add(new SqlParameter("@RecordId", id));

            //        // 4. Determine type of command (Stored procedure)
            //        command.CommandType = CommandType.StoredProcedure;

            //        // 5. Connecting Sql command with adapter so adapter knows what command to run
            //        sqlDataAdapter.SelectCommand = command;

            //        // 6. Execute adapter
            //        // DataSet used to stored multiple tables, DataTable is one table (our command returns only one table)
            //        DataSet dataSet = new DataSet();
            //        sqlDataAdapter.Fill(dataSet);

            //        foreach (DataTable data in dataSet.Tables)
            //        {
            //            foreach (DataRow row in data.Rows)
            //            {
            //                foreach (DataColumn col in data.Columns)
            //                {
            //                    Console.WriteLine(row[col.ColumnName]);
            //                }
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.Message);
            //    }
            //}

            //DbManager dbManager = new DbManager(connectionString);
            //Console.WriteLine("Enter record Id");
            //int id = Convert.ToInt32(Console.ReadLine());

            //// Implicit upcasting of SqlParamater to DbParamater because our interface is generic and takes DbParamater as a paramater so any type of paramater can be used when calling the function
            //List<DbParameter> list = new List<DbParameter>
            //{
            //    // Adding one SqlParamater to the list
            //    new SqlParameter("@RecordId", id)                
            //};

            //try
            //{
            //    DataSet dataSet = dbManager.ExecuteStoredProcedure("GetRecord", list);

            //    foreach (DataTable data in dataSet.Tables)
            //    {
            //        foreach (DataRow row in data.Rows)
            //        {
            //            foreach (DataColumn col in data.Columns)
            //            {
            //                Console.WriteLine(row[col.ColumnName]);
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}            
        }
    }
}
