using Cnblogs.Droid.Model;
using System.Collections.Generic;

namespace Cnblogs.Droid.UI.Views
{
    public interface ISearchColumnView
    {
        void SearchFail(string msg);
        void SearchSuccess(List<SearchModel> lists);
    }
}