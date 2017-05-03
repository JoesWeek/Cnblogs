using Cnblogs.Droid.Model;
using Cnblogs.Droid.UI.Views;
using Cnblogs.Droid.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cnblogs.Droid.Presenter
{
    public class QuestionAnswersAddPresenter : IQuestionAnswersAddPresenter
    {
        private IQuestionAnswersAddView answersView;
        public QuestionAnswersAddPresenter(IQuestionAnswersAddView answersView)
        {
            this.answersView = answersView;
        }
        public void AnswersAdd(AccessToken token, int questionId, string content)
        {
            try
            {
                var url = string.Format(ApiUtils.QuestionsAnswerAdd, questionId);

                var param = new List<OkHttpUtils.Param>()
                {
                    new OkHttpUtils.Param("Answer",content) ,
                };

                OkHttpUtils.Instance(token).Post(url, param, async (call, response) =>
                {
                    var code = response.Code();
                    var body = await response.Body().StringAsync();
                    if (code == (int)System.Net.HttpStatusCode.OK)
                    {
                        answersView.AnswersAddSuccess(null);
                    }
                    else
                    {
                        try
                        {
                            var error = JsonConvert.DeserializeObject<ErrorMessage>(body);
                            answersView.AnswersAddFail(error.Message);
                        }
                        catch (Exception e)
                        {
                            answersView.AnswersAddFail(e.Message);
                        }
                    }
                }, (call, ex) =>
                {
                    answersView.AnswersAddFail(ex.Message);
                });
            }
            catch (Exception e)
            {
                answersView.AnswersAddFail(e.Message);
            }
        }
        public async void CheckAnswersByUser(AccessToken token, int questionId)
        {
            try
            {
                var user = await SQLiteUtils.Instance().QueryUser();
                var url = string.Format(ApiUtils.QuestionsAnswerByUser, questionId, user.SpaceUserId);

                if (await OkHttpUtils.Instance(token).HeadAsyn(url))
                {
                    answersView.CheckAnswersUserSuccess();
                }
                else
                {
                    answersView.CheckAnswersUserFail(null);
                }
            }
            catch (Exception e)
            {
                answersView.CheckAnswersUserFail(e.Message);
            }
        }
    }
}