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
using System.Windows.Threading;

namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for VerticalMenu.xaml
    /// </summary>
    public partial class VerticalMenu : Window
    {

        public VerticalMenu(string TextValue)
        {
            InitializeComponent();
            TextToShow.Text = TextValue;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StartCloseTimer();
        }

        private void StartCloseTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.5d);
            timer.Tick += TimerTick;
            timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();
            timer.Tick -= TimerTick;
            Close();
        }

    }
}
