using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using System.IO;
using EnsureRisk.Resources;

namespace EnsureRisk.Pages
{
    /// <summary>
    /// Interaction logic for SetConnectionPage.xaml
    /// </summary>
    public partial class SetConnectionPage : Page
    {
        public DataTable tableConnectionServer { get; set; }
        public const string CONNECTION = "ConnectionServer.xml";
        public SetConnectionPage()
        {
            InitializeComponent();
            tableConnectionServer = new DataTable();
            ChangeLanguage();
        }

        public void ChangeLanguage()
        {
            MaterialDesignThemes.Wpf.HintAssist.SetHint(TextServer, StringResources.ServerAddressText);
            BtnSet.Content = StringResources.SetButton;
        }

        private void BtnSet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRow rowServer = tableConnectionServer.NewRow();
                rowServer["Server"] = TextServer.Text;
                rowServer["Date"] = DateTime.UtcNow;
                tableConnectionServer.Rows[0].Delete();
                tableConnectionServer.Rows.InsertAt(rowServer, 0);
                tableConnectionServer.WriteXml(CONNECTION, XmlWriteMode.WriteSchema);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void BtnSet_Loaded(object sender, RoutedEventArgs e)
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
                throw new Exception(ex.Message);
            }
        }
    }
}
