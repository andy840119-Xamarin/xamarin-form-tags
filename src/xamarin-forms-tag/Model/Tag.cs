using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace XamarinFormTag.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class Tag : ITag , ITagType, ITagColor
    {
        /// <summary>
        /// Text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        public TabType TabType { get; set; }

        /// <summary>
        /// Hex color
        /// </summary>
        public string TagHexColor { get; set; }

        /// <summary>
        ///     Event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Invoke
        /// </summary>
        /// <param name="propertyName"></param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
