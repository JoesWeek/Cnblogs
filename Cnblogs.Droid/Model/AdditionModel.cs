using SQLite;

namespace Cnblogs.Droid.Model
{
    public class AdditionModel
    {
        [PrimaryKey, Indexed]
        public int QID { get; set; }
        public string Content { get; set; }
        public string ConvertedContent { get; set; }
    }
}