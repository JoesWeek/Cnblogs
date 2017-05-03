using Cnblogs.Droid.Model;
using Cnblogs.Droid.UI.Views;
using Cnblogs.Droid.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Cnblogs.Droid.Presenter
{
    public class BookmarkAddPresenter : IBookmarkAddPresenter
    {
        private IBookmarkAddView bookmarkView;
        public BookmarkAddPresenter(IBookmarkAddView bookmarkView)
        {
            this.bookmarkView = bookmarkView;
        }
        public void BookmarkAdd(AccessToken token, BookmarksModel bookmark)
        {
            try
            {
                var url = ApiUtils.BookmarkAdd;

                var param = new List<OkHttpUtils.Param>()
                {
                    new OkHttpUtils.Param("LinkUrl",bookmark.LinkUrl) ,
                    new OkHttpUtils.Param("Title",bookmark.Title) ,
                    new OkHttpUtils.Param("Summary",bookmark.Summary) ,
                    new OkHttpUtils.Param("Tags",bookmark.Tag) ,
                    new OkHttpUtils.Param("FromCNBlogs",bookmark.FromCNBlogs.ToString())
                };

                OkHttpUtils.Instance(token).Post(url, param, async (call, response) =>
                {
                    var code = response.Code();
                    var body = await response.Body().StringAsync();
                    if (code == (int)System.Net.HttpStatusCode.Created)
                    {
                        bookmarkView.BookmarkAddSuccess();
                    }
                    else
                    {
                        try
                        {
                            var error = JsonConvert.DeserializeObject<ErrorMessage>(body);
                            bookmarkView.BookmarkAddFail(error.Message);
                        }
                        catch (Exception e)
                        {
                            bookmarkView.BookmarkAddFail(e.Message);
                        }
                    }
                }, (call, ex) =>
                {
                    bookmarkView.BookmarkAddFail(ex.Message);
                });
            }
            catch (Exception e)
            {
                bookmarkView.BookmarkAddFail(e.Message);
            }
        }
    }
}