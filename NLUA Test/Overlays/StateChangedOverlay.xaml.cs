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
using System.Threading;
using System.Windows.Media.Animation;

namespace NLUA_Test
{
    /// <summary>
    /// Interaction logic for StateChangedOverlay.xaml
    /// </summary>
    public partial class StateChangedOverlay : Window
    {
        private CancellationTokenSource? Token;
        public void SetScreenSide(OverlaySettings.ScreenSide side)
        {
            switch (side) {
                case OverlaySettings.ScreenSide.Left:
                    Left = 0;
                    BorderControl.CornerRadius = new CornerRadius(0, 0, 10, 0);
                    break;
                case OverlaySettings.ScreenSide.Right:
                    Left = SystemParameters.FullPrimaryScreenWidth - Width;
                    BorderControl.CornerRadius = new CornerRadius(0, 0, 0, 10);
                    break;
                case OverlaySettings.ScreenSide.Center:
                    Left = (SystemParameters.FullPrimaryScreenWidth/2) - (Width/2);
                    BorderControl.CornerRadius = new CornerRadius(0, 0, 10, 10);
                    break;
            } 
        }

        internal void SettingsUpdated()
        {
            SetScreenSide(OverlaySettings.OverlaySide);
        }

        public void DisplayMessage(string name,bool status)
        {
            Token?.Cancel();
            Animate(true);
            Token = new CancellationTokenSource();
            //this.Visibility = Visibility.Visible;
            NameBlock.Text = name.Length>27? name.Substring(0,24).TrimEnd(' ')+"...":name;
            StatusBlock.Text = status ? "Enabled" : "Disabled";
            StatusBlock.Foreground = Application.Current.Resources[(status ? "Enabled" : "Disabled") + "Text"] as SolidColorBrush;
            ThreadPool.QueueUserWorkItem(new WaitCallback(DisplayTimer), Token.Token);
        }

        private void DisplayTimer(object? state)
        {
            Thread.Sleep(OverlaySettings.ToggleDisplayTime);
            if (!((CancellationToken)state).IsCancellationRequested)
            {
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    Animate(false);
                }));
            }
        }
        private void Animate(bool vis)
        {
            DoubleAnimation anim = new DoubleAnimation(vis? 1:0,TimeSpan.FromMilliseconds(100));
            this.BeginAnimation(OpacityProperty, anim);
        }
        public StateChangedOverlay()
        {
            InitializeComponent();
        }

      
    }
}
