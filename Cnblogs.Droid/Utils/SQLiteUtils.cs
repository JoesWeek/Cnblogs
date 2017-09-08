using Android.OS;
using Android.Util;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Cnblogs.Droid.Utils
{
    public class SQLiteUtils
    {
        private static readonly object _lock = new object();
        public class Database : SQLiteAsyncConnection
        {
            public Database(string path) : base(path)
            {
                CreateTable();
            }
            public async void CreateTable()
            {
                await CreateTableAsync<Model.UserModel>().ContinueWith((results) =>
                {
                    Log.Error("CreateTable", "Create UserModel Table Success");
                });
                await CreateTableAsync<Model.ArticlesModel>().ContinueWith((results) =>
                {
                    Log.Error("CreateTable", "Create ArticlesModel Table Success");
                });
                await CreateTableAsync<Model.NewsModel>().ContinueWith((results) =>
                {
                    Log.Error("CreateTable", "Create NewsModel Table Success");
                });
                await CreateTableAsync<Model.KbArticlesModel>().ContinueWith((results) =>
                {
                    Log.Error("CreateTable", "Create KbArticlesModel Table Success");
                });
                await CreateTableAsync<Model.StatusModel>().ContinueWith((results) =>
                {
                    Log.Error("CreateTable", "Create StatusModel Table Success");
                });
                await CreateTableAsync<Model.QuestionsModel>().ContinueWith((results) =>
                {
                    Log.Error("CreateTable", "Create QuestionsModel Table Success");
                });
                await CreateTableAsync<Model.QuestionUserInfoModel>().ContinueWith((results) =>
                {
                    Log.Error("CreateTable", "Create QuestionUserInfoModel Table Success");
                });
                await CreateTableAsync<Model.AdditionModel>().ContinueWith((results) =>
                {
                    Log.Error("CreateTable", "Create AdditionModel Table Success");
                });
                await CreateTableAsync<Model.BookmarksModel>().ContinueWith((results) =>
                {
                    Log.Error("CreateTable", "Create BookmarksModel Table Success");
                });
                await CreateTableAsync<Model.BlogModel>().ContinueWith((results) =>
                {
                    Log.Error("CreateTable", "Create BlogModel Table Success");
                });
            }

            #region ArticlesModel
            public async Task<Model.ArticlesModel> QueryArticle(int id)
            {
                return await Table<Model.ArticlesModel>().Where(a => a.Id == id).FirstOrDefaultAsync();
            }
            public async Task<List<Model.ArticlesModel>> QueryArticles(int pageSize)
            {
                return await Table<Model.ArticlesModel>().OrderByDescending(a => a.PostDate).Skip(0).Take(pageSize).ToListAsync();
            }
            public async Task<List<Model.ArticlesModel>> QueryArticlesByBlogApp(string blogApp)
            {
                return await Table<Model.ArticlesModel>().Where(a => a.BlogApp == blogApp).OrderByDescending(a => a.PostDate).Skip(0).Take(10).ToListAsync();
            }
            public async Task<List<Model.ArticlesModel>> QueryArticlesByDigg(int pageSize)
            {
                return await Table<Model.ArticlesModel>().Where(a => a.DiggCount > 20).OrderByDescending(a => a.PostDate).Skip(0).Take(pageSize).ToListAsync();
            }
            public async Task UpdateArticles(List<Model.ArticlesModel> lists)
            {
                foreach (var item in lists)
                {
                    if (await QueryArticle(item.Id) == null)
                    {
                        await InsertAsync(item);
                    }
                    else
                    {
                        await UpdateAsync(item);
                    }
                }
            }
            public async Task UpdateArticle(Model.ArticlesModel model)
            {
                await UpdateAsync(model);
            }
            #endregion

            #region UserModel
            public async Task<Model.UserModel> QueryUser()
            {
                return await Table<Model.UserModel>().FirstOrDefaultAsync();
            }
            public async Task UpdateUser(Model.UserModel model)
            {
                await DeleteAsync(model).ContinueWith(async (response) =>
                {
                    await InsertAsync(model);
                });
            }
            /// <summary>
            /// 删除用户相关信息
            /// </summary>
            /// <returns></returns>
            public async Task DeleteUserAll()
            {
                var user = await QueryUser();
                if (user != null)
                {
                    var books = await Table<Model.BookmarksModel>().ToListAsync();
                    if (books != null && books.Count > 0)
                    {
                        books.ForEach(async b => await DeleteAsync(b));
                    }
                    var status = await Table<Model.StatusModel>().Where(a => a.UserGuid == user.UserId).ToListAsync();
                    if (status != null && status.Count > 0)
                    {
                        status.ForEach(async s => await DeleteAsync(s));
                    }
                    await DeleteAsync(user);
                }
            }
            #endregion

            #region NewsModel
            public async Task<Model.NewsModel> QueryNew(int id)
            {
                return await Table<Model.NewsModel>().Where(a => a.Id == id).FirstOrDefaultAsync();
            }
            public async Task<List<Model.NewsModel>> QueryNews(int pageSize)
            {
                return await Table<Model.NewsModel>().OrderByDescending(a => a.DateAdded).Skip(0).Take(pageSize).ToListAsync();
            }
            public async Task<List<Model.NewsModel>> QueryNewsByRecommend(int pageSize)
            {
                return await Table<Model.NewsModel>().Where(a => a.IsRecommend).OrderByDescending(a => a.DateAdded).Skip(0).Take(pageSize).ToListAsync();
            }
            public async Task<List<Model.NewsModel>> QueryNewsByWorkHot(int pageSize, DateTime startdate)
            {
                return await Table<Model.NewsModel>().Where(a => a.IsHot && a.DateAdded > startdate).OrderByDescending(a => a.DateAdded).Skip(0).Take(pageSize).ToListAsync();
            }
            public async Task UpdateNews(List<Model.NewsModel> lists)
            {
                foreach (var item in lists)
                {
                    if (await QueryNew(item.Id) == null)
                    {
                        await InsertAsync(item);
                    }
                    else
                    {
                        await UpdateAsync(item);
                    }
                }
            }
            public async Task UpdateNew(Model.NewsModel model)
            {
                await UpdateAsync(model);
            }
            #endregion

            #region KbArticlesModel
            public async Task<Model.KbArticlesModel> QueryKbArticle(int id)
            {
                return await Table<Model.KbArticlesModel>().Where(a => a.Id == id).FirstOrDefaultAsync();
            }
            public async Task<List<Model.KbArticlesModel>> QueryKbArticles(int pageSize)
            {
                return await Table<Model.KbArticlesModel>().OrderByDescending(a => a.DateAdded).Skip(0).Take(pageSize).ToListAsync();
            }
            public async Task UpdateKbArticles(List<Model.KbArticlesModel> lists)
            {
                foreach (var item in lists)
                {
                    if (await QueryKbArticle(item.Id) == null)
                    {
                        await InsertAsync(item);
                    }
                    else
                    {
                        await UpdateAsync(item);
                    }
                }
            }
            public async Task UpdateKbArticle(Model.KbArticlesModel model)
            {
                await UpdateAsync(model);
            }
            #endregion

            #region StatusModel
            public async Task<Model.StatusModel> QueryStatus(int id)
            {
                return await Table<Model.StatusModel>().Where(a => a.Id == id).FirstOrDefaultAsync();
            }
            public async Task<List<Model.StatusModel>> QueryStatuses(int pageSize)
            {
                return await Table<Model.StatusModel>().OrderByDescending(a => a.DateAdded).Skip(0).Take(pageSize).ToListAsync();
            }
            public async Task DeleteStatusByUser(Guid id)
            {
                var lists = await Table<Model.StatusModel>().Where(a => a.UserGuid == id).ToListAsync();
                lists.ForEach(async s => await DeleteAsync(s));
            }
            public async Task UpdateStatuses(List<Model.StatusModel> lists)
            {
                foreach (var item in lists)
                {
                    if (await QueryStatus(item.Id) == null)
                    {
                        await InsertAsync(item);
                    }
                    else
                    {
                        await UpdateAsync(item);
                    }
                }
            }
            public async Task UpdateStatus(Model.StatusModel model)
            {
                await UpdateAsync(model);
            }
            #endregion

            #region QuestionsModel
            public async Task<Model.QuestionsModel> QueryQuestion(int id)
            {
                var model = await Table<Model.QuestionsModel>().Where(a => a.Qid == id).FirstOrDefaultAsync();
                if (model != null && model.UserInfoID > 0)
                {
                    model.QuestionUserInfo = await QueryQuestionUserInfo(model.UserInfoID);
                }
                if (model != null && model.AdditionID > 0)
                {
                    model.Addition = await QueryAddition(model.AdditionID);
                }
                return model;
            }
            public async Task<List<Model.QuestionsModel>> QueryQuestions(int pageSize)
            {
                var list = await Table<Model.QuestionsModel>().OrderByDescending(a => a.DateAdded).Skip(0).Take(pageSize).ToListAsync();
                list.ForEach(async (q) =>
                {
                    q.QuestionUserInfo = await QueryQuestionUserInfo(q.UserInfoID);
                    q.Addition = await QueryAddition(q.AdditionID);
                });
                return list;
            }
            public async Task<List<Model.QuestionsModel>> QueryQuestionsByType(int type, int pageSize)
            {
                List<Model.QuestionsModel> list = new List<Model.QuestionsModel>();
                switch (type)
                {
                    case 0:
                        list = await Table<Model.QuestionsModel>().OrderByDescending(a => a.DateAdded).Skip(0).Take(pageSize).ToListAsync();
                        break;
                    case 1:
                        list = await Table<Model.QuestionsModel>().Where(a => a.Award >= 50).OrderByDescending(a => a.DateAdded).Skip(0).Take(pageSize).ToListAsync();
                        break;
                    case 2:
                        list = await Table<Model.QuestionsModel>().Where(a => a.AnswerCount == 0).OrderByDescending(a => a.DateAdded).Skip(0).Take(pageSize).ToListAsync();
                        break;
                    case 3:
                        list = await Table<Model.QuestionsModel>().Where(a => a.DealFlag == 1).OrderByDescending(a => a.DateAdded).Skip(0).Take(pageSize).ToListAsync();
                        break;
                    case 4:
                        return list;
                }
                list.ForEach(async (q) =>
                {
                    q.QuestionUserInfo = await QueryQuestionUserInfo(q.UserInfoID);
                    q.Addition = await QueryAddition(q.AdditionID);
                });
                return list;
            }
            public async Task UpdateQuestions(List<Model.QuestionsModel> lists)
            {
                foreach (var item in lists)
                {
                    await UpdateQuestion(item);
                }
            }
            public async Task UpdateQuestion(Model.QuestionsModel model)
            {
                model.UserInfoID = model.QuestionUserInfo.UserID;
                await UpdateQuestionUserInfo(model.QuestionUserInfo);
                if (model.Addition != null)
                {
                    model.AdditionID = model.Addition.QID;
                    await UpdateAddition(model.Addition);
                }
                if (await QueryQuestion(model.Qid) == null)
                {
                    await InsertAsync(model);
                }
                else
                {
                    await UpdateAsync(model);
                }
            }
            public async Task DeleteQuestions()
            {
                await DeleteAsync(await Table<Model.QuestionsModel>().ToListAsync());
            }
            #endregion

            #region QuestionUserInfoModel
            public async Task<Model.QuestionUserInfoModel> QueryQuestionUserInfo(int id)
            {
                return await Table<Model.QuestionUserInfoModel>().Where(a => a.UserID == id).FirstOrDefaultAsync();
            }
            public async Task UpdateQuestionUserInfo(Model.QuestionUserInfoModel model)
            {
                if (await QueryQuestionUserInfo(model.UserID) == null)
                {
                    await InsertAsync(model);
                }
                else
                {
                    await UpdateAsync(model);
                }
            }
            #endregion

            #region AdditionModel
            public async Task<Model.AdditionModel> QueryAddition(int id)
            {
                return await Table<Model.AdditionModel>().Where(a => a.QID == id).FirstOrDefaultAsync();
            }
            public async Task UpdateAddition(Model.AdditionModel model)
            {
                if (await QueryAddition(model.QID) == null)
                {
                    await InsertAsync(model);
                }
                else
                {
                    await UpdateAsync(model);
                }
            }
            #endregion

            #region BookmarksModel
            public async Task<Model.BookmarksModel> QueryBookmark(int id)
            {
                return await Table<Model.BookmarksModel>().Where(a => a.WzLinkId == id).FirstOrDefaultAsync();
            }
            public async Task<List<Model.BookmarksModel>> QueryBookmarks(int pageSize)
            {
                return await Table<Model.BookmarksModel>().OrderByDescending(a => a.DateAdded).Skip(0).Take(pageSize).ToListAsync();
            }
            public async Task<int> DeleteBookmark(int id)
            {
                var item = await QueryBookmark(id);
                var count = await DeleteAsync(item);
                return count;
            }
            public async Task DeleteBookmarks()
            {
                await DeleteAsync(await Table<Model.BookmarksModel>().ToListAsync());
            }
            public async Task UpdateBookmarks(List<Model.BookmarksModel> lists)
            {
                foreach (var item in lists)
                {
                    if (await QueryBookmark(item.WzLinkId) == null)
                    {
                        await InsertAsync(item);
                    }
                    else
                    {
                        await UpdateAsync(item);
                    }
                }
            }
            public async Task UpdateBookmark(Model.BookmarksModel model)
            {
                await UpdateAsync(model);
            }
            #endregion

            #region BlogModel
            public async Task<Model.BlogModel> QueryBlog(int id)
            {
                return await Table<Model.BlogModel>().Where(a => a.BlogId == id).FirstOrDefaultAsync();
            }
            public async Task<Model.BlogModel> QueryBlog(string blogApp)
            {
                return await Table<Model.BlogModel>().Where(a => a.BlogApp == blogApp).FirstOrDefaultAsync();
            }
            public async Task UpdateBlog(Model.BlogModel model)
            {
                await UpdateAsync(model);
            }
            #endregion
        }
        private static Database instance;
        public static Database Instance()
        {
            if (instance == null)
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "cnblogs.db");
                        instance = new Database(dbPath);
                    }
                }
            }
            return instance;
        }
    }
}