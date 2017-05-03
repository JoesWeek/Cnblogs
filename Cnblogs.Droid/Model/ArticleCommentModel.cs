using System;
using SQLite;

namespace Cnblogs.Droid.Model
{
    public class ArticleCommentModel
    {
        [PrimaryKey, Indexed]
        public int Id { get; set; }

        public string Body { get; set; }

        public string Author { get; set; }

        public string AuthorUrl { get; set; }

        public string FaceUrl { get; set; }

        public int Floor { get; set; }

        public DateTime DateAdded { get; set; }
    }
}