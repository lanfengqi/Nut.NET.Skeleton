using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Foundatio.Logging;
using Foundatio.Logging.NLog;
using Foundatio.Skeleton.Core.Extensions;
using Foundatio.Skeleton.Core.Utility;

namespace Foundatio.Skeleton.Domain {
    public class Settings : SettingsBase<Settings> {
        public bool EnableSsl { get; private set; }

        public string ApiUrl { get; private set; }

        public string WebHookUrl { get; private set; }

        public string AppUrl { get; private set; }

        public string OAuthRedirectUrl => String.Concat(AppUrl, "/redirect.html");

        public AppMode AppMode { get; private set; }

        public string AppScope { get; private set; }

        public bool HasAppScope => !String.IsNullOrEmpty(AppScope);

        public string AppScopePrefix => HasAppScope ? AppScope + "-" : String.Empty;

        public string TestEmailAddress { get; private set; }

        public List<string> AllowedOutboundAddresses { get; private set; }

        public bool RunJobsInProcess { get; private set; }

        public bool LogJobLocks { get; private set; }

        public bool LogJobEvents { get; private set; }

        public bool LogJobCompleted { get; private set; }

        public bool EnableSignalR { get; private set; }

        public int ApiThrottleLimit { get; private set; }

        public string MetricsServer { get; private set; }

        public int MetricsServerPort { get; private set; }

        public string MetricsPrefix { get; private set; }

        public bool EnableMetricsReporting { get; private set; }

        public string RedisConnectionString { get; private set; }

        public bool EnableRedis { get; private set; }

        public string Version { get; private set; }

        public string InformationalVersion { get; private set; }

        public bool EnableIntercom => !String.IsNullOrEmpty(IntercomAppId) && !String.IsNullOrEmpty(IntercomAppSecret);

        public string IntercomAppId { get; private set; }

        public string IntercomAppSecret { get; private set; }

        public bool EnableAccountCreation { get; private set; }

        public string GeocodeApiKey { get; private set; }

        public string StripePublishableApiKey { get; private set; }

        public string StorageFolder { get; private set; }

        public string PrivateAzureStorageConnectionString { get; set; }

        public string PublicAzureStorageConnectionString { get; set; }

        public string PrivateS3StorageConnectionString { get; set; }

        public string PrivateS3StorageFolder { get; set; }

        public string PublicS3StorageConnectionString { get; set; }

        public string PublicS3StorageFolder { get; set; }

        public string PublicAzureStorageContainerName { get; set; }

        public string PrivateAzureStorageContainerName { get; set; }

        public string PublicStorageUrlPrefix { get; set; }

        public bool EnableAzureStorage { get; private set; }

        public bool EnableS3Storage { get; private set; }

        public int TokenExpiressMinutes { get; set; }

        public string CustomOrderNumberMask { get; set; }


        public override void Initialize() {
            EnableSsl = GetBool("EnableSsl", false);
            TokenExpiressMinutes = GetInt("TokenExpiressMinutes", 30);

            if (AppScope == null)
                AppScope = String.Empty;
            AppMode = GetEnum<AppMode>("AppMode", AppMode.Local);
            AppScope = GetString("AppScope", String.Empty);
            ApiUrl = GetString("ApiUrl", "http://localhost:51000/api/v1");
            ApiUrl = ApiUrl.TrimEnd('/');
            WebHookUrl = GetString("WebHookUrl", ApiUrl);
            WebHookUrl = WebHookUrl.TrimEnd('/');
            string defaultAppUrl = "http://lm-app.localtest.me:52000/";
            if (!ApiUrl.Contains("localhost"))
                defaultAppUrl = ApiUrl.ReplaceFirst("api", "app").Replace("51000", "52000").Replace("api/v1", String.Empty);
            AppUrl = GetString("AppUrl", defaultAppUrl);
            AppUrl = AppUrl.TrimEnd('/');
            if (EnableSsl && AppUrl.StartsWith("http:"))
                AppUrl = AppUrl.ReplaceFirst("http:", "https:");
            else if (!EnableSsl && AppUrl.StartsWith("https:"))
                AppUrl = AppUrl.ReplaceFirst("https:", "http:");

            TestEmailAddress = GetString("TestEmailAddress", "support@foundatio.com");
            AllowedOutboundAddresses = GetStringList("AllowedOutboundAddresses", "foundatio.com,slideroom.com,mailinator.com").Select(v => v.ToLower()).ToList();
            RunJobsInProcess = GetBool("RunJobsInProcess", true);
            LogJobLocks = GetBool("LogJobLocks", false);
            LogJobEvents = GetBool("LogJobEvents", false);
            LogJobCompleted = GetBool("LogJobCompleted", false);
            EnableSignalR = GetBool("EnableSignalR", true);
            ApiThrottleLimit = GetInt("ApiThrottleLimit", Int32.MaxValue);
            MetricsServer = GetString("MetricsServer");
            MetricsServerPort = GetInt("MetricsServerPort", 8125);
            string environment = !AppScope.IsNullOrEmpty() ? AppScope : (AppMode == AppMode.Production ? "prod" : "qa");
            MetricsPrefix = GetString("MetricsPrefix", environment + "-" + Environment.MachineName);
            EnableMetricsReporting = GetBool("EnableMetrics", true);
            IntercomAppId = GetString("IntercomAppId");
            IntercomAppSecret = GetString("IntercomAppSecret");
            EnableAccountCreation = GetBool("EnableAccountCreation", true);
           
            StripePublishableApiKey = GetString("StripePublishableApiKey");
            StorageFolder = GetString("StorageFolder", "|DataDirectory|\\storage");
            PublicStorageUrlPrefix = GetString("PublicStorageUrlPrefix", "http://localhost:51000/" + AppScopePrefix + "public");
            if (PublicStorageUrlPrefix.EndsWith("/"))
                PublicStorageUrlPrefix = PublicStorageUrlPrefix.TrimEnd('/');

            RedisConnectionString = GetConnectionString("RedisConnectionString");
            EnableRedis = GetBool("EnableRedis", !String.IsNullOrEmpty(RedisConnectionString));

            PrivateAzureStorageConnectionString = GetConnectionString("PrivateAzureStorageConnectionString");
            PrivateAzureStorageContainerName = GetString("PrivateAzureStorageContainerName") ?? AppScopePrefix + "private";
            PublicAzureStorageConnectionString = GetConnectionString("PublicAzureStorageConnectionString", PrivateAzureStorageConnectionString);
            PublicAzureStorageContainerName = GetString("PublicAzureStorageContainerName") ?? AppScopePrefix + "public";
            EnableAzureStorage = GetBool("EnableAzureStorage", !String.IsNullOrEmpty(PrivateAzureStorageConnectionString));

            PrivateS3StorageConnectionString = GetConnectionString("PrivateS3StorageConnectionString");
            PublicS3StorageConnectionString = GetConnectionString("PublicS3StorageConnectionString", PrivateS3StorageConnectionString);
            bool isSingleS3 = PrivateS3StorageConnectionString == PublicS3StorageConnectionString;
            PrivateS3StorageFolder = GetString("PrivateS3StorageFolder") ?? (isSingleS3 ? "private/" + AppScope : AppScope);
            if (PrivateS3StorageFolder.EndsWith("/"))
                PrivateS3StorageFolder = PrivateS3StorageFolder.TrimEnd('/');
            PublicS3StorageFolder = GetString("PublicS3StorageFolder") ?? (isSingleS3 ? "public/" + AppScope : AppScope);
            if (PublicS3StorageFolder.EndsWith("/"))
                PublicS3StorageFolder = PublicS3StorageFolder.TrimEnd('/');
            EnableS3Storage = GetBool("EnableS3Storage", !String.IsNullOrEmpty(PrivateS3StorageConnectionString));

            CustomOrderNumberMask = GetString("CustomOrderNumberMask") ?? "{YYYY}{MM}{DD}{ID}";
            try {
                var versionInfo = FileVersionInfo.GetVersionInfo(typeof(Settings).Assembly.Location);
                Version = versionInfo.FileVersion;
                InformationalVersion = versionInfo.ProductVersion;
            } catch { }
        }

        public const string JobBootstrappedServiceProvider = "Foundatio.Skeleton.Insulation.Jobs.JobBootstrappedServiceProvider,Foundatio.Skeleton.Insulation";

        public static LoggerFactory GetLoggerFactory() {

            var loggerFactory = new LoggerFactory();
            loggerFactory.DefaultLogLevel = LogLevel.Trace;
            loggerFactory.AddNLog();
            Log = loggerFactory.CreateLogger<Settings>();

            return loggerFactory;
        }
    }

    public enum AppMode {
        Production,
        NonProduction,
        Local
    }
}
