﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Calendar
{
    /// <summary>
    /// Interaction logic for DynamicScalingCalendar.xaml
    /// </summary>
    public partial class DynamicScalingCalendar : UserControl
    {
        SolidColorBrush darkBlue = new SolidColorBrush(Color.FromArgb(255, 39, 47, 50));
        SolidColorBrush lightBlue = new SolidColorBrush(Color.FromArgb(255, 157, 189, 198));
        SolidColorBrush beige = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        SolidColorBrush red = new SolidColorBrush(Color.FromArgb(255, 255, 61, 46));
        SolidColorBrush grey = new SolidColorBrush(Color.FromArgb(255, 218, 234, 239));

        enum States { day, dayToWeek, week, weekToMonth, month };
        States state = States.day;

        double dayMinVal = 100;
        double dayMaxVal = 800;
        double dayViewHeight = 2000;
        double dayWeekViewHeight = 1000;
        double dayWeekViewWidth = 250;
        double dayMonthViewHeight = 150;

        double daySuLabelMinWidth = 55;
        double daySuLabelMaxWidth = 180;
        double dayMLabelMinWidth = 45;
        double dayMLabelMaxWidth = 195;
        double dayTuLabelMinWidth = 55;
        double dayTuLabelMaxWidth = 195;
        double dayWLabelMinWidth = 49;
        double dayWLabelMaxWidth = 275;
        double dayThLabelMinWidth = 58;
        double dayThLabelMaxWidth = 220;
        double dayFLabelMinWidth = 25;
        double dayFLabelMaxWidth = 155;
        double daySaLabelMinWidth = 53;
        double daySaLabelMaxWidth = 210;

        double maxScreenHeight = 1100;
        double maxScreenWidth = 2000;

        int gestureCounter = 0;
        int gestureCounterEnd = 100;
        double desiredWidth = 0;
        double desiredHeight = 0;
        double startWidth = 0;
        double startHeight = 0;
        Timer gestureSizingTimer;
        bool resizing = false;

        // Represents the selected week out of the possible 6 shown on the calendar
        int selectedWeek = 0;
        // Represents the selected day of the week. Sunday = 0
        int selectedDay = 0;

        List<Day> days = new List<Day>();
        List<TextBlock> dayLabels = new List<TextBlock>();
        List<Path> dividers = new List<Path>();
        private Day testDay = new Day(100, 100, 1);

        private bool handGestureStarted = false;
        private Timer gestureStartedTimer;
        private bool handOpen = false;
        private bool handClosed = false;
        private Timer gestureCompletedTimer;

        public const double MonthToWeekChangePoint = 2;
        public const double WeekToDayChangePoint = 3;

        private Timer timeTimer;

        public void ResizeCalendar(double height, double width)
        {
            maxScreenHeight = height;
            dayViewHeight = maxScreenHeight - (DaysCanvas.Height + MonthCanvas.Height);
            dayMonthViewHeight = dayViewHeight / 6;
            maxScreenWidth = width;
            dayWeekViewWidth = maxScreenWidth / 7;

            repositionDayLabels(dayWeekViewWidth);
            repositionDividers(dayWeekViewWidth);

            double currentDayWidth;
            if (CurrentScrollPos <= MonthToWeekChangePoint)
            {
                currentDayWidth = dayWeekViewWidth;
            }
            else
            {
                currentDayWidth = Map(Math.Min(CurrentScrollPos, 3), MonthToWeekChangePoint, 3, dayWeekViewWidth, maxScreenWidth);
            }

            double currentDayHeight;
            if (CurrentScrollPos >= MonthToWeekChangePoint)
            {
                currentDayHeight = maxScreenHeight;
            }
            else
            {
                currentDayHeight = Map(CurrentScrollPos, 0.01, MonthToWeekChangePoint, dayMonthViewHeight,
                    maxScreenHeight);
            }

            resizeDays(currentDayHeight, currentDayWidth);
        }

        public DynamicScalingCalendar(double initialHeight, double initialWidth)
        {
            InitializeComponent();

            dayLabels.Add(SundayLabel);
            dayLabels.Add(MondayLabel);
            dayLabels.Add(TuesdayLabel);
            dayLabels.Add(WednesdayLabel);
            dayLabels.Add(ThursdayLabel);
            dayLabels.Add(FridayLabel);
            dayLabels.Add(SaturdayLabel);
            
            dividers.Add(Divder1);
            dividers.Add(Divder2);
            dividers.Add(Divder3);
            dividers.Add(Divder4);
            dividers.Add(Divder5);
            dividers.Add(Divder6);
            
            ResizeCalendar(initialHeight, initialWidth);

            intializeColors();

            DateTime today = DateTime.Today;
            CreateMonthForDay(today);

            int[] todaysIndices = findIndices(today);
            selectedWeek = todaysIndices[0];
            selectedDay = todaysIndices[1];

            testDay = findDay(today);
            testDay.EventsBox.Items.Add(new Event(null, "Reschedule appointment"));
            testDay.EventsBox.Items.Add(new Event("10:00 am", "581 Demo"));
            testDay.DayNum.Foreground = red;

            Day testDay2 = (Day)DaysPanel3.Children[3];
            testDay2.EventsBox.Items.Add(new Event("7:00 am", "Doctor appointment"));

            Day testDay3 = (Day)DaysPanel2.Children[5];
            testDay3.EventsBox.Items.Add(new Event("10:00 am", "Adobe Premiere Tutorial"));

            timeTimer = new Timer(updateTime, null, 1, Timeout.Infinite);

            changeState(States.day);

            double newSize = maxScreenWidth;

            repositionDividers(newSize);
            repositionDayLabels(newSize);
            resizeDayLabels(3);

            repositionWeeksH(3);
            repositionWeeksV();
            repositionDaysCanvas(3);

            resizeDays(dayViewHeight, newSize);

            ResizeToDay();
        }

        private void updateTime(Object state)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                string time = "";
                DateTime now = DateTime.Now;
                bool isAM = false;
                if (now.Hour == 0)
                {
                    isAM = true;
                    time += "12";
                }
                else if (now.Hour > 0 && now.Hour < 12)
                {
                    isAM = true;
                    time += now.Hour;
                }
                else if (now.Hour == 12)
                {
                    isAM = false;
                    time += now.Hour;
                }
                else
                {
                    isAM = false;
                    time += now.Hour - 12;
                }
                time += ":";
                if (now.Minute < 10)
                {
                    time += "0";
                }
                time += now.Minute;
                time += " ";

                if (isAM)
                {
                    time += "AM";
                }
                else
                {
                    time += "PM";
                }

                this.TimeLabel.Text = time;
                timeTimer.Change(60000, Timeout.Infinite);
            }));
        }

        private Day findDay(DateTime d)
        {
            Day day;

            int[] indices = findIndices(d);

            if (indices[0] == 0)
            {
                day = (Day)DaysPanel1.Children[indices[1]];
            }
            else if (indices[0] == 1)
            {
                day = (Day)DaysPanel2.Children[indices[1]];
            }
            else if (indices[0] == 2)
            {
                day = (Day)DaysPanel3.Children[indices[1]];
            }
            else if (indices[0] == 3)
            {
                day = (Day)DaysPanel4.Children[indices[1]];
            }
            else if (indices[0] == 4)
            {
                day = (Day)DaysPanel5.Children[indices[1]];
            }
            else if (indices[0] == 5)
            {
                day = (Day)DaysPanel6.Children[indices[1]];
            }
            else
            {
                day = null;
            }

            return day;
        }

        private int[] findIndices(DateTime d)
        {
            int[] indices;
            int verticalIndex = -1;
            int horizontalIndex = -1;

            if (d.Day >= 1 && d.Day <= 5)
            {
                verticalIndex = 0;
            }
            else if (d.Day >= 6 && d.Day <= 12)
            {
                verticalIndex = 1;
            }
            else if (d.Day >= 13 && d.Day <= 19)
            {
                verticalIndex = 2;
            }
            else if (d.Day >= 20 && d.Day <= 26)
            {
                verticalIndex = 3;
            }
            else if (d.Day >= 27 && d.Day <= 31)
            {
                verticalIndex = 4;
            }

            if (d.DayOfWeek == DayOfWeek.Sunday)
            {
                horizontalIndex = 0;
            }
            else if (d.DayOfWeek == DayOfWeek.Monday)
            {
                horizontalIndex = 1;
            }
            else if (d.DayOfWeek == DayOfWeek.Tuesday)
            {
                horizontalIndex = 2;
            }
            else if (d.DayOfWeek == DayOfWeek.Wednesday)
            {
                horizontalIndex = 3;
            }
            else if (d.DayOfWeek == DayOfWeek.Thursday)
            {
                horizontalIndex = 4;
            }
            else if (d.DayOfWeek == DayOfWeek.Friday)
            {
                horizontalIndex = 5;
            }
            else
            {
                horizontalIndex = 6;
            }

            indices = new int[] { verticalIndex, horizontalIndex };
            return indices;
        }

        private void intializeColors()
        {
            this.Container.Background = darkBlue;
            this.MonthCanvas.Background = darkBlue;
            this.DaysCanvas.Background = grey;

            this.MonthLabel.Foreground = lightBlue;
            this.TimeLabel.Foreground = lightBlue;

            foreach (Path p in dividers)
            {
                p.Fill = darkBlue;
            }


        }

        private void testMonthInitialization()
        {
            for (int i = 0; i < 7; i++)
            {
                DaysPanel1.Children.Add(new Day(1100, 2000, i + 1));
                DaysPanel2.Children.Add(new Day(100, 100, i + 8));
                DaysPanel3.Children.Add(new Day(100, 100, i + 15));
                DaysPanel4.Children.Add(new Day(100, 100, i + 22));
                DaysPanel5.Children.Add(new Day(100, 100, i + 29));
                DaysPanel6.Children.Add(new Day(100, 100, i + 36));
            }
        }

        private void decemeberMonthInitialization()
        {
            DaysPanel1.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, -1));
            DaysPanel1.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, -1));
            DaysPanel1.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, 1));
            DaysPanel1.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, 2));
            DaysPanel1.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, 3));
            DaysPanel1.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, 4));
            DaysPanel1.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, 5));

            DaysPanel2.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, 6));
            DaysPanel2.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, 7));
            DaysPanel2.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, 8));
            DaysPanel2.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, 9));
            DaysPanel2.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, 10));
            DaysPanel2.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, 11));
            DaysPanel2.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, 12));

            DaysPanel3.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, 13));
            DaysPanel3.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, 14));
            DaysPanel3.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, 15));
            DaysPanel3.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, 16));
            DaysPanel3.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, 17));
            DaysPanel3.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, 18));
            DaysPanel3.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, 19));

            DaysPanel4.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, 20));
            DaysPanel4.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, 21));
            DaysPanel4.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, 22));
            DaysPanel4.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, 23));
            DaysPanel4.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, 24));
            DaysPanel4.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, 25));
            DaysPanel4.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, 26));

            DaysPanel5.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, 27));
            DaysPanel5.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, 28));
            DaysPanel5.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, 29));
            DaysPanel5.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, 30));
            DaysPanel5.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, 31));
            DaysPanel5.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, -1));
            DaysPanel5.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, -1));

            DaysPanel6.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, -1));
            DaysPanel6.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, -1));
            DaysPanel6.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, -1));
            DaysPanel6.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, -1));
            DaysPanel6.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, -1));
            DaysPanel6.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, -1));
            DaysPanel6.Children.Add(new Day(dayMonthViewHeight, dayWeekViewWidth, -1));
        }

        private void CreateMonthForDay(DateTime dayInMonth)
        {
            var firstDay = new DateTime(dayInMonth.Year, dayInMonth.Month, 1);

            var startingOffset = (int)firstDay.DayOfWeek;

            for (var i = 0; i < 42; i++)
            {
                var dayNumber = (i - startingOffset) + 1;
                if (dayNumber > firstDay.AddMonths(1).AddDays(-1).Day)
                {
                    dayNumber = -1;
                }

                var day = new Day(dayMonthViewHeight, dayWeekViewWidth, dayNumber);

                StackPanel row = null;

                var rowNum = i / 7;
                switch (rowNum)
                {
                    case 0:
                        row = DaysPanel1;
                        break;
                    case 1:
                        row = DaysPanel2;
                        break;
                    case 2:
                        row = DaysPanel3;
                        break;
                    case 3:
                        row = DaysPanel4;
                        break;
                    case 4:
                        row = DaysPanel5;
                        break;
                    case 5:
                        row = DaysPanel6;
                        break;
                }

                row.Children.Add(day);
            }

            MonthLabel.Text = dayInMonth.ToString("MMMM, yyyy");
        }

        // Ensures that the day lables are the proper size for week and month views
        private void resizeDayLabels()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                SundayLabel.Width = daySuLabelMinWidth;
                MondayLabel.Width = dayMLabelMinWidth;
                TuesdayLabel.Width = dayTuLabelMinWidth;
                WednesdayLabel.Width = dayWLabelMinWidth;
                ThursdayLabel.Width = dayThLabelMinWidth;
                FridayLabel.Width = dayFLabelMinWidth;
                SaturdayLabel.Width = daySaLabelMinWidth;
            }));
        }

        // Resizes the day labels for the day to week transition
        private void resizeDayLabels(double depth)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                SundayLabel.Width = Map(depth, 2.1, 3, daySuLabelMinWidth, daySuLabelMaxWidth);
                MondayLabel.Width = Map(depth, 2.1, 3, dayMLabelMinWidth, dayMLabelMaxWidth);
                TuesdayLabel.Width = Map(depth, 2.1, 3, dayTuLabelMinWidth, dayTuLabelMaxWidth);
                WednesdayLabel.Width = Map(depth, 2.1, 3, dayWLabelMinWidth, dayWLabelMaxWidth);
                ThursdayLabel.Width = Map(depth, 2.1, 3, dayThLabelMinWidth, dayThLabelMaxWidth);
                FridayLabel.Width = Map(depth, 2.1, 3, dayFLabelMinWidth, dayFLabelMaxWidth);
                SaturdayLabel.Width = Map(depth, 2.1, 3, daySaLabelMinWidth, daySaLabelMaxWidth);
            }));
        }

        // Repositions the day labels for a day to week transition
        private void repositionDayLabels(double width)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                for (int i = 0; i < dayLabels.Count; i++)
                {
                    Canvas.SetLeft(dayLabels[i], (width / 2) - (dayLabels[i].Width / 2) + (width * i));
                }
            }));
        }

        /*
        Pass the desired distance between each divider
        */
        private void repositionDividers(double width)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                for (int i = 0; i < dividers.Count; i++)
                {
                    Canvas.SetLeft(dividers[i], width * (i + 1));
                }
            }));
        }

        // Repositions the weeks horizontally based on the selected day to transition from day to week
        private void repositionWeeksH(double depth)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                Canvas.SetLeft(WeeksPanel, -(Map(depth, 2.1, 3, 0, selectedDay * maxScreenWidth)));
            }));
        }

        // Ensures correct horizontal placement of the weeks for a month to week transition
        private void repositionWeeksH()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                Canvas.SetLeft(WeeksPanel, 0);
            }));
        }

        // Repositions the weeks vertically based on the selected week to transition from month to week
        private void repositionWeeksV(double depth)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                Canvas.SetTop(WeeksPanel, -(Map(depth, 1, 2, 0, (dayViewHeight * selectedWeek))) + 180);
            }));
        }

        // Ensures proper vertical placement of the weeks based on the selected week for the transition from week to day
        private void repositionWeeksV()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                Canvas.SetTop(WeeksPanel, -(dayViewHeight * selectedWeek) + 180);
            }));
        }

        private void repositionDaysCanvas(double depth)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                Canvas.SetLeft(DaysCanvas, -(Map(depth, 2.1, 3, 0, selectedDay * maxScreenWidth)));
            }));
        }

        private void repositionDaysCanvas()
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                Canvas.SetLeft(DaysCanvas, 0);
            }));
        }

        public double Map(double value, double min, double max, double newMin, double newMax)
        {
            double result;

            if (value == min)
            {
                result = newMin;
            }
            else if (value == max)
            {
                result = newMax;
            }
            else
            {
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
            }

            return result;
        }

        public void resizeDays(double newHeight, double newWidth)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                foreach (Day d in DaysPanel1.Children)
                {
                    d.changeSize(newHeight, newWidth);
                }
                foreach (Day d in DaysPanel2.Children)
                {
                    d.changeSize(newHeight, newWidth);
                }
                foreach (Day d in DaysPanel3.Children)
                {
                    d.changeSize(newHeight, newWidth);
                }
                foreach (Day d in DaysPanel4.Children)
                {
                    d.changeSize(newHeight, newWidth);
                }
                foreach (Day d in DaysPanel5.Children)
                {
                    d.changeSize(newHeight, newWidth);
                }
                foreach (Day d in DaysPanel6.Children)
                {
                    d.changeSize(newHeight, newWidth);
                }
            }));
        }

        private void resize(Object state)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                double newWidth = Map(gestureCounter, 0, gestureCounterEnd, startWidth, desiredWidth);
                double newHeight = Map(gestureCounter, 0, gestureCounterEnd, startHeight, desiredHeight);

                if ((int)startWidth != (int)desiredWidth)
                {
                    repositionDividers(newWidth);
                    repositionDayLabels(newWidth);

                    double simDepth;
                    if ((int)desiredWidth > (int)startWidth)
                    {
                        simDepth = Map(testDay.Width, dayWeekViewWidth, maxScreenWidth, 2.1, 3.0);
                        //simDepth = Map(gestureCounter, 0, gestureCounterEnd, 2.1, 3.0);
                    }
                    else
                    {
                        simDepth = Map(testDay.Width, maxScreenWidth, dayWeekViewWidth, 3.0, 2.1);
                        //simDepth = Map(gestureCounter, 0, gestureCounterEnd, 3.0, 2.1);
                    }

                    resizeDayLabels(simDepth);

                    repositionWeeksH(simDepth);
                    repositionWeeksV();
                    repositionDaysCanvas(simDepth);
                }
                else
                {
                    repositionDividers(desiredWidth);
                    repositionDayLabels(desiredWidth);

                    double simDepth;
                    if ((int)desiredHeight > (int)startHeight)
                    {
                        simDepth = Map(testDay.Height, dayMonthViewHeight, dayViewHeight, 1.0, 2.0);
                        //simDepth = Map(gestureCounter, 0, gestureCounterEnd, 1.0, 2.0);
                    }
                    else
                    {
                        simDepth = Map(testDay.Height, dayViewHeight, dayMonthViewHeight, 2.0, 1.0);
                        //simDepth = Map(gestureCounter, 0, gestureCounterEnd, 2.0, 1.0);
                    }

                    resizeDayLabels();

                    repositionWeeksH();
                    repositionWeeksV(simDepth);
                    repositionDaysCanvas();
                }

                resizeDays(newHeight, newWidth);

                if (gestureCounter != gestureCounterEnd)
                {
                    gestureCounter++;
                    gestureSizingTimer.Change(10, Timeout.Infinite);
                }
                else
                {
                    resizing = false;
                    gestureCounter = 0;
                    if (desiredHeight == dayViewHeight && desiredWidth == maxScreenWidth)
                    {
                        changeState(States.day);

                        double newSize = maxScreenWidth;

                        repositionDividers(newSize);
                        repositionDayLabels(newSize);
                        resizeDayLabels(3);

                        repositionWeeksH(3);
                        repositionWeeksV();
                        repositionDaysCanvas(3);

                        resizeDays(dayViewHeight, newSize);
                    }
                    else if (desiredHeight == dayViewHeight && desiredWidth == dayWeekViewWidth)
                    {
                        changeState(States.week);

                        double newSize = dayWeekViewWidth;

                        repositionDividers(newSize);
                        repositionDayLabels(newSize);
                        resizeDayLabels(2.1);

                        repositionWeeksH(2.1);
                        repositionWeeksV();
                        repositionDaysCanvas(2.1);

                        resizeDays(dayViewHeight, newSize);
                    }
                    else if (desiredHeight == dayMonthViewHeight && desiredWidth == dayWeekViewWidth)
                    {
                        changeState(States.month);

                        double newSize = dayMonthViewHeight;

                        repositionDividers(dayWeekViewWidth);
                        repositionDayLabels(dayWeekViewWidth);
                        resizeDayLabels();

                        repositionWeeksH();
                        repositionWeeksV(1);
                        repositionDaysCanvas();

                        resizeDays(newSize, dayWeekViewWidth);
                    }
                    desiredHeight = 0;
                    desiredWidth = 0;
                    startHeight = 0;
                    startWidth = 0;
                }
            }));
        }

        public void ResizeToWeek()
        {
            startWidth = testDay.getWidth();
            startHeight = testDay.getHeight();
            desiredWidth = dayWeekViewWidth;
            desiredHeight = dayViewHeight;
            gestureCounter = 0;
            resizing = true;
            gestureSizingTimer = new Timer(resize, null, 10, Timeout.Infinite);
        }

        public void ResizeToMonth()
        {
            startWidth = testDay.getWidth();
            startHeight = testDay.getHeight();
            desiredWidth = dayWeekViewWidth;
            desiredHeight = dayMonthViewHeight;
            gestureCounter = 0;
            resizing = true;
            gestureSizingTimer = new Timer(resize, null, 10, Timeout.Infinite);
        }

        public void ResizeToDay()
        {
            startWidth = testDay.getWidth();
            startHeight = testDay.getHeight();
            desiredWidth = maxScreenWidth;
            desiredHeight = dayViewHeight;
            gestureCounter = 0;
            resizing = true;
            gestureSizingTimer = new Timer(resize, null, 10, Timeout.Infinite);
        }

        private void changeState(States s)
        {
            state = s;
        }

        public void gestureOver(Object state)
        {
            handGestureStarted = false;
            handOpen = false;
            handClosed = false;
        }

        public void Resize(double resizeFactor)
        {
            if (resizeFactor >= 3 && !resizing)
            {
                changeState(States.day);

                double newSize = maxScreenWidth;

                repositionDividers(newSize);
                repositionDayLabels(newSize);
                resizeDayLabels(3);

                repositionWeeksH(3);
                repositionWeeksV();
                repositionDaysCanvas(3);

                resizeDays(dayViewHeight, newSize);
            }
            // Day to week transition
            else if (resizeFactor > 2.1 && resizeFactor < 3 && !resizing)
            {
                changeState(States.dayToWeek);

                double newSize = Map(resizeFactor, 2.1, 3, dayWeekViewWidth, maxScreenWidth);

                repositionDividers(newSize);
                repositionDayLabels(newSize);
                resizeDayLabels(resizeFactor);

                repositionWeeksH(resizeFactor);
                repositionWeeksV();
                repositionDaysCanvas(resizeFactor);

                resizeDays(dayViewHeight, newSize);

                // Week
            }
            else if (resizeFactor <= 2.1 && resizeFactor > 2.0 && !resizing)
            {
                changeState(States.week);

                double newSize = dayWeekViewWidth;

                repositionDividers(newSize);
                repositionDayLabels(newSize);
                resizeDayLabels(2.1);

                repositionWeeksH(2.1);
                repositionWeeksV();
                repositionDaysCanvas(2.1);

                resizeDays(dayViewHeight, newSize);
            }
            else if (resizeFactor < 2 && resizeFactor > 1 && !resizing)
            {
                changeState(States.weekToMonth);

                // Week to month tranisition
                double newSize = Map(resizeFactor, 1, 2, dayMonthViewHeight, dayViewHeight);

                repositionDividers(dayWeekViewWidth);
                repositionDayLabels(dayWeekViewWidth);
                resizeDayLabels();

                repositionWeeksH();
                repositionWeeksV(resizeFactor);
                repositionDaysCanvas();

                resizeDays(newSize, dayWeekViewWidth);
            }
            else if (resizeFactor <= 1 && !resizing)
            {
                // Month
                changeState(States.month);

                double newSize = dayMonthViewHeight;

                repositionDividers(dayWeekViewWidth);
                repositionDayLabels(dayWeekViewWidth);
                resizeDayLabels();

                repositionWeeksH();
                repositionWeeksV(1);
                repositionDaysCanvas();

                resizeDays(newSize, dayWeekViewWidth);
            }
        }

        private double CurrentScrollPos { get;set; }

        private void Container_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            CurrentScrollPos += e.Delta > 0 ? 0.01 : -0.01;
            Resize(CurrentScrollPos);
            Console.WriteLine(CurrentScrollPos);
        }
    }
}
