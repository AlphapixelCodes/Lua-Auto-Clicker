using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Microsoft.Win32;
using NLua;
using NLUA_Test.Lua_Editor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Xml;
using WindowsInput.Native;

namespace NLUA_Test
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class LuaEditorPage : Page
    {
        string TextToLoad;
        private bool readNextKeyInput = false;
        private OutputConsoleWindow? outputConsoleWindow;
        private MenuItem ReadNextKeyInputMenuItem = new MenuItem();
        bool ReadNextKeyInput { get { return readNextKeyInput; }
            set
            {
                readNextKeyInput = value;
                if (!value)
                    MainMenu.Items.Remove(ReadNextKeyInputMenuItem);
                else
                    MainMenu.Items.Add(ReadNextKeyInputMenuItem);
            }
        }
        public static RoutedCommand getKeyNameCommand = new RoutedCommand(), ShowFuncs = new RoutedCommand(), ColorPicker = new RoutedCommand(),consoleWindow = new RoutedCommand();
        private LuaRobotV2 Data;
        public LuaEditorPage(LuaRobotV2 data)
        {
            TextToLoad = data.LuaCode;
            Data = data;
            
            
            getKeyNameCommand.InputGestures.Add(new KeyGesture(Key.Q, ModifierKeys.Control));
            ColorPicker.InputGestures.Add(new KeyGesture(Key.D1, ModifierKeys.Control));
            ShowFuncs.InputGestures.Add(new KeyGesture(Key.D2, ModifierKeys.Control));
            consoleWindow.InputGestures.Add(new KeyGesture(Key.D3, ModifierKeys.Control));

            ReadNextKeyInputMenuItem.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
            ReadNextKeyInputMenuItem.Header = "Quick KeyBind Active";
            ReadNextKeyInputMenuItem.Click += (a,b) =>ReadNextKeyInput=false;
            InitializeComponent();
        }

        private void textEditor_Loaded(object sender, RoutedEventArgs e)
        {
            textEditor.ShowLineNumbers = true;

            using (XmlTextReader reader = new XmlTextReader(new StringReader(Properties.Resources.LuaHighlightSyntax)))
            {
                var xshd = HighlightingLoader.LoadXshd(reader);
                textEditor.SyntaxHighlighting = HighlightingLoader.Load(xshd, HighlightingManager.Instance);
            }
            textEditor.Text = TextToLoad;
        }

        private void CollapseableGridColumn(ColumnDefinition col,UIElement elm,MenuItem item)
        {

            if (col.ActualWidth > 0)
            {
                item.IsChecked = false;
                col.Width = new GridLength(0, GridUnitType.Pixel);
                elm.Visibility = Visibility.Collapsed;
            }
            else
            {
                item.IsChecked = true;
                elm.Visibility = Visibility.Visible;
                col.Width = new GridLength(150, GridUnitType.Pixel);
            }
        }
        private void HelpSideBar_Click(object sender, RoutedEventArgs e)
        {
                CollapseableGridColumn(SideBarColumnDef, EnumFuncControl, HelpViewerMenu);
        }

        private void PixelColorTool_Click(object sender, RoutedEventArgs e)
        {
            CollapseableGridColumn(ColorPixelCoulmnDef, PixelColorData, PixelColorToolMenu);
            
        }

        internal bool Validate()
        {
            if (!TryCode())
            {
                MessageBox.Show("Code could not be complined", "Invalid Code", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            /*if(!Regex.IsMatch(textEditor.Text, @"function +onTick\([^\)]+\)"))
            {
                MessageBox.Show("Code does not include onTick(ticktype) function", "Invalid Code", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }*/
            return true;
        }

        internal void UpdateClass()
        {
            Data.LuaCode=textEditor.Text;
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            var sfd=new SaveFileDialog();
            sfd.Filter = "Lua File (*.lua)|*.lua|All Files (*.*)|*.*";
            var res = sfd.ShowDialog();
            if (res.HasValue && res.Value)
            {
                File.WriteAllText(sfd.FileName, textEditor.Text);
            }
        }

       

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new OpenFileDialog();
            sfd.Filter = "Lua File (*.lua)|*.lua|All Files (*.*)|*.*";
            var res = sfd.ShowDialog();
            if (res.HasValue && res.Value)
            {
                try
                {
                    textEditor.Text = File.ReadAllText(sfd.FileName);
                }
                catch
                {
                    MessageBox.Show("Failed To Open File", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public String GetLuaCode()
        {
            return textEditor.Text;
        }
        public bool TryCode()
        {
            try
            {

                /*if (!Regex.IsMatch(textEditor.Text, @"function +onTick\([^\)]+\)"))
                {
                    OutputBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                    OutputBlock.Text = "Code must include an onTick(ticktype) function";
                    return false;
                }*/
                Lua state = new Lua();
                LuaFunctions.Init(state);
                Stopwatch stopwatch = Stopwatch.StartNew();
                state.DoString(textEditor.Text);
                stopwatch.Stop();
                
              
                state.Close();
                OutputBlock.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 0));
                OutputBlock.Text = $"Code successfully compiled in {stopwatch.ElapsedMilliseconds}Ms.";
                return true;
            }
            catch(Exception e)
            {
                OutputBlock.Foreground = new SolidColorBrush(Color.FromRgb(255,0,0));
                OutputBlock.Text = e.Message;
                return false;
            }
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            TryCode();
        }

        private void QuickKeybind_Click(object sender, RoutedEventArgs e)
        {
            ReadNextKeyInput = !ReadNextKeyInput;
        }

        private void textEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (ReadNextKeyInput)
            {
                e.Handled = true;
                var txt = "Key." + ((VirtualKeyCode)KeyInterop.VirtualKeyFromKey(e.Key)).ToString();
                Debug.WriteLine(txt);
                textEditor.Document.Insert(textEditor.CaretOffset, txt);
                ReadNextKeyInput = false;
            }
        }

        private void OpenConsoleWindow_Click(object sender, ExecutedRoutedEventArgs e)
        {
            outputConsoleWindow?.Close();
            outputConsoleWindow = new OutputConsoleWindow();
            outputConsoleWindow.Show();
        }
    }
}
