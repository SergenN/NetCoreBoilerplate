namespace NetCoreBoilerplate.Models.Settings
{
    public class MailSettings
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string FromEmail { get; set; }

        public string FromName { get; set; }

        public string Domain { get; set; }
    }
}