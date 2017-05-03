using SQLite;
using System;

namespace Cnblogs.Droid.Model
{
    public class QuestionCommentsModel
    {
        [PrimaryKey, Indexed]
        public int CommentID { get; set; }
        public string Content { get; set; }
        public string ConvertedContent { get; set; }
        public int FormatType { get; set; }
        public int ParentCommentId { get; set; }
        public int PostUserID { get; set; }
        public string PostUserName { get; set; }
        public string PostUserQTitle { get; set; }
        public DateTime DateAdded { get; set; }
        public int ContentType { get; set; }
        public int ContentID { get; set; }
        public QuestionUserInfoModel PostUserInfo { get; set; }
        public int DiggCount { get; set; }
        public int BuryCount { get; set; }
    }
}