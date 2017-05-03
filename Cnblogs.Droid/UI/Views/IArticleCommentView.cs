using Cnblogs.Droid.Model;
using System.Collections.Generic;

namespace Cnblogs.Droid.UI.Views
{
    public interface IArticleCommentView
    {
        void GetCommentFail(string msg);
        void GetCommentSuccess(List<ArticleCommentModel> comments);
        void PostCommentFail(string msg);
        void PostCommentSuccess(ArticleCommentModel article);
    }
}