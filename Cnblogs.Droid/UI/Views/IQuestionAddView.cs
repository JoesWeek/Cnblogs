using Cnblogs.Droid.Model;

namespace Cnblogs.Droid.UI.Views
{
    public interface IQuestionAddView
    {
        void QuestionAddFail(string msg);
        void QuestionAddSuccess(QuestionsModel model);
    }
}