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
using System.Windows.Shapes;

namespace NLUA_Test
{
    /// <summary>
    /// Interaction logic for OutputConsoleWindow.xaml
    /// </summary>
    public partial class OutputConsoleWindow : Window
    {
        public OutputConsoleWindow()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            LuaFunctions.PrintEvent += LuaFunctions_PrintEvent;
        }

        private void LuaFunctions_PrintEvent(object? sender, EventArgs e)
        {
            OutputBox.Text += sender?.ToString();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            LuaFunctions.PrintEvent -= LuaFunctions_PrintEvent;
        }
    }
}
