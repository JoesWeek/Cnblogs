using Cnblogs.Droid.Model;
using System.Collections.Generic;

namespace Cnblogs.Droid.UI.Views
{
    public interface IBlogView
    {
        void GetServiceBlogFail(string msg);
        void GetServiceBlogSuccess(BlogModel model);
        void GetServiceBlogPostsFail(string msg);
        void GetServiceBlogPostsSuccess(List<ArticlesModel> lists);
        void GetClientBlogSuccess(BlogModel model);
        void GetClientBlogPostsSuccess(List<ArticlesModel> lists);
    }
}