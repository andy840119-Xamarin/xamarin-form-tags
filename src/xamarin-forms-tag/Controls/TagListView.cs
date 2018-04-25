using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinFormTag.Model;
using PropertyChangingEventArgs = Xamarin.Forms.PropertyChangingEventArgs;

namespace XamarinFormTag.Controls
{
    /// <summary>
    ///     TagView
    /// </summary>
    public class TagListView : TagListView<TagView, Tag>
    {
        /// <summary>
        ///     Create tag
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
    ///     TagView
    ///     Code is refrence from :
    ///     https://github.com/daniel-luberda/DLToolkit.Forms.Controls/tree/master/TagEntryView/DLToolkit.Forms.Controls.TagEntryView
    /// </summary>
    /// <typeparam name="TagView"></typeparam>
    /// <typeparam name="TagModel"></typeparam>
    public abstract class TagListView<TagView, TagModel> : Layout<TagView>, IDisposable
        where TagView : View, new() where TagModel : ITag
    {
        public static readonly BindableProperty TagItemTemplateProperty =
            BindableProperty.Create(nameof(TagItemTemplate), typeof(DataTemplate),
                typeof(TagListView<TagView, TagModel>), default(DataTemplate));

        public static BindableProperty TagTappedCommandProperty = BindableProperty.Create(nameof(TagTappedCommand),
            typeof(ICommand), typeof(TagListView<TagView, TagModel>), default(ICommand));

        public static readonly BindableProperty TagSeparatorsProperty = BindableProperty.Create(nameof(TagSeparators),
            typeof(IList<string>), typeof(TagListView<TagView, TagModel>), new List<string> {" "});


        public static readonly BindableProperty EntryMinimumWidthProperty =
            BindableProperty.Create(nameof(EntryMinimumWidth), typeof(double), typeof(TagListView<TagView, TagModel>),
                150d);


        public static readonly BindableProperty TagItemsProperty = BindableProperty.Create(nameof(TagItems),
            typeof(IList), typeof(TagListView<TagView, TagModel>), default(IList), BindingMode.TwoWay);


        public static readonly BindableProperty SpacingProperty = BindableProperty.Create(nameof(Spacing),
            typeof(double), typeof(TagListView<TagView, TagModel>), 6d,
            propertyChanged: (bindable, oldvalue, newvalue) =>
                ((TagListView<TagView, TagModel>) bindable).OnSizeChanged());

        private Func<TagView> _tagViewFactory;


        public TagListView()
        {
            PropertyChanged += TagEntryViewPropertyChanged;
            PropertyChanging += TagEntryViewPropertyChanging;
        }

        /// <summary>
        ///     Observe collection
        /// </summary>
        public ObservableCollection<TagModel> ItemSource { get; set; } = new ObservableCollection<TagModel>();

        public Func<string, object> TagValidatorFactory { get; set; }

        [Obsolete("Use XAML compatible TagItemTemplate property")]
        public Func<TagView> TagViewFactory
        {
            get => _tagViewFactory;

            set
            {
                TagItemTemplate = new DataTemplate(value);
                _tagViewFactory = value;
            }
        }

        public DataTemplate TagItemTemplate
        {
            get => (DataTemplate) GetValue(TagItemTemplateProperty);
            set => SetValue(TagItemTemplateProperty, value);
        }

        public ICommand TagTappedCommand
        {
            get => (ICommand) GetValue(TagTappedCommandProperty);
            set => SetValue(TagTappedCommandProperty, value);
        }

        public IList<string> TagSeparators
        {
            get => (IList<string>) GetValue(TagSeparatorsProperty);
            set => SetValue(TagSeparatorsProperty, value);
        }

        public double EntryMinimumWidth
        {
            get => (double) GetValue(EntryMinimumWidthProperty);
            set => SetValue(EntryMinimumWidthProperty, value);
        }

        public IList TagItems
        {
            get => (IList) GetValue(TagItemsProperty);
            set => SetValue(TagItemsProperty, value);
        }

        public double Spacing
        {
            get => (double) GetValue(SpacingProperty);
            set => SetValue(SpacingProperty, value);
        }

        public void Dispose()
        {
            PropertyChanged -= TagEntryViewPropertyChanged;
            PropertyChanging -= TagEntryViewPropertyChanging;

            var tagItems = TagItems as INotifyCollectionChanged;
            if (tagItems != null) tagItems.CollectionChanged -= TagItemsCollectionChanged;
        }

        /// <summary>
        ///     Create Tag
        /// </summary>
        protected virtual TagView CreateTag(TagModel model)
        {
            var tag = new TagView();
            tag.BindingContext = model;
            return tag;
        }

        public event EventHandler<ItemTappedEventArgs> TagTapped;

        internal void PerformTagTap(object item)
        {
            TagTapped?.Invoke(this, new ItemTappedEventArgs(null, item));

            var command = TagTappedCommand;
            if (command != null && command.CanExecute(item)) command.Execute(item);
        }


        private void TagEntryViewPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            if (e.PropertyName == TagItemsProperty.PropertyName)
            {
                var tagItems = TagItems as INotifyCollectionChanged;
                if (tagItems != null)
                    tagItems.CollectionChanged -= TagItemsCollectionChanged;
            }
        }

        private void TagEntryViewPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == TagItemsProperty.PropertyName)
            {
                var tagItems = TagItems as INotifyCollectionChanged;
                if (tagItems != null)
                    tagItems.CollectionChanged += TagItemsCollectionChanged;

                ForceReload();
            }
        }

        private void TagItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ForceReload();
        }

        public void ForceReload()
        {
            Children.Clear();

            for (var i = 0; i < TagItems.Count; i++)
            {
                TagView view = null;

                var templateSelector = TagItemTemplate as DataTemplateSelector;
                if (templateSelector != null)
                {
                    var template = templateSelector.SelectTemplate(TagItems[i], null);
                    view = (TagView) template.CreateContent();
                }
                else
                {
                    view = (TagView) TagItemTemplate.CreateContent();
                }

                view.BindingContext = TagItems[i];

                view.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() => PerformTagTap(view.BindingContext))
                });

                Children.Add(view);
            }
        }

        private void OnSizeChanged()
        {
            ForceLayout();
        }

        protected override SizeRequest OnSizeRequest(double widthConstraint, double heightConstraint)
        {
            if (WidthRequest > 0)
                widthConstraint = Math.Min(widthConstraint, WidthRequest);
            if (HeightRequest > 0)
                heightConstraint = Math.Min(heightConstraint, HeightRequest);

            var internalWidth = double.IsPositiveInfinity(widthConstraint)
                ? double.PositiveInfinity
                : Math.Max(0, widthConstraint);
            var internalHeight = double.IsPositiveInfinity(heightConstraint)
                ? double.PositiveInfinity
                : Math.Max(0, heightConstraint);

            return DoHorizontalMeasure(internalWidth, internalHeight);
        }

        private SizeRequest DoHorizontalMeasure(double widthConstraint, double heightConstraint)
        {
            var rowCount = 1;

            double width = 0;
            double height = 0;
            double minWidth = 0;
            double minHeight = 0;
            double widthUsed = 0;

            foreach (var item in Children)
            {
                var size = item.GetSizeRequest(widthConstraint, heightConstraint);
                height = Math.Max(height, size.Request.Height);

                var newWidth = width + size.Request.Width + Spacing;
                if (newWidth > widthConstraint)
                {
                    rowCount++;
                    widthUsed = Math.Max(width, widthUsed);
                    width = size.Request.Width;
                }
                else
                {
                    width = newWidth;
                }

                minHeight = Math.Max(minHeight, size.Minimum.Height);
                minWidth = Math.Max(minWidth, size.Minimum.Width);
            }

            if (rowCount > 1)
            {
                width = Math.Max(width, widthUsed);
                height = (height + Spacing) * rowCount - Spacing; // via MitchMilam 
            }

            return new SizeRequest(new Size(width, height), new Size(minWidth, minHeight));
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            double rowHeight = 0;
            double yPos = y, xPos = x;

            foreach (var child in Children.Where(c => c.IsVisible))
            {
                var request = child.GetSizeRequest(width, height);

                var childWidth = request.Request.Width;
                var childHeight = request.Request.Height;

                rowHeight = Math.Max(rowHeight, childHeight);

                if (xPos + childWidth > width)
                {
                    xPos = x;
                    yPos += rowHeight + Spacing;
                    rowHeight = 0;
                }

                var region = new Rectangle(xPos, yPos, childWidth, childHeight);
                LayoutChildIntoBoundingRegion(child, region);
                xPos += region.Width + Spacing;
            }
        }
    }
}