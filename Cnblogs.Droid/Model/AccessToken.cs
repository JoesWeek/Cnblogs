using System;

namespace Cnblogs.Droid.Model
{
    public class AccessToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public bool IsIdentityUser { get; set; }
        public string refresh_token { get; set; }
        public DateTime RefreshTime { get; set; }
    }
}
