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
using System.Windows.Media.Animation;

namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for WindowOpening.xaml
    /// </summary>
    public partial class WindowOpening : Window
    {
        public WindowOpening()
        {
            InitializeComponent();
        }

        private Storyboard Animacion()
        {
            return (Storyboard)Resources["splashAnimacion"];
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Animacion().Begin();
        }

        private void Storyboard_Completed(object sender, EventArgs e)
        {
            Close();
        }
    }
}
