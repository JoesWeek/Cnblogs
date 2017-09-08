using Cnblogs.Droid.Model;
using Cnblogs.Droid.UI.Views;
using Cnblogs.Droid.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Views;

namespace Cnblogs.Droid.Presenter
{
    public class BookmarksPresenter : IBookmarksPresenter
    {
        private IBookmarksView bookmarksView;
        private int pageSize = 10;
        public BookmarksPresenter(IBookmarksView bookmarksView)
        {
            this.bookmarksView = bookmarksView;
        }
        public async Task GetServiceBookmarks(AccessToken token, int pageIndex = 1)
        {
            try
            {
                var result = await OkHttpUtils.Instance(token).GetAsyn(string.Format(ApiUtils.Bookmarks, pageIndex, pageSize));
                if (result.IsError)
                {
                    bookmarksView.GetServiceBookmarksFail(result.Message);
                }
                else
                {
                    var bookmarks = JsonConvert.DeserializeObject<List<BookmarksModel>>(result.Message);
                    await SQLiteUtils.Instance().UpdateBookmarks(bookmarks);
                    bookmarksView.GetServiceBookmarksSuccess(bookmarks);
                }
            }
            catch (Exception ex)
            {
                bookmarksView.GetServiceBookmarksFail(ex.Message);
            }
        }
        public async Task GetClientBookmarks()
        {
            bookmarksView.GetClientBookmarksSuccess(await SQLiteUtils.Instance().QueryBookmarks(pageSize));
        }
        public async void DeleteBookmarkAsync(AccessToken token, int id)
        {
            try
            {
                var url = string.Format(ApiUtils.BookmarkDelete, id);
                await SQLiteUtils.Instance().DeleteBookmark(id);

                OkHttpUtils.Instance(token).Delete(url, async (call, response) =>
                {
                    var code = response.Code();
                    var body = await response.Body().StringAsync();
                    if (code == (int)System.Net.HttpStatusCode.OK)
                    {
                        bookmarksView.DeleteBookmarkSuccess(id);
                    }
                    else
                    {
                        try
                        {
                            var error = JsonConvert.DeserializeObject<ErrorMessage>(body);
                            bookmarksView.DeleteBookmarkFail(id, error.Message);
                        }
                        catch (Exception e)
                        {
                            bookmarksView.DeleteBookmarkFail(id, e.Message);
                        }
                    }
                }, (call, ex) =>
                {
                    bookmarksView.DeleteBookmarkFail(id, ex.Message);
                });
            }
            catch (Exception ex)
            {
                bookmarksView.DeleteBookmarkFail(id, ex.Message);
            }
        }
    }
}