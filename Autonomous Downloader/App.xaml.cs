using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Autonomous_Downloader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            string startupUri = ConfigurationManager.AppSettings["startupUri"];
            if (!string.IsNullOrEmpty(startupUri)) {
                StartupUri = new Uri(startupUri, UriKind.RelativeOrAbsolute);
            }
            base.OnStartup(e);
        }
    }
}
