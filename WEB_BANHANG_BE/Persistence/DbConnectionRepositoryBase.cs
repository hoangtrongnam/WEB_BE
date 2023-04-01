using System.Data;

namespace WEB_BANHANG_BE.Persistence
{
    public class DbConnectionRepositoryBase
    {
        public IDbConnection _conn { get; private set; }

        public DbConnectionRepositoryBase(IDbConnectionFactory dbConnectionFactory)
        {
            // Now it's the time to pick the right connection string!
            // Enum is used. No magic string!
            this._conn = dbConnectionFactory.CreateDbConnection(DatabaseConnectionName.Connection1);
        }
    }
}
