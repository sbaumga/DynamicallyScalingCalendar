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
    public sealed partial class EventUC : UserControl
    {
        public EventUC()
        {
            this.InitializeComponent();
        }

        public EventUC(Event e)
        {
            this.InitializeComponent();

            this.eventName.Text = e.Name;
            this.eventTime.Text = e.Date.TimeOfDay.ToString();
        }
    }
}
