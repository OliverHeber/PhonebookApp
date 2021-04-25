using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace DataAccessLayer
{
    public interface IDB
    {
        public DataSet ExecuteQuery(string query);
        public DataSet ExecuteStoredProcedure(string procedureName);

        // Generic T, but giving some security by ensuring T inherits from DbParameter
        public DataSet ExecuteStoredProcedure<T>(string procedureName, List<T> parameters) where T : DbParameter;
    }
}
