using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace PhoneCalendar
{
    public sealed partial class Day : UserControl
    {
        public Day()
        {
            this.InitializeComponent();
        }

        public Day(int height, int width, int day)
        {
            this.InitializeComponent();

            changeHeight(height);

            this.Width = width;
            this.Border.Width = width;
            this.Container.Width = width - 2;

            if (day > 0)
            {
                this.DayNum.Text = day.ToString();
            } else
            {
                this.Container.Background = new SolidColorBrush(Windows.UI.Colors.Gray);
                this.DayNum.Text = "";
                this.DayMarker.Opacity = 0;
            }
        }

        public void changeHeight(int h)
        {
            this.Height = h;
            this.Border.Height = h;
            this.Container.Height = h - 2;
        }

        public void changeWidth(int h)
        {
            this.Width = h;
            this.Border.Width = h;
            this.Container.Width = h - 2;
        }

        public void changeSize(int h, int w)
        {
            changeHeight(h);
            changeWidth(w);
        }
    }
}
