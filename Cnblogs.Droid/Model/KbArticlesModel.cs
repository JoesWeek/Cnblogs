using SQLite;
using System;

namespace Cnblogs.Droid.Model
{
    public class KbArticlesModel
    {
        [PrimaryKey, Indexed]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Author { get; set; }
        public DateTime DateAdded { get; set; }
        public int ViewCount { get; set; }
        public int DiggCount { get; set; }
        public string Body { get; set; }
    }
}