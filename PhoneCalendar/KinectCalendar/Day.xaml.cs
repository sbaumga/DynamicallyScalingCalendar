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

namespace KinectCalendar
{
    /// <summary>
    /// Interaction logic for Day.xaml
    /// </summary>
    public partial class Day : UserControl
    {
        int baseFontSize = 20;

        int baseHeight = 60;
        int baseWidth = 60;

        int baseMarkerRadius = 25;
        int baseMarkerX = 20;
        int baseMarkerY = 30;

        int baseBoxHeight = 34;
        int baseBoxWidth = 58;
        int baseBoxY = 24;

        int dayNum = -1;

        SolidColorBrush lightBlue = new SolidColorBrush(Color.FromArgb(255, 157, 189, 198));
        SolidColorBrush red = new SolidColorBrush(Color.FromArgb(255, 255, 61, 46));

        public Day()
        {
            this.InitializeComponent();
        }

        public Day(double height, double width, int day)
        {
            this.InitializeComponent();

            this.Container.Background = lightBlue;
            this.DayMarker.Fill = red;

            dayNum = day;
            changeSize(height, width);

            if (day > 0)
            {
                this.DayNum.Text = day.ToString();
            }
            else
            {
                this.Container.Background = new SolidColorBrush(Colors.Gray);
                this.DayNum.Text = "";
                this.DayMarker.Visibility = Visibility.Collapsed;
                this.EventsBox.Visibility = Visibility.Collapsed;
            }
        }

        public double getHeight()
        {
            return this.Height;
        }

        public void changeHeight(double h)
        {
            this.Height = h;
            this.Border.Height = h;
            this.Container.Height = h - 2;
        }

        public double getWidth()
        {
            return this.Width;
        }

        public void changeWidth(double h)
        {
            this.Width = h;
            this.Border.Width = h;
            this.Container.Width = h - 2;
        }

        public void changeSize(double h, double w)
        {
            changeHeight(h);
            changeWidth(w);

            double mod;
            if (h < w)
            {
                mod = h / baseHeight;
            } else
            {
                mod = w / baseWidth;
            }

            if (dayNum > 0)
            {
                if (h < 250)
                {
                    EventsBox.Visibility = Visibility.Collapsed;
                    if (EventsBox.HasItems)
                    {
                        DayMarker.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    DayMarker.Visibility = Visibility.Collapsed;
                    EventsBox.Visibility = Visibility.Visible;
                }
            }

            DayNum.FontSize = mod * baseFontSize;

            DayMarker.Height = mod * baseMarkerRadius;
            DayMarker.Width = mod * baseMarkerRadius;
            Canvas.SetLeft(DayMarker, mod * baseMarkerX);
            Canvas.SetTop(DayMarker, mod * baseMarkerY);

            EventsBox.Height = (h / baseHeight) * baseBoxHeight;
            EventsBox.Width = (w / baseWidth) * baseBoxWidth;
            Canvas.SetTop(EventsBox, mod * baseBoxY);
        }
    }
}
