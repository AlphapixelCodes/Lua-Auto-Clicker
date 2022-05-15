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

namespace NLUA_Test.Lua_Editor
{
    /// <summary>
    /// Interaction logic for PixelXYColor.xaml
    /// </summary>
    public partial class PixelXYColor : UserControl
    {
        public PixelXYColor()
        {
            InitializeComponent();
        }

    
    string Color = "", XyStr = "",MMTo="",CAXYE="",gPC="";


    private void ColorBlock_MouseDown(object sender, MouseButtonEventArgs e)
    {
        Clipboard.SetText(Color);
        ColorBlock.Text = "Copied To Clipboard";
    }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var p = LuaFunctions.GetCursorPosition();
            var c = LuaFunctions.GetPixelColor(p[0], p[1]);
            Color = $"{String.Join(", ", c)}";
            ColorBlock.Text = Color;

            XyStr = $"{string.Join(", ", p)}";
            XyBlock.Text = XyStr;

            MMTo = $"Mouse.MoveTo({p[0]},{p[1]})";
            MouseMoveToBox.Text = MMTo;

            CAXYE = $"ColorAtXYEquals({p[0]},{p[1]},{c[0]},{c[1]},{c[2]})";
            ColorAtXYEqualsBox.Text = CAXYE;

            gPC=$"getPixelColor({p[0]},{p[1]})";
            getPixelColorBox.Text = gPC;
        }

        private void getPixelColor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(gPC);
            getPixelColorBox.Text = "Copied To Clipboard";
        }

        private void ColorAtXYEquals_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(CAXYE);
            ColorAtXYEqualsBox.Text = "Copied To Clipboard";
        }

        private void MouseMoveTo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(MMTo);
            MouseMoveToBox.Text = "Copied To Clipboard";
        }

        private void XyBlock_MouseDown(object sender, MouseButtonEventArgs e)
    {
        Clipboard.SetText(XyStr);
        XyBlock.Text = "Copied To Clipboard";
    }
}
}
