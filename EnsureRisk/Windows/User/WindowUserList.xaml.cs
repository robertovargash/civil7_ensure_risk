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
        public WindowUserList()
        {
            InitializeComponent();
            ChangeLanguage();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc);
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
                    User = Ds.Tables[DT_User.User_TABLA].NewRow(),
                    RoleTable = Ds.Tables[DT_User_Role.TABLE_NAME].Clone(),
                    Operation = General.INSERT,
                    Icon = Icon
                };
                if (adduser.ShowDialog() == true)
                {
                    DataSet temp = new DataSet();
                    Ds.Tables[DT_User.User_TABLA].Rows.Add(adduser.User);
                    Ds.Tables[DT_User_Role.TABLE_NAME].Merge(adduser.RoleTable);
                    if (Ds.HasChanges())
                    {
                        ServiceUserController.WebServiceUser user = new ServiceUserController.WebServiceUser();
                        temp = Ds.GetChanges();
                        temp = user.SaveUser(temp);
                        Ds.Merge(temp);
                        Ds.AcceptChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int intcell = dgUser.SelectedIndex;
                if (intcell >= 0)
                {
                    DataRow dr = Ds.Tables[DT_User.User_TABLA].Rows[dgUser.SelectedIndex];
                    if (dr[DT_User.USERNAME_COLUMNA].ToString() != "admin")
                    {
                        WindowUser adduser = new WindowUser
                        {
                            User = dr,
                            RoleTable = Ds.Tables[DT_User_Role.TABLE_NAME].Copy(),
                            Operation = General.UPDATE,
                            Icon = Icon
                        };
                        adduser.ShowDialog();
                        if (adduser.DialogResult == true)
                        {
                            DataSet temp = new DataSet();
                            Ds.Tables[DT_User_Role.TABLE_NAME].Merge(adduser.RoleTable);
                            if (Ds.HasChanges())
                            {
                                ServiceUserController.WebServiceUser user = new ServiceUserController.WebServiceUser();
                                temp = Ds.GetChanges();
                                temp = user.SaveUser(temp);
                                Ds.Merge(temp);
                                Ds.AcceptChanges();
                            }
                        }
                    }
                    else
                    {
                        new WindowMessageOK("'admin' user can´t be changed!").ShowDialog();
                    }                   
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int intcell = dgUser.SelectedIndex;
                if (intcell >= 0)
                {
                    if (new WindowMessageYesNo(StringResources.DELETE_MESSAGE + " [" + Ds.Tables[DT_User.User_TABLA].Rows[intcell][DT_User.USERNAME_COLUMNA] + "]?").ShowDialog() == true)
                    {
                        if (Ds.Tables[DT_User.User_TABLA].Rows[intcell][DT_User.USERNAME_COLUMNA].ToString() != "admin")
                        {
                            Ds.Tables[DT_User.User_TABLA].Rows[intcell].Delete();
                            DataSet temp = new DataSet();
                            if (Ds.HasChanges())
                            {
                                ServiceUserController.WebServiceUser user = new ServiceUserController.WebServiceUser();
                                temp = Ds.GetChanges();
                                temp = user.SaveUser(temp);
                                Ds.Merge(temp);
                                Ds.AcceptChanges();
                            }
                        }
                        else
                        {
                            new WindowMessageOK("'admin' user can´t be deleted!").ShowDialog();
                        }
                    }                        
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Ds = new UserDataSet();
                ServiceUserController.WebServiceUser user = new ServiceUserController.WebServiceUser();
                Ds.Merge(user.GetUserData());
                dgUser.ItemsSource = Ds.Tables[DT_User.User_TABLA].DefaultView;
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void dgUser_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BtnEdit_Click(sender, e);
        }
    }
}
