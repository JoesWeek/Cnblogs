using Cnblogs.Droid.Model;

namespace Cnblogs.Droid.UI.Views
{
    public interface IQuestionAnswersAddView
    {
        void AnswersAddFail(string msg);
        void AnswersAddSuccess(QuestionAnswersModel model);
        void CheckAnswersUserFail(string msg);
        void CheckAnswersUserSuccess();
    }
}