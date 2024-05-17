using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.AppConfig;
using static System.Formats.Asn1.AsnWriter;
using System.IO;

namespace PoP
{
    public class Program
    {
        static void Main(string[] args)
        {
            string clientId = Environment.GetEnvironmentVariable("clientId") ?? throw new ArgumentNullException("clientId");
            string clientSecret = Environment.GetEnvironmentVariable("clientSecret") ?? throw new ArgumentNullException("clientSecret");
            string tenantId = Environment.GetEnvironmentVariable("tenantId") ?? throw new ArgumentNullException("tenantId");
            string scope = Environment.GetEnvironmentVariable("scope") ?? throw new ArgumentNullException("scope");
            string host = Environment.GetEnvironmentVariable("host") ?? throw new ArgumentNullException("host");

            IConfidentialClientApplication app;

            app = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(new Uri($"https://login.microsoftonline.com/{tenantId}"))
                .WithExperimentalFeatures()
                .Build();

            PoPAuthenticationConfiguration popConfig = new PoPAuthenticationConfiguration(new Uri(host));
            popConfig.Nonce = Guid.NewGuid().ToString();

            popConfig.HttpHost = host;
            popConfig.HttpMethod = HttpMethod.Post;
            popConfig.HttpPath = "$odata";

            var PopResult = app.AcquireTokenForClient(new string[] { scope })
                               .WithProofOfPossession(popConfig)
                               .ExecuteAsync()
                               .ConfigureAwait(false)
                               .GetAwaiter()
                               .GetResult();

            Console.WriteLine(PopResult.AccessToken);
        }
    }
}
