using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Input;
using WindowsInput;
using WindowsInput.Native;
using NLua;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Xml;
using System.IO;
using Microsoft.Win32;
using NLUA_Test.Lua_Editor;

namespace NLUA_Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private LuaRobotV2 currentRobotV2;
        public void LoadRobotV2(LuaRobotV2 r)
        {
            BindKeyHandlers(false);
            currentRobotV2 = r;
            NameBlock.Text = r.Name;
            DescBlock.Text = r.Description;
            DescBlock.Visibility = (r.Description == "") ? Visibility.Collapsed : Visibility.Visible;
            NameBlock.Margin = (r.Description == "") ? new Thickness(0, 5, 0, 10) : new Thickness(0, 5, 0, 0);
            BindStackPanel.Children.Clear();
            foreach (var kb in r.Keybinds)
            {
                BindStackPanel.Children.Add(new KeybindClassViewer(kb));
            }
            BindKeyHandlers(true);
            RobotRunnerV2.LoadScript(r);
        }
        private void BindKeyHandlers(bool bind)
        {
            if (currentRobotV2 != null)
            {
                foreach (var kb in currentRobotV2.Keybinds)
                {
                    kb.Stop();
                    kb.Bind(bind);
                }
            }
        }
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            // new Settings().ShowDialog();
            OverlaySettings.Init();
            
            OverlaySettings.SetOverlaySide(OverlaySettings.ScreenSide.Center);
            OverlaySettings.SetToggleDisplayMessage("fuck them hoes and make some mother fuckin money", false);
            StorageManager.InitStorage();
            UpdateRecentFiles();
            //StorageManager.BrowseLocalFiles();
            LoadRobotV2(LuaRobotV2.Default());

            KeyboardHook.KeysManager.CtrlAltDeletePressed += CurrentKeysDown_CtrlAltDeletePressed;
            KeyboardHook.KeysManager.Hook();
            bool LoadMostRecent = true;
            if (LoadMostRecent)
            {
                var rec = getRecents();
                if(rec.Count>0)
                    LoadRobotV2(LuaRobotV2.LoadFromString(File.ReadAllText(rec[0].FullName)));
            }
        }

        

        private void CurrentKeysDown_CtrlAltDeletePressed(object? sender, EventArgs e)
        {
            RobotRunnerV2.Stop();
            currentRobotV2.Keybinds.ForEach(e => e.Stop());
        }

        private void UpdateRecentFiles()
        {
            RecentsMenu.Items.Clear();
            var fs = new DirectoryInfo(StorageManager.ResourcesPath).GetFiles().Where(e => e.Extension == ".RobotLua").OrderByDescending(w => w.LastAccessTime).Take(5);
            
            foreach (var item in getRecents())
            {
                Debug.WriteLine(item.Extension);
                var m = new MenuItem();
                m.Header= System.IO.Path.GetFileNameWithoutExtension(item.Name);
                m.Click += (a, b) => {
                    LoadRobotV2(LuaRobotV2.LoadFromString(File.ReadAllText(item.FullName)));
                };
                RecentsMenu.Items.Add(m);
            }
        }
        private List<FileInfo> getRecents()
        {
            return new DirectoryInfo(StorageManager.ResourcesPath).GetFiles().Where(e => e.Extension == ".RobotLua").OrderByDescending(w => w.LastAccessTime).Take(5).ToList();
            
        }
        private void editRobot(LuaRobotV2 rob)
        {
            var r = new LuaEditor(rob);
            BindKeyHandlers(false);
            var resp = r.ShowDialog();
            if (resp.HasValue && resp.Value)
            {
                LoadRobotV2(r.ReturnRobot);
                StorageManager.SaveFile(r.ReturnRobot.Name + ".RobotLua", r.ReturnRobot.Save());
                UpdateRecentFiles();
            }
            else
            {
                BindKeyHandlers(true);
            }
        }
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            editRobot(currentRobotV2);
        }

        private void OpenFile_Click(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Robot Lua Files (*.RobotLua)|*.RobotLua|All Files (*.*)|*.*";
            ofd.InitialDirectory = StorageManager.ResourcesPath;
            BindKeyHandlers(false);
            var resp = ofd.ShowDialog();
            if (resp.HasValue && resp.Value)
            {
                try
                {
                    var r = LuaRobotV2.LoadFromString(File.ReadAllText(ofd.FileName));
                    LoadRobotV2(r);
                    UpdateRecentFiles();
                }
                catch
                {
                    MessageBox.Show("An Error occured while reading the file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                BindKeyHandlers(true);
            }
            
        }

        private void ExportFile_Click(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Robot Lua Files (*.RobotLua)|*.RobotLua|All Files (*.*)|*.*";
            sfd.FileName = currentRobotV2.Name;
            BindKeyHandlers(false);
            var resp = sfd.ShowDialog();
            if (resp.HasValue && resp.Value)
            {
                File.WriteAllText(sfd.FileName, currentRobotV2.Save());
            }
            BindKeyHandlers(true);
        }

        private void New_Click(object sender, ExecutedRoutedEventArgs e)
        {
            var rob = LuaRobotV2.Default();
            rob.Name = "Unnamed";
            editRobot(rob);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            BindKeyHandlers(false);
            new Settings().ShowDialog();
            BindKeyHandlers(true);
        }

        private void Topbar_Drag(object sender, MouseButtonEventArgs e)
        {
            //Console.WriteLine("pressed");
            if (e.LeftButton == MouseButtonState.Pressed)
            {
              //  Console.WriteLine("dragging");
                this.DragMove();
                e.Handled = true;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
    
}
