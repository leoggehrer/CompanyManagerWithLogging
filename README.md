# CompanyManager With EF Logging

**Lernziele:**

- Wie die SQL-Statements protokolliert werden.

**Hinweis:** Als Startpunkt wird die Vorlage [CompanyManagerWithSettings]https://github.com/leoggehrer/CompanyManagerWithSettings) verwendet.

## Vorbereitung

Bevor mit der Umsetzung begonnen wird, sollte die Vorlage heruntergeladen und die Funktionalität verstanden werden. Zusätzlich sollte die Präsentation zum Thema 'EF-Logging' durchgearbeitet werden. Die Präsentation finden Sie [hier](https://github.com/leoggehrer/Slides/tree/main/EFLogging).

## 'AppSettings' konfigurieren

Die Datei 'appsettings.json' wird wie folgt konfiguriert:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    },
    "Sql": {
      "Active": "true",
      "StartUp": "Delete",
      "FilePath": "el-log.txt"
    }
  },
  "AllowedHosts": "*",
  "Database": {
    "Type": "SqlServer"
  }
}
```

Im Abschnitt "Logging:Sql" werden einige Einstellungen für das Protokollieren der SQL-Statements definiert. Die folgende Tabelle erläutert die Werte:

| Schlüssel | Wert       | Beschreibung |
| --------- | ---------- | ------------ |
| Active    | true       | Aktiviert das Protokollieren der SQL-Statements. |
| StartUp   | Delete     | Löscht die Protokolldatei beim Start der Anwendung. |
| FilePath  | el-log.txt | Der Pfad zur Protokolldatei. |

## Auswertung der Einstellungen und Implementierung des EF-Loggings

```csharp
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
```

## Testen des Systems

Testen Sie die Anwendung mit der Datenbank mit den angegebenen Einstellungen und prüfen Sie die Protokoll-Datei.

## Hilfsmittel

- keine

## Abgabe

- Termin: 1 Woche nach der Ausgabe
- Klasse:
- Name:

## Quellen

- keine Angabe

> **Viel Erfolg!**
