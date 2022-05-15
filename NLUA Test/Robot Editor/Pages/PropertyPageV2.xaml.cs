using ICSharpCode.AvalonEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace NLUA_Test.Lua_Editor.Pages
{
    /// <summary>
    /// Interaction logic for PropertyPageV2.xaml
    /// </summary>
    public partial class PropertyPageV2 : Page
    {
        public PropertyPageV2(LuaRobotV2 robot,TextEditor textEditor)
        {
            TextEditor = textEditor;
            Robot= robot;
            InitializeComponent();
            
        }

        private TextEditor TextEditor;
        private LuaRobotV2 Robot;

        private void AddKeyBind_Click(object sender, RoutedEventArgs e)
        {
            BindWrapPanel.Children.Add(new KeyBindingUserControl(KeybindClass.Default(),TextEditor));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            NameBox.Text = Robot.Name;
            DescriptionBox.Text = Robot.Description;
            foreach (var kb in Robot.Keybinds)
            {
                BindWrapPanel.Children.Add(new KeyBindingUserControl(kb, TextEditor));
            }
        }
        public bool Validate()
        {
            var name = NameBox.Text;
            
            if (name.Length < 3 || name.Length > 40 || !Regex.IsMatch(name, @"^[a-z A-Z0-9]+$"))
            {
                MessageBox.Show("Name must be between 3-40 characters and only contain A-Z, 0-9 and space.", "Invalid Property Input", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (BindWrapPanel.Children.Count == 0)
            {
                MessageBox.Show("Properties must have atleast one keybind", "Invalid Properties", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            foreach (KeyBindingUserControl item in BindWrapPanel.Children)
            {
                if (!item.Validate())
                    return false;
            }   
            return true;
        }
        public void UpdateClass()
        {
            Robot.Name = NameBox.Text;
            Robot.Description=DescriptionBox.Text;
            Robot.Keybinds=BindWrapPanel.Children.OfType<KeyBindingUserControl>().Select(x => x.GetKeyBindClass()).ToList();
        }

        internal string GetName()
        {
            return NameBox.Text;
        }
    }
}
