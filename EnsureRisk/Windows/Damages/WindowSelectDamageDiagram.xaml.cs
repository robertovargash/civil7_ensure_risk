using DataMapping.Data;
using EnsureBusinesss;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

namespace EnsureRisk.Windows.Damages
{
    /// <summary>
    /// Interaction logic for WindowSelectDamageDiagram.xaml
    /// </summary>
    public partial class WindowSelectDamageDiagram : Window, INotifyPropertyChanged
    {
        private int selectedDamage;
        private string damage;
        private DataTable dtDamage;
        public int SelectedDamage { get { return selectedDamage; } set { selectedDamage = value; OnPropertyChanged("SelectedDamage"); } }
        public string DAMAGE { get { return damage; } set { damage = value; OnPropertyChanged("DAMAGE"); } }
        public DataRow Drow { get; set; }
        public DataTable DamageDiagramTable { get; set; }
        public DataTable DamageTable { get { return dtDamage; } set { dtDamage = value; OnPropertyChanged("DamageTable"); } }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public WindowSelectDamageDiagram()
        {
            InitializeComponent();
            TextTopRisk.DataContext = this;
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
                ServiceTopRiskController.WebServiceTopRisk ws = new ServiceTopRiskController.WebServiceTopRisk();
                DataSet ds = ws.GetAllTopRisk();
                DamageTable = ds.Tables[DT_Damage.TopRisk_TABLA].Copy();
                ws.Dispose();
                DamageTable = General.DeleteExists(DamageTable, DamageDiagramTable, DT_Diagram_Damages.ID_DAMAGE);
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
                if (DAMAGE != "")
                {
                    if (DamageTable.Select(DT_Damage.TOP_RISK_COLUMN + " = '" + DAMAGE + "'").Any())
                    {
                        Drow[DT_Diagram_Damages.DAMAGE] = DAMAGE;
                        Drow[DT_Diagram_Damages.ID_DAMAGE] = SelectedDamage;
                        Drow[DT_Diagram_Damages.COLOR] = DamageTable.Rows.Find(SelectedDamage)[DT_Damage.COLORID_COLUMNA];
                    }
                    else
                    {
                        DataRow drDamage = DamageTable.NewRow();
                        drDamage[DT_Damage.TOP_RISK_COLUMN] = DAMAGE;
                        System.Drawing.Color color = System.Drawing.Color.FromArgb(colorPiker.SelectedColor.Value.A, colorPiker.SelectedColor.Value.R, colorPiker.SelectedColor.Value.G, colorPiker.SelectedColor.Value.B);
                        drDamage[DT_Damage.COLORID_COLUMNA] = color.ToArgb().ToString();
                        ServiceTopRiskController.WebServiceTopRisk ws = new ServiceTopRiskController.WebServiceTopRisk();
                        DamageTable.Rows.Add(drDamage);
                        DataSet ds = new DataSet();
                        ds.Merge(DamageTable);
                        ds = ws.SaveTopRisk(ds);
                        ws.Dispose();
                        DamageTable.Merge(ds.Tables[DT_Damage.TopRisk_TABLA]);                        
                        Drow[DT_Diagram_Damages.DAMAGE] = DAMAGE;
                        Drow[DT_Diagram_Damages.ID_DAMAGE] = drDamage[DT_Damage.ID_COLUMNA];
                        Drow[DT_Diagram_Damages.COLOR] = drDamage[DT_Damage.COLORID_COLUMNA];
                    }                    
                    this.DialogResult = true;
                }
                else
                {
                    MostrarErrorDialog("Please, select a Damage!!!");
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }


        private void TextTopRisk_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                DAMAGE = TextTopRisk.Text;
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void TextTopRisk_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (DamageTable.Rows.Contains(SelectedDamage))
                {
                    int colorete = int.Parse(DamageTable.Rows.Find(SelectedDamage)[DT_Damage.COLORID_COLUMNA].ToString());
                    System.Drawing.Color color = System.Drawing.Color.FromArgb(colorete);
                    colorPiker.SelectedColor = Color.FromArgb(color.A, color.R, color.G, color.B);
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }            
        }
    }
}
