namespace Cnblogs.Droid.Utils
{
    public class ApiUtils
    {
        public const string Host = "https://api.cnblogs.com";
        public const string Api = "/api";
        public const string Token = "https://oauth.cnblogs.com/connect/token";
        public const string Authorize = "https://oauth.cnblogs.com/connect/authorize?client_id={0}&scope=openid profile CnBlogsApi offline_access&response_type=code id_token&redirect_uri=https://oauth.cnblogs.com/auth/callback&state=cnblogs.com&nonce=cnblogs.com";
        
        public const string Users = Host + Api + "/Users";

        public const string BlogApp = Host + Api + "/blogs/{0}";
        public const string BlogPosts = Host + Api + "/blogs/{0}/posts?pageIndex={1}";

        public const string ArticleHome = Host + Api + "/blogposts/@sitehome?pageIndex={0}&pageSize={1}";
        public const string ArticleHot = Host + Api + "/blogposts/@picked?pageIndex={0}&pageSize={1}";
        public const string ArticleBody = Host + Api + "/blogposts/{0}/body";
        public const string ArticleComment = Host + Api + "/blogs/{0}/posts/{1}/comments?pageIndex={2}&pageSize={3}";
        public const string ArticleCommentAdd = Host + Api + "/blogs/{0}/posts/{1}/comments";

        public const string News = Host + Api + "/NewsItems?pageIndex={0}&pageSize={1}";
        public const string NewsHome = Host + Api + "/newsitems?pageIndex={0}&pageSize={1}";
        public const string NewsRecommend = Host + Api + "/newsitems/@recommended?pageIndex={0}&pageSize={1}";
        public const string NewsWorkHot = Host + Api + "/newsitems/@hot-week?pageIndex={0}&pageSize={1}";
        public const string NewsBody = Host + Api + "/newsitems/{0}/body";
        public const string NewsComment = Host + Api + "/news/{0}/comments?pageIndex={1}&pageSize={2}";
        public const string NewsCommentAdd = Host + Api + "/news/{0}/comments";
        public const string NewsCommentDelete = Host + Api + "/newscomments/{0}";

        public const string KbArticles = Host + Api + "/KbArticles?pageIndex={0}&pageSize={1}";
        public const string KbArticlesBody = Host + Api + "/kbarticles/{0}/body";

        public const string Status = Host + Api + "/statuses/@{0}?pageIndex={1}&pageSize={2}&tag=";
        public const string StatusBody = Host + Api + "/statuses/{0}";
        public const string StatusADD = Host + Api + "/statuses";
        public const string StatusDelete = Host + Api + "/statuses/{0}";
        public const string StatusComments = Host + Api + "/statuses/{0}/comments";
        public const string StatusCommentAdd = Host + Api + "/statuses/{0}/comments";
        public const string StatusCommentDelete = Host + Api + "/statuses/{0}/comments/{1}";

        public const string Questions = Host + Api + "/questions/@sitehome?pageIndex={0}&pageSize={1}";
        public const string QuestionsType = Host + Api + "/questions/@{0}?pageIndex={1}&pageSize={2}";
        public const string QuestionADD = Host + Api + "/questions";
        public const string QuestionDetails = Host + Api + "/questions/{0}";
        public const string QuestionsAnswers = Host + Api + "/questions/{0}/answers";
        public const string QuestionsAnswerByUser = Host + Api + "/questions/{0}?userId={1}";
        public const string QuestionsAnswerAdd = Host + Api + "/questions/{0}/answers";
        public const string QuestionsAnswerDelete = Host + Api + "/questions/{0}/answers/{1}";
        public const string QuestionsAnswerComments = Host + Api + "/questions/answers/{0}/comments";
        public const string QuestionsAnswerCommentsAdd = Host + Api + "/questions/{0}/answers/{1}/comments";
        public const string QuestionsAnswerCommentsDelete = Host + Api + "/questions/{0}/answers/{1}/comments/{2}";

        public const string Bookmarks = Host + Api + "/Bookmarks?pageIndex={0}&pageSize={1}";
        public const string BookmarkDelete = Host + Api + "/bookmarks/{0}";
        public const string BookmarkEdit = Host + Api + "/bookmarks/{0}";
        public const string BookmarkAdd = Host + Api + "/Bookmarks";

        public const string Search = Host + Api + "/ZzkDocuments/{0}?keyWords={1}&pageIndex={2}&startDate=&endDate=&viewTimesAtLeast=0";

    }
}
