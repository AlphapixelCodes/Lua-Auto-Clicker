using NLUA_Test.Lua_Editor;
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

namespace NLUA_Test
{
    /// <summary>
    /// Interaction logic for KeybindClassViewer.xaml
    /// </summary>
    public partial class KeybindClassViewer : UserControl
    {
        private KeybindClass KeybindClass;
        
        public KeybindClassViewer(KeybindClass kbc)
        {
            KeybindClass = kbc;
            InitializeComponent();
        }
        public KeybindClassViewer()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (KeybindClass != null)
            {
                NameBlock.Text = KeybindClass.Name;
                TypeBlock.Text = KeybindClass.TickType.ToString();
                BindingBlock.Text=LuaRobotV2.getBindingString(KeybindClass.Keybind);
                if (KeybindClass.TickType == LuaFunctions.TickType.Toggle || KeybindClass.TickType == LuaFunctions.TickType.Hold)
                {
                    KeybindClass_ToggleChangedEvent(false, EventArgs.Empty);
                    KeybindClass.ToggleChangedEvent += KeybindClass_ToggleChangedEvent;
                }
                UpdateEnabledButton();
            }
        }

        private void UpdateEnabledButton()
        {
            var img = (Image)EnabledButton.Content;
            if (KeybindClass.GetEnabled())
            {
                EnabledButton.ToolTip = "Disable";
                img.Source = (Application.Current.Resources["CheckMark"] as Image).Source;
            }
            else
            {
                EnabledButton.ToolTip = "Enable";
                img.Source = (Application.Current.Resources["CancelMark"] as Image).Source;
            }
        }
        private void EnableButton_Click(object sender, RoutedEventArgs e)
        {
            KeybindClass.SetEnabled(!KeybindClass.GetEnabled());
            UpdateEnabledButton();
        }

        private void KeybindClass_ToggleChangedEvent(object? sender, EventArgs e)
        {
            var b = (bool)sender;
            BindingBlock.Foreground = b ? new SolidColorBrush(Color.FromRgb(0, 255, 0)) : new SolidColorBrush(Color.FromRgb(255, 0, 0));
        }
    }
}
