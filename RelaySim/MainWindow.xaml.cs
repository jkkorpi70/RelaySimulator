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

namespace RelaySim
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly Workbench WB = new Workbench();

        public MainWindow()
        {
            InitializeComponent();
            this.MainGrid.Children.Add(WB); // Create Workbench. Not much else here
        }

        private void MainWin_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Haluatko jatkaa?", "Ohjelma suljetaan!", MessageBoxButton.YesNo) == MessageBoxResult.No) e.Cancel = true;
            else 
            {
            Environment.Exit(1);
            Application.Current.Shutdown();
            }
        }
    }
}
