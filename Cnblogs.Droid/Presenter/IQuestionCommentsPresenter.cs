using Android.Views;
using Cnblogs.Droid.Model;
using System.Threading.Tasks;

namespace Cnblogs.Droid.Presenter
{
    public interface IQuestionCommentsPresenter
    {
        Task GetServiceComments(AccessToken token, int id);
        void PostComment(AccessToken token, int questionId, int answerId, string content);
        void DeleteComment(AccessToken token, int questionId, int answerId, int commentId);
    }
}