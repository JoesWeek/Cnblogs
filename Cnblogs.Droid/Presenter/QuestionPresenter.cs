using Cnblogs.Droid.Model;
using Cnblogs.Droid.UI.Views;
using Cnblogs.Droid.Utils;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Cnblogs.Droid.Presenter
{
    public class QuestionPresenter : IQuestionPresenter
    {
        private IQuestionView questionView;
        public QuestionPresenter(IQuestionView questionView)
        {
            this.questionView = questionView;
        }
        public async Task GetServiceQuestion(AccessToken token, int id)
        {
            try
            {
                var result = await OkHttpUtils.Instance(token).GetAsyn(string.Format(ApiUtils.QuestionDetails, id));
                if (result.IsError)
                {
                    questionView.GetServiceQuestionFail(result.Message);
                }
                else
                {
                    var question = JsonConvert.DeserializeObject<QuestionsModel>(result.Message);
                    await SQLiteUtils.Instance().UpdateQuestion(question);
                    questionView.GetServiceQuestionSuccess(question);
                }
            }
            catch (Exception ex)
            {
                questionView.GetServiceQuestionFail(ex.Message);
            }
        }

        public async Task GetClientQuestion(int id)
        {
            questionView.GetClientQuestionSuccess(await SQLiteUtils.Instance().QueryQuestion(id));
        }
    }
}