using System.ComponentModel;

namespace XamarinFormTag.Model
{
    public interface ITag : INotifyPropertyChanged
    {
        string Text { get; set; }
    }
}