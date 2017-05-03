using Cnblogs.Droid.Model;
using System.Threading.Tasks;

namespace Cnblogs.Droid.Presenter
{
    public interface INewsCommentsPresenter
    {
        Task GetComment(AccessToken token, int id, int pageIndex = 1);
        void PostComment(AccessToken token, int id, string content);
        void DeleteComment(AccessToken token, int id);
    }
}