using Cnblogs.Droid.Model;
using Cnblogs.Droid.UI.Views;
using Cnblogs.Droid.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cnblogs.Droid.Presenter
{
    public class QuestionAddPresenter : IQuestionAddPresenter
    {
        private IQuestionAddView questionView;
        public QuestionAddPresenter(IQuestionAddView questionView)
        {
            this.questionView = questionView;
        }
        public void QuestionAdd(AccessToken token, string title, string content, string tags, int flags)
        {
            try
            {
                var url = string.Format(ApiUtils.QuestionADD);

                var param = new List<OkHttpUtils.Param>()
                {
                    new OkHttpUtils.Param("Title",title) ,
                    new OkHttpUtils.Param("Content",content) ,
                    new OkHttpUtils.Param("Tags",tags) ,
                    new OkHttpUtils.Param("Flags",flags.ToString()) ,
                };

                OkHttpUtils.Instance(token).Post(url, param, async (call, response) =>
                {
                    var code = response.Code();
                    var body = await response.Body().StringAsync();
                    if (code == (int)System.Net.HttpStatusCode.OK)
                    {
                        questionView.QuestionAddSuccess(null);
                    }
                    else
                    {
                        try
                        {
                            var error = JsonConvert.DeserializeObject<ErrorMessage>(body);
                            questionView.QuestionAddFail(error.Message);
                        }
                        catch (Exception e)
                        {
                            questionView.QuestionAddFail(e.Message);
                        }
                    }
                }, (call, ex) =>
                {
                    questionView.QuestionAddFail(ex.Message);
                });
            }
            catch (Exception e)
            {
                questionView.QuestionAddFail(e.Message);
            }
        }
    }
}