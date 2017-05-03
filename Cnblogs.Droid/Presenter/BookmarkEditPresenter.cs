using Cnblogs.Droid.Model;
using Cnblogs.Droid.UI.Views;
using Cnblogs.Droid.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Cnblogs.Droid.Presenter
{
    public class BookmarkEditPresenter : IBookmarkEditPresenter
    {
        private IBookmarkEditView bookmarkView;
        public BookmarkEditPresenter(IBookmarkEditView bookmarkView)
        {
            this.bookmarkView = bookmarkView;
        }
        public void BookmarkEdit(AccessToken token, BookmarksModel bookmark,int position)
        {
            try
            {
                var url = string.Format(ApiUtils.BookmarkEdit, bookmark.WzLinkId);

                var param = new List<OkHttpUtils.Param>()
                {
                    new OkHttpUtils.Param("WzLinkId",bookmark.WzLinkId.ToString()) ,
                    new OkHttpUtils.Param("LinkUrl",bookmark.LinkUrl) ,
                    new OkHttpUtils.Param("Title",bookmark.Title) ,
                    new OkHttpUtils.Param("Summary",bookmark.Summary) ,
                    new OkHttpUtils.Param("Tags",bookmark.Tag) ,
                    new OkHttpUtils.Param("DateAdded",bookmark.DateAdded.ToString()) ,
                    new OkHttpUtils.Param("FromCNBlogs",bookmark.FromCNBlogs.ToString()) 
                };

                OkHttpUtils.Instance(token).Patch(url, param, async (call, response) =>
                {
                    var code = response.Code();
                    var body = await response.Body().StringAsync();
                    if (code == (int)System.Net.HttpStatusCode.OK)
                    {
                        await SQLiteUtils.Instance().UpdateBookmark(bookmark);
                        bookmarkView.BookmarkEditSuccess(position);
                    }
                    else
                    {
                        try
                        {
                            var error = JsonConvert.DeserializeObject<ErrorMessage>(body);
                            bookmarkView.BookmarkEditFail(error.Message);
                        }
                        catch (Exception e)
                        {
                            bookmarkView.BookmarkEditFail(e.Message);
                        }
                    }
                }, (call, ex) =>
                {
                    bookmarkView.BookmarkEditFail(ex.Message);
                });
            }
            catch (Exception e)
            {
                bookmarkView.BookmarkEditFail(e.Message);
            }
        }
    }
}