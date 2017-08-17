using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using W.WebApi.Helper;

namespace W.WebApi.Handlers
{
    public class PreprocessHandler: DelegatingHandler
    {
        private Task<HttpResponseMessage> failureResponseTask(Result result)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            response.Content = new StringContent(result.toString(), Encoding.UTF8, "application/json");

            var tsc = new TaskCompletionSource<HttpResponseMessage>();
            tsc.SetResult(response);
            return tsc.Task;
        }
        private Task<HttpResponseMessage> failureResponseTask(string message)
        {
            return failureResponseTask(Result.FAILURE(message));
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //检查签名
            //Result resultCheck = TokenHelper.Check(request);

            //if (resultCheck.success == false)
            //{
            //    return failureResponseTask(resultCheck);
            //}

            ////解码Request Body
            //string requestBody = request.Content.ReadAsStringAsync().Result;
            //if (requestBody.Length > 0 && request.Method != HttpMethod.Get)
            //{
            //    try
            //    {
            //        string privateKey = TokenHelper.getApp(request.Headers).key;
            //        string content = SecurityHelper.decrypt3DES(requestBody, privateKey);
            //        request.Content = new StringContent(content, Encoding.UTF8, "application/json");
            //    }
            //    catch (Exception)
            //    {
            //        return failureResponseTask("解析参数错误:BODY");
            //    }
            //}

            return base.SendAsync(request, cancellationToken);
        }
    }
}
