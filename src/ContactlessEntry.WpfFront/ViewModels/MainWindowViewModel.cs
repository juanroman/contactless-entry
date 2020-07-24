using PropertyChanged;
using System;
using System.Windows;
using System.Windows.Threading;

namespace ContactlessEntry.WpfFront.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        private readonly DispatcherTimer _dateTimeDispatcherTimer;

        public MainWindowViewModel()
        {
            IsOnline = true;
            DateTime = DateTime.Now;
            DoorWeather = 21.5;
            DoorName = "Polanco";

            _dateTimeDispatcherTimer = new DispatcherTimer(
                TimeSpan.FromMinutes(1),
                DispatcherPriority.Render,
                (sender, args) => DateTime = DateTime.Now,
                Application.Current.Dispatcher);
            _dateTimeDispatcherTimer.Start();
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
                _dateTimeDispatcherTimer.Stop();
            }
        }
    }
}
