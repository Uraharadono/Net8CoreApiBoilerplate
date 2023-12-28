namespace Net8CoreApiBoilerplate.Infrastructure.Settings
{
    public interface IEmailSettings
    {
        public string EmailTemplatesFolder { get; }
        int SmtpPort { get; }
        string SmtpServer { get; }
        bool DefaultCredentials { get; }
        string SmtpUsername { get; }
        string SmtpPassword { get; }

        string EmailSourceName { get; }
        string EmailSourceAddress { get; }

        int MaxEmailsToSendPerBatch { get; }
        string PickupDirectory { get; }
    }
}
