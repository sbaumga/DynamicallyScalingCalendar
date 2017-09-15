﻿using System;
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

namespace Calendar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DynamicScalingCalendar calendar;

        public MainWindow()
        {
            InitializeComponent();

            calendar = new DynamicScalingCalendar(this.Height, this.Width);
            MainGrid.Children.Add(calendar);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            calendar.ResizeCalendar(Height, Width);
        }
    }
}