using System;
using System.Diagnostics;
using System.Windows.Input;

namespace ContactlessEntry.WpfFront.ViewModels
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public abstract class ViewModelBase
    {
        public bool IsBusy { get; set; }
    }
}
