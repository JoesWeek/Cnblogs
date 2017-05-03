using Cnblogs.Droid.Model;
using Cnblogs.Droid.UI.Views;
using Cnblogs.Droid.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cnblogs.Droid.Presenter
{
    public class BlogPresenter : IBlogPresenter
    {
        private IBlogView blogView;
        public BlogPresenter(IBlogView blogView)
        {
            this.blogView = blogView;
        }
        public async Task GetServiceBlog(AccessToken token, string blogApp)
        {
            try
            {
                var result = await OkHttpUtils.Instance(token).GetAsyn(string.Format(ApiUtils.BlogApp, blogApp));
                if (result.IsError)
                {
                    blogView.GetServiceBlogFail(result.Message);
                }
                else
                {
                    var blog = JsonConvert.DeserializeObject<BlogModel>(result.Message);
                    blog.BlogApp = blogApp;
                    await SQLiteUtils.Instance().UpdateBlog(blog);
                    blogView.GetServiceBlogSuccess(blog);
                }
            }
            catch (Exception ex)
            {
                blogView.GetServiceBlogFail(ex.Message);
            }
        }
        public async Task GetServiceBlogPosts(AccessToken token, string blogApp, int pageIndex)
        {
            try
            {
                var result = await OkHttpUtils.Instance(token).GetAsyn(string.Format(ApiUtils.BlogPosts, blogApp, pageIndex));
                if (result.IsError)
                {
                    blogView.GetServiceBlogPostsFail(result.Message);
                }
                else
                {
                    var articles = JsonConvert.DeserializeObject<List<ArticlesModel>>(result.Message);
                    await SQLiteUtils.Instance().UpdateArticles(articles);
                    blogView.GetServiceBlogPostsSuccess(articles);
                }
            }
            catch (Exception ex)
            {
                blogView.GetServiceBlogPostsFail(ex.Message);
            }
        }
        public async Task GetClientBlog(string blogApp)
        {
            blogView.GetClientBlogSuccess(await SQLiteUtils.Instance().QueryBlog(blogApp));
        }
        public async Task GetClientBlogPosts(string blogApp)
        {
            blogView.GetClientBlogPostsSuccess(await SQLiteUtils.Instance().QueryArticlesByBlogApp(blogApp));
        }
    }
}