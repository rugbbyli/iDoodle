using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using iDoodle.Resources;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;

namespace iDoodle
{
    public partial class MainPage : PhoneApplicationPage
    {
        // 构造函数
        public MainPage()
        {
            InitializeComponent();

            board.PenWidth = 10;
        }

        private void Btn_Save_Click(object sender, EventArgs e)
        {
            var bmp = board.GetImage(true);

            var lib = new Microsoft.Xna.Framework.Media.MediaLibrary();

            using(var stream = new System.IO.MemoryStream())
            {
                System.Windows.Media.Imaging.Extensions.SaveJpeg(bmp,stream,bmp.PixelWidth,bmp.PixelHeight,0,100);
                stream.Flush();
                var name = "i_doodle_" + DateTime.Now.ToString("yyyyMMddHHmmss");

                lib.SavePicture(name, stream.GetBuffer());
            }
        }

        private void Btn_Back_Click(object sender, EventArgs e)
        {
            if (board.CanStepBack) board.StepBack();
        }

        private void Btn_Next_Click(object sender, EventArgs e)
        {
            if (board.CanStepForward) board.StepForward();
        }

        private void Btn_Clear_Click(object sender, EventArgs e)
        {
            board.Clear();
        }

        private void Btn_Refresh_Click(object sender, EventArgs e)
        {
            var bmp = board.GetImage();

            previewWnd.Height = bmp.PixelHeight / 4;
            previewWnd.Width = bmp.PixelWidth / 4;
            previewWnd.Source = bmp;
        }

        private void Btn_PenWidth_Click(object sender, RoutedEventArgs e)
        {
            if (Slider_PW.Visibility == System.Windows.Visibility.Visible)
                Slider_PW.Visibility = System.Windows.Visibility.Collapsed;
            else
                Slider_PW.Visibility = System.Windows.Visibility.Visible;
        }

        private void Slider_PW_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (board != null) board.PenWidth = (e.NewValue - 1) * 4 + 1;
        }

        private void Slider_PC_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Color c = Colors.Black;
            switch ((int)e.NewValue)
            {
                case 1: c = Colors.White; break;
                case 2: c = Colors.Red; break;
                case 3: c = Colors.Yellow; break;
                case 4: c = Colors.Blue; break;
                case 5: c = Colors.Green; break;
                case 6: c = Colors.Black; break;
            }
            SolidColorBrush brush = new SolidColorBrush(c);
            (sender as Slider).Foreground = brush;

            if (board != null)
            {
                board.PenStroke = brush;
            }
        }

        private void Btn_PenColor_Click(object sender, RoutedEventArgs e)
        {
            if (Slider_PC.Visibility == System.Windows.Visibility.Visible)
                Slider_PC.Visibility = System.Windows.Visibility.Collapsed;
            else
                Slider_PC.Visibility = System.Windows.Visibility.Visible;
        }

        private void Slider_PO_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var opa = e.NewValue * 0.16 + 0.04;
            ((sender as Slider).Foreground) = new SolidColorBrush(Colors.Red) { Opacity = opa, };
            if (board != null)
            {
                board.PenOpacity = opa;
            }
        }

        private void Btn_PenOpacity_Click(object sender, RoutedEventArgs e)
        {
            if (Slider_PO.Visibility == System.Windows.Visibility.Visible)
                Slider_PO.Visibility = System.Windows.Visibility.Collapsed;
            else
                Slider_PO.Visibility = System.Windows.Visibility.Visible;
        }

        private void Btn_Open_Click(object sender, EventArgs e)
        {
            Microsoft.Phone.Tasks.PhotoChooserTask task = new Microsoft.Phone.Tasks.PhotoChooserTask();
            task.ShowCamera = true;
            task.Completed += (ss, ee) =>
            {
                if (ee.TaskResult == Microsoft.Phone.Tasks.TaskResult.OK)
                {
                    ImageBrush brush = new ImageBrush();
                    BitmapImage img = new BitmapImage();
                    img.SetSource(ee.ChosenPhoto);
                    brush.ImageSource = img;
                    board.BoardBrush = brush;
                }
            };
            task.Show();
        }

        void SliderInAnim(Control control, AnimDirection dir, int to = 0, int time = 500)
        {
            ExponentialEase ease = new ExponentialEase();
            ease.EasingMode = EasingMode.EaseOut;
            ease.Exponent = 5;
            
        }
    }

    enum AnimDirection
    {
        Left,
        Right,
        Top,
        Bottom,
    }
}