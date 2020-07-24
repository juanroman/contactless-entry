using ContactlessEntry.WpfFront.Services;
using ContactlessEntry.WpfFront.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ContactlessEntry.WpfFront
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            BuildDependencies();
        }

        private static void BuildDependencies()
        {
            Locator.Instance.Register<MainWindowViewModel>();
            Locator.Instance.Build();
        }
    }
}
