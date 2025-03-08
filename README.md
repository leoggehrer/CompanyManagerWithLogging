# CompanyManager With AppSettings

**Lernziele:**

- Wie die AppSettings in den Projekten verwendet wird.

**Hinweis:** Als Startpunkt wird die Vorlage [CompanyManagerWithSqlite](https://github.com/leoggehrer/CompanyManagerWithSqlite) verwendet.

## Vorbereitung

Bevor mit der Umsetzung begonnen wird, sollte die Vorlage heruntergeladen und die Funktionalität verstanden werden. Zusätzlich sollte die Präsentation zum Thema 'AppSettings' durchgearbeitet werden. Die Präsentation finden Sie [hier](https://github.com/leoggehrer/Slides/tree/main/DotnetAppSettings).

## Packages installieren

Das Laden der 'AppSettings' sollte von allen Projekten erfolgen können. Um diese Anforderung zu erfüllen, sollte das Laden der Einstellungen im Projekt `CompanyManager.Common` implementiert werden. Es müssen folgende Packages im Projekt `CompanyManager.Common` installiert werden:

- Microsoft.Extensions.Configuration
- Microsoft.Extensions.Configuration.Json
- Microsoft.Extensions.Configuration.EnvironmentVariables

## Erstellen der Klasse `AppSettings`

Im Projekt `CompanyManager.Common` erstellen wir einen Ordner **Modules** und einen Unterordner **Configuration**. Anschließend erstellen wir in diesem Ordner eine Klasse mit dem Namen `AppSettings`. Diese Klasse wird als `Singleton` konzipiert. Die Umsetzung der Klasse ist wie folgt:

```csharp
namespace CompanyManager.Common.Modules.Configuration
{
    /// <summary>
    /// Singleton class to manage application settings.
    /// </summary>
    public sealed class AppSettings : ISettings
    {
        #region fields
        private static AppSettings _instance = new AppSettings();
        private static IConfigurationRoot _configurationRoot;
        #endregion fields

        /// <summary>
        /// Static constructor to initialize the configuration.
        /// </summary>
        static AppSettings()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{environmentName ?? "Development"}.json", optional: true)
                    .AddEnvironmentVariables();

            _configurationRoot = builder.Build();
        }

        #region properties
        /// <summary>
        /// Gets the singleton instance of the AppSettings class.
        /// </summary>
        public static AppSettings Instance => _instance;
        #endregion properties

        /// <summary>
        /// Private constructor to prevent instantiation.
        /// </summary>
        private AppSettings()
        {
        }

        /// <summary>
        /// Indexer to get the configuration value by key.
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <returns>The configuration value.</returns>
        public string? this[string key] => _configurationRoot[key];
    }
}
```

Damit diese Klasse auch in einen **Dependency Injection (DI)-Container** registriert werden kann, erstellen wir zu dieser Klasse eine Schnittstelle. Diese Schnittstelle befindet sich im Ordner **Contracts** bei den anderen Schnittstellen. Der Aufbau der Schnittstelle ist wie folgt definiert:

```csharp
namespace CompanyManager.Common.Contracts
{
    public interface ISettings
    {
        string? this[string key] { get; }
    }
}
```

## Auslesen von AppSettings-Daten

Damit `AppSettings`-Daten definiert werden können, müssen in den ausführbaren Projekten die `AppSettings`-Dateien erstellt werden und die Daten im Format `json` definiert werden. In unserem Fall werden zwei Dateien erstellt. Die erste Datei hat den Namen `appsettings.json` und die zweite Datei hat den Namen `appsettings.Development.json`. 

Die Datei `appsettings.json` enthält die allgemeinen Daten, die in allen Umgebungen verwendet werden. Die Datei `appsettings.Development.json` enthält die Daten, die nur in der Entwicklungsphase verwendet werden. Die Daten in den Dateien sehen wie folgt aus:

***appsettings.json***

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Database": {
    "Type": "SqlServer"
  }
}
```

***appsettings.Development.json***

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "SqliteConnectionString": "Data Source=CompanyManagerDb.db",
    "SqlServerConnectionString": "Data Source=127.0.0.1,1433; Database=CompanyManagerDb;User Id=sa; Password=passme!1234;TrustServerCertificate=true"
  }
}
```

> **ACHTUNG:** Die Dateien müssen als `Copy if newer` oder `Copy always` in den Eigenschaften eingestellt werden.

## Verwendung der AppSettings-Daten

Im `DataContext` werden die `AppSettings`-Daten ausgewertet. Dabei wird der Database-Typ, 'SqlServer' oder 'Sqlite', ausgewertet und der entsprechende 'ConnectionString' geladen. Die Auswertung der Daten erfolgt im statischen Konstruktor der `DataContext`-Klasse. Bevor allerdings die Datenbank 'SqlServer' verwendet werden kann, muss das Package `Microsoft.EntityFrameworkCore.SqlServer` im Projekt 'CompanyManager.Logic' installiert werden. 

Hier ein Beispiel, wie die Daten ausgewertet werden:

```csharp
namespace CompanyManager.Logic.DataContext
{
    internal class CompanyContext : DbContext, Common.Contracts.IContext
    {
        #region fields
        private static string DatabaseType = "Sqlite";
        private static string ConnectionString = "data source=CompanyManager.db";
        #endregion fields

        #region properties
        ...
        #endregion properties

        static CompanyContext()
        {
            var appSettings = Modules.Configuration.AppSettings.Instance;

            DatabaseType = appSettings["Database:Type"] ?? DatabaseType;
            ConnectionString = appSettings[$"ConnectionStrings:{DatabaseType}ConnectionString"] ?? ConnectionString;
        }

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

            base.OnConfiguring(optionsBuilder);
        }
        ...
    }
}
```

## Testen des Systems

Testen Sie die Anwendung mit der Datenbank 'SqlServer'. Dazu müssen Sie in den `appsettings.json` die Datenbank auf 'SqlServer' umstellen.

## Hilfsmittel

- keine

## Abgabe

- Termin: 1 Woche nach der Ausgabe
- Klasse:
- Name:

## Quellen

- keine Angabe

> **Viel Erfolg!**
