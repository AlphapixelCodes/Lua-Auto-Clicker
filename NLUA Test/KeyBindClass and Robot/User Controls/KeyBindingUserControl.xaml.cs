using NLUA_Test.Lua_Editor;
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
using WindowsInput.Native;

namespace NLUA_Test
{
    /// <summary>
    /// Interaction logic for KeyBindingUserControl.xaml
    /// </summary>
    public partial class KeyBindingUserControl : UserControl
    {
        private List<VirtualKeyCode> binding;
        public KeyBindingUserControl()
        {
            InitializeComponent();
        }
        private KeybindClass Data;
        private ICSharpCode.AvalonEdit.TextEditor TextEditor;
        public KeyBindingUserControl(KeybindClass data, ICSharpCode.AvalonEdit.TextEditor textEditor)
        {
            Data = data;
            TextEditor= textEditor;
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            TypeCombo.SelectionChanged += TypeCombo_SelectionChanged;
            foreach (var name in Enum.GetValues(new LuaFunctions.TickType().GetType()))
            {
                TypeCombo.Items.Add(name);
            }
            if (Data == null)
            {
                TypeCombo.SelectedItem = LuaFunctions.TickType.Toggle;
            }
            else
            {
                NameTextBox.Text = Data.Name;
                FunctionBox.Text = Data.FunctionName;
                TickSpeedBox.Text = Data.TickTime.ToString();
                TypeCombo.SelectedItem = Data.TickType;
                binding = Data.Keybind;
                UpdateBinding();
            }
        }
        public bool Validate()
        {
            var code = TextEditor.Text;
            var name = NameTextBox.Text;
            var funcName = FunctionBox.Text;
            var tickSpeed = TickSpeedBox.Text;
            if (name.Length < 3 || name.Length > 40 || !Regex.IsMatch(name, @"^[a-z A-Z0-9]+$"))
            {                
                MessageBox.Show("Name must be between 3-40 characters and only contain A-Z, 0-9 and space.", "Invalid Keybind Input", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (funcName.Length < 3 || funcName.Length>40 || !Regex.IsMatch(funcName, @"^[a-z A-Z0-9]+$"))
            {   
                MessageBox.Show("Function Name must be between 3-40 characters and only contain A-Z, 0-9 and space: " + funcName, "Invalid Keybind Input", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if(!Regex.IsMatch(code, funcName + @" *\([^,\n]+,[^\)\n]+\)"))
            {
               MessageBox.Show("Function Name must be in lua code as \n'function FuncName(tickType,name)'\n" + funcName+"\n"+name, "Invalid Keybind Input", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            
            if ((TickSpeedBox.Visibility != Visibility.Collapsed) && (tickSpeed.Length == 0 || !Regex.IsMatch(tickSpeed, @"^\d+$")))
            {
                MessageBox.Show("Tick Speed must be a number between 5-360000 for " + name, "Invalid Keybind Input", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;

            }
            else if (int.Parse(tickSpeed)<5 || int.Parse(tickSpeed)>360000)
            {
                MessageBox.Show("Tick Speed must be a number between 5-360000 for " + name, "Invalid Keybind Input", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (binding == null)
            {
                MessageBox.Show("Unset binding for " + name, "Invalid Keybind Input", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
        public KeybindClass GetKeyBindClass()
        {
            var ret = new KeybindClass();
            ret.Name = NameTextBox.Text;
            ret.FunctionName = FunctionBox.Text;
            ret.Keybind = binding;
            ret.TickTime = (TickSpeedBox.Visibility != Visibility.Collapsed) ? int.Parse(TickSpeedBox.Text) : 500;
            ret.TickType = (LuaFunctions.TickType)TypeCombo.SelectedItem;
            return ret;
        }

        private void TypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(TypeCombo.SelectedItem.Equals(LuaFunctions.TickType.Toggle) ||
               TypeCombo.SelectedItem.Equals(LuaFunctions.TickType.Hold))
            {
                TickSpeedBlock.Visibility = Visibility.Visible;
                TickSpeedBox.Visibility = Visibility.Visible;
            }
            else
            {
                TickSpeedBlock.Visibility = Visibility.Collapsed;
                TickSpeedBox.Visibility = Visibility.Collapsed;
            }
        }

        private void UpdateBinding()
        {
            KeyBindBlock.Text = LuaRobotV2.getBindingString(binding);
        }
        
        private void ChangeBinding_Click(object sender, RoutedEventArgs e)
        {
            var f=new KeybindForm("Set Binds");
            var res=f.ShowDialog();
            if (res.HasValue && res.Value)
            {
                binding = f.ReturnKeys;
                UpdateBinding();
            }
        }

        private void Clearbinding_Click(object sender, RoutedEventArgs e)
        {
            binding=null;
            UpdateBinding();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var res=MessageBox.Show("Are you sure you want to delete this key binding:\n"+NameTextBox.Text, "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if(res==MessageBoxResult.Yes)
                ((WrapPanel)Parent).Children.Remove(this);
        }

        private void Validate_Click(object sender, RoutedEventArgs e)
        {
            if (Validate())
            {
                MessageBox.Show("Keybind is Valid", "Valid", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
