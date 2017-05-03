using Android.Views;
using Cnblogs.Droid.Model;
using System.Collections.Generic;

namespace Cnblogs.Droid.UI.Views
{
    public interface IQuestionCommentsView
    {
        void GetCommentsFail(string msg);
        void GetCommentsSuccess(List<QuestionCommentsModel> lists);
        void PostCommentFail(string msg);
        void PostCommentSuccess(QuestionCommentsModel comment);
        void DeleteCommentFail(int commentId, string msg);
        void DeleteCommentSuccess(int commentId);
    }
}