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

        public DataSet ExecuteStoredProcedure<T>(string procedureName, List<T> parameters) where T : DbParameter;
    }
}
