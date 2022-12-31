using System;
using Microsoft.Extensions.Configuration;
using Net7CoreApiBoilerplate.Utility.Exceptions;
using Net7CoreApiBoilerplate.Utility.Extensions;

namespace Net7CoreApiBoilerplate.Infrastructure.Settings
{
    public interface IAppSettings : IJwtSettings, IClientAppSettings, IEmailSettings, IDocumentSettings
    {
        int OrganizationType { get;  }
        bool IsDebug { get; }
        string AdminEmail { get; }
    }

    public class AppSettings : IAppSettings
    {
        private readonly IConfiguration _config;
        
        public AppSettings(IConfiguration config)
        {
            _config = config;
        }

        public int OrganizationType => ReadInt("OrganizationType");
        public bool IsDebug => ReadBoolean("IsDebug");
        public string AdminEmail => ReadString("AdminEmail");

        // Client App Settings
        public string ClientBaseUrl => ReadString("ClientBaseUrl");
        public string EmailConfirmationPath => ReadString("EmailConfirmationPath");
        public string ResetPasswordPath => ReadString("ResetPasswordPath");

        // Email
        public int SmtpPort => ReadInt("SmtpPort");
        public int MaxEmailsToSendPerBatch => ReadInt("MaxEmailsToSendPerBatch");
        public string SmtpServer => ReadString("SmtpServer");
        public bool DefaultCredentials => ReadBoolean("DefaultCredentials");
        public string SmtpUsername => ReadString("SmtpUsername");
        public string SmtpPassword => ReadString("SmtpPassword");
        public string EmailSourceName => ReadString("EmailSourceName");
        public string EmailSourceAddress => ReadString("EmailSourceAddress");
        public string PickupDirectory => ReadString("PickupDirectory");

        // JWT related stuff
        public string Key => ReadString("Key");
        public string Issuer => ReadString("Issuer");
        public string Audience => ReadString("Audience");
        public double DurationInMinutes => ReadDouble("DurationInMinutes");
        public int RememberMeDurationInHours => ReadInt("RememberMeDurationInHours");

        // E-mail template settings
        public string EmailTemplatesFolder => ReadString("EmailTemplatesFolder");

        // Document settings - mostly save paths
        public string BaseFolder => ReadString("BaseFolder");
        public string ArticleDocumentsFolder => ReadString("ArticleDocumentsFolder");
        public string WordTemplatesFolder => ReadString("WordTemplatesFolder");

        // Utility functions
        private string ReadString(string key)
        {
            var settings = _config.GetSection("AppSettings");
            return settings[key];
        }

        private int ReadInt(string key)
        {
            return ReadString(key).ParseInt();
        }

        private bool ReadBoolean(string key)
        {
            var val = ReadString(key);
            return val != null && bool.Parse(val);
        }

        private long ReadLong(string key)
        {
            return Convert.ToInt64(ReadString(key));
        }

        private double ReadDouble(string key)
        {
            return Convert.ToDouble(ReadString(key));
            // return ReadString(key).con();
        }

        private TEnum ReadEnum<TEnum>(string key) where TEnum : struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum)
                throw new AppException("Expected an enum.");

            var value = (TEnum)(object)ReadInt(key);
            var isDefined = Enum.IsDefined(typeof(TEnum), value);

            if (!isDefined)
                throw new AppException($"Provided value: {value} is not defined in enum {typeof(TEnum)}");

            return value;
        }

        //private string ReadAsAbsolutePath(string key)
        //{
        //    var path = ReadString(key);
        //    var basePath = _env.WebRootPath;
        //    return Path.Combine(basePath, path);
        //}

        private long[] ReadLongArray(string key)
        {
            // return System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.List<long>>(ReadString(key));
            return System.Text.Json.JsonSerializer.Deserialize<long[]>(ReadString(key));
        }
    }
}
