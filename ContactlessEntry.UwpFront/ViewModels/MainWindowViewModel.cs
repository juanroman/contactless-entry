using ContactlessEntry.UwpFront.Services;
using ContactlessEntry.UwpFront.Services.Connectivity;
using PropertyChanged;
using System;

namespace ContactlessEntry.UwpFront.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        //private readonly DispatcherTimer _dateTimeDispatcherTimer;
        private readonly IConnectivityService _connectivityService;

        public MainWindowViewModel()
        {
            _connectivityService = Locator.Instance.Resolve<IConnectivityService>();

            IsOnline = true;
            DateTime = DateTime.Now;
            DoorWeather = 21.5;
            DoorName = "Polanco";

            //_dateTimeDispatcherTimer = new DispatcherTimer(
            //    TimeSpan.FromMinutes(1),
            //    DispatcherPriority.Render,
            //    async (sender, args) =>
            //    {
            //        DateTime = DateTime.Now;
            //        IsOnline = await _connectivityService.CheckIfConnectedToInternet();
            //    },
            //    Application.Current.Dispatcher);
            //_dateTimeDispatcherTimer.Start();
        }

        [DependsOn(nameof(IsOnline))]
        public string Connectivity => IsOnline ? "Online" : "Offline";

        public bool IsOnline { get; set; }

        public DateTime DateTime { get; set; }

        public double DoorWeather { get; set; }

        public string DoorName { get; set; }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //_dateTimeDispatcherTimer.Stop();
            }
        }
    }
}
