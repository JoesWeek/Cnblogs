using Cnblogs.Droid.Model;
using Cnblogs.Droid.UI.Views;
using Cnblogs.Droid.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Cnblogs.Droid.Presenter
{
    public class LoginPresenter : ILoginPresenter
    {
        private ILoginView loginView;
        public LoginPresenter(ILoginView loginView)
        {
            this.loginView = loginView;
        }
        public void Login(AccessToken token, string basic, string account, string password)
        {
            try
            {
                var param = new List<OkHttpUtils.Param>()
                {
                    new OkHttpUtils.Param("grant_type","password") ,
                    new OkHttpUtils.Param("username",account) ,
                    new OkHttpUtils.Param("password",password)
                };
                OkHttpUtils.Instance(token).Post(ApiUtils.Token, basic, param, async (call, response) =>
                 {
                     var code = response.Code();
                     var body = await response.Body().StringAsync();
                     if (code == (int)System.Net.HttpStatusCode.OK)
                     {
                         token = JsonConvert.DeserializeObject<AccessToken>(body);
                         token.RefreshTime = DateTime.Now;
                         var result = await OkHttpUtils.Instance(token).GetAsyn(ApiUtils.Users);
                         if (result.IsError)
                         {
                             loginView.LoginFail(result.Message);
                         }
                         else
                         {
                             var user = JsonConvert.DeserializeObject<UserModel>(result.Message);
                             await SQLiteUtils.Instance().UpdateUser(user);
                             loginView.LoginSuccess(token, user);
                         }
                     }
                     else
                     {
                         try
                         {
                             var error = JsonConvert.DeserializeObject<LoginErrorMessage>(body);
                             if (error != null && error.Error != null)
                             {
                                 loginView.LoginFail("µÇÂ¼Ê§°Ü,ÓÃ»§Ãû»òÃÜÂë´íÎó");
                             }
                         }
                         catch (Exception e)
                         {
                             loginView.LoginFail(e.Message);
                         }
                     }
                 }, (call, ex) =>
              {
                  loginView.LoginFail(ex.Message);
              });
            }
            catch (Exception ex)
            {
                loginView.LoginFail(ex.Message);
            }
        }
    }
    public class LoginErrorMessage
    {
        public string Error { get; set; }
    }
}