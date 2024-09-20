using Microsoft.Extensions.Configuration;


namespace FreeSSLAutoRequest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().AddUserSecrets<Program>();

            IConfiguration configuration = builder.Build();

            var accessKeyId = configuration["AccessKeyId"];
            var accessKeySecret = configuration["AccessKeySecret"];
            AlibabaCloud.SDK.Cas20200407.Client client = CasManager.CreateClient(accessKeyId, accessKeySecret);
            DateTime expiredTime = CasManager.GetCertExpiredTime(client, 14892809);
            Console.WriteLine(expiredTime);
        }
    }
}
