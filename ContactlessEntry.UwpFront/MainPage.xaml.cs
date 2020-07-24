using ContactlessEntry.UwpFront.Services;
using ContactlessEntry.UwpFront.ViewModels;
using Windows.UI.Xaml.Controls;

namespace ContactlessEntry.UwpFront
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            DataContext = Locator.Instance.Resolve<MainWindowViewModel>();
        }

        public MainWindowViewModel ViewModel => DataContext as MainWindowViewModel;
    }
}
