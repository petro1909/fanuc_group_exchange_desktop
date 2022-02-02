using Microsoft.Win32;
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
using System.IO;
using fanuc_group_exchange_desktop.Model;
using fanuc_group_exchange_desktop.ViewModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace fanuc_group_exchange_desktop.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isFullScreen;


        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ApplicationViewModel(this);
        }

        private void GroupNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //WinParameters.Drag(Application.Current.MainWindow , isFullScreen);
                ChangeSinzeOfWindowImage.Source = new BitmapImage(new Uri(@"resources\img\maximize.png", UriKind.Relative));
                this.DragMove();
            }
        }

        private void MinimizeWindowClick(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }


        private void ChangeSinzeOfWindowClick(object sender, RoutedEventArgs e)
        {
            if (isFullScreen == true)
            {
                WinParameters.ChangeSize(Application.Current.MainWindow, isFullScreen);
                isFullScreen = false;
                ChangeSinzeOfWindowImage.Source = new BitmapImage(new Uri(@"resources\img\maximize.png", UriKind.Relative));
            }
            else
            {
                WinParameters.ChangeSize(Application.Current.MainWindow, isFullScreen);
                isFullScreen = true;
                ChangeSinzeOfWindowImage.Source = new BitmapImage(new Uri(@"resources\img\normalize.png", UriKind.Relative));
            }
        }

        private void CloseWindowClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public class WinParameters
        {
            static double Height;
            static double Width;
            static double Left;
            static double Top;

            public WinParameters()
            {

            }

            public static void ChangeSize(Window window, bool isFullScreen)
            {
                if (isFullScreen == false)
                {
                    Height = window.Height;
                    Width = window.Width;
                    Left = window.Left;
                    Top = window.Top;
                    window.Height = SystemParameters.WorkArea.Height;
                    window.Width = SystemParameters.WorkArea.Width;
                    window.Left = SystemParameters.WorkArea.Left;
                    window.Top = SystemParameters.WorkArea.Top;
                }
                else
                {
                    window.Height = Height;
                    window.Width = Width;
                    window.Left = Left;
                    window.Top = Top;
                }
            }

            public static void Drag(Window window, bool isFullScreen)
            {
                if (isFullScreen == true)
                {
                    window.Height = Height;
                    window.Width = Width;
                }
            }
        }
    }
}