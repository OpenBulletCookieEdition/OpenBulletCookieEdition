using RuriLib;
using System.Windows.Controls;

namespace OpenBulletCE.Views.Main.Configs.OtherOptions
{
    /// <summary>
    /// Logica di interazione per ConfigOtherOptionsProxies.xaml
    /// </summary>
    public partial class Proxies : Page
    {
        public Proxies()
        {
            InitializeComponent();
            DataContext = OB.ConfigManager.CurrentConfig.Config.Settings;
        }
    }
}
