using Cnblogs.Droid.Model;
using System.Threading.Tasks;

namespace Cnblogs.Droid.Presenter
{
    public interface IQuestionAnswersPresenter
    {
        Task GetAnswers(AccessToken token, int id, int pageIndex = 1);
        void DeleteAnswer(AccessToken token, int questionId, int answerId);
    }
}