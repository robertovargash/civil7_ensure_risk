﻿using DataMapping.Data;
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
        private decimal selectedDamage;
        private string damage,um;
        private DataTable dtDamage;
        public decimal SelectedDamage { get { return selectedDamage; } set { selectedDamage = value; OnPropertyChanged("SelectedDamage"); } }
        public string DAMAGE { get { return damage; } set { damage = value; OnPropertyChanged("DAMAGE"); } }
        public string UM { get { return um; } set { um = value; OnPropertyChanged("UM"); } }
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
            UMText.DataContext = this;
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
                DamageTable = ds.Tables[DT_Damage.TABLE_NAME].Copy();
                ws.Dispose();
                DamageTable = General.DeleteExists(DamageTable, DamageDiagramTable, DT_Diagram_Damages.ID_DAMAGE);
                ColorField.Visibility = Visibility.Collapsed;
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
                    if (DamageTable.Select(DT_Damage.TOP_RISK_COLUMN + " = '" + DAMAGE + "' AND " + DT_Damage.UM + " = '" + UM + "'").Any())
                    {
                        Drow[DT_Diagram_Damages.TOP_RISK] = DAMAGE;
                        Drow[DT_Diagram_Damages.DAMAGE] = DAMAGE + "(" + UM + ")";
                        Drow[DT_Diagram_Damages.UM] = UM;
                        Drow[DT_Diagram_Damages.ID_DAMAGE] = SelectedDamage;
                        Drow[DT_Diagram_Damages.COLOR] = DamageTable.Rows.Find(SelectedDamage)[DT_Damage.COLORID_COLUMNA];
                    }
                    else
                    {
                        DataRow drDamage = DamageTable.NewRow();
                        drDamage[DT_Damage.TOP_RISK_COLUMN] = DAMAGE;
                        drDamage[DT_Damage.UM] = UM;
                        drDamage[DT_Damage.COLORID_COLUMNA] = colorPiker.SelectedColor.ToString();
                        ServiceTopRiskController.WebServiceTopRisk ws = new ServiceTopRiskController.WebServiceTopRisk();
                        DamageTable.Rows.Add(drDamage);
                        DataSet ds = new DataSet();
                        ds.Merge(DamageTable);
                        ds = ws.SaveTopRisk(ds);
                        ws.Dispose();
                        DamageTable.Merge(ds.Tables[DT_Damage.TABLE_NAME]);                        
                        Drow[DT_Diagram_Damages.TOP_RISK] = DAMAGE;
                        Drow[DT_Diagram_Damages.UM] = UM;
                        Drow[DT_Diagram_Damages.DAMAGE] = DAMAGE + "(" + UM + ")"; ;
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
                if (DamageTable.Select(DT_Damage.TOP_RISK_COLUMN + " = '" + DAMAGE + "' AND " + DT_Damage.UM + " = '" + UM + "'").Any())
                {
                    ColorField.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ColorField.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void UMText_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                UM = UMText.Text;
                if (DamageTable.Select(DT_Damage.TOP_RISK_COLUMN + " = '" + DAMAGE + "' AND " + DT_Damage.UM + " = '" + UM + "'").Any())
                {
                    ColorField.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ColorField.Visibility = Visibility.Visible;
                }
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
                    colorPiker.SelectedColor = ((SolidColorBrush)new BrushConverter().ConvertFrom(DamageTable.Rows.Find(SelectedDamage)[DT_Damage.COLORID_COLUMNA].ToString())).Color;
                    UMText.Text = DamageTable.Rows.Find(SelectedDamage)[DT_Damage.UM].ToString();
                    UM = UMText.Text;
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }            
        }
    }
}
