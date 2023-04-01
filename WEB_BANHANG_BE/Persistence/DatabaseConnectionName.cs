using System.Data;
using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;

namespace WEB_BANHANG_BE.Persistence
{
    public enum DatabaseConnectionName
    {
        Connection1,
        Connection2
    }
    public interface IDbConnectionFactory
    {
        IDbConnection CreateDbConnection(DatabaseConnectionName connectionName);
    }
    public class DapperDbConnectionFactory : IDbConnectionFactory
    {
        private readonly IDictionary<DatabaseConnectionName, string> _connectionDict;

        public DapperDbConnectionFactory(IDictionary<DatabaseConnectionName, string> connectionDict)
        {
            _connectionDict = connectionDict;
        }

        public IDbConnection CreateDbConnection(DatabaseConnectionName connectionName)
        {
            string connectionString = "";
            if (_connectionDict.TryGetValue(connectionName, out connectionString))
            {
                return new OracleConnection(connectionString);
                //return new SqlConnection(connectionString);
            }

            throw new ArgumentNullException();
        }
    }
}
