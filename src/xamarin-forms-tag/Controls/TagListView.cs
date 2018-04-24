using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        /// <summary>
        /// Create tag
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected override TagView CreateTag(Tag model)
        {
            var tag = new TagView();
            //TODO : add some property

            return tag;
        }
    }

    /// <summary>
    /// TagView
    /// </summary>
    /// <typeparam name="TagView"></typeparam>
    /// <typeparam name="TagModel"></typeparam>
    public abstract class TagListView<TagView, TagModel> : Layout<TagView> where TagView : View , new() where TagModel : ITag
    {
        /// <summary>
        /// Observe collection
        /// </summary>
        public ObservableCollection<TagModel> ItemSource { get; set; } = new ObservableCollection<TagModel>();

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create Tag
        /// </summary>
        protected virtual TagView CreateTag(TagModel model)
        {
            var tag = new TagView();
            tag.BindingContext = model;
            return tag;
        }
    }
}
