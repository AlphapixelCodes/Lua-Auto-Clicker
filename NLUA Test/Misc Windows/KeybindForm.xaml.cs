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
using WindowsInput.Native;

namespace NLUA_Test
{
    /// <summary>
    /// Interaction logic for KeybindForm.xaml
    /// </summary>
    public partial class KeybindForm : Window
    {
        public KeybindForm()
        {
            InitializeComponent();
        }
        private string Title;
        public KeybindForm(string title)
        {
            Title= title;
            InitializeComponent();
        }
        private List<VirtualKeyCode> KeysToVirtual(List<Key> keys)
        {
            return keys.Select(x =>(VirtualKeyCode)KeyInterop.VirtualKeyFromKey(x)).ToList(); 
        }
        private HashSet<Key> keysDown = new();
        public List<VirtualKeyCode> ReturnKeys;
        private void updateKeysDownText()
        {
            KeysDownBlock.Text = string.Join(" + ", keysDown.OrderByDescending(e=>e.ToString().Length));;
        }
        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            keysDown.Add(e.Key);
            updateKeysDownText();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            keysDown.Clear();
            updateKeysDownText();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (keysDown.Count > 0)
            {
                DialogResult = true;
                ReturnKeys = KeysToVirtual(keysDown.ToList());
                Close();
                return;
            }
            MessageBox.Show("Keybind Cannot Be Blank.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(Title!=null)
                TitleBlock.Text = Title;
        }
    }
}
