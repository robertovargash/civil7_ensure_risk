using System;
using System.Windows;
using DataMapping.Data;
using System.Data;
using EnsureBusinesss;
using EnsureRisk.Resources;
using System.ComponentModel;

namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for WindowUser.xaml
    /// </summary>
    public partial class WindowUser : Window, INotifyPropertyChanged
    {
        private string _user;
        private string _contra;
        private string _confirm;
        public string Usuario { get { return _user; } set { _user = value; OnPropertyChanged("Usuario"); } }
        public string Contrasenna { get { return _contra; } set { _contra = value; OnPropertyChanged("Contrasenna"); } }
        public string ConfContrasenna { get { return _confirm; } set { _confirm = value; OnPropertyChanged("ConfContrasenna"); } }
        public bool IS_DELETING { get; set; } = false;
        public DataRow User { get; set; }
        public string Operation { get; set; }
        public DataTable RoleTable { get; set; }
        //public DataTable WBSTable { get; set; }
        public DataView DVWBS { get; set; }

        private bool changepassword;
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public WindowUser()
        {
            InitializeComponent();
            ChangeLanguage();
            TextConfirm.DataContext = this;
            TextUser.DataContext = this;
            TextPasword.DataContext = this;
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
            MaterialDesignThemes.Wpf.HintAssist.SetHint(TextConfirm, StringResources.ConfirmPasswordText);
            MaterialDesignThemes.Wpf.HintAssist.SetHint(TextDescription, StringResources.UserDescriptionText);
            BtnCancel.Content = StringResources.CancelButton;
            Title = StringResources.UserDataTitle;
        }
        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (Operation == General.UPDATE)
            {
                if (changepassword)
                {
                    if (TextPasword.Password == TextConfirm.Password)
                    {
                        User[DT_User.USERNAME] = TextUser.Text;
                        User[DT_User.USERPASSWORD] = General.Encrypt(TextPasword.Password);
                        User[DT_User.POSITIONDESCRIPTION] = TextDescription.Text;
                        User[DT_User.FULL_NAME] = TextFullName.Text;
                        DialogResult = true;
                    }
                    else
                    {
                        MostrarErrorDialog(StringResources.PASSWORD_MATCH);
                    }
                }
                else
                {
                    User[DT_User.USERNAME] = TextUser.Text;
                    User[DT_User.USERPASSWORD] = General.Encrypt(TextPasword.Password);
                    User[DT_User.POSITIONDESCRIPTION] = TextDescription.Text;
                    User[DT_User.FULL_NAME] = TextFullName.Text;
                    DialogResult = true;
                }
            }
            else
            {
                if (TextPasword.Password == TextConfirm.Password)
                {
                    User[DT_User.USERNAME] = TextUser.Text;
                    if (changepassword)
                    {
                        User[DT_User.USERPASSWORD] = General.Encrypt(TextPasword.Password);
                    }
                    User[DT_User.POSITIONDESCRIPTION] = TextDescription.Text;
                    User[DT_User.FULL_NAME] = TextFullName.Text;
                    DialogResult = true;
                }
                else
                {
                    MostrarErrorDialog(StringResources.PASSWORD_MATCH);
                }
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
                    Dt = OpCod,
                    //dt = General.EliminarExistenColumString(OpCod, RoleTable, RoleDatos.ROLE_COLUM),
                    DcolumToShow = new string[] { DT_Role.ROLE_TABLE },
                    DcolumToShowAlias = new string[] { DT_Role.ROLE_COLUM },
                    Title = "Role"
                };
                frm.P.FilterString = "Role name";
                frm.ColumnToFilter = DT_Role.ROLE_COLUM;
                if (frm.ShowDialog() == true)
                {
                    foreach (DataRow item in frm.RowsSelected)
                    {
                        DataRow newrow = RoleTable.NewRow();
                        newrow[DT_User_Role.ROLE] = item[DT_Role.ROLE_COLUM];
                        newrow[DT_User_Role.IDROL_COLUMN] = item[DT_Role.IDROL_COLUMN];
                        newrow[DT_User_Role.USER] = TextUser.Text;
                        RoleTable.Rows.Add(newrow);
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        public void MostrarDialogYesNo(string textAlert)
        {
            YesNoDialog.IsOpen = true;
            TextYesNoMessage.Text = textAlert;
        }

        private void BtnDelRole_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgRoles.SelectedIndex > 0)
                {
                    IS_DELETING = true;
                    MostrarDialogYesNo(StringResources.DELETE_MESSAGE + " [" + RoleTable.Rows[dgRoles.SelectedIndex][DT_User_Role.ROLE] + "]?");
                }
            }
            catch (Exception ex)
            {
                IS_DELETING = false;
                MostrarErrorDialog(ex.Message);
            }
        }

        private void Delete_Role()
        {
            try
            {
                if (dgRoles.SelectedIndex > 0)
                {
                    RoleTable.Rows[dgRoles.SelectedIndex].Delete();
                }
                IS_DELETING = false;
            }
            catch (Exception ex)
            {
                IS_DELETING = false;
                MostrarErrorDialog(ex.Message);
            }            
        }

        private void YesNoDialog_DialogClosing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, true))
            {
                return;
            }
            if (Equals(eventArgs.Parameter, true))
            {
                if (IS_DELETING)
                {
                    Delete_Role();
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Operation == General.INSERT)
            {
                dgRoles.ItemsSource = RoleTable.DefaultView;
                DVWBS = new DataView();
                //DVWBS.Table = WBSTable;

                //dgWBS.ItemsSource = DVWBS;
                //dgWBS.ItemsSource = WBSTable.DefaultView;
                //DVWBS.RowFilter = DT_User_WBS.USER + " = '" + TextUser.Text + "'";
            }

            if (Operation == General.UPDATE)
            {
                try
                {
                    ServiceUserController.WebServiceUser ws = new ServiceUserController.WebServiceUser();
                    DataSet tempDS = ws.GetUserRolesData(new object[] { User[DT_User.USERNAME].ToString() });
                    RoleTable = tempDS.Tables[DT_User_Role.TABLE_NAME].Copy();
                    //WBSTable = tempDS.Tables[DT_User_WBS.TABLE_NAME].Copy();
                    dgRoles.ItemsSource = RoleTable.DefaultView;

                    //dgWBS.ItemsSource = WBSTable.DefaultView;

                    //WBSTable.DefaultView.RowFilter = DT_User_WBS.USER + " like '" + TextUser.Text + "'";

                    TextUser.Text = User[DT_User.USERNAME].ToString();
                    TextPasword.Password = General.ByteArrayToString((byte[])User[DT_User.USERPASSWORD]);
                    TextConfirm.Password = General.ByteArrayToString((byte[])User[DT_User.USERPASSWORD]);
                    TextDescription.Text = User[DT_User.POSITIONDESCRIPTION].ToString();
                    TextFullName.Text = User[DT_User.FULL_NAME].ToString();
                }
                catch (Exception ex)
                {
                    MostrarErrorDialog(ex.Message);
                }
            }
        }

        private void TextPasword_LostFocus(object sender, RoutedEventArgs e)
        {
            changepassword = true;
        }

        private void BtnDelWBS_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnAddWBS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ServiceWBS.WebServiceWBS ws = new ServiceWBS.WebServiceWBS();
                DataTable OpCod = ws.GetAllWBS().Tables[DT_WBS.TABLE_NAME].Copy();
                WindowSelection frm = new WindowSelection
                {
                    Dt = OpCod,
                    //dt = General.EliminarExistenColumString(OpCod, RoleTable, RoleDatos.ROLE_COLUM),
                    DcolumToShow = new string[] { "WBS", "Project" },
                    DcolumToShowAlias = new string[] { "WBS", "Project" },
                    Title = "WBS"
                };
                frm.P.FilterString = "WBS";
                frm.ColumnToFilter = "WBS";
                if (frm.ShowDialog() == true)
                {
                    foreach (DataRow item in frm.RowsSelected)
                    {
                        //DataRow newrow = WBSTable.NewRow();
                        //newrow[DT_User_WBS.ID_WBS] = item[DT_WBS.ID_WBS];
                        //newrow[DT_User_WBS.ID_PROJECT] = item[DT_WBS.IDPROJECT];
                        //newrow[DT_User_WBS.USER] = TextUser.Text;
                        //newrow[DT_User_WBS.WBS] = item["WBS"];
                        //WBSTable.Rows.Add(newrow);
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message); ;
            }

        }
    }
}
