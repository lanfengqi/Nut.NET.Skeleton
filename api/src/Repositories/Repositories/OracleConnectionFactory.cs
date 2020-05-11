using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.EntityFramework;

namespace Foundatio.Skeleton.Repositories.Repositories
{
  public   class OracleConnectionFactory: IDbConnectionFactory
    {
        private readonly string _baseConnectionString;
        private Func<string, DbProviderFactory> _providerFactoryCreator;

        public OracleConnectionFactory()
        {
        }

        public OracleConnectionFactory(string baseConnectionString)
        {
            this._baseConnectionString = baseConnectionString;
        }

        public DbConnection CreateConnection(string nameOrConnectionString)
        {
            string connectionString = nameOrConnectionString;

            bool treatAsConnectionString = nameOrConnectionString.IndexOf('=') >= 0;

            if (!treatAsConnectionString)
            {
                OracleConnectionStringBuilder builder = new OracleConnectionStringBuilder(this.BaseConnectionString);

                //MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder(this.BaseConnectionString);
                //builder.Server = nameOrConnectionString;
                connectionString = builder.ConnectionString;
            }
            DbConnection connection = null;
            try
            {
                connection = this.ProviderFactory("Oracle.DataAccess.Client").CreateConnection();
                connection.ConnectionString = connectionString;
            }
            catch
            {
                connection = new OracleConnection(connectionString);
            }
            return connection;
        }

        public string BaseConnectionString
        {
            get
            {
                return this._baseConnectionString;
            }
        }

        internal Func<string, DbProviderFactory> ProviderFactory
        {
            get
            {
                Func<string, DbProviderFactory> func1 = this._providerFactoryCreator;
                return delegate (string name) {
                    return DbProviderFactories.GetFactory(name);
                };
            }
            set
            {
                this._providerFactoryCreator = value;
            }
        }
    }
}
