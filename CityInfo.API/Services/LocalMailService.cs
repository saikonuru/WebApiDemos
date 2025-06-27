namespace CityInfo.API.Services
{
    public class LocalMailService
    {
        public void SenMail(string message)
        {
            Console.WriteLine($"Sending mail with message {message}");
        }
    }
}
