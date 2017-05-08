using System;
using System.Collections.Generic;
using Square.OkHttp3;
using Java.Util.Concurrent;
using Java.Net;
using System.Threading.Tasks;
using Cnblogs.Droid.Model;
using System.Text;
using System.Security.Authentication;
using System.Net.Http;
using Com.Umeng.Socialize.Utils;
using Com.Umeng.Analytics;
using Android.Content;

namespace Cnblogs.Droid.Utils
{
    public class OkHttpUtils
    {
        private OkHttpClient okHttpClient;
        public static Context Context { get; set; }

        public OkHttpUtils(AccessToken token)
        {
            okHttpClient = new OkHttpClient.Builder()
                .ConnectTimeout(10, TimeUnit.Seconds)
                .ReadTimeout(30, TimeUnit.Seconds)
                .WriteTimeout(10, TimeUnit.Seconds)
                .Authenticator(new UserAuthenticator(token))
                .Build();
        }

        public static OkHttpUtils Instance(AccessToken token)
        {
            return new OkHttpUtils(token);
        }
        public async Task<ResultMessage> GetAsyn(string url)
        {
            Request request = new Request.Builder().Url(url).Build();
            Response response = await okHttpClient.NewCall(request).ExecuteAsync();
            var body = await response.Body().StringAsync();
            var code = response.Code();
            switch (code)
            {
                case (int)System.Net.HttpStatusCode.OK:
                    return new ResultMessage() { IsError = false, Message = body };
                case (int)System.Net.HttpStatusCode.NotFound:
                    MobclickAgent.ReportError(Context, body);
                    return new ResultMessage() { IsError = true, Message = "404 NotFound" };
                case (int)System.Net.HttpStatusCode.Unauthorized:
                    MobclickAgent.ReportError(Context, body);
                    return new ResultMessage() { IsError = true, Message = "401 Unauthorized" };
                case (int)System.Net.HttpStatusCode.InternalServerError:
                    MobclickAgent.ReportError(Context, body);
                    return new ResultMessage() { IsError = true, Message = "500 InternalServerError" };
                case (int)System.Net.HttpStatusCode.BadGateway:
                    MobclickAgent.ReportError(Context, body);
                    return new ResultMessage() { IsError = true, Message = "502 BadGateway" };
                default:
                    MobclickAgent.ReportError(Context, body);
                    return new ResultMessage() { IsError = true, Message = "网络链接不可用 ,请稍后再试" };
            }
        }
        public void Post(string url, string basic, List<Param> param, Action<ICall, Response> onResponse, Action<ICall, Java.IO.IOException> onFailure)
        {
            FormBody.Builder builder = new FormBody.Builder();
            foreach (var item in param)
            {
                builder.Add(item.Key, item.Value);
            }
            Request request = new Request.Builder().AddHeader("Authorization", basic).Url(url).Post(builder.Build()).Build();
            okHttpClient.NewCall(request).Enqueue(onResponse, onFailure);
        }
        public void Post(string url, List<Param> param, Action<ICall, Response> onResponse, Action<ICall, Java.IO.IOException> onFailure)
        {
            FormBody.Builder builder = new FormBody.Builder();
            foreach (var item in param)
            {
                builder.Add(item.Key, item.Value);
            }
            var body = builder.Build();
            Request request = new Request.Builder().Url(url).Post(body).Build();
            okHttpClient.NewCall(request).Enqueue(onResponse, onFailure);
        }
        public void Post(string url, string content, Action<ICall, Response> onResponse, Action<ICall, Java.IO.IOException> onFailure)
        {
            var body = RequestBody.Create(MediaType.Parse("text/plain"), Encoding.UTF8.GetBytes(content));

            Request request = new Request.Builder().Url(url).Post(body).Build();
            okHttpClient.NewCall(request).Enqueue(onResponse, onFailure);
        }
        public void Patch(string url, List<Param> param, Action<ICall, Response> onResponse, Action<ICall, Java.IO.IOException> onFailure)
        {
            FormBody.Builder builder = new FormBody.Builder();
            foreach (var item in param)
            {
                builder.Add(item.Key, item.Value);
            }
            var body = builder.Build();
            Request request = new Request.Builder().Url(url).Patch(body).Build();
            okHttpClient.NewCall(request).Enqueue(onResponse, onFailure);
        }
        public void Delete(string url, Action<ICall, Response> onResponse, Action<ICall, Java.IO.IOException> onFailure)
        {
            Request request = new Request.Builder().Url(url).Delete().Build();
            okHttpClient.NewCall(request).Enqueue(onResponse, onFailure);
            okHttpClient.Dispose();
        }

        public async Task<bool> HeadAsyn(string url)
        {
            Request request = new Request.Builder().Url(url).Head().Build();
            Response response = await okHttpClient.NewCall(request).ExecuteAsync();
            var code = response.Code();
            okHttpClient.Dispose();
            return code == (int)System.Net.HttpStatusCode.OK ? true : false;
        }
        public class Param
        {
            public string Key { get; set; }
            public string Value { get; set; }

            public Param(string key, string value)
            {
                this.Key = key;
                this.Value = value;
            }
        }
        public class ResultMessage
        {
            public bool IsError { get; set; }
            public string Message { get; set; }
        }
    }
    public class UserAuthenticator : Java.Lang.Object, IAuthenticator
    {
        private AccessToken token;
        private int connectCount = 0;
        public UserAuthenticator(AccessToken token)
        {
            this.token = token;
        }
        public Request Authenticate(Route route, Response response)
        {
            if (connectCount++ > 0)
            {
                return null;
            }
            else
            {
                return response.Request().NewBuilder().Header("Authorization", token.token_type + " " + token.access_token).Build();
            }
        }

        public Request AuthenticateProxy(Proxy proxy, Response response)
        {
            return null;
        }
    }
}