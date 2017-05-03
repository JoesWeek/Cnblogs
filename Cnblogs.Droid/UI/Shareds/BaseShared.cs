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
using Android.Text;
using Android.Provider;
using Android.Telephony;
using Cnblogs.Droid.Utils;

namespace Cnblogs.Droid.UI.Shareds
{
    public class BaseShared
    {
        private volatile static string SecretKey;
        private ISharedPreferences sp;
        private BaseShared(Context context, string name)
        {
            sp = context.GetSharedPreferences(GetDigestKey(name), FileCreationMode.Private);
        }

        public static BaseShared Instance(Context context, string name)
        {
            if (TextUtils.IsEmpty(SecretKey))
            {
                lock (typeof(BaseShared))
                {
                    //if (TextUtils.IsEmpty(SecretKey))
                    //{
                    //    try
                    //    {
                    //        TelephonyManager tm = (TelephonyManager)context.GetSystemService(Context.TelephonyService);
                    //        SecretKey = tm.DeviceId;
                    //    }
                    //    catch
                    //    {
                    //    }
                    //}
                    if (TextUtils.IsEmpty(SecretKey))
                    {
                        try
                        {
                            SecretKey = Settings.Secure.GetString(context.ContentResolver, Settings.Secure.AndroidId);
                        }
                        catch
                        {
                        }
                    }
                    if (TextUtils.IsEmpty(SecretKey))
                    {
                        try
                        {
                            SecretKey = Build.Serial;
                        }
                        catch
                        {
                        }
                    }
                    if (TextUtils.IsEmpty(SecretKey))
                    {
                        try
                        {
                            SecretKey = context.PackageName;
                        }
                        catch
                        {
                        }
                    }
                }
            }
            return new BaseShared(context, name);
        }
        private string GetSecretKey()
        {
            return MD5Utils.MD5(SecretKey);
        }
        private string GetDigestKey(string key)
        {
            return MD5Utils.MD5(key);
        }
        public int GetInt(string key, int defValue)
        {
            return Convert.ToInt32(Get(key, defValue.ToString()));
        }
        public string GetString(string key, string defValue)
        {
            return Get(key, defValue);
        }
        public DateTime GetDateTime(string key)
        {
            return Convert.ToDateTime(Get(key, DateTime.MinValue.ToString()));
        }
        public bool GetBool(string key, bool defValue)
        {
            return Convert.ToBoolean(Get(key, defValue.ToString()));
        }
        public string Get(string key, string defValue)
        {
            try
            {
                string spValue = sp.GetString(GetDigestKey(key), "");
                string value = DESUtils.Decrypt(spValue, GetSecretKey());
                if (TextUtils.IsEmpty(value))
                {
                    return defValue;
                }
                else
                {
                    return value;
                }
            }
            catch (Exception e)
            {
                return defValue;
            }
        }
        public void Set(string key, string value)
        {
            ISharedPreferencesEditor editor = sp.Edit();
            try
            {
                string spValue = DESUtils.Encrypt(value.ToString(), GetSecretKey());
                editor.PutString(GetDigestKey(key), spValue);
            }
            catch (Exception e)
            {
                editor.PutString(GetDigestKey(key), "");
            }
            editor.Apply();
        }
        public void SetString(string key, string value)
        {
            Set(key, value);
        }
        public void SetInt(string key, int value)
        {
            Set(key, value.ToString());
        }
        public void SetDateTime(string key, DateTime value)
        {
            Set(key, value.ToString());
        }
        public void SetBool(string key, bool value)
        {
            Set(key, value.ToString());
        }
        public void Clear()
        {
            sp.Edit().Clear().Apply();
        }
    }
}