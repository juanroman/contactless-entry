using ContactlessEntry.UwpFront.Utilities;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;

namespace ContactlessEntry.UwpFront.ViewModels
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class PermissionsViewModel : ViewModelBase
    {
        public ICommand OpenSettingsCommand => new AsyncCommand(OpenSettingsAsync, () => !IsBusy);

        private async Task OpenSettingsAsync()
        {
            try
            {
                IsBusy = true;

                await Launcher.LaunchUriAsync(new Uri("ms-settings:appsfeatures-app"));
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
