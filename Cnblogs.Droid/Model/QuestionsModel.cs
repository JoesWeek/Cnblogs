using SQLite;
using System;

namespace Cnblogs.Droid.Model
{
    public class QuestionsModel
    {
        [PrimaryKey, Indexed]
        public int Qid { get; set; }
        public int DealFlag { get; set; }
        public int ViewCount { get; set; }
        public string Summary { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DateAdded { get; set; }
        public string Supply { get; set; }
        public string ConvertedContent { get; set; }
        public int FormatType { get; set; }
        public string Tags { get; set; }
        public int AnswerCount { get; set; }
        public int UserId { get; set; }
        public int Award { get; set; }
        public int DiggCount { get; set; }
        public int BuryCount { get; set; }
        public bool IsBest { get; set; }
        public string AnswerUserId { get; set; }
        public int Flags { get; set; }
        public string DateEnded { get; set; }
        public int UserInfoID { get; set; }
        [Ignore]
        public QuestionUserInfoModel QuestionUserInfo { get; set; }
        public int AdditionID { get; set; }
        [Ignore]
        public AdditionModel Addition { get; set; }
    }
}