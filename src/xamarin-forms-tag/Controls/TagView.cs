using Xamarin.Forms;
using XamarinFormTag.Model;

namespace XamarinFormTag.Controls
{
    public class TagView : Button
    {
        /// <summary>
        ///     binding context
        /// </summary>
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (BindingContext is ITag tag)
            {
                tag.PropertyChanged += (a, b) => { ChangeStyle(a as ITag); };
                ChangeStyle(tag);
            }
        }

        protected virtual void ChangeStyle(ITag tag)
        {
            if (tag == null)
                return;

            //set text
            Text = tag.Text;

            //Change default color
            var defaultColor = Color.Black;
            if (tag is ITagColor tagColor) defaultColor = Color.FromHex(tagColor.TagHexColor);

            IsEnabled = false;

            TextColor = defaultColor;
            BackgroundColor = Color.White;
            BorderColor = defaultColor;


            //set tyle
            if (tag is ITagType tagType)
                switch (tagType.TabType)
                {
                    //TODO : set type
                    case TabType.Normal:

                        break;
                    case TabType.HighLight:
                        TextColor = Color.White;
                        BackgroundColor = defaultColor;
                        break;
                    case TabType.Alert:
                        TextColor = Color.White;
                        BackgroundColor = Color.Red;
                        BorderColor = Color.DarkRed;
                        break;
                    case TabType.Forbidden:
                        IsEnabled = false;
                        break;
                }
        }
    }
}