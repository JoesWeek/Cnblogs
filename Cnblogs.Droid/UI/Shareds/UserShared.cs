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
using Cnblogs.Droid.Model;

namespace Cnblogs.Droid.UI.Shareds
{
    public class UserShared
    {
        private const string Tag = "UserAccessToken";

        private const string KeyAccessToken = "access_token";
        private const string KeyTokenType = "token_type";
        private const string KeyExpiresIn = "expires_in";
        private const string KeyIsIdentityUser = "IsIdentityUser";
        private const string KeyRefreshTime = "RefreshTime";

        public static void Update(Context context, AccessToken token)
        {
            BaseShared.Instance(context, Tag).SetString(KeyAccessToken, token.access_token);
            BaseShared.Instance(context, Tag).SetString(KeyTokenType, token.token_type);
            BaseShared.Instance(context, Tag).SetInt(KeyExpiresIn, token.expires_in);
            BaseShared.Instance(context, Tag).SetBool(KeyIsIdentityUser, token.IsIdentityUser);
            BaseShared.Instance(context, Tag).SetDateTime(KeyRefreshTime, token.RefreshTime);
        }
        public static AccessToken GetAccessToken(Context context)
        {
            return new AccessToken()
            {
                access_token = BaseShared.Instance(context, Tag).GetString(KeyAccessToken, ""),
                token_type = BaseShared.Instance(context, Tag).GetString(KeyTokenType, ""),
                expires_in = BaseShared.Instance(context, Tag).GetInt(KeyExpiresIn, 0),
                IsIdentityUser = BaseShared.Instance(context, Tag).GetBool(KeyIsIdentityUser, false),
                RefreshTime = BaseShared.Instance(context, Tag).GetDateTime(KeyRefreshTime),
            };
        }
    }
}