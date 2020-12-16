using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Data.Common;

namespace DiscordBot1
{
    public class DatabaseContextFactory : IDesignTimeDbContextFactory<DBContextBot>
    {
        /// <summary>
        /// Use the default connection in Entity Framework to establish communication with the
        /// database
        /// </summary>
        public bool UseDefaultConnection { get; set; }

        /// <summary>
        /// DbConnection used by the DbContext if UseDefaultConnection has been set to false
        /// </summary>
        public DbConnection DbConnection { get; set; }

        /// <summary>
        /// DbConnection used by the DbContext if UseDefaultConnection has been set to false
        /// </summary>
        public DbTransaction DbTransaction { get; set; }

        public DatabaseContextFactory()
        {
            this.UseDefaultConnection = true;
        }

        public DBContextBot Create()
        {
            if (!this.UseDefaultConnection)
            {
                DBContextBot context = null;
                
                // new DBContextBot(DbConnection, false);
                //context.Configuration.ProxyCreationEnabled = false;
                //context.Database.UseTransaction(DbTransaction);


                return context;
            }
            else
            {
                DBContextBot context = new DBContextBot();
                //context.Configuration.ProxyCreationEnabled = false;

                return context;
            }
        }
        
        public DBContextBot CreateDbContext(string[] args)
        {
            return new DBContextBot();
        }
    }
}
