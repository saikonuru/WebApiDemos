﻿namespace CityInfo.API.Services
{
    public class LocalMailService : IMailService
    {
        private readonly string _mailTo;
        private readonly string _mailFrom;

        public LocalMailService(IConfiguration configuration)
        {
            // Use null-coalescing operator to ensure non-null values
            _mailTo = configuration["mailSettings:mailTo"] ?? throw new ArgumentNullException(nameof(configuration), "mailSettings:mailTo is not configured.");
            _mailFrom = configuration["mailSettings:mailFrom"] ?? throw new ArgumentNullException(nameof(configuration), "mailSettings:mailFrom is not configured.");
        }

        public void SenMail(string message)
        {
            Console.WriteLine($"Sending mail with message {message} from {nameof(LocalMailService)}");
            Console.WriteLine($"Mail from {_mailFrom}; Mail to : {_mailTo}");
        }
    }
}
