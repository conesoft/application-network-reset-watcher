using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;

namespace NetworkReset;

public class Notification()
{
    public static async Task<bool> Notify(string title, string message, string url)
    {
        try
        {
            var configuration = new ConfigurationBuilder().AddJsonFile(@"D:\Hosting\Settings\Websites\settings.json").Build();
            var conesoftSecret = configuration["conesoft:secret"] ?? throw new Exception("Configuration broken");

            var query = new QueryBuilder
            {
                { "token", conesoftSecret },
                { "title", title },
                { "message", message },
                { "url", url }
            };

            var request = new HttpClient().GetAsync($@"https://conesoft.net/notify" + query.ToQueryString());
            var timeout = Task.Delay(1000);
            var result = await Task.WhenAny(timeout, request);
            return result == request && request.IsCompletedSuccessfully;
        }
        catch (Exception)
        {
            return false;
        }
    }
}