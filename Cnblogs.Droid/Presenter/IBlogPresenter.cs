using Cnblogs.Droid.Model;
using System.Threading.Tasks;

namespace Cnblogs.Droid.Presenter
{
    public interface IBlogPresenter
    {
        Task GetServiceBlog(AccessToken token, string blogApp);
        Task GetServiceBlogPosts(AccessToken token, string blogApp,int pageIndex);
        Task GetClientBlog(string blogApp);
        Task GetClientBlogPosts(string blogApp);
    }
}