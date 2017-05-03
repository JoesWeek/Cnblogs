using SQLite;
using System;
using System.Collections.Generic;

namespace Cnblogs.Droid.Model
{
    public class StatusModel
    {
        [PrimaryKey, Indexed]
        public int Id { get; set; }
        public string Content { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsLucky { get; set; }
        public int CommentCount { get; set; }
        public string UserAlias { get; set; }
        public string UserDisplayName { get; set; }
        public DateTime DateAdded { get; set; }
        public string UserIconUrl { get; set; }
        public int UserId { get; set; }
        public Guid UserGuid { get; set; }
        [Ignore]
        public int ParentCommentId { get; set; }
        [Ignore]
        public string ParentCommentContent { get; set; }
        [Ignore]
        public int StatusId { get; set; }
        [Ignore]
        public string StatusUserAlias { get; set; }
        [Ignore]
        public string StatusContent { get; set; }
        [Ignore]
        public List<StatusCommentsModel> Comments { get; set; }
    }
}