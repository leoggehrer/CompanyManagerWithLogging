using CompanyManager.Logic.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CompanyManager.Logic.DataContext
{
    internal class CompanyContext : DbContext, IContext
    {
        #region fields
        private static readonly string DatabaseType = "";
        private static readonly string ConnectionString = "";
        private static readonly bool SqlLogging = false;
        private static readonly string SqlStartUp = string.Empty;
        private static readonly string SqlLogFilePath = string.Empty;
        #endregion fields

        static CompanyContext()
        {
            var appSettings = Common.Modules.Configuration.AppSettings.Instance;

            DatabaseType = appSettings["Database:Type"] ?? DatabaseType;
            ConnectionString = appSettings[$"ConnectionStrings:{DatabaseType}ConnectionString"] ?? ConnectionString;

            // Start: Sql-Logging
            bool.TryParse(appSettings["Logging:Sql:Active"], out SqlLogging);
            SqlStartUp = appSettings["Logging:Sql:StartUp"] ?? SqlStartUp;
            SqlLogFilePath = appSettings["Logging:Sql:FilePath"] ?? SqlLogFilePath;

            if (SqlLogging)
            {
                if (SqlStartUp.Equals("Delete", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (File.Exists(SqlLogFilePath))
                    {
                        File.Delete(SqlLogFilePath);
                    }
                }
            }
            // End: Sql-Logging
        }

        #region properties
        public DbSet<Entities.Company> CompanySet { get; set; }
        public DbSet<Entities.Customer> CustomerSet { get; set; }
        public DbSet<Entities.Employee> EmployeeSet { get; set; }
        #endregion properties

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (DatabaseType == "Sqlite")
            {
                optionsBuilder.UseSqlite(ConnectionString);

            }
            else if (DatabaseType == "SqlServer")
            {
                optionsBuilder.UseSqlServer(ConnectionString);
            }

            if (SqlLogging)
            {
                optionsBuilder.LogTo(msg => File.AppendAllText(SqlLogFilePath, msg + Environment.NewLine), LogLevel.Information)
                              .EnableSensitiveDataLogging()
                              .EnableSensitiveDataLogging(true);
            }

            base.OnConfiguring(optionsBuilder);
        }
    }
}
