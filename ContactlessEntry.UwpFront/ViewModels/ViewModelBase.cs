namespace ContactlessEntry.UwpFront.ViewModels
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public abstract class ViewModelBase
    {
        public bool IsBusy { get; set; }
    }
}
