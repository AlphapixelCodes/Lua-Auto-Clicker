using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using NLUA_Test.Lua_Editor;
using NLUA_Test.Lua_Editor.Pages;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml;
namespace NLUA_Test
{
    /// <summary>
    /// Interaction logic for LuaEditor.xaml
    /// </summary>
    public partial class LuaEditor : Window
    {
        private Page Editor, Api,PropertiesPage;
        public LuaRobotV2 ReturnRobot;
        private bool CtrlDown;
        private void Editor_Click(object sender, RoutedEventArgs e)
        {
            ApiFrame.Visibility= Visibility.Collapsed;
            PropertiesFrame.Visibility = Visibility.Collapsed;
            EditorFrame.Visibility = Visibility.Visible;
        }

        private void Api_Click(object sender, RoutedEventArgs e)
        {
            ApiFrame.Visibility = Visibility.Visible;
            EditorFrame.Visibility = Visibility.Collapsed;
            PropertiesFrame.Visibility = Visibility.Collapsed;

        }

        private void Properties_Click(object sender, RoutedEventArgs e)
        {
            ApiFrame.Visibility = Visibility.Collapsed;
            PropertiesFrame.Visibility = Visibility.Visible;
            EditorFrame.Visibility = Visibility.Collapsed;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            EditorFrame.Content = Editor;
            ApiFrame.Content= Api;
            PropertiesFrame.Content= PropertiesPage;
            PropertiesFrame.Visibility= Visibility.Collapsed;
            ApiFrame.Visibility=Visibility.Collapsed;
        }

       

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to discard your changes?", "Discard Changes", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                DialogResult = false;
                Close();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!DialogResult.HasValue)
            {
                if(MessageBoxResult.No == MessageBox.Show("Are you sure you want to close without saving?","Discard Changes", MessageBoxButton.YesNo, MessageBoxImage.Warning))
                {
                    e.Cancel = true;
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key== Key.LeftCtrl)
            {
                CtrlDown = e.IsDown;
            }
            else if(e.Key==Key.Tab && e.IsDown)
            {
                 if(EditorFrame.Visibility == Visibility.Visible)
                 {
                    Properties_Click(null, new RoutedEventArgs());
                 }else if (PropertiesFrame.Visibility == Visibility.Visible)
                {
                    Api_Click(null, new RoutedEventArgs());
                }
                else
                {
                    Editor_Click(null, new RoutedEventArgs());
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            //var prop = (PropertiesPage)PropertiesPage;
            var prop = (PropertyPageV2)PropertiesPage;
            var editor = (LuaEditorPage)Editor;
            if (prop.Validate() && editor.Validate())
            {
                if (ReturnRobot.Name != prop.GetName())
                {
                    if (StorageManager.FileExists(prop.GetName() + ".RobotLua"))
                    {
                        if(MessageBoxResult.No==MessageBox.Show($"A Robot already exists by the Name \"{prop.GetName()}\"\nOverwrite?", "Overwrite?", MessageBoxButton.YesNo, MessageBoxImage.Warning))
                        {
                            return;
                        }
                    }
                }
                prop.UpdateClass();
                editor.UpdateClass();
                DialogResult = true;
                Close();
            }
        }

        public LuaEditor(LuaRobotV2 data)
        {
            //Data=data.Clone();
            ReturnRobot=data.Clone();
            Editor = new LuaEditorPage(ReturnRobot);
            PropertiesPage = new PropertyPageV2(ReturnRobot,((LuaEditorPage)Editor).textEditor);
            Api = new APIPage();
            InitializeComponent();
        }


    }
}

