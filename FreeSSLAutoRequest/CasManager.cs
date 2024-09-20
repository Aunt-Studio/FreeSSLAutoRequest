using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tea;


namespace FreeSSLAutoRequest
{
    internal class CasManager
    {
        /// <term><b>Description:</b></term>
        /// <description>
        /// <para>使用AK&amp;SK初始化账号Client</para>
        /// </description>
        /// 
        /// <returns>
        /// Client
        /// </returns>
        /// 
        /// <term><b>Exception:</b></term>
        /// Exception
        public static AlibabaCloud.SDK.Cas20200407.Client CreateClient(string AKId, string AKSecret)
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

        public static DateTime GetCertExpiredTime(AlibabaCloud.SDK.Cas20200407.Client client, long certId)
        {
            DateTime expiredTime = DateTime.MinValue;
            // 构造请求对象
            AlibabaCloud.SDK.Cas20200407.Models.GetUserCertificateDetailRequest getUserCertificateDetailRequest = new AlibabaCloud.SDK.Cas20200407.Models.
                GetUserCertificateDetailRequest
            {
                CertId = certId,
                CertFilter = true
            };
            // 构造运行时参数
            AlibabaCloud.TeaUtil.Models.RuntimeOptions runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions();
            try
            {
                // 获取响应对象
                AlibabaCloud.SDK.Cas20200407.Models.GetUserCertificateDetailResponseBody body = client.GetUserCertificateDetailWithOptions(getUserCertificateDetailRequest, runtime).Body;
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
    }
}
