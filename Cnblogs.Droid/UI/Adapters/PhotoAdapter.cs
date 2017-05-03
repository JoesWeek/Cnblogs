using Android.OS;
using Android.Support.V4.View;
using Android.Views;
using Square.Picasso;
using System.Collections.Generic;
using UK.CO.Senab.Photoview;

namespace Cnblogs.Droid.UI.Adapters
{
    public class PhotoAdapter : PagerAdapter
    {
        private Handler handler;
        private List<string> srcList;
        public PhotoAdapter(List<string> srcList)
        {
            this.srcList = srcList;
            this.handler = new Handler();
        }
        public override int Count => srcList.Count;

        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {

            View view = LayoutInflater.From(container.Context).Inflate(Resource.Layout.photo_item, container, false);
            handler.Post(() =>
            {
                PhotoView image = view.FindViewById<PhotoView>(Resource.Id.photo);
                Picasso.With(container.Context).Load(srcList[position]).Into(image);
                container.AddView(view);
            });
            return view;
        }
        public override bool IsViewFromObject(View view, Java.Lang.Object @object)
        {
            return view == @object;
        }
        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object @object)
        {
            container.RemoveView((View)@object);
        }
    }
}