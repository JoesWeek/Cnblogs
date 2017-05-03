using Android.Views;
using Cnblogs.Droid.Model;
using System.Collections.Generic;

namespace Cnblogs.Droid.UI.Views
{
    public interface IStatusCommentsView
    {
        void GetServiceCommentsFail(string msg);
        void GetServiceCommentsSuccess(List<StatusCommentsModel> lists);
        void PostCommentFail(string msg);
        void PostCommentSuccess(StatusCommentsModel comment);
        void DeleteCommentFail(int id, string msg);
        void DeleteCommentSuccess(int id);
    }
}