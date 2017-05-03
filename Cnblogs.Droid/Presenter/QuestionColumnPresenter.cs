using Cnblogs.Droid.Model;
using Cnblogs.Droid.UI.Views;
using Cnblogs.Droid.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cnblogs.Droid.Presenter
{
    public class QuestionColumnPresenter : IQuestionColumnPresenter
    {
        private IQuestionColumnView questionView;
        private int pageSize = 10;
        public QuestionColumnPresenter(IQuestionColumnView questionView)
        {
            this.questionView = questionView;
        }
        public async Task GetServiceQuestions(AccessToken token, int position, int pageIndex = 1)
        {
            try
            {
                string url = "";
                switch (position)
                {
                    case 0:
                        url = string.Format(ApiUtils.Questions, pageIndex, pageSize);
                        break;
                    case 1:
                        url = string.Format(ApiUtils.QuestionsType, "highscore", pageIndex, pageSize);
                        break;
                    case 2:
                        url = string.Format(ApiUtils.QuestionsType, "noanswer", pageIndex, pageSize);
                        break;
                    case 3:
                        url = string.Format(ApiUtils.QuestionsType, "solved", pageIndex, pageSize);
                        break;
                    case 4:
                        url = string.Format(ApiUtils.QuestionsType, "myquestion", pageIndex, pageSize);
                        break;
                }
                var result = await OkHttpUtils.Instance(token).GetAsyn(url);
                if (result.IsError)
                {
                    questionView.GetServiceQuestionsFail(result.Message);
                }
                else
                {
                    var questions = JsonConvert.DeserializeObject<List<QuestionsModel>>(result.Message);
                    if (position != 4)
                        await SQLiteUtils.Instance().UpdateQuestions(questions);
                    questionView.GetServiceQuestionsSuccess(questions);
                }
            }
            catch (Exception ex)
            {
                questionView.GetServiceQuestionsFail(ex.Message);
            }
        }
        public async Task GetClientQuestions(int position)
        {
            questionView.GetClientQuestionsSuccess(await SQLiteUtils.Instance().QueryQuestionsByType(position, pageSize));
        }
    }
}