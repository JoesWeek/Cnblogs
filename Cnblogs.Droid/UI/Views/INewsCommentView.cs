using Android.Views;
using Cnblogs.Droid.Model;
using System.Collections.Generic;

namespace Cnblogs.Droid.UI.Views
{
    public interface INewsCommentView
    {
        void GetCommentFail(string msg);
        void GetCommentSuccess(List<NewsCommentModel> comments);
        void PostCommentFail(string msg);
        void PostCommentSuccess(NewsCommentModel article);
        void DeleteCommentFail(int id, string msg);
        void DeleteCommentSuccess(int id);
    }
}