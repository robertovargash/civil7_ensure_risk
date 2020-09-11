using DataMapping.Data;
using System;
using System.Data;
using System.Windows;
using System.ComponentModel;


namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for WindowWBSChild.xaml
    /// </summary>
    public partial class WindowWBSChild : Window, INotifyPropertyChanged
    {
        private string _user;
        private string _level;
        private string _wbs;
        private DataTable dt;
        public string Usuario { get { return _user; } set { _user = value; OnPropertyChanged("Usuario"); } }
        public string Nivel { get { return _level; } set { _level = value; OnPropertyChanged("Nivel"); } }
        public string WBSName { get { return _wbs; } set { _wbs = value; OnPropertyChanged("WBSName"); } }
        public DataTable DtUsuarios { get { return dt; } set { dt = value; OnPropertyChanged("DtUsuarios"); } }
        public DataRow DrWBS { get; set; }
        public int Cantidad { get; set; }
        public WindowWBS MyFather { get; set; }
        public DataRow DrWBS_Structure { get; set; }
        public decimal IdProject { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public WindowWBSChild()
        {
            InitializeComponent();
            TextName.DataContext = this;
            TextLevel.DataContext = this;
            cbUser.DataContext = this;
        }

        public void MostrarErrorDialog(string text)
        {
            ErrorMessageDialog.IsOpen = true;
            TextMessage.Text = text;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (ServiceUserController.WebServiceUser wsu = new ServiceUserController.WebServiceUser())
            {
                DtUsuarios = wsu.GetUserData().Tables[DT_User.User_TABLA].Copy();
            }
            
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DrWBS_Structure[DT_WBS_STRUCTURE.ID_CHILD] = DrWBS[DT_WBS.ID_WBS];
                DrWBS_Structure[DT_WBS_STRUCTURE.CHILD] = TextName.Text;
                DrWBS_Structure[DT_WBS_STRUCTURE.CNIVEL] = TextLevel.Text;
                DrWBS_Structure[DT_WBS_STRUCTURE.CHILD_USER] = cbUser.Text;
                DrWBS[DT_WBS.WBS_NAME] = TextName.Text;
                DrWBS[DT_WBS.NIVEL] = TextLevel.Text;
                DrWBS[DT_WBS.IDPROJECT] = IdProject;
                DrWBS[DT_WBS.USERNAME] = cbUser.Text;


                MyFather.WBS_Encoder.Rows.Add(DrWBS);
                MyFather.WBS_Structure.Rows.Add(DrWBS_Structure);

                DrWBS = MyFather.WBS_Encoder.NewRow();
                DrWBS_Structure = MyFather.WBS_Structure.NewRow();
                DrWBS_Structure[DT_WBS_STRUCTURE.ID_FATHER] = MyFather.DrWBS[DT_WBS.ID_WBS];
                DrWBS_Structure[DT_WBS_STRUCTURE.FATHER] = MyFather.DrWBS[DT_WBS.WBS_NAME];
                Cantidad++;
                TextLevel.Text = MyFather.TextLevel.Text + "." + Cantidad;
                TextName.Clear();
                cbUser.SelectedIndex = -1;
                TextName.Focus();
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

        private void TextName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            WBSName = TextName.Text;
        }

        private void TextLevel_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Nivel = TextLevel.Text;
        }
    }
}
