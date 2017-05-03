using Android.Views;
using Cnblogs.Droid.Model;
using System.Collections.Generic;

namespace Cnblogs.Droid.UI.Views
{
    public interface IQuestionAnswersView
    {
        void GetAnswersFail(string msg);
        void GetAnswersSuccess(List<QuestionAnswersModel> answers);
        void DeleteAnswerFail(int answerId, string msg);
        void DeleteAnswerSuccess(int answerId);
    }
}