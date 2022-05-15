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
using WindowsInput.Native;

namespace NLUA_Test
{
    /// <summary>
    /// Interaction logic for EnumEditorViewer.xaml
    /// </summary>
    public partial class EnumEditorViewer : UserControl
    {
        private Dictionary<string, Tuple<String,List<String>>> Data = new();
        public EnumEditorViewer()
        {
            Data.Add("Global Enums:", null);
            Data.Add("Key", new Tuple<String,List<String>>("Key.",Enum.GetNames(new VirtualKeyCode().GetType()).ToList()));
            Data.Add("MouseButton", new Tuple<String, List<String>>("MouseButton.", Enum.GetNames(new LuaFunctions.MouseButton().GetType()).ToList()));
            Data.Add("TickType", new Tuple<String, List<String>>("TickType.", Enum.GetNames(new LuaFunctions.TickType().GetType()).ToList()));

            Data.Add("Functions:", null);
            Data.Add("Keyboard", new Tuple<String, List<String>>("Keyboard.", new List<string>() { "Down", "Up", "Press", "PressList", "TextEntry", "ModifiedKeyStroke", "ModifiedKeyStrokeList" }));
            Data.Add("Mouse", new Tuple<String, List<String>>("Mouse.", new List<string>() { "Down", "Up", "Click", "DoubleClick", "VerticalScroll", "HorizontalScroll", "MoveTo", "MoveBy" }));
            Data.Add("Extra", new Tuple<String, List<String>>("", new List<string>() { "getPixelColor", "ColorsEqual", "ColorAtXYEquals", "getCursorPosition", "Print", "Sleep" }));
            InitializeComponent();
        }
        private void loadItems(string name)
        {
            ListBox.Items.Clear();
            ListName.Text = name;
            var dat = Data[name];
            foreach (var item in dat.Item2)
            {
                var lbi = new ListBoxItem();
                lbi.Content = item;
                
                lbi.PreviewMouseDown += (a, b) => Clipboard.SetText(dat.Item1+item);
                ListBox.Items.Add(lbi);
            }

        }
        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object selectedItem = TypeComboBox.SelectedItem;
            if (selectedItem != null && Data[((ListBoxItem)selectedItem).Content.ToString()]!=null)
            {
                loadItems(((ListBoxItem)selectedItem).Content.ToString());
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var item in Data.Keys)
            {
                var boxitem = new ComboBoxItem();
                
                if (Data[item] == null)
                {
                    boxitem.BorderBrush = Application.Current.Resources["Border"] as SolidColorBrush;
                    boxitem.BorderThickness = new Thickness(0,0,0,1);
                    //boxitem.Height = 2;
                }
                boxitem.Content = item;
                TypeComboBox.Items.Add(boxitem);
            }
            loadItems("Key");
        }
    }
}

