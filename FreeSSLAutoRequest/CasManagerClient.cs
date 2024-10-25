using AlibabaCloud.SDK.Cas20200407;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tea;
using Tea.Utils;


namespace FreeSSLAutoRequest
{
    /// <summary>
    /// 所有Cas API 相关操作方法
    /// </summary>
    internal class CasManagerClient
    {
        public AlibabaCloud.SDK.Cas20200407.Client Client { get; }
        /// <summary>
        /// 使用已初始化的账号Client 以构造<see cref="CasManagerClient"/>对象。
        /// </summary>
        /// <param name="client">用以构造<see cref="CasManagerClient"/>对象的<see cref="AlibabaCloud.SDK.Cas20200407.Client"/>实例。</param>
        public CasManagerClient(Client client)
        {
            Client = client;
        }
        /// <summary>
        /// 使用AK&amp;SK初始化账号Client 并构造<see cref="CasManagerClient"/>对象。
        /// </summary>
        /// <param name="AKId">AccessKey ID</param>
        /// <param name="AKSecret">AccessKey Secret</param>
        public CasManagerClient(string AKId, string AKSecret)
        {
            Client = CreateClient(AKId, AKSecret);
        }
        /// <summary>
        /// 使用AK&amp;SK初始化账号Client
        /// </summary>
        /// <param name="AKId">AccessKey ID</param>
        /// <param name="AKSecret">AccessKey Secret</param>
        /// <returns><see cref="AlibabaCloud.SDK.Cas20200407.Client"/></returns>
        private static AlibabaCloud.SDK.Cas20200407.Client CreateClient(string AKId, string AKSecret)
        {
            AlibabaCloud.OpenApiClient.Models.Config config = new AlibabaCloud.OpenApiClient.Models.Config
            {
                AccessKeyId = AKId,
                AccessKeySecret = AKSecret,
            };
            // Endpoint 请参考 https://api.aliyun.com/product/cas
            config.Endpoint = "cas.aliyuncs.com";
            return new AlibabaCloud.SDK.Cas20200407.Client(config);
        }

        /// <summary>
        /// 获取已签发证书的过期日期。
        /// </summary>
        /// <param name="signedCertificate">已签发证书的<see cref="SignedCertificate"/>对象。</param>
        /// <returns>该证书的过期日期，精确到日。</returns>
        public DateTime GetCertExpiredTime(SignedCertificate signedCertificate)
        {
            DateTime expiredTime = DateTime.MinValue;
            // 构造请求对象
            AlibabaCloud.SDK.Cas20200407.Models.GetUserCertificateDetailRequest getUserCertificateDetailRequest = new AlibabaCloud.SDK.Cas20200407.Models
                .GetUserCertificateDetailRequest
            {
                CertId = signedCertificate.CertID,
                CertFilter = true
            };
            // 构造运行时参数
            AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions();
            try
            {
                // 获取响应对象
                AlibabaCloud.SDK.Cas20200407.Models.GetUserCertificateDetailResponseBody body = Client.GetUserCertificateDetailWithOptions(getUserCertificateDetailRequest, runtime).Body;
                expiredTime = DateTime.Parse(body.EndDate);
            }
            catch (TeaException error)
            {
                // 此处仅做打印展示，请谨慎对待异常处理，在工程项目中切勿直接忽略异常。
                // 错误 message
                Console.WriteLine(error.Message);
                // 诊断地址
                Console.WriteLine(error.Data["Recommend"]);
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 此处仅做打印展示，请谨慎对待异常处理，在工程项目中切勿直接忽略异常。
                // 错误 message
                Console.WriteLine(error.Message);
                // 诊断地址
                Console.WriteLine(error.Data["Recommend"]);
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }
            return expiredTime;
        }

        /// <summary>
        /// 获取账户下所有已签发证书数组。
        /// </summary>
        /// <returns>包含所有该账号下已签发证书的<see cref="SignedCertificate"/>实例数组。</returns>
        public SignedCertificate[] GetAllSignedCertificates()
        {
            List<SignedCertificate> certs = new List<SignedCertificate>();
            AlibabaCloud.SDK.Cas20200407.Models.ListUserCertificateOrderRequest listUserCertificateOrderRequest = new AlibabaCloud.SDK.Cas20200407.Models.ListUserCertificateOrderRequest
            {
                Status = "ISSUED",
                OrderType = "CERT",
                ShowSize = 25565
            };
            AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions();
            try
            {
                AlibabaCloud.SDK.Cas20200407.Models.ListUserCertificateOrderResponseBody body = Client.ListUserCertificateOrderWithOptions(listUserCertificateOrderRequest, runtime).Body;
                for (int i = 0; i < body.TotalCount; i++) {
                    certs.Add(new SignedCertificate(body.CertificateOrderList[i].CommonName, body.CertificateOrderList[i].CertificateId ?? throw new InvalidOperationException("CertificateID is null.\n It might caused by bad API argument."), body.CertificateOrderList[i].Fingerprint));
                }
            }
            catch (TeaException error)
            {
                // 此处仅做打印展示，请谨慎对待异常处理，在工程项目中切勿直接忽略异常。
                // 错误 message
                Console.WriteLine(error.Message);
                // 诊断地址
                Console.WriteLine(error.Data["Recommend"]);
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 此处仅做打印展示，请谨慎对待异常处理，在工程项目中切勿直接忽略异常。
                // 错误 message
                Console.WriteLine(error.Message);
                // 诊断地址
                Console.WriteLine(error.Data["Recommend"]);
                AlibabaCloud.TeaUtil.Common.AssertAsString(error.Message);
            }
            return [.. certs];
        }

    }
}
