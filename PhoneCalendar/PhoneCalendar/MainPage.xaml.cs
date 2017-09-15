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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace PhoneCalendar
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private List<Day> days;
        int standardDayHeight = 60;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            days = new List<Day>();
            setupDecember();
        }

        private void setupDecember()
        {
            clearCalendar();
            this.MondayLabel.Text = "December";

            DaysHPanel1.Children.Add(new Day(60, 48, -1));
            DaysHPanel1.Children.Add(new Day(60, 60, -1));
            DaysHPanel1.Children.Add(new Day(60, 60, 1));
            DaysHPanel1.Children.Add(new Day(60, 60, 2));
            DaysHPanel1.Children.Add(new Day(60, 60, 3));
            DaysHPanel1.Children.Add(new Day(60, 60, 4));
            DaysHPanel1.Children.Add(new Day(60, 52, 5));

            DaysHPanel2.Children.Add(new Day(60, 48, 6));
            DaysHPanel2.Children.Add(new Day(60, 60, 7));
            DaysHPanel2.Children.Add(new Day(60, 60, 8));
            DaysHPanel2.Children.Add(new Day(60, 60, 9));
            DaysHPanel2.Children.Add(new Day(60, 60, 10));
            DaysHPanel2.Children.Add(new Day(60, 60, 11));
            DaysHPanel2.Children.Add(new Day(60, 52, 12));

            DaysHPanel3.Children.Add(new Day(60, 48, 13));
            DaysHPanel3.Children.Add(new Day(60, 60, 14));
            DaysHPanel3.Children.Add(new Day(60, 60, 15));
            DaysHPanel3.Children.Add(new Day(60, 60, 16));
            DaysHPanel3.Children.Add(new Day(60, 60, 17));
            DaysHPanel3.Children.Add(new Day(60, 60, 18));
            DaysHPanel3.Children.Add(new Day(60, 52, 19));

            DaysHPanel4.Children.Add(new Day(60, 48, 20));
            DaysHPanel4.Children.Add(new Day(60, 60, 21));
            DaysHPanel4.Children.Add(new Day(60, 60, 22));
            DaysHPanel4.Children.Add(new Day(60, 60, 23));
            DaysHPanel4.Children.Add(new Day(60, 60, 24));
            DaysHPanel4.Children.Add(new Day(60, 60, 25));
            DaysHPanel4.Children.Add(new Day(60, 52, 26));

            DaysHPanel5.Children.Add(new Day(60, 48, 27));
            DaysHPanel5.Children.Add(new Day(60, 60, 28));
            DaysHPanel5.Children.Add(new Day(60, 60, 29));
            DaysHPanel5.Children.Add(new Day(60, 60, 30));
            DaysHPanel5.Children.Add(new Day(60, 60, 31));
            DaysHPanel5.Children.Add(new Day(60, 60, -1));
            DaysHPanel5.Children.Add(new Day(60, 52, -1));

            DaysHPanel6.Children.Add(new Day(60, 48, -6));
            DaysHPanel6.Children.Add(new Day(60, 60, -7));
            DaysHPanel6.Children.Add(new Day(60, 60, -8));
            DaysHPanel6.Children.Add(new Day(60, 60, -9));
            DaysHPanel6.Children.Add(new Day(60, 60, -10));
            DaysHPanel6.Children.Add(new Day(60, 60, -11));
            DaysHPanel6.Children.Add(new Day(60, 52, -12));

            Event e = new Event("Test", DateTime.Now);
            EventUC testEvent = new EventUC(e);
            this.EventsBox.Items.Add(testEvent);
        }

        private void clearCalendar()
        {
            DaysHPanel1.Children.Clear();
            DaysHPanel2.Children.Clear();
            DaysHPanel3.Children.Clear();
            DaysHPanel4.Children.Clear();
            DaysHPanel5.Children.Clear();
            DaysHPanel6.Children.Clear();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }
    }
}
