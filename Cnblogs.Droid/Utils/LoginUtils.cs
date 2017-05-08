using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Cnblogs.Droid.UI.Shareds;
using System.Threading.Tasks;
using Cnblogs.Droid.Model;

namespace Cnblogs.Droid.Utils
{
    public class LoginUtils
    {
        private static readonly object _lock = new object();
        private Context context;
        private bool loginStatus = false;
        public LoginUtils(Context context)
        {
            this.context = context;
        }
        /// <summary>
        /// 获取登录状态
        /// </summary>
        /// <returns></returns>
        public bool GetLoginStatus()
        {
            //判断是否登录失效
            var user = UserShared.GetAccessToken(context);
            if (user.access_token == "" || user.RefreshTime.AddSeconds(user.expires_in) < DateTime.Now)
            {
                DeleteUser();
                loginStatus = false;
            }
            else
            {
                loginStatus = true;
            }
            return loginStatus;
        }
        /// <summary>
        /// 获取登录的用户信息
        /// </summary>
        /// <returns></returns>
        public async Task<UserModel> GetUser()
        {
            return await SQLiteUtils.Instance().QueryUser();
        }
        /// <summary>
        /// 删除用户信息
        /// </summary>
        public async void DeleteUser()
        {
            UserShared.Update(context, new AccessToken());
            await SQLiteUtils.Instance().DeleteUserAll();
        }
        private static LoginUtils instance;
        public static LoginUtils Instance(Context context)
        {
            if (instance == null)
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new LoginUtils(context);
                    }
                }
            }
            return instance;
        }
    }
}