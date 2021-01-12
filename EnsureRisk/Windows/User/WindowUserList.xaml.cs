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
using DataMapping.Data;
using System.Data;
using EnsureBusinesss;
using EnsureRisk.Resources;

namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for WindowUserList.xaml
    /// </summary>
    public partial class WindowUserList : Window
    {

        public DataSet Ds { get; set; }
        public bool IS_DELETING { get; private set; } = false;

        public WindowUserList()
        {
            InitializeComponent();
            ChangeLanguage();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
        }

        public void MostrarErrorDialog(string text)
        {
            ErrorMessageDialog.IsOpen = true;
            TextMessage.Text = text;
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        public void ChangeLanguage()
        {
            BtnAdd.Content = StringResources.AddButton;
            BtnEdit.Content = StringResources.EditButton;
            BtnDel.Content = StringResources.DeleteButton;
            Title = StringResources.UserListMenu;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowUser adduser = new WindowUser
                {
                    User = Ds.Tables[DT_User.TABLE_NAME].NewRow(),
                    RoleTable = Ds.Tables[DT_User_Role.TABLE_NAME].Clone(),
                    //WBSTable = Ds.Tables[DT_User_WBS.TABLE_NAME].Copy(),
                    Operation = General.INSERT,
                    Icon = Icon
                };
                if (adduser.ShowDialog() == true)
                {
                    //DataSet temp = new DataSet();
                    Ds.Tables[DT_User.TABLE_NAME].Rows.Add(adduser.User);
                    Ds.Tables[DT_User_Role.TABLE_NAME].Merge(adduser.RoleTable);
                    //Ds.Tables[DT_User_WBS.TABLE_NAME].Merge(adduser.WBSTable);
                    if (Ds.HasChanges())
                    {
                        ServiceUserController.WebServiceUser user = new ServiceUserController.WebServiceUser();
                        DataSet temp = Ds.GetChanges();
                        temp = user.SaveUser(temp);
                        Ds.Merge(temp);
                        Ds.AcceptChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int intcell = dgUser.SelectedIndex;
                if (intcell >= 0)
                {
                    DataRow dr = Ds.Tables[DT_User.TABLE_NAME].Rows[dgUser.SelectedIndex];
                    if (dr[DT_User.USERNAME].ToString() != "admin")
                    {
                        WindowUser adduser = new WindowUser
                        {
                            User = dr,
                            RoleTable = Ds.Tables[DT_User_Role.TABLE_NAME].Copy(),
                            //WBSTable = Ds.Tables[DT_User_WBS.TABLE_NAME].Copy(),
                            Operation = General.UPDATE,
                            Icon = Icon
                        };
                        adduser.ShowDialog();
                        if (adduser.DialogResult == true)
                        {
                            //DataSet temp = new DataSet();
                            Ds.Tables[DT_User_Role.TABLE_NAME].Merge(adduser.RoleTable);
                            //Ds.Tables[DT_User_WBS.TABLE_NAME].Merge(adduser.WBSTable);
                            if (Ds.HasChanges())
                            {
                                ServiceUserController.WebServiceUser user = new ServiceUserController.WebServiceUser();
                                DataSet temp = Ds.GetChanges();
                                temp = user.SaveUser(temp);
                                Ds.Merge(temp);
                                Ds.AcceptChanges();
                            }
                        }
                    }
                    else
                    {
                        MostrarErrorDialog("'admin' user can´t be changed!");
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void Delete()
        {
            try
            {
                if (dgUser.SelectedIndex >= 0)
                {
                    if (Ds.Tables[DT_User.TABLE_NAME].Rows[dgUser.SelectedIndex][DT_User.USERNAME].ToString() != "admin")
                    {
                        Ds.Tables[DT_User.TABLE_NAME].Rows[dgUser.SelectedIndex].Delete();
                        if (Ds.HasChanges())
                        {
                            using (ServiceUserController.WebServiceUser user = new ServiceUserController.WebServiceUser())
                            {
                                DataSet temp = Ds.GetChanges();
                                temp = user.SaveUser(temp);
                                Ds.Merge(temp);
                                Ds.AcceptChanges();
                            }                            
                        }
                    }
                    else
                    {
                        MostrarErrorDialog("[admin] user can´t be deleted!");
                    }
                }
                IS_DELETING = false;
            }
            catch (Exception ex)
            {
                IS_DELETING = false;
                MostrarErrorDialog(ex.Message);
            }
        }

        public void MostrarDialogYesNo(string textAlert)
        {
            YesNoDialog.IsOpen = true;
            TextYesNoMessage.Text = textAlert;
        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgUser.SelectedIndex >= 0)
                {
                    IS_DELETING = true;
                    MostrarDialogYesNo(StringResources.DELETE_MESSAGE + " [" + Ds.Tables[DT_User.TABLE_NAME].Rows[dgUser.SelectedIndex][DT_User.USERNAME] + "]?");
                }
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
                    Delete();
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Ds = new UserDataSet();
                ServiceUserController.WebServiceUser user = new ServiceUserController.WebServiceUser();
                Ds.Merge(user.GetUserData());
                dgUser.ItemsSource = Ds.Tables[DT_User.TABLE_NAME].DefaultView;
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void DgUser_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BtnEdit_Click(sender, e);
        }
    }
}
