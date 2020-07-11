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

                foreach (XElement val in Enumerable.Where(tempVar.Descendants("value"), (xEl) => Microsoft.VisualBasic.CompilerServices.StringType.StrLike(xEl.Value, "http://*", Microsoft.VisualBasic.CompareMethod.Binary)).AsParallel())
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

        /// <summary>
        /// Delete the children's for any members of List "Colection. CM and Risk are treated as the same, like lines"
        /// </summary>
        /// <param name="collection">List of collection to delete its childrens</param>
        /// <param name="ds"></param>
        public static void DeleteRiskAndCMFirst(List<DataRow> collection, DataSet ds)
        {
            foreach (DataRow itemi in collection)
            {
                if (IsLeaf(itemi, ds.Tables[DT_Risk.TABLE_NAME]))
                {
                    DataRow[] drs = ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID + " = " + itemi[DT_Risk.ID]);
                    foreach (DataRow item in drs)
                    {
                        int cantidad = ds.Tables[DT_CounterM.TABLE_NAME].Select(DT_CounterM.ID_RISK + " = " + item[DT_Risk.ID]).Count();
                        for (int i = 0; i < cantidad; i++)
                        {
                            ds.Tables[DT_CounterM.TABLE_NAME].Select(DT_CounterM.ID_RISK + " = " + item[DT_Risk.ID]).First().Delete();
                        }
                        ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(item[DT_Risk.ID]).Delete();
                    }
                }
                else
                {
                    DeleteRiskAndCMFirst(GetChildss(itemi, ds.Tables[DT_Risk.TABLE_NAME]), ds);
                    int cantidad = ds.Tables[DT_CounterM.TABLE_NAME].Select(DT_CounterM.ID_RISK + " = " + itemi[DT_Risk.ID]).Count();
                    for (int i = 0; i < cantidad; i++)
                    {
                        ds.Tables[DT_CounterM.TABLE_NAME].Select(DT_CounterM.ID_RISK + " = " + itemi[DT_Risk.ID]).First().Delete();
                    }
                    ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(itemi[DT_Risk.ID]).Delete();
                }
            }
        }

        public static void EliminarMisHijosWBS(List<DataRow> hijos, DataTable dtWBS, DataTable dtStrucutre)
        {
            foreach (DataRow item in hijos)
            {
                if (dtStrucutre.Select(DT_WBS_STRUCTURE.ID_FATHER + " = " + item[DT_WBS.ID_WBS]).Any())
                {
                    EliminarMisHijosWBS(MyWBSChildren(item, dtWBS, dtStrucutre), dtWBS, dtStrucutre);
                }
                dtWBS.Rows.Find(item[DT_WBS.ID_WBS]).Delete();
            }
        }

        public static int ConvertToInt(string word)
        {
            string valuestr = "";
            foreach (var item in word)
            {
                if (Versioned.IsNumeric(item))
                {
                    valuestr += item.ToString();
                }
            }
            return Convert.ToInt32(valuestr);
        }

        public static List<DataRow> MyWBSChildren(DataRow drFather, DataTable dtWBS, DataTable dtStructure)
        {
            List<DataRow> returnList = new List<DataRow>();

            foreach (DataRow item in dtStructure.Select(DT_WBS_STRUCTURE.ID_FATHER + " = " + drFather[DT_WBS.ID_WBS].ToString()))
            {
                returnList.Add(dtWBS.Rows.Find(item[DT_WBS_STRUCTURE.ID_CHILD]));
            }
            return returnList;
        }

        #endregion

        #region CalculateFishHead

        public static List<DataRow> GetChildss(DataRow drFather, DataTable dtRisk)
        {
            List<DataRow> returnList = new List<DataRow>();

            foreach (DataRow item in dtRisk.Select(DT_Risk.IDRISK_FATHER + " = " + drFather[DT_Risk.ID].ToString()))
            {
                returnList.Add(item);
            }
            return returnList;
        }

        public static bool IsLeaf(DataRow drChild, DataTable dtStructure)
        {
            return !dtStructure.Select(DT_Risk.IDRISK_FATHER + " = " + drChild[DT_Risk.ID]).Any();
        }


        /// <summary>
        /// Calculate and returns the value of the "Damage" of the "risk"
        /// </summary>
        public static decimal CalculateTopRiskValue(DataRow risk, DataTable Risk_TopRisk, int idToprisk)
        {
            try
            {
                if ((bool)risk[DT_Risk.ENABLED])
                {
                    if (!(Risk_TopRisk.Select(DT_Risk_Damages.ID_RISK + " = " + risk[DT_Risk.ID] + " AND " +
                                DT_Risk_Damages.ID_DAMAGE + " = " + idToprisk).Any()))
                    {
                        return 0;
                    }
                    else
                    {
                        return (decimal)Risk_TopRisk.Select(DT_Risk_Damages.ID_RISK + " = " + risk[DT_Risk.ID] + " AND " +
                            DT_Risk_Damages.ID_DAMAGE + " = " + idToprisk).First()[DT_Risk_Damages.VALUE];
                    }
                }
                else
                    return 0;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Calculate and returns the value of the "Damage (TopRisk)" of the "cm"
        /// </summary>
        public static decimal CalculateCMTopRiskValue(DataRow CM, DataTable CM_TopRisk, int idToprisk)
        {
            if (!(CM_TopRisk.Select(DT_CounterM_Damage.ID_COUNTERM + " = " + CM[DT_CounterM.ID] + " AND " +
                DT_CounterM_Damage.ID_DAMAGE + " = " + idToprisk).Any()))
            {
                return 0;
            }
            else
            {
                return (decimal)CM_TopRisk.Select(DT_CounterM_Damage.ID_COUNTERM + " = " + CM[DT_CounterM.ID] + " AND " +
                    DT_CounterM_Damage.ID_DAMAGE + " = " + idToprisk).First()[DT_CounterM_Damage.VALUE];
            }
        }

        /// <summary>
        /// Return an Array with the CM of the Risk
        /// </summary>
        public static DataRow[] MyCounterMeasure(DataRow risk, DataTable CM)
        {
            return CM.Select(DT_CounterM.ID_RISK + " = " + risk[DT_Risk.ID] + " and " + DT_CounterM.ENABLED + " = " + true);
        }

        /// <summary>
        /// Calculate and returns the value of the Damage (TopRisk) of the Risk
        /// </summary>
        public static decimal CalculateTopRiskTreeValue(DataRow drRoot, DataTable dtRisk, int idtoprisk, DataTable dtRisk_TopRisk,
            DataTable dtCM, DataTable dtCM_TopRisk)
        {
            //THIS FUNCTION CALCULATES THE VALUE OF A TOPRISK, 
            //THE CODING AND STRUCTURE TABLES, HANDLE THE RISK AND ITS POSITION WITHIN THE TREE TO CALCULATE THE VALUE OF ITS CHILDREN.
            decimal riskValue = CalculateTopRiskValue(drRoot, dtRisk_TopRisk, idtoprisk);//HERE IS CALCULATED THE VALUE OF THE FATHER ONLY 
            decimal cmValue = 0;

            //HERE WE SELECT ALL THE COUNTERMEASURE OF THE RISK AND SUM ALL HIS VALUES AND RESTAMOS
            foreach (DataRow item in MyCounterMeasure(drRoot, dtCM))
            {
                cmValue += CalculateCMTopRiskValue(item, dtCM_TopRisk, idtoprisk);
            }
            riskValue += cmValue;//ORIGINALMENTE ERA MENOS, PERO POR LO QUE DIJO LUCAS CAMBIE A +
            //NOW, FOR EACH CHILDS OF 'drRoot' WE EVALUATE IF HAS CHILDREN
            foreach (DataRow item in GetChildss(drRoot, dtRisk)) //FOR EACH CHILDS OF 'drRoot'
            {
                if (IsLeaf(item, dtRisk))//HERE IS EVALUATED IF drRoot CHILDS HAVE CHILDREN
                {
                    //IF NOT HAVE CHILDREN AS THE VALUE IS ADDED TO THE VALUE OF THE FATHER
                    riskValue += CalculateTopRiskValue(item, dtRisk_TopRisk, idtoprisk);
                    foreach (DataRow item2 in MyCounterMeasure(item, dtCM))
                    {
                        cmValue = CalculateCMTopRiskValue(item2, dtCM_TopRisk, idtoprisk);
                        riskValue += cmValue;//ORIGINALMENTE ERA MENOS, PERO POR LO QUE DIJO LUCAS CAMBIE A +
                    }
                }
                else
                {
                    //ELSE FOR EACH CHILDS EXECUTE THE FUNCTION RECURSIVELY
                    if ((bool)item[DT_Risk.ENABLED])
                    {
                        riskValue += CalculateTopRiskTreeValue(item, dtRisk, idtoprisk, dtRisk_TopRisk, dtCM, dtCM_TopRisk);
                    }
                }
            }
            return riskValue;
        }


        /// <summary>
        /// Calculate the Acumulated Likelihood of the line. The results depends of the children.
        /// </summary>
        public static decimal AcumulatedLikelihood(RiskPolyLine LineFather)
        {
            decimal ValueToReturn;//This will be the value to return
            bool hasChildren = false;//the flag ill be activated if the risk has children,
            List<decimal> Probability_List = new List<decimal>();
            List<decimal> CM_Probabilities = new List<decimal>();
            foreach (var item in LineFather.Children)
            {
                if (item.IsCM)
                {
                    if (item.IsActivated)
                    {
                        CM_Probabilities.Add(item.Probability);
                    }
                }
                else
                {
                    hasChildren = true;
                    if (item.IsLeaf())
                    {
                        if (!(item.IsActivated))
                        {
                            Probability_List.Add(1);
                        }
                        else
                        {
                            Probability_List.Add(item.Probability);//if don´t have child, Acum. Likelihhod = its Probability
                        }
                    }
                    else
                    {
                        if (item.IsActivated)
                        {
                            Probability_List.Add(AcumulatedLikelihood(item));//else, call the function as recursive
                        }
                        else
                        {
                            Probability_List.Add(1);
                        }
                    }
                }
            }

            if (hasChildren)
            {
                //Here the formula, the probability of the father mult. by the probabilities of their children according with the In_Exclusion_Formula
                ValueToReturn = (LineFather.IsActivated ? LineFather.Probability : 1) * EL_Inclusion_Exclusion(Probability_List);
                foreach (var item in CM_Probabilities)
                {
                    ValueToReturn *= (1M - item);//adding to the return value the Risk Reduction Formula for each CounterMeasure
                }
            }
            else
            {
                ValueToReturn = LineFather.IsActivated ? LineFather.Probability : 1;//If don´t have child, Acum. Likelihood = its Probability
                foreach (var item in CM_Probabilities)
                {
                    ValueToReturn *= (1M - item);//adding to the return value the Risk Reduction Formula for each CounterMeasure
                }
            }
            if (ValueToReturn > 1)
            {
                return 1;
            }
            else
            {
                return ValueToReturn;
            }
        }
        
        /// <summary>
        /// Calculating Inclusion_Exclusion likelihood 
        /// </summary>
        public static decimal EL_Inclusion_Exclusion(List<decimal> p)
        {
            if (p.Count > 1)
            {
                decimal temp = 0;
                for (int i = 0; i < p.Count - 1; i++)
                {
                    temp = ProbabilityOr(p[i], p[i + 1]);
                    p[i + 1] = temp;
                }
                return temp;
            }
            else
            {
                return p[0];
            }
        }

        /// <summary>
        /// Calculating the probability of A U B
        /// </summary>
        public static decimal ProbabilityOr(decimal A, decimal B)
        {
            return (A + B) - (A * B);
        }

        
        public static void UpdateThickness(List<RiskPolyLine> linesList)
        {           
            decimal min = 0;
            decimal max = 0;
            if (linesList.Where(p => !p.IsRoot).Any())
            {
                min = linesList.Where(p => !p.IsRoot).Min(l => l.AcDamage);
                max = linesList.Where(p => !p.IsRoot).Max(l => l.AcDamage);
            }

            foreach (var item in linesList)
            {
                if (!(item.IsCM))
                {
                    item.SetThickness(item.AcDamage, min, max);
                    //item.Min = min;
                    //item.Max = max;
                }
                //SetMargin(item);
            }
            RiskPolyLine rootPolyLine = linesList.Find(r => r.IsRoot);
            rootPolyLine.StrokeThickness = MaxThickness;
            UpdateSegmentsStrokeThickness(rootPolyLine);
            //IEnumerable<RiskPolyLine> rootChildren = linesList.FindAll(p => !p.IsRoot && !p.IsCM && p.Father.IsRoot);
            //IEnumerable<RiskPolyLine> orderedRootChildren = rootChildren.OrderBy(p => p.Points[1].X);
            //foreach (RiskPolyLine polyLine in orderedRootChildren)
            //{
            //    UpdateSegmentsStrokeThickness(polyLine);
            //}
        }


        private static void UpdateSegmentsStrokeThickness(RiskPolyLine riskPolyLine)
        {
            if (riskPolyLine.Children.Any())
            {
                IEnumerable<RiskPolyLine> orderedChild = riskPolyLine.Children.OrderBy(pl => pl.Points[1].X);
                foreach (RiskPolyLine polyLine in orderedChild)
                {
                    UpdateSegmentsStrokeThickness(polyLine);
                    polyLine.UpdateSegmentsStrokeThickness();
                }
            }
            riskPolyLine.UpdateSegmentsStrokeThickness();
        }


        #endregion

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
            table.Rows.Add("Nederlands", "NL-NL");
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
