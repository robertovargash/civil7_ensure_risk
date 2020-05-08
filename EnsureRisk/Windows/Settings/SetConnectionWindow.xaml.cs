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
using System.Data;
using System.IO;
using EnsureBusinesss;

namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for SetConnectionWindow.xaml
    /// </summary>
    public partial class SetConnectionWindow : Window
    {
        public string servername { get; set; }
        public DataTable tableConnectionServer { get; set; }
        public const string CONNECTION = "ConnectionServer.xml";

        public SetConnectionWindow()
        {
            InitializeComponent();
            tableConnectionServer = new DataTable();
        }

        public void MostrarErrorDialog(string text)
        {
            ErrorMessageDialog.IsOpen = true;
            TextMessage.Text = text;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRow rowServer = tableConnectionServer.NewRow();
                rowServer["Server"] = TextServer.Text;
                rowServer["Date"] = DateTime.UtcNow;
                tableConnectionServer.Rows.InsertAt(rowServer, 0);
                tableConnectionServer.WriteXml(CONNECTION, XmlWriteMode.WriteSchema);
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnClose_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(CONNECTION))
                {
                    tableConnectionServer.ReadXml(CONNECTION);
                    TextServer.Text = tableConnectionServer.Select().First()["Server"].ToString();
                    //Auto();
                }

            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }
    }
}
