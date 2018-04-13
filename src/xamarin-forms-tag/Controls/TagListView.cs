using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using XamarinFormTag.Model;

namespace XamarinFormTag.Controls
{
    /// <summary>
    /// TagView
    /// </summary>
    public class TagListView : TagListView<TagView, Tag>
    {

    }

    /// <summary>
    /// TagView
    /// </summary>
    /// <typeparam name="TagView"></typeparam>
    /// <typeparam name="TagModel"></typeparam>
    public class TagListView<TagView, TagModel> : Layout<TagView> where TagView : View where TagModel : ITag
    {
        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            throw new NotImplementedException();
        }
    }
}
