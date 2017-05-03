using Cnblogs.Droid.Model;
using System.Collections.Generic;

namespace Cnblogs.Droid.UI.Views
{
    public interface IQuestionColumnView
    {
        void GetServiceQuestionsFail(string msg);
        void GetServiceQuestionsSuccess(List<QuestionsModel> lists);
        void GetClientQuestionsSuccess(List<QuestionsModel> lists);
    }
}