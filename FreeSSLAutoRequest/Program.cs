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
            CasManagerClient client = new CasManagerClient(accessKeyId, accessKeySecret);
            SignedCertificate[] allSignedCertificates = client.GetAllSignedCertificates();
            SubscribedCertificates subscribedCertificates = new SubscribedCertificates(null, client, 3);

            for (int i = 0; i < allSignedCertificates.Length; i++)
            {
                Console.WriteLine($"""
                    - [{i}] 已签发的证书 - {allSignedCertificates[i].CertID}
                            | 证书指纹: {allSignedCertificates[i].Fingerprint}
                            | 绑定主域名: {allSignedCertificates[i].BindedDomainName}
                    """);
            }
            Console.Write("输入证书前的序号以为该证书绑定的域名添加订阅, 留空按回车以结束: ");
            string? userSelectedCertificateRaw;
            while (!string.IsNullOrWhiteSpace(userSelectedCertificateRaw = Console.ReadLine()))
            {
                if (int.TryParse(userSelectedCertificateRaw, out int userSelectedCertificate)
                   && userSelectedCertificate >= 0
                   && userSelectedCertificate < allSignedCertificates.Length)
                {
                    if (!subscribedCertificates.SubscribedCertificatesList.Contains(allSignedCertificates[userSelectedCertificate]))
                    {
                        subscribedCertificates.SubscribedCertificatesList.Add(allSignedCertificates[userSelectedCertificate]);
                        Console.WriteLine($"Added {allSignedCertificates[userSelectedCertificate].BindedDomainName} (CertID = {allSignedCertificates[userSelectedCertificate].CertID}).");
                        DateTime expiredDate = client.GetCertExpiredTime(allSignedCertificates[userSelectedCertificate]);
                        DateTime currentTime = DateTime.Now;
                        Console.WriteLine($"Cert Expired Date: {expiredDate}");
                        Console.WriteLine($"Time left: {expiredDate - currentTime}");
                        Console.Write("输入证书前的序号以为该证书绑定的域名添加订阅, 留空按回车以结束: ");
                    }
                    else
                    {
                        Console.WriteLine("已存在的证书。不应重复添加。");
                        Console.Write("输入证书前的序号以为该证书绑定的域名添加订阅, 留空按回车以结束: ");
                    }

                }
                else
                {
                    Console.WriteLine("无效的输入，请检查。");
                    Console.Write("输入证书前的序号以为该证书绑定的域名添加订阅, 留空按回车以结束: ");
                }

            }

            Console.WriteLine("已添加订阅证书绑定域名列表: ");
            foreach (SignedCertificate certificate in subscribedCertificates.SubscribedCertificatesList)
            {
                Console.WriteLine(certificate.BindedDomainName);
            }
            subscribedCertificates.CheckAllDate();
        }
    }
}
