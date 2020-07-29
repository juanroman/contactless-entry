using ContactlessEntry.UwpFront.Services;
using ContactlessEntry.UwpFront.ViewModels;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ContactlessEntry.UwpFront.Pages
{
    public sealed partial class MainPage : Page
    {
        private const string CameraAccessDeniedError = "CameraAccessDenied";

        public MainPage()
        {
            this.InitializeComponent();

            DataContext = Locator.Instance.Resolve<MainWindowViewModel>();
        }

        //public MainWindowViewModel ViewModel => DataContext as MainWindowViewModel;

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await OpenCamera();
        }

        private async Task OpenCamera()
        {
            CameraPreview.PreviewFailed += OnCameraPreviewPreviewFailedAsync;
            await CameraPreview.StartAsync();
            CameraPreview.CameraHelper.FrameArrived += OnCameraHelperFrameArrived;
        }

        private async void OnCameraPreviewPreviewFailedAsync(object sender, Microsoft.Toolkit.Uwp.UI.Controls.PreviewFailedEventArgs e)
        {
            if (0 == StringComparer.InvariantCultureIgnoreCase.Compare(CameraAccessDeniedError, e.Error))
            {
                Frame.Navigate(typeof(PermissionsPage));
            }
            else
            {
                var messageDialog = new MessageDialog(e.Error, "Error");
                messageDialog.Commands.Add(new UICommand("OK"));
                await messageDialog.ShowAsync();
            }
        }

        private void OnCameraHelperFrameArrived(object sender, FrameEventArgs e)
        {
            var videoFrame = e.VideoFrame;
            var softwareBitmap = videoFrame.SoftwareBitmap;
        }
    }
}
