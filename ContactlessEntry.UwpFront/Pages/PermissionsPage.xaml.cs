using ContactlessEntry.UwpFront.Services;
using ContactlessEntry.UwpFront.ViewModels;
using Windows.UI.Xaml.Controls;

namespace ContactlessEntry.UwpFront.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PermissionsPage : Page
    {
        public PermissionsPage()
        {
            InitializeComponent();

            DataContext = Locator.Instance.Resolve<PermissionsViewModel>();

            OkButton.Click += (s, e) => Frame.GoBack();
        }
    }
}
