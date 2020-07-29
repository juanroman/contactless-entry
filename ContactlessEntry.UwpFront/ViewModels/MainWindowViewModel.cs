using ContactlessEntry.UwpFront.Services;
using ContactlessEntry.UwpFront.Services.Connectivity;
using PropertyChanged;
using System;

namespace ContactlessEntry.UwpFront.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainWindowViewModel : ViewModelBase
    {
        //private readonly DispatcherTimer _dateTimeDispatcherTimer;
        //private readonly IConnectivityService _connectivityService;

        public MainWindowViewModel()
        {
            //_connectivityService = Locator.Instance.Resolve<IConnectivityService>();

            IsOnline = true;
            DoorWeather = 21.5;
            DoorName = "Polanco";
        }

        [DependsOn(nameof(IsOnline))]
        public string Connectivity => IsOnline ? "Online" : "Offline";

        public bool IsOnline { get; set; }

        public string FormattedDateTime => DateTime.Now.ToString("H:mm   ddd dd MMM");

        public double DoorWeather { get; set; }

        public string DoorName { get; set; }
    }
}
