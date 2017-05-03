using Cnblogs.Droid.Model;

namespace Cnblogs.Droid.Presenter
{
    public interface IQuestionAnswersAddPresenter
    {
        void CheckAnswersByUser(AccessToken token, int questionId);
        void AnswersAdd(AccessToken token, int questionId, string content);
    }
}