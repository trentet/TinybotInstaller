using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static TinybotInstaller.RegistryUtil;

namespace TinybotInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            /*var keyPath = @"HKEY_CURRENT_USER\SYSTEM\CurrentControlSet\Control\Session Manager";
            //var keyPath = @"SYSTEM\CurrentControlSet\Control\Session Manager";
            var keyValueName = "RunLevelValidate";
            var desiredKeyValueData = "ServiceControlManager";
            var result = RegistryUtil.RegistryKeyValueDataExists(RegistryHives.LOCAL_MACHINE, keyPath, keyValueName, desiredKeyValueData);*/
            Class1 c = new Class1();
            
        }
    }
}
