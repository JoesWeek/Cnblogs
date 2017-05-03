using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cnblogs.Droid.Model
{
    public class BookmarksModel
    {
        [PrimaryKey, Indexed]
        public int WzLinkId { get; set; }
        public string Title { get; set; }
        public string LinkUrl { get; set; }
        public string Summary { get; set; }
        public DateTime DateAdded { get; set; }
        private string tag;
        public string Tag
        {
            get
            {
                if (Tags != null)
                {
                    string t = null;
                    for (int i = 0; i < Tags.Count; i++)
                    {
                        t += Tags[i] + ",";
                    }
                    tag = t;
                }
                return tag.TrimEnd(',');
            }
            set { tag = value; }
        }
        [Ignore]
        public List<string> Tags { get; set; }
        public bool FromCNBlogs { get; set; }
    }
}