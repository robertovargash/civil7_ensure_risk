using DataMapping.Data;
using System;
using System.Data;
using System.Windows;
using EnsureBusinesss;
using EnsureRisk.Resources;
using System.ComponentModel;
using System.Collections.Generic;

namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for WindowWBS.xaml
    /// </summary>
    public partial class WindowWBS : Window, INotifyPropertyChanged
    {
        private string _user;
        private string _level;
        private string _wbs;
        private DataTable dt;
        public string Usuario { get { return _user; } set { _user = value; OnPropertyChanged("Usuario"); } }
        public string Nivel { get { return _level; } set { _level = value; OnPropertyChanged("Nivel"); } }
        public string WBSName { get { return _wbs; } set { _wbs = value; OnPropertyChanged("WBSName"); } }
        public DataTable DtUsuarios { get { return dt; } set { dt = value; OnPropertyChanged("DtUsuarios"); } }
        public bool IS_DELETING { get; set; } = false;
        public string Operation { get; set; }
        public DataRow DrWBS { get; set; }
        public int IdProject { get; set; }
        public DataTable WBS_Structure { get; set; }
        public DataTable WBS_Encoder { get; set; }
        public DataView DvWBS { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public WindowWBS()
        {
            InitializeComponent();
            TextLevel.DataContext = this;
            TextName.DataContext = this;
            cbUser.DataContext = this;
        }

        public void MostrarErrorDialog(string text)
        {
            ErrorMessageDialog.IsOpen = true;
            TextMessage.Text = text;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using (ServiceUserController.WebServiceUser wsu = new ServiceUserController.WebServiceUser())
                {
                    DtUsuarios = wsu.GetUserData().Tables[DT_User.User_TABLA].Copy();
                    //cbUser.ItemsSource = DtUsuarios.DefaultView;
                    //cbUser.SelectedValuePath = DT_User.USERNAME;
                    //cbUser.DisplayMemberPath = DT_User.USERNAME;
                    if (Operation == General.UPDATE)
                    {
                        TextName.Text = DrWBS[DT_WBS.WBS_NAME].ToString();
                        TextLevel.Text = DrWBS[DT_WBS.NIVEL].ToString();
                        //chkIsManager.IsChecked = (bool)DrWBS[DT_WBS.IS_MANAGER];
                        cbUser.Text = DrWBS[DT_WBS.USERNAME].ToString();
                    }
                    if (TextName.Text == "")
                    {
                        BtnAdd.IsEnabled = false;
                    }
                    DvWBS = WBS_Structure.DefaultView;
                    DvWBS.RowFilter = DT_WBS_STRUCTURE.ID_FATHER + " = " + DrWBS[DT_WBS.ID_WBS];
                    dgWBS.ItemsSource = DvWBS;
                }
                
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Operation == General.INSERT)
                {
                    DrWBS[DT_WBS.WBS_NAME] = TextName.Text;
                    DrWBS[DT_WBS.NIVEL] = TextLevel.Text;
                    DrWBS[DT_WBS.IDPROJECT] = IdProject;
                    //DrWBS[DT_WBS.IS_MANAGER] = chkIsManager.IsChecked;
                    DrWBS[DT_WBS.USERNAME] = cbUser.Text;
                }
                if (Operation == General.UPDATE)
                {
                    DrWBS[DT_WBS.IDPROJECT] = IdProject;
                    //DrWBS[DT_WBS.IS_MANAGER] = chkIsManager.IsChecked;
                    DrWBS[DT_WBS.USERNAME] = cbUser.Text;
                    WBS_Encoder.Rows.Find(DrWBS[DT_WBS.ID_WBS])[DT_WBS.WBS_NAME] = TextName.Text;
                    //WBS_Encoder.Rows.Find(DrWBS[DT_WBS.ID_WBS])[DT_WBS.IS_MANAGER] = chkIsManager.IsChecked;
                    WBS_Encoder.Rows.Find(DrWBS[DT_WBS.ID_WBS])[DT_WBS.USERNAME] = cbUser.Text;
                }

                DialogResult = true;
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            int count = DvWBS.Count + 1;
            WindowWBSChild wBSChild = new WindowWBSChild
            {
                DrWBS_Structure = WBS_Structure.NewRow(),
                Cantidad = count,
                MyFather = this,
                DrWBS = WBS_Encoder.NewRow(),
                IdProject = IdProject
            };
            wBSChild.TextLevel.Text = TextLevel.Text + "." + count;
            wBSChild.DrWBS_Structure[DT_WBS_STRUCTURE.ID_FATHER] = DrWBS[DT_WBS.ID_WBS];
            wBSChild.DrWBS_Structure[DT_WBS_STRUCTURE.FATHER] = DrWBS[DT_WBS.WBS_NAME];
            wBSChild.ShowDialog();
            //if (wBSChild.ShowDialog() == true)
            //{                
            //    WBS_Encoder.Rows.Add(wBSChild.DrWBS);
            //    WBS_Structure.Rows.Add(wBSChild.DrWBS_Structure);
            //}
        }

        private void Delete()
        {
            try
            {
                if (dgWBS.SelectedIndex > 0)
                    WBS_Structure.Rows[dgWBS.SelectedIndex].Delete();
                IS_DELETING = false;
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
                IS_DELETING = false;
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
                if (dgWBS.SelectedIndex > 0)
                {
                    IS_DELETING = true;
                    MostrarDialogYesNo(StringResources.DELETE_MESSAGE + " [" + WBS_Structure.Rows[dgWBS.SelectedIndex][DT_WBS_STRUCTURE.CHILD] + "]?");
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

        private void TextName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                if (TextName.Text.Trim() == "")
                {
                    BtnAdd.IsEnabled = false;
                }
                else
                {
                    BtnAdd.IsEnabled = true;
                }
                WBSName = TextName.Text;
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void TextLevel_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Nivel = TextLevel.Text;
        }
    }
}
