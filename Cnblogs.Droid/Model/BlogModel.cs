using SQLite;

namespace Cnblogs.Droid.Model
{
    public class BlogModel
    {
        [PrimaryKey, Indexed]
        public int BlogId { get; set; }
        [Indexed]
        public string BlogApp { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public int PostCount { get; set; }
        public int PageSize { get; set; }
        public bool EnableScript { get; set; }
    }
}