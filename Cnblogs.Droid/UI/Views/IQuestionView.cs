using Cnblogs.Droid.Model;

namespace Cnblogs.Droid.UI.Views
{
    public interface IQuestionView
    {
        void GetServiceQuestionSuccess(QuestionsModel model);
        void GetServiceQuestionFail(string msg);
        void GetClientQuestionSuccess(QuestionsModel model);
    }
}