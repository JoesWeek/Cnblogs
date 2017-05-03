using Cnblogs.Droid.Model;

namespace Cnblogs.Droid.Presenter
{
    public interface IQuestionAddPresenter
    {
        void QuestionAdd(AccessToken token, string title, string content, string tags, int flags);
    }
}