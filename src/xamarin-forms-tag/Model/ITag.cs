using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace XamarinFormTag.Model
{
    public interface ITag : INotifyPropertyChanged
    {
        string Text { get; set; }
    }
}
