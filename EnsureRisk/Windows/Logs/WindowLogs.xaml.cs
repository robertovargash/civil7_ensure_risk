using DataMapping.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
using System.Windows.Shapes;

namespace EnsureRisk.Windows.Logs
{
    /// <summary>
    /// Interaction logic for WindowLogs.xaml
    /// </summary>
    public partial class WindowLogs : Window
    {

        public DataView Dv { get; set; }

        public WindowLogs()
        {
            InitializeComponent();
            CultureInfo ci = CultureInfo.CreateSpecificCulture(CultureInfo.CurrentCulture.Name);
            ci.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            Thread.CurrentThread.CurrentCulture = ci;
        }

        public bool IsNumeric(string value)
        {
            return value.All(char.IsNumber);
        }

        private void FindLogs()
        {
            using (ServiceLog.WebServiceMyLog ws = new ServiceLog.WebServiceMyLog())
            {
                decimal id = -9999;
                if (!(string.IsNullOrWhiteSpace(textID.Text)) && IsNumeric(textID.Text))
                {
                    id = decimal.Parse(textID.Text);
                }
                
                string user = textUser.Text;
                string log = textLog.Text;

                string serverStart = string.IsNullOrWhiteSpace(dtpServerStart.Text) ? "01/01/2020" : dtpServerStart.Text;
                string serverEnd = string.IsNullOrWhiteSpace(dtpServerEnd.Text) ? DateTime.Today.ToString("dd/MM/yyyy") : dtpServerEnd.Text;


                Dv = new DataView
                {
                    Table = ws.GetAllLogs(new object[] { id, log, user, serverStart, serverEnd }).Tables[DT_MyLog.TABLE_NAME]
                };
                dgLogs.ItemsSource = Dv;
            }
        }

        private void btnSerch_Click(object sender, RoutedEventArgs e)
        {
            FindLogs();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FindLogs();
        }
    }
}
