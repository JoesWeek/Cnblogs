using Cnblogs.Droid.Model;
using Cnblogs.Droid.UI.Views;
using Cnblogs.Droid.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cnblogs.Droid.Presenter
{
    public class QuestionAnswersPresenter : IQuestionAnswersPresenter
    {
        private IQuestionAnswersView answersView;
        public QuestionAnswersPresenter(IQuestionAnswersView answersView)
        {
            this.answersView = answersView;
        }
        public async Task GetAnswers(AccessToken token, int id, int pageIndex = 1)
        {
            try
            {
                var result = await OkHttpUtils.Instance(token).GetAsyn(string.Format(ApiUtils.QuestionsAnswers, id));
                if (result.IsError)
                {
                    answersView.GetAnswersFail(result.Message);
                }
                else
                {
                    var answers = JsonConvert.DeserializeObject<List<QuestionAnswersModel>>(result.Message);
                    answersView.GetAnswersSuccess(answers);
                }
            }
            catch (Exception ex)
            {
                answersView.GetAnswersFail(ex.Message);
            }
        }
        public void DeleteAnswer(AccessToken token, int questionId, int answerId)
        {
            try
            {
                var url = string.Format(ApiUtils.QuestionsAnswerDelete, questionId, answerId);

                OkHttpUtils.Instance(token).Delete(url, async (call, response) =>
                 {
                     var code = response.Code();
                     var body = await response.Body().StringAsync();
                     if (code == (int)System.Net.HttpStatusCode.OK)
                     {
                         answersView.DeleteAnswerSuccess(answerId);
                     }
                     else
                     {
                         try
                         {
                             var error = JsonConvert.DeserializeObject<ErrorMessage>(body);
                             answersView.DeleteAnswerFail(answerId, error.Message);
                         }
                         catch (Exception e)
                         {
                             answersView.DeleteAnswerFail(answerId, e.Message);
                         }
                     }
                 }, (call, ex) =>
                 {
                     answersView.DeleteAnswerFail(answerId, ex.Message);
                 });
            }
            catch (Exception ex)
            {
                answersView.DeleteAnswerFail(answerId, ex.Message);
            }
        }
    }
}