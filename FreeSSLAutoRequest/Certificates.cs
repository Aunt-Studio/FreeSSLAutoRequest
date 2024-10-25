using AlibabaCloud.GatewaySpi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tea;

namespace FreeSSLAutoRequest
{
    internal class SubscribedCertificates
    {
        private int _dateOffset;

        /// <summary>
        /// 控制所有订阅的列表对象。
        /// </summary>
        public List<SignedCertificate> SubscribedCertificatesList { get; }

        /// <summary>
        /// 管理阿里云CAS OpenAPI 的<seealso cref="CasManagerClient"/>对象。
        /// </summary>
        public CasManagerClient Client { get; }

        /// <summary>
        /// 维护了一个包含所有即将到期的证书绑定的主要域名列表，用于再次申请这些域名证书。
        /// </summary>
        public List<string> UpcomingExpiryDomains { get; }

        /// <summary>
        /// 控制在证书过期的几天前就将证书添加到即将过期列表中以等待申请。
        /// </summary>
        public int DateOffset
        {
            get => _dateOffset;
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegative(value);
                _dateOffset = value;
            }
        }

        /// <summary>
        /// 使用已有证书List初始化证书列表。
        /// </summary>
        /// <param name="certificates"></param>
        public SubscribedCertificates(List<SignedCertificate>? certificates, CasManagerClient client, int dateOffset)
        {
            certificates ??= new List<SignedCertificate>();
            SubscribedCertificatesList = certificates;
            Client = client;
            UpcomingExpiryDomains = new List<string>();
            DateOffset = dateOffset;
        }

        /// <summary>
        /// 检查<see cref="SubscribedCertificatesList"/>中的所有证书是否临期。
        /// </summary>
        /// <returns>若存在一个或更多证书临期，将返回 true。否则为 false。</returns>
        public bool CheckAllDate()
        {
            bool inQueue = false;
            foreach (SignedCertificate certificate in SubscribedCertificatesList)
            {
                Console.WriteLine($"Checking {certificate.BindedDomainName}..");
                DateTime certificateExpiredDate = Client.GetCertExpiredTime(certificate);
                if (certificateExpiredDate <= DateTime.Now.Date.AddDays(DateOffset))
                {
                    UpcomingExpiryDomains.Add(certificate.BindedDomainName);
                    Console.WriteLine($"Domain {certificate.BindedDomainName} with certificate {certificate.CertID} will be expired in {(DateTime.Now.Date.AddDays(DateOffset) - certificateExpiredDate).Days} day(s)." +
                        $"\n Already added into re-request queue.");
                    inQueue = true;
                }
            }
            return inQueue;
        }


    }

    /// <summary>
    /// 已经签发的证书实例类。由<seealso cref="Certificate"/>类派生。
    /// </summary>
    class SignedCertificate : Certificate
    {
        public SignedCertificate(string bindedDomainName, long certID, string fingerprint) : base(bindedDomainName)
        {
            CertID = certID;
            Fingerprint = fingerprint;
        }

        /// <summary>
        /// 证书ID, 该字段类型为 Long, 在序列化/反序列化的过程中可能导致精度丢失, 请注意数值不得大于 9007199254740991。
        /// </summary>
        public long CertID { get; }

        /// <summary>
        /// 证书指纹值。
        /// </summary>
        public string Fingerprint { get; }
    }

    /// <summary>
    /// 尚未签发但已经创建订单的证书实例类。由<seealso cref="Certificate"/>类派生。
    /// </summary>
    class CertificateInOrder
    {
        public long OrderID { get; }
        public long AliyunOrderId { get; }

        public CertificateInOrder(long orderID, long aliyunOrderId)
        {
            OrderID = orderID;
            AliyunOrderId = aliyunOrderId;
        }
    }
    /// <summary>
    /// 通用的证书类。可用于已创建订单、验证中、签发后等等证书状态。
    /// </summary>
    class Certificate
    {

        /// <summary>
        /// 证书绑定的主域名。由于免费证书能且仅能绑定单个域名，所以再次申请新证书时将仅继承主域名。
        /// </summary>
        public string BindedDomainName { get; }

        public Certificate(string bindedDomainName)
        {
            BindedDomainName = bindedDomainName;
        }
    }
}
