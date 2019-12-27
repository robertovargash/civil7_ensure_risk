using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DataMapping.Data;
using System.Data;
using EnsureBusinesss;
using EnsureRisk.Resources;

namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for WindowUser.xaml
    /// </summary>
    public partial class WindowUser : Window
    {
        public DataRow User { get; set; }
        public string Operation { get; set; }
        public DataTable RoleTable { get; set; }
        public DataView Dataview { get; set; }

        private bool changepassword;
        public WindowUser()
        {
            InitializeComponent();
            ChangeLanguage();
        }

        public void ChangeLanguage()
        {
            MaterialDesignThemes.Wpf.HintAssist.SetHint(TextUser, StringResources.UserText);
            MaterialDesignThemes.Wpf.HintAssist.SetHint(TextPasword, StringResources.PasswordText);
            MaterialDesignThemes.Wpf.HintAssist.SetHint(TextConfirm, StringResources.ConfirmPasswordText);
            MaterialDesignThemes.Wpf.HintAssist.SetHint(TextDescription, StringResources.UserDescriptionText);
            BtnCancel.Content = StringResources.CancelButton;
            Title = StringResources.UserDataTitle;
        }
        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (TextPasword.Password == TextConfirm.Password)
            {
                User[DT_User.USERNAME_COLUMNA] = TextUser.Text;
                if (changepassword)
                {
                    User[DT_User.USERPASSWORD_COLUMNA] = General.Encrypt(TextPasword.Password);
                }                
                User[DT_User.POSITIONDESCRIPTION_COLUMNA] = TextDescription.Text;
                DialogResult = true;
            }
            else
            {
                new WindowMessageOK(StringResources.PASSWORD_MATCH).ShowDialog();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnAddRole_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ServiceRoleController.WebServiceRole ws = new ServiceRoleController.WebServiceRole();
                DataTable OpCod = ws.GetRolesData().Tables[DT_Role.ROLE_TABLE].Copy();
                WindowSelection frm = new WindowSelection
                {
                    dt = OpCod,
                    //dt = General.EliminarExistenColumString(OpCod, RoleTable, RoleDatos.ROLE_COLUM),
                    dcolumToShow = new string[] { DT_Role.ROLE_TABLE },
                    dcolumToShowAlias = new string[] { DT_Role.ROLE_COLUM },
                    Title = "Role"
                };
                if (frm.ShowDialog() == true)
                {
                    foreach (DataRow item in frm.RowsSelected)
                    {
                        DataRow newrow = RoleTable.NewRow();
                        newrow[DT_User_Role.ROLE_COLUMN] = item[DT_Role.ROLE_COLUM];
                        newrow[DT_User_Role.IDROL_COLUMN] = item[DT_Role.IDROL_COLUMN];
                        newrow[DT_User_Role.USER_COLUMN] = TextUser.Text;
                        RoleTable.Rows.Add(newrow);
                    }
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();;
            }
        }

        private void BtnDelRole_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgRoles.SelectedIndex > 0)
                {
                     if (new WindowMessageYesNo(StringResources.DELETE_MESSAGE + " [" + RoleTable.Rows[dgRoles.SelectedIndex][DT_User_Role.ROLE_COLUMN] +"]?").ShowDialog() == true)
                        RoleTable.Rows[dgRoles.SelectedIndex].Delete();
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Operation == General.INSERT)
            {
                dgRoles.ItemsSource = RoleTable.DefaultView;
            }

            if (Operation == General.UPDATE)
            {
                try
                {
                    ServiceUserController.WebServiceUser ws = new ServiceUserController.WebServiceUser();
                    RoleTable = ws.GetUserRolesData(new object[] { User[DT_User.USERNAME_COLUMNA].ToString() }).Tables[DT_User_Role.TABLE_NAME].Copy();
                    dgRoles.ItemsSource = RoleTable.DefaultView;
                    TextUser.Text = User[DT_User.USERNAME_COLUMNA].ToString();
                    TextPasword.Password = General.ByteArrayToString((byte[])User[DT_User.USERPASSWORD_COLUMNA]);
                    TextConfirm.Password = General.ByteArrayToString((byte[])User[DT_User.USERPASSWORD_COLUMNA]);
                    TextDescription.Text = User[DT_User.POSITIONDESCRIPTION_COLUMNA].ToString();
                }
                catch (Exception ex)
                {
                    new WindowMessageOK(ex.Message).ShowDialog();
                }
            }
        }

        private void TextPasword_LostFocus(object sender, RoutedEventArgs e)
        {
            changepassword = true;
        }
    }
}
