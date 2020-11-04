using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using DataMapping.Data;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Xml.Linq;
using EnsureBusinesss.Properties;
using EnsureBusinesss.Business;
using Microsoft.VisualBasic.CompilerServices;

namespace EnsureBusinesss
{
    public class General
    {
        //public const string DELETE_MESSAGE = 
        public const string DELETE_HEADER = "Delete record?";
        public const string UPDATE = "Update";
        public const string DELETE = "Delete";
        public const string INSERT = "Insert";
        public const string LOCATION = "pack://application:,,/Resources/";
        public const double basicX = 3;
        public const double basicY = 7;
        public const int NumberOfClasses = 20;

        public const double MaxThickness = 20;

        public const string EXPANDIDO = "pack://application:,,,/Images/Expandido.png";
        public const string CONTRAIDO = "pack://application:,,,/Images/Contraido.png";

        public const string ESPAÑOL = "pack://application:,,,/Images/Espannol.png";
        public const string NETHERLANDS = "pack://application:,,,/Images/Netherlands.png";
        public const string ENGLISH = "pack://application:,,,/Images/English.png";
        public const string CLEAR = "pack://application:,,,/Images/clear.png";

        public string Usser { get; set; }

        private static readonly System.Text.RegularExpressions.Regex wsExprex = new System.Text.RegularExpressions.Regex("\\/WebService([a-zA-Z_\\-0-9]+)?\\/", System.Text.RegularExpressions.RegexOptions.IgnoreCase); //,

        public General()
        {

        }

        public static DataTable CrossTable(ref DataTable dtSource, string Pivot, string[] ColValue, DataColumn[] PK_Col)
        {
            string filter = string.Empty;
            //Buscar todos los posibles valores de la columna pivote
            IEnumerable<string> PivotValues = from aDataRow in dtSource.AsEnumerable()
                                              group aDataRow by (aDataRow.Field<string>(Pivot)) into aPivotValues
                                              let aValorPivote = aPivotValues.Key
                                              select aValorPivote;
            //Definir tabla resultante
            DataTable dtResults = dtSource.Copy();

            dtResults.PrimaryKey = null;

            //Se eliminan de dtResultado las filas duplicadas por la columna Pivote
            if (PivotValues.Count() > 0)
            {
                DataColumn dc = dtSource.Columns[Pivot];
                if (dc.DataType.Equals(typeof(int)) || dc.DataType.Equals(typeof(short)) || dc.DataType.Equals(typeof(byte)))
                {
                    filter = Pivot + " <> " + Convert.ToString(PivotValues.ElementAtOrDefault(0));
                }
                else
                {
                    if (dc.DataType.Equals(typeof(string)))
                    {
                        filter = Pivot + " <> " + "'" + Convert.ToString(PivotValues.ElementAtOrDefault(0)) + "'";
                    }
                    else
                    {
                        filter = Pivot + " <> " + Convert.ToString(PivotValues.ElementAtOrDefault(0));
                    }
                }
                DataRow[] AnotherOne = dtResults.Select(filter);
                foreach (DataRow dr in AnotherOne)
                {
                    dr.Delete();
                }
                dtResults.AcceptChanges();
            }

            // Se eliminan de la tabla resultantes las columnas Pivote y Valor
            dtResults.Columns.Remove(Pivot);
            foreach (string colvalue in ColValue)
            {
                dtResults.Columns.Remove(colvalue);
            }

            //Adicionar columnas en la tabla resultante para cada valor encontrado de la columna pivote
            foreach (string colValue in ColValue)
            {
                foreach (string pivotValue in PivotValues)
                {
                    string colName = colValue.Trim() + "_" + pivotValue.Trim();
                    colName = pivotValue.Trim();
                    DataColumn newColumn = new DataColumn(colName, dtSource.Columns[colValue].DataType);
                    switch (dtSource.Columns[colValue].DataType.ToString().ToUpper())
                    {
                        case "SYSTEM.DECIMAL":
                            newColumn.DefaultValue = 0M;
                            break;
                        case "SYSTEM.STRING":
                            newColumn.DefaultValue = string.Empty;
                            break;
                        default:
                            newColumn.DefaultValue = string.Empty;
                            break;
                    }
                    dtResults.Columns.Add(newColumn);
                }
            }
            //Recorrer la tabla dtResultado para ponerle valor a las columnas adicionadas
            foreach (DataRow drResult in dtResults.Rows)
            {
                filter = string.Empty;
                foreach (DataColumn dcPK in PK_Col)
                {
                    if (dcPK.ColumnName != Pivot)
                    {
                        if (dcPK.DataType.Equals(typeof(int)) || dcPK.DataType.Equals(typeof(short)) || dcPK.DataType.Equals(typeof(byte)))
                        {
                            filter = filter + dcPK.ColumnName + " = " + Convert.ToString(drResult[dcPK.ColumnName]) + " AND ";
                        }
                        else
                        {
                            if (dcPK.DataType.Equals(typeof(string)))
                            {
                                filter = filter + dcPK.ColumnName + " = " + "'" + Convert.ToString(drResult[dcPK.ColumnName]) + "'" + " AND ";
                            }
                            else
                            {
                                if (dcPK.DataType.Equals(typeof(decimal)))
                                {
                                    filter = filter + dcPK.ColumnName + " = " + Convert.ToString(drResult[dcPK.ColumnName], System.Globalization.CultureInfo.InvariantCulture) + " AND ";
                                }
                                else
                                {
                                    filter = filter + dcPK.ColumnName + " = " + Convert.ToString(drResult[dcPK.ColumnName]) + " AND ";
                                }
                            }
                        }
                    }
                }
                filter = filter.Remove(filter.Length - 5, 5);

                DataRow[] colFilter = dtSource.Select(filter);
                string colName = string.Empty;
                foreach (DataRow drFilter in colFilter)
                {
                    foreach (string col_value in ColValue)
                    {
                        colName = Convert.ToString(drFilter[Pivot]);
                        drResult[colName] = drFilter[col_value];
                    }
                }
            }
            return dtResults;
        }

        #region CambiarConfiguration

        private static void UpdateConfigurationFile(string file, string server)
        {
            if (File.Exists(file))
            {
                XDocument tempVar = XDocument.Load(file);

                foreach (XElement val in Enumerable.Where(tempVar.Descendants("value"), (xEl) => StringType.StrLike(xEl.Value, "http://*", Microsoft.VisualBasic.CompareMethod.Binary)).AsParallel())
                {
                    val.Value = ChangeWebReference(val.Value, server);
                }
                tempVar.Save(file);
            }
        }

        public static string ChangeWebReference(string url, string host)
        {
            return new UriBuilder("http://" + host) { Path = (new UriBuilder(wsExprex.Replace(url, "/" + Settings.Default.EnsureRisk.ToString() + "/"))).Path }.Uri.ToString();
        }

        public static void UpdateConfigurationFile(string server)
        {
            UpdateConfigurationFile("EnsureRisk.exe.config", server);
#if DEBUG 
            UpdateConfigurationFile("EnsureRisk.vshost.exe.config", server);
#endif
            Settings.Default.Reload();
        }

        #endregion

        #region BorrarElementoRecursivoDatatable

        

        public static decimal ConvertToDec(string word)
        {
            string valuestr = "";
            foreach (var item in word)
            {
                if (Versioned.IsNumeric(item))
                {
                    valuestr += item.ToString();
                }
            }
            return Convert.ToDecimal(valuestr);
        }


        #endregion

        public static void UpdateLinesThickness(List<RiskPolyLine> linesList)
        {
            decimal min = 0;
            decimal max = 0;
            if (linesList.Where(p => !p.IsRoot).Any())
            {
                //min = linesList.Where(p => !p.IsRoot).Min(l => l.AcDamage);
                //max = linesList.Where(p => !p.IsRoot).Max(l => l.AcDamage);

                min = linesList.Where(p => !p.IsRoot).Min(l => l.AcDamage2);
                max = linesList.Where(p => !p.IsRoot).Max(l => l.AcDamage2);
            }

            foreach (var item in linesList)
            {
                if (!(item.IsCM))
                {
                    if (item.IsLeaf())
                    {
                        item.SetThickness(item.AcDamage, min, max);
                    }
                    else
                    {
                        item.SetThickness(item.Children.First().AcDamage2, min, max);
                    }
                }
            }
            RiskPolyLine rootPolyLine = linesList.Find(r => r.IsRoot);
            rootPolyLine.StrokeThickness = MaxThickness;
            //UpdateSegmentsStrokeThickness(rootPolyLine);
            UpdateSegmentsStrokeThickness(rootPolyLine, min, max);
        }

        //private static void UpdateSegmentsStrokeThickness(RiskPolyLine riskPolyLine)
        //{
        //    if (riskPolyLine.Children.Any())
        //    {
        //        IEnumerable<RiskPolyLine> orderedChild = riskPolyLine.Children.OrderBy(pl => pl.Points[1].X);
        //        foreach (RiskPolyLine polyLine in orderedChild)
        //        {
        //            UpdateSegmentsStrokeThickness(polyLine);
        //            polyLine.UpdateSegmentsStrokeThickness();
        //        }
        //    }
        //    riskPolyLine.UpdateSegmentsStrokeThickness();
        //}

        private static void UpdateSegmentsStrokeThickness(RiskPolyLine riskPolyLine, decimal min, decimal max)
        {
            if (riskPolyLine.Children.Any())
            {
                IEnumerable<RiskPolyLine> orderedChild = riskPolyLine.Children.OrderBy(pl => pl.Points[1].X);
                foreach (RiskPolyLine polyLine in orderedChild)
                {
                    //UpdateSegmentsStrokeThickness(polyLine);
                    UpdateSegmentsStrokeThickness(polyLine, min, max);
                    polyLine.UpdateSegmentsStrokeThickness(min, max);
                }
            }
            riskPolyLine.UpdateSegmentsStrokeThickness(min, max);
        }

        public static void RecalculateProbability(DataRow drLine, DataSet ds, decimal newProbability, bool isCM)
        {
            if (isCM)
            {
                foreach (DataRow rowRiskDamage in ds.Tables[DT_CounterM_Damage.TABLE_NAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + drLine[DT_CounterM.ID]))
                {
                    rowRiskDamage[DT_CounterM_Damage.RISK_REDUCTION] = newProbability;
                }
                foreach (DataRow rowRiskWBS in ds.Tables[DT_CM_WBS.TABLE_NAME].Select(DT_CM_WBS.ID_CM + " = " + drLine[DT_CounterM.ID]))
                {
                    rowRiskWBS[DT_CM_WBS.PROBABILITY] = newProbability;
                }
            }
            else
            {
                foreach (DataRow rowRiskDamage in ds.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + drLine[DT_Risk.ID]))
                {
                    rowRiskDamage[DT_Risk_Damages.PROBABILITY] = newProbability;
                }
                foreach (DataRow rowRiskWBS in ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + drLine[DT_Risk.ID]))
                {
                    rowRiskWBS[DT_RISK_WBS.PROBABILITY] = newProbability;
                }
            }
        }


        #region OtherFunctions
        public static byte[] Encrypt(string source)
        {
            UnicodeEncoding uEncode = new UnicodeEncoding();
            byte[] array = uEncode.GetBytes(source);
            using (MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider())
            {
                MD5CryptoServiceProvider md5 = mD5CryptoServiceProvider;
                return md5.ComputeHash(array);
            }           
        }

        public static string ByteArrayToString(byte[] ByteArrayString)
        {
            return Convert.ToBase64String(ByteArrayString);
        }

        public static DataTable DeleteExists(DataTable dtFrom, DataTable dtToCompare, string ID_Column)
        {
            DataTable tableReturn = dtFrom.Clone();
            DataTable tableTemp = dtFrom.Copy();
            foreach (DataRow item in dtToCompare.Rows)
            {
                if (dtFrom.Rows.Contains(item[columnName: ID_Column]))
                {
                    if (tableTemp.Select(filterExpression: ID_Column + " = " + item[columnName: ID_Column]).Any())
                    {
                        tableTemp.Select(filterExpression: ID_Column + " = " + item[columnName: ID_Column]).First().Delete();
                    }
                }
            }
            tableTemp.AcceptChanges();
            foreach (DataRow item in tableTemp.Rows)
            {
                tableReturn.ImportRow(item);
            }
            return tableReturn;
        }

        public static DataTable TableLanguages()
        {
            DT_Language table = new DT_Language();
            table.Rows.Add("English", "EN-US");
            table.Rows.Add("Español", "ES-ES");
            table.Rows.Add("Netherlander", "NL-NL");
            return table;
        }

        public static decimal MyRound(decimal N, int CntDecimales)
        {
            decimal pow = Convert.ToDecimal(Math.Pow(10, CntDecimales));
            return Convert.ToDecimal(Math.Floor(N * pow + 0.5M) / pow);
        }
        
        #endregion
    }
}
