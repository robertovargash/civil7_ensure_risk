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
using DataMapping.Data;
using EnsureBusinesss;
using EnsureRisk.Resources;

namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private General gen;
        private byte[] password;
        public List<int> AccessList { get; set; }
        public string Usser { get; set; }

        public LoginWindow()
        {
            InitializeComponent();
            gen = new General();
            AccessList = new List<int>();
            ChangeLanguage();
        }

        public void MostrarErrorDialog(string text)
        {
            ErrorMessageDialog.IsOpen = true;
            TextMessage.Text = text;
        }

        public void ChangeLanguage()
        {
            MaterialDesignThemes.Wpf.HintAssist.SetHint(TextUser, StringResources.UserText);
            MaterialDesignThemes.Wpf.HintAssist.SetHint(TextPasword, StringResources.PasswordText);
            BtnLogin.Content = StringResources.LoginButton;
            BtnClose.Content = StringResources.CloseButton;
            Title = StringResources.LoginTitle;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Usser = "";
            this.DialogResult = false;
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                gen.Usser = TextUser.Text;
                password = General.Encrypt(TextPasword.Password);
                UserDataSet ds = new UserDataSet();
                ds.Merge(GetAccess());
                if (ds.Tables[DT_User.TABLE_NAME].Rows.Count != 0)
                {
                    Usser = ds.Tables[DT_User.TABLE_NAME].Rows[0][DT_User.USERNAME].ToString();
                    this.DialogResult = true;
                    foreach (DataRow item in ds.Tables[DT_User_Operation.TABLE_NAME].Rows)
                    {
                        AccessList.Add((int)item[DT_User_Operation.ID_OPERATION_COLUMN]);
                    }
                }
                else
                {
                    MostrarErrorDialog(StringResources.WRONG_PASSWORD);
                    TextPasword.Focus();
                    TextPasword.SelectAll();
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private DataSet GetAccess()
        {
            DataSet tempObtenerAcceso = new DataSet();
            try
            {
                ServiceAuthentication.WebServiceAuthentication ws = new ServiceAuthentication.WebServiceAuthentication();
                string Mensaje = ".";
                string Sesion = ".";
                tempObtenerAcceso = ws.AuthenticateUsers(TextUser.Text, General.Encrypt(TextPasword.Password), ref Mensaje, Environment.UserName,
                    Environment.UserDomainName + "\\" + Environment.MachineName, ref Sesion);
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
            return tempObtenerAcceso;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TextUser.Focus();
        }

        private void TextPasword_GotFocus(object sender, RoutedEventArgs e)
        {
            TextPasword.SelectAll();
        }

        private void TextUser_GotFocus(object sender, RoutedEventArgs e)
        {
            TextUser.SelectAll();
        }
    }
}
