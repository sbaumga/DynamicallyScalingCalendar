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
using System.Threading;

namespace KinectCalendar
{
    /// <summary>
    /// Interaction logic for Event.xaml
    /// </summary>
    public partial class Event : UserControl
    {

        SolidColorBrush grey = new SolidColorBrush(Color.FromArgb(255, 218, 234, 239));
        SolidColorBrush red = new SolidColorBrush(Color.FromArgb(255, 255, 61, 46));

        private int currentA;
        private int currentR;
        private int currentG;
        private int currentB;

        private int endR = 255;
        private int endG = 61;
        private int endB = 46;

        private int counter = 0;

        private Timer colorTimer;

        private String time;
        public String Time
        {
            get
            {
                return time;
            }
            set
            {
                time = value;
                if (time != null)
                {
                    this.TimeLabel.Text = time;
                } else
                {
                    this.TimeLabel.Visibility = Visibility.Collapsed;
                    Canvas.SetLeft(this.DescriptionLabel, 0);

                    currentA = 255;
                    currentR = 218;
                    currentG = 234;
                    currentB = 239;
                    
                    colorTimer = new Timer(updateColor, null, 500, Timeout.Infinite);
                }
            }
        }

        private String description;
        private String Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
                this.DescriptionLabel.Text = description;
            }
        }

        private void updateColor(Object state)
        {
            this.Dispatcher.Invoke((Action)(() =>
                {
                    if (currentR != endR && currentG != endG && currentB != endB)
                    {
                        counter++;
                        currentR = (int)Map(counter, 0, 240, 218, 255);
                        currentG = (int)Map(counter, 0, 240, 234, 61);
                        Console.WriteLine(currentG);
                        currentB = (int)Map(counter, 0, 240, 239, 46);

                        this.Container.Background = new SolidColorBrush(Color.FromArgb((byte)currentA, (byte)currentR, (byte)currentG, (byte)currentB));
                        colorTimer.Change(500, Timeout.Infinite);
                    }
            }));
        }

        public double Map(double value, double min, double max, double newMin, double newMax)
        {
            double result;

            // Find number of "steps" needed to get from old min to old max
            double numPossibleSteps1 = max - min;
            // Find number of "steps" needed to get from new min to new max
            double numPossibleSteps2 = newMax - newMin;

            // Get a ratio of old steps to new steps
            double mod = numPossibleSteps2 / numPossibleSteps1;

            // Find the number of "steps" needed to get from the old min value to the passed value
            double numStepsNeeded = value - min;

            // Use the ratio calculated to translate the steps needed to get from old min value to the passed value
            // to the number of steps needed to get to the result value from the new min
            double newNumSteps = mod * numStepsNeeded;

            // Add the calculated number of steps to the new min to get the result
            result = newMin + newNumSteps;

            return result;
        }

        public Event()
        {
            InitializeComponent();
        }

        public Event(String t, String d)
        {
            InitializeComponent();

            this.Container.Background = grey;

            Time = t;
            Description = d;
        }
    }
}
