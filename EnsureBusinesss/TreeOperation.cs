using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using DataMapping.Data;
using System.Windows;
using Petzold.Media2D;
using EnsureBusinesss.Business;
using System.Windows.Media.Imaging;

namespace EnsureBusinesss
{
    public class TreeOperation
    {
        private const double skewAngle = 22.375;
        private const int hmax = 101;
        private static List<RiskPolyLine> LinesUp = new List<RiskPolyLine>();
        private static List<RiskPolyLine> LinesDown = new List<RiskPolyLine>();
        #region MovingLines
        /// <summary>
        /// Move the entire diagram to position  "X,Y" including the Damages
        /// </summary>
        public static void MoveEntireTree(List<RiskPolyLine> lineList, int x, int y, List<MyDamage> Rectangles)
        {
            try
            {
                foreach (RiskPolyLine item in lineList)
                {
                    item.Move(x, y);
                    (item).DrawEntireLine(item.ShortName);
                }
                foreach (MyDamage item in Rectangles)
                {
                    item.Margin = new Thickness(item.Margin.Left + x, item.Margin.Top + y, 0, 0);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Move the "lineList" in the "x" y "y" directions
        /// </summary>
        public static void MoveLines(List<RiskPolyLine> lineList, double x, double y)
        {
            try
            {
                foreach (RiskPolyLine item in lineList)
                {
                    item.Move((int)x, (int)y);
                    (item).DrawEntireLine(item.ShortName);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Move&Copy
        public static void CreateCopy(RiskPolyLine Line, int idFather, DataSet Ds)
        {
            if (Line.IsLeaf())
            {
                if (Line.IsCM)
                {
                    DataRow drCM = Ds.Tables[DT_CounterM.TABLE_NAME].NewRow();
                    drCM[DT_CounterM.DETAIL] = Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(Line.ID)[DT_CounterM.DETAIL];
                    drCM[DT_CounterM.ENABLED] = Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(Line.ID)[DT_CounterM.ENABLED];
                    drCM[DT_CounterM.ID_RISK_TREE] = Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(Line.ID)[DT_CounterM.ID_RISK_TREE];
                    drCM[DT_CounterM.NAMESHORT] = Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(Line.ID)[DT_CounterM.NAMESHORT];
                    drCM[DT_CounterM.POSITION] = Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(Line.ID)[DT_CounterM.POSITION];
                    drCM[DT_CounterM.PROBABILITY] = Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(Line.ID)[DT_CounterM.PROBABILITY];
                    drCM[DT_CounterM.ID_RISK] = idFather;
                    drCM[DT_CounterM.RISK_NAMESHORT] = Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(idFather)[DT_Risk.NAMESHORT];
                    drCM[DT_CounterM.WBS_NAME] = Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(Line.ID)[DT_CounterM.WBS_NAME];
                    drCM[DT_CounterM.ID_WBS] = Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(Line.ID)[DT_CounterM.ID_WBS];
                    drCM[DT_CounterM.USER_NAME] = Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(Line.ID)[DT_CounterM.USER_NAME];
                    Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Add(drCM);
                    foreach (DataRow itemi in Ds.Tables[DT_CounterM_Damage.TABLENAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + Line.ID))
                    {
                        DataRow top = Ds.Tables[DT_CounterM_Damage.TABLENAME].NewRow();
                        top[DT_CounterM_Damage.COLOR] = itemi[DT_CounterM_Damage.COLOR];
                        top[DT_CounterM_Damage.ID_DAMAGE] = itemi[DT_CounterM_Damage.ID_DAMAGE];
                        top[DT_CounterM_Damage.ID_COUNTERM] = drCM[DT_CounterM.ID];
                        top[DT_CounterM_Damage.VALUE] = itemi[DT_CounterM_Damage.VALUE];
                        top[DT_CounterM_Damage.TOP_RISK] = itemi[DT_CounterM_Damage.TOP_RISK];
                        top[DT_CounterM_Damage.COUNTERM_NAMESHORT] = itemi[DT_CounterM_Damage.COUNTERM_NAMESHORT];
                        top[DT_CounterM_Damage.DAMAGE] = itemi[DT_CounterM_Damage.DAMAGE];
                        top[DT_CounterM_Damage.GROUPENAME] = itemi[DT_CounterM_Damage.GROUPENAME];
                        top[DT_CounterM_Damage.IDRISK] = drCM[DT_CounterM.ID_RISK];
                        top[DT_CounterM_Damage.ID_GROUPE] = itemi[DT_CounterM_Damage.ID_GROUPE];
                        top[DT_CounterM_Damage.ID_RISK_TREE] = drCM[DT_CounterM.ID_RISK_TREE];
                        top[DT_CounterM_Damage.RISK] = drCM[DT_CounterM.RISK_NAMESHORT];
                        top[DT_CounterM_Damage.RISK_REDUCTION] = drCM[DT_CounterM.PROBABILITY];
                        top[DT_CounterM_Damage.STATUS] = itemi[DT_CounterM_Damage.STATUS];
                        Ds.Tables[DT_CounterM_Damage.TABLENAME].Rows.Add(top);
                    }
                    foreach (DataRow item in Ds.Tables[DT_Role_CM.TABLENAME].Select(DT_Role_CM.ID_CM + " = " + Line.ID))
                    {
                        if (!(Ds.Tables[DT_Role_CM.TABLENAME].Rows.Contains(new object[] { drCM[DT_CounterM.ID], item[DT_Role_CM.IDROL_COLUMN] })))
                        {
                            DataRow newRow = Ds.Tables[DT_Role_CM.TABLENAME].NewRow();
                            newRow[DT_Role_CM.ID_CM] = drCM[DT_CounterM.ID];
                            newRow[DT_Role_CM.IDROL_COLUMN] = item[DT_Role_CM.IDROL_COLUMN];
                            newRow[DT_Role_CM.Role] = item[DT_Role_CM.Role];
                            Ds.Tables[DT_Role_CM.TABLENAME].Rows.Add(newRow);
                        }
                    }
                    foreach (DataRow item in Ds.Tables[DT_CM_WBS.TABLENAME].Select(DT_CM_WBS.ID_CM + " = " + Line.ID))
                    {
                        if (!(Ds.Tables[DT_CM_WBS.TABLENAME].Rows.Contains(new object[] { drCM[DT_CounterM.ID], item[DT_CM_WBS.ID_WBS] })))
                        {
                            DataRow newRow = Ds.Tables[DT_CM_WBS.TABLENAME].NewRow();
                            newRow[DT_CM_WBS.ID_CM] = drCM[DT_CounterM.ID];
                            newRow[DT_CM_WBS.ID_WBS] = item[DT_CM_WBS.ID_WBS];
                            newRow[DT_CM_WBS.WBS] = item[DT_CM_WBS.WBS];
                            newRow[DT_CM_WBS.USERNAME] = item[DT_CM_WBS.USERNAME];
                            newRow[DT_CM_WBS.IS_PRIMARY] = item[DT_CM_WBS.IS_PRIMARY];
                            newRow[DT_CM_WBS.PRIMARY] = item[DT_CM_WBS.PRIMARY];
                            newRow[DT_CM_WBS.NIVEL] = item[DT_CM_WBS.NIVEL];
                            Ds.Tables[DT_CM_WBS.TABLENAME].Rows.Add(newRow);
                        }
                    }
                    //Añadiendo los wbs y  los WBS con Daños del padre
                    foreach (DataRow item in Ds.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_RISK + " = " + idFather))
                    {
                        if (!(Ds.Tables[DT_CM_WBS.TABLENAME].Rows.Contains(new object[] { drCM[DT_CounterM.ID], item[DT_CM_WBS.ID_WBS] })))
                        {
                            DataRow newRow = Ds.Tables[DT_CM_WBS.TABLENAME].NewRow();
                            newRow[DT_CM_WBS.ID_CM] = drCM[DT_CounterM.ID];
                            newRow[DT_CM_WBS.ID_WBS] = item[DT_RISK_WBS.ID_WBS];
                            newRow[DT_CM_WBS.WBS] = item[DT_RISK_WBS.WBS];
                            newRow[DT_CM_WBS.USERNAME] = item[DT_RISK_WBS.USERNAME];
                            newRow[DT_CM_WBS.IS_PRIMARY] = false;
                            newRow[DT_CM_WBS.PRIMARY] = "";
                            newRow[DT_CM_WBS.NIVEL] = item[DT_RISK_WBS.NIVEL];
                            Ds.Tables[DT_CM_WBS.TABLENAME].Rows.Add(newRow);
                        }
                    }
                    foreach (DataRow item in Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + idFather))
                    {
                        DataRow newRow = Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].NewRow();
                        newRow[DT_WBS_CM_Damage.ID_CM] = drCM[DT_CounterM.ID];
                        newRow[DT_WBS_CM_Damage.ID_WBS] = item[DT_WBS_RISK_DAMAGE.ID_WBS];
                        newRow[DT_WBS_CM_Damage.WBS] = item[DT_WBS_RISK_DAMAGE.WBS];
                        newRow[DT_WBS_CM_Damage.ID_DAMAGE] = item[DT_WBS_RISK_DAMAGE.ID_DAMAGE];
                        newRow[DT_WBS_CM_Damage.DAMAGE] = item[DT_WBS_RISK_DAMAGE.DAMAGE];
                        newRow[DT_WBS_CM_Damage.VALUE] = 0;
                        if (!(Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Rows.Contains(new object[] { newRow[DT_WBS_CM_Damage.ID_WBS], newRow[DT_WBS_CM_Damage.ID_DAMAGE], drCM[DT_CounterM.ID] })))
                        {                           
                            Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Rows.Add(newRow);
                        }
                    }
                    if (Ds.Tables[DT_CM_WBS.TABLENAME].Select(DT_CM_WBS.ID_CM + " = " + drCM[DT_CounterM.ID] + " and " + DT_CM_WBS.IS_PRIMARY + " = 1").Any())
                    {
                        DataRow drWBS = Ds.Tables[DT_CM_WBS.TABLENAME].Select(DT_CM_WBS.ID_CM + " = " + drCM[DT_CounterM.ID] + " and " + DT_CM_WBS.IS_PRIMARY + " = 1").First();
                        foreach (DataRow itemi in Ds.Tables[DT_CounterM_Damage.TABLENAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + drCM[DT_CounterM.ID]))
                        {
                            itemi[DT_CounterM_Damage.ID_WBS] = drWBS[DT_CM_WBS.ID_WBS];
                            itemi[DT_CounterM_Damage.WBS_NAME] = drWBS[DT_CM_WBS.NIVEL] + " " + drWBS[DT_CM_WBS.WBS];
                            itemi[DT_CounterM_Damage.USERNAME] = drWBS[DT_CM_WBS.USERNAME];
                        }
                        drCM[DT_CounterM.ID_WBS] = drWBS[DT_RISK_WBS.ID_WBS];
                        drCM[DT_CounterM.WBS_NAME] = drWBS[DT_CM_WBS.NIVEL] + " " + drWBS[DT_CM_WBS.WBS];
                    }
                }
                else
                {
                    DataRow drRisk = CreateNewRisk(Ds, Line, idFather);
                    CreateRiskUntilLine(Line, idFather, drRisk, Ds);
                }
            }
            else
            {
                DataRow drRisk = CreateNewRisk(Ds, Line, idFather);
                CreateRiskUntilLine(Line, idFather, drRisk, Ds);
                foreach (var item in Line.Children)
                {
                    CreateCopy(item, (Int32)drRisk[DT_Risk.ID], Ds);
                }
            }
        }

        public static DataRow CreateNewRisk(DataSet Ds, RiskPolyLine Line, int idFather)
        {
            DataRow drRisk = Ds.Tables[DT_Risk.TABLE_NAME].NewRow();
            drRisk[DT_Risk.COMMENTS] = Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line.ID)[DT_Risk.COMMENTS];
            drRisk[DT_Risk.ENABLED] = Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line.ID)[DT_Risk.ENABLED];
            drRisk[DT_Risk.FROM_TOP] = Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line.ID)[DT_Risk.FROM_TOP];
            drRisk[DT_Risk.ID_DIAGRAM] = Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line.ID)[DT_Risk.ID_DIAGRAM];
            drRisk[DT_Risk.ISCOLLAPSED] = Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line.ID)[DT_Risk.ISCOLLAPSED];
            drRisk[DT_Risk.IS_ROOT] = false;
            drRisk[DT_Risk.NAMESHORT] = Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line.ID)[DT_Risk.NAMESHORT];
            drRisk[DT_Risk.POSITION] = Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line.ID)[DT_Risk.POSITION];
            drRisk[DT_Risk.PROBABILITY] = Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line.ID)[DT_Risk.PROBABILITY];
            drRisk[DT_Risk.IDRISK_FATHER] = idFather;
            drRisk[DT_Risk.FATHER] = Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(idFather)[DT_Risk.NAMESHORT];
            drRisk[DT_Risk.USER_NAME] = Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line.ID)[DT_Risk.USER_NAME];
            drRisk[DT_Risk.ID_WBS] = Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line.ID)[DT_Risk.ID_WBS];
            drRisk[DT_Risk.WBS_NAME] = Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line.ID)[DT_Risk.WBS_NAME];
            Ds.Tables[DT_Risk.TABLE_NAME].Rows.Add(drRisk);
            return drRisk;
        }

        public static void CreateRiskUntilLine(RiskPolyLine Line, int idFather, DataRow drRisk, DataSet Ds)
        {
            DataRow rowstructure = Ds.Tables[DT_RiskStructure.TABLE_NAME].NewRow();
            rowstructure[DT_RiskStructure.IDRISK] = drRisk[DT_Risk.ID];
            rowstructure[DT_RiskStructure.IDRISK_FATHER] = idFather;
            Ds.Tables[DT_RiskStructure.TABLE_NAME].Rows.Add(rowstructure);
            foreach (DataRow itemi in Ds.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_RISK + " = " + Line.ID))
            {
                DataRow top = Ds.Tables[DT_Risk_Damages.TABLENAME].NewRow();
                top[DT_Risk_Damages.COLOR] = itemi[DT_Risk_Damages.COLOR];
                top[DT_Risk_Damages.ID_DAMAGE] = itemi[DT_Risk_Damages.ID_DAMAGE];
                top[DT_Risk_Damages.ID_RISK] = drRisk[DT_Risk.ID];
                top[DT_Risk_Damages.VALUE] = itemi[DT_Risk_Damages.VALUE];
                top[DT_Risk_Damages.TOP_RISK] = itemi[DT_Risk_Damages.TOP_RISK];
                top[DT_Risk_Damages.DAMAGE] = itemi[DT_Risk_Damages.DAMAGE];
                top[DT_Risk_Damages.FATHER] = Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(idFather)[DT_Risk.NAMESHORT];
                top[DT_Risk_Damages.GROUPE_NAME] = itemi[DT_Risk_Damages.GROUPE_NAME];
                top[DT_Risk_Damages.ID_FATHER] = idFather;
                top[DT_Risk_Damages.ID_GROUPE] = itemi[DT_Risk_Damages.ID_GROUPE];
                top[DT_Risk_Damages.ID_RISK_TREE] = drRisk[DT_Risk.ID_DIAGRAM];
                top[DT_Risk_Damages.IS_ROOT] = drRisk[DT_Risk.IS_ROOT];
                top[DT_Risk_Damages.PROBABILITY] = drRisk[DT_Risk.PROBABILITY];
                top[DT_Risk_Damages.RISK_NAMESHORT] = drRisk[DT_Risk.NAMESHORT];
                top[DT_Risk_Damages.RISK_TREE] = itemi[DT_Risk_Damages.RISK_TREE];
                top[DT_Risk_Damages.STATUS] = itemi[DT_Risk_Damages.STATUS];
                Ds.Tables[DT_Risk_Damages.TABLENAME].Rows.Add(top);
            }

            foreach (DataRow itemo in Ds.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + Line.ID))
            {
                if (!(Ds.Tables[DT_Role_Risk.TABLENAME].Rows.Contains(new object[] { drRisk[DT_Risk.ID], itemo[DT_Role_Risk.IDROL_COLUMN] })))
                {
                    DataRow newRow = Ds.Tables[DT_Role_Risk.TABLENAME].NewRow();
                    newRow[DT_Role_Risk.ID_RISK] = drRisk[DT_Risk.ID];
                    newRow[DT_Role_Risk.IDROL_COLUMN] = itemo[DT_Role_Risk.IDROL_COLUMN];
                    newRow[DT_Role_Risk.Role] = itemo[DT_Role_Risk.Role];
                    Ds.Tables[DT_Role_Risk.TABLENAME].Rows.Add(newRow);
                }
            }

            foreach (DataRow item in Ds.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_RISK + " = " + Line.ID))
            {
                if (!(Ds.Tables[DT_RISK_WBS.TABLENAME].Rows.Contains(new object[] { drRisk[DT_Risk.ID], item[DT_RISK_WBS.ID_WBS] })))
                {
                    DataRow newRow = Ds.Tables[DT_RISK_WBS.TABLENAME].NewRow();
                    newRow[DT_RISK_WBS.ID_RISK] = drRisk[DT_Risk.ID];
                    newRow[DT_RISK_WBS.ID_WBS] = item[DT_RISK_WBS.ID_WBS];
                    newRow[DT_RISK_WBS.WBS] = item[DT_RISK_WBS.WBS];
                    newRow[DT_RISK_WBS.NIVEL] = item[DT_RISK_WBS.NIVEL];
                    newRow[DT_RISK_WBS.PRIMARY] = item[DT_RISK_WBS.PRIMARY];
                    newRow[DT_RISK_WBS.IS_PRIMARY] = item[DT_RISK_WBS.IS_PRIMARY];
                    newRow[DT_RISK_WBS.USERNAME] = item[DT_RISK_WBS.USERNAME];
                    Ds.Tables[DT_RISK_WBS.TABLENAME].Rows.Add(newRow);
                }
            }
            foreach (DataRow item in Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + Line.ID))
            {
                DataRow newRow = Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].NewRow();
                newRow[DT_WBS_RISK_DAMAGE.ID_RISK] = drRisk[DT_Risk.ID];
                newRow[DT_WBS_RISK_DAMAGE.ID_WBS] = item[DT_WBS_RISK_DAMAGE.ID_WBS];
                newRow[DT_WBS_RISK_DAMAGE.WBS] = item[DT_WBS_RISK_DAMAGE.WBS];
                newRow[DT_WBS_RISK_DAMAGE.ID_DAMAGE] = item[DT_WBS_RISK_DAMAGE.ID_DAMAGE];
                newRow[DT_WBS_RISK_DAMAGE.DAMAGE] = item[DT_WBS_RISK_DAMAGE.DAMAGE];
                newRow[DT_WBS_RISK_DAMAGE.VALUE] = item[DT_WBS_RISK_DAMAGE.VALUE];
                if (!(Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Contains(new object[] { item[DT_WBS_RISK_DAMAGE.ID_WBS], item[DT_WBS_RISK_DAMAGE.ID_DAMAGE], drRisk[DT_Risk.ID] })))
                {
                    Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Add(newRow);
                }
                
            }
            AddWBSOfMyFather(idFather, Ds, drRisk);
            if (Ds.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_RISK + " = " + drRisk[DT_Risk.ID] + " and " + DT_RISK_WBS.IS_PRIMARY + " = 1").Any())
            {
                DataRow drWBS = Ds.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_RISK + " = " + drRisk[DT_Risk.ID] + " and " + DT_RISK_WBS.IS_PRIMARY + " = 1").First();
                foreach (DataRow itemi in Ds.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_RISK + " = " + drRisk[DT_Risk.ID]))
                {
                    itemi[DT_Risk_Damages.ID_WBS] = drWBS[DT_RISK_WBS.ID_WBS];
                    itemi[DT_Risk_Damages.WBS_NAME] = drWBS[DT_RISK_WBS.NIVEL]+ " " + drWBS[DT_RISK_WBS.WBS];
                    itemi[DT_Risk_Damages.USERNAME] = drWBS[DT_RISK_WBS.USERNAME];
                }
                drRisk[DT_Risk.ID_WBS] = drWBS[DT_RISK_WBS.ID_WBS];
                drRisk[DT_Risk.WBS_NAME] = drWBS[DT_RISK_WBS.NIVEL] + " " + drWBS[DT_RISK_WBS.WBS];
            }
            
            Line.ID = (Int32)drRisk[DT_Risk.ID];
        }

        private static void AddWBSOfMyFather(int idFather, DataSet dataSet, DataRow drRisk)
        {
            foreach (DataRow item in dataSet.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_RISK + " = " + idFather))
            {
                if (!(dataSet.Tables[DT_RISK_WBS.TABLENAME].Rows.Contains(new object[] { drRisk[DT_Risk.ID], item[DT_RISK_WBS.ID_WBS] })))
                {
                    DataRow newRow = dataSet.Tables[DT_RISK_WBS.TABLENAME].NewRow();
                    newRow[DT_RISK_WBS.ID_RISK] = drRisk[DT_Risk.ID];
                    newRow[DT_RISK_WBS.ID_WBS] = item[DT_RISK_WBS.ID_WBS];
                    newRow[DT_RISK_WBS.WBS] = item[DT_RISK_WBS.WBS];
                    newRow[DT_RISK_WBS.NIVEL] = item[DT_RISK_WBS.NIVEL];
                    newRow[DT_RISK_WBS.PRIMARY] = "";
                    newRow[DT_RISK_WBS.IS_PRIMARY] = false;
                    newRow[DT_RISK_WBS.USERNAME] = item[DT_RISK_WBS.USERNAME];
                    dataSet.Tables[DT_RISK_WBS.TABLENAME].Rows.Add(newRow);
                }
            }
            foreach (DataRow item in dataSet.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + idFather))
            {
                DataRow newRow = dataSet.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].NewRow();
                newRow[DT_WBS_RISK_DAMAGE.ID_RISK] = drRisk[DT_Risk.ID];
                newRow[DT_WBS_RISK_DAMAGE.ID_WBS] = item[DT_WBS_RISK_DAMAGE.ID_WBS];
                newRow[DT_WBS_RISK_DAMAGE.WBS] = item[DT_WBS_RISK_DAMAGE.WBS];
                newRow[DT_WBS_RISK_DAMAGE.ID_DAMAGE] = item[DT_WBS_RISK_DAMAGE.ID_DAMAGE];
                newRow[DT_WBS_RISK_DAMAGE.DAMAGE] = item[DT_WBS_RISK_DAMAGE.DAMAGE];
                newRow[DT_WBS_RISK_DAMAGE.VALUE] = 0;
                if (!(dataSet.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Contains(new object[] { item[DT_WBS_RISK_DAMAGE.ID_WBS], item[DT_WBS_RISK_DAMAGE.ID_DAMAGE], drRisk[DT_Risk.ID] })))
                {
                    dataSet.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Add(newRow);
                }
            }
        }

        public static void DeleteLine(RiskPolyLine LineToDelete, DataSet DsMain)
        {
            try
            {
                if (!(LineToDelete.IsCM))
                {
                    List<DataRow> lista = new List<DataRow>
                    {
                        DsMain.Tables[DT_Risk.TABLE_NAME].Rows.Find(LineToDelete.ID)
                    };

                    General.DeleteRiskAndCMFirst(lista, DsMain);
                }
                else
                {
                    DsMain.Tables[DT_CounterM.TABLE_NAME].Rows.Find(LineToDelete.ID).Delete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        public static void AjustarPosicionHijos(RiskPolyLine line, DataSet Ds)
        {
            int count = 0;
            foreach (var item in line.Children.OrderBy(x => x.Position))
            {
                if (item.IsCM)
                {
                    Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(item.ID)[DT_CounterM.POSITION] = count;
                }
                else
                {
                    Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(item.ID)[DT_Risk.POSITION] = count;
                }
                count++;
                if (item.Children.Count > 0)
                {
                    AjustarPosicionHijos(item, Ds);
                }
            }
        }

        public static void AjustarPosicionHijosInExcel(RiskPolyLine line, DataSet Ds)
        {
            int count = 0;
            foreach (var item in line.Children.OrderByDescending(x => x.IsCM))
            {
                if (item.IsCM)
                {
                    Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(item.ID)[DT_CounterM.POSITION] = count;
                }
                else
                {
                    Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(item.ID)[DT_Risk.POSITION] = count;
                }
                count++;
                if (item.Children.Count > 0)
                {
                    AjustarPosicionHijosInExcel(item, Ds);
                }
            }
        }

        public static void OrderTableHierarquical(DataTable table, List<RiskPolyLine> lines, string id)
        {
            DataTable temTable = table.Copy();
            table.Clear();
            List<RiskPolyLine> listaPreorden = new List<RiskPolyLine>();
            foreach (var item in lines.FindAll(r => r.IsRoot == false))
            {
                if (item.Father.IsRoot)
                {
                    listaPreorden.AddRange(PreOrden(item));
                }
            }
            foreach (var item in listaPreorden)
            {
                if (!(table.Select(id + " = " + item.ID)).Any())
                {
                    if (temTable.Select(id + " = " + item.ID).Any())
                    {
                        table.ImportRow(temTable.Select(id + " = " + item.ID).First());
                    }
                }
            }
        }

        public static List<RiskPolyLine> PreOrden(RiskPolyLine Father)
        {
            List<RiskPolyLine> preorden = new List<RiskPolyLine>
            {
                Father
            };
            foreach (var item in Father.Children)
            {
                preorden.AddRange(PreOrden(item));
            }
            return preorden;
        }

        public static int DetectClickPosition(Point mousePosition, RiskPolyLine FatherLine)
        {
            try
            {
                RiskPolyLine hermanoMenor = new RiskPolyLine();
                bool hayHermanoMenor = false;
                if (FatherLine.IsDiagonal)
                {
                    if (FatherLine.FromTop)
                    {
                        if (FatherLine.Children.Where(r => r.Points[1].Y < mousePosition.Y).OrderByDescending(r => r.Points[1].Y).Any())
                        {
                            hermanoMenor = FatherLine.Children.Where(r => r.Points[1].Y < mousePosition.Y).OrderByDescending(r => r.Points[1].Y).First();
                            hayHermanoMenor = true;
                        }
                    }
                    else
                    {
                        if (FatherLine.Children.Where(r => r.Points[1].Y > mousePosition.Y).OrderBy(r => r.Points[1].Y).Any())
                        {
                            hermanoMenor = FatherLine.Children.Where(r => r.Points[1].Y > mousePosition.Y).OrderBy(r => r.Points[1].Y).First();
                            hayHermanoMenor = true;
                        }
                    }
                }
                else
                {
                    if (FatherLine.Children.Where(r => r.Points[1].X < mousePosition.X).OrderByDescending(x => x.Points[1].X).Any())
                    {
                        hermanoMenor = FatherLine.Children.Where(r => r.Points[1].X < mousePosition.X).OrderByDescending(x => x.Points[1].X).First();
                        hayHermanoMenor = true;
                    }
                }
                if (hayHermanoMenor)
                {
                    return FatherLine.Children.FindIndex(l => l.ID == hermanoMenor.ID);
                }
                else
                {
                    return FatherLine.Children.Count;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void DetectarMiPosicionActual(RiskPolyLine linea, Point mousePosition, DataSet Ds)
        {
            RiskPolyLine hermanoMenor = new RiskPolyLine();
            bool hayHermanoMenor = false;
            if (linea.Father.IsDiagonal)
            {
                if (linea.Father.FromTop)
                {
                    if (linea.Father.Children.Where(r => r.ID != linea.ID && r.Points[1].Y < mousePosition.Y).OrderByDescending(r => r.Points[1].Y).Any())
                    {
                        hermanoMenor = linea.Father.Children.Where(r => r.ID != linea.ID && r.Points[1].Y < mousePosition.Y).OrderByDescending(r => r.Points[1].Y).First();
                        hayHermanoMenor = true;
                    }
                }
                else
                {
                    if (linea.Father.Children.Where(r => r.ID != linea.ID && r.Points[1].Y > mousePosition.Y).OrderBy(r => r.Points[1].Y).Any())
                    {
                        hermanoMenor = linea.Father.Children.Where(r => r.ID != linea.ID && r.Points[1].Y > mousePosition.Y).OrderBy(r => r.Points[1].Y).First();
                        hayHermanoMenor = true;
                    }
                }
            }
            else
            {
                if (linea.Father.Children.Where(r => r.ID != linea.ID && r.Points[1].X < mousePosition.X).OrderByDescending(x => x.Points[1].X).Any())
                {
                    hermanoMenor = linea.Father.Children.Where(r => r.ID != linea.ID && r.Points[1].X < mousePosition.X).OrderByDescending(x => x.Points[1].X).First();
                    hayHermanoMenor = true;
                }
            }
            int oldindex = linea.Father.Children.FindIndex(l => l.ID == linea.ID);
            int newIdex = linea.Father.Children.Count;
            if (hayHermanoMenor)
            {
                newIdex = linea.Father.Children.FindIndex(l => l.ID == hermanoMenor.ID);
            }
            //if (newIdex > oldindex)
            //{
            //    newIdex -= 1;
            //}
            ShiftElement(linea.Father.Children, oldindex, newIdex);
            for (int i = 0; i < linea.Father.Children.Count; i++)
            {
                if (linea.Father.Children[i].IsCM)
                {
                    Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(linea.Father.Children[i].ID)[DT_CounterM.POSITION] = i;
                }
                else
                {
                    Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(linea.Father.Children[i].ID)[DT_Risk.POSITION] = i;
                }
            }
        }
        /// <summary>
        /// Adiciona la cola a todos los CounterMeasure
        /// </summary>
        /// <param name="Lines"></param>
        /// <param name="Up"></param>
        public static void FixCounterMesure(List<RiskPolyLine> Lines, Boolean Up)
        {
            if (Up)
            {
                foreach (var line in Lines)
                {
                    if (line.IsCM)
                    {
                        if (line.IsDiagonal)
                        {
                            line.Points.Add(new Point(line.Points[1].X + RiskPolyLine.diagonalCMTailX, 2.5 * (RiskPolyLine.diagonalCMTailX) + line.Points[1].Y));
                        }
                        else
                        {
                            line.Points.Add(new Point(line.Points[1].X + RiskPolyLine.horizontalCMTailX, line.Points[1].Y));
                        }
                    }

                    if (!line.Collapsed && line.Children.Count > 0)
                    {
                        FixCounterMesure(line.Children, Up);
                    }

                }
            }
            else
            {
                foreach (var line in Lines)
                {
                    if (line.IsCM)
                    {
                        if (line.IsDiagonal)
                        {
                            line.Points.Add(new Point(line.Points[1].X + RiskPolyLine.diagonalCMTailX, -2.5 * (RiskPolyLine.diagonalCMTailX) + line.Points[1].Y));
                        }
                        else
                        {
                            line.Points.Add(new Point(line.Points[1].X + RiskPolyLine.horizontalCMTailX, line.Points[1].Y));
                        }
                    }

                    if (!line.Collapsed && line.Children.Count > 0)
                    {
                        FixCounterMesure(line.Children, Up);
                    }
                }
            }
        }

        /// <summary>
        /// Adiciona la cola a todas los Risk que lo requieren. 
        /// Para ello calcula el valor de Y basado en la funcion y =m*-9+y0
        /// m=2.5 (pendiente), -9 es el valor del desplazamiento en el eje x 
        /// y y0 el un valor y por donde pasa el segmento
        /// </summary>
        /// <param name="lineas"></param>
        /// <param name="MainLine"></param>
        //public static void FixMainLine(List<RiskPolyLine> lineas, RiskPolyLine MainLine)
        //{
        //    //this function find the line with the minimal X size and asign this value to Main Line X.           
        //    //AddTail(lineas);
        //    if (MainLine.Segments.Any())
        //    {
        //        ReorderMainLineSegments(MainLine);
        //    }

        //    if (MainLine.Children.Count > 0)
        //    {
        //        if (MainLine.Children.Where(rl => rl.Collapsed == false).Any())
        //        {
        //            RiskPolyLine lineaMasX = MainLine.Children.Where(rl => rl.Collapsed == false).OrderBy(rl => rl.XTremee()).First();
        //            //the X of the main line will be the minor X of their entire generation
        //            if (MainLine.Segments != null && MainLine.Segments.Any())
        //            {
        //                //MainLine.Segments[MainLine.Segments.Count - 1].Points[0] = new Point(lineaMasX.XTremee(), lineas.Find(rl => rl.IsRoot == true).Points[0].Y);
        //                //MainLine.AddTail();
        //                if (MainLine.Segments.Count > 1)
        //                {
        //                    MainLine.Segments[MainLine.Segments.Count - 1].Points[0] = new Point(lineaMasX.XTremee(), MainLine.Points[0].Y);
        //                    MainLine.Segments[MainLine.Segments.Count - 1].Points[1] = new Point(MainLine.Segments[MainLine.Segments.Count - 2].Points[0].X, MainLine.Segments[MainLine.Segments.Count - 2].Points[0].Y);
        //                }
        //                else
        //                {
        //                    MainLine.Segments[MainLine.Segments.Count - 1].Points[0] = new Point(lineaMasX.XTremee(), MainLine.Points[0].Y);
        //                }
        //            }
        //            else
        //            {
        //                lineas.Find(rl => rl.IsRoot == true).Points[0] = new Point(lineaMasX.XTremee(), lineas.Find(rl => rl.IsRoot == true).Points[0].Y);
        //            }
        //        }
        //    }
        //}

        private static void FixRisk(List<RiskPolyLine> lineas)
        {
            //RiskPolyLine lastChild;
            foreach (RiskPolyLine item in lineas)
            {
                if (!item.IsRoot && !(item.Collapsed) && (item.Children.Count > 0))
                {
                    item.AddTail();
                    //if (item.Children.Count > 0)
                    //{
                    //    item.AddTail();
                    //lastChild = item.Children[item.Children.Count - 1];
                    //if (item.IsDiagonal)
                    //{
                    //    if (item.FromTop)
                    //    {
                    //        //item.Points[0] = new Point(item.Children[item.Children.Count - 1].Points[1].X - (General.basicX * 5), item.Children[item.Children.Count - 1].Points[1].Y - (General.basicY * 5));
                    //        if (item.Segments != null && item.Segments.Any())
                    //        {
                    //            item.Segments[item.Segments.Count - 1].Points[0] = new Point(lastChild.Points[1].X - General.basicX * 5, 2.5 * (-General.basicX * 5) + lastChild.Points[1].Y);

                    //        }
                    //        else
                    //        {
                    //            item.Points[0] = new Point(lastChild.Points[1].X - General.basicX * 5, 2.5 * (-General.basicX * 5) + lastChild.Points[1].Y);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        //item.Points[0] = new Point(item.Children[item.Children.Count - 1].Points[1].X - (General.basicX * 5), item.Children[item.Children.Count - 1].Points[1].Y + (General.basicY * 5));
                    //        if (item.Segments != null && item.Segments.Any())
                    //        {
                    //            item.Segments[item.Segments.Count - 1].Points[0] = new Point(lastChild.Points[1].X - General.basicX * 5, -2.5 * (-General.basicX * 5) + lastChild.Points[1].Y);
                    //        }
                    //        else
                    //        {
                    //            item.Points[0] = new Point(lastChild.Points[1].X - General.basicX * 5, -2.5 * (-General.basicX * 5) + lastChild.Points[1].Y);
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    //item.Points[0] = new Point(item.Children[item.Children.Count - 1].Points[1].X - 25, item.Children[item.Children.Count - 1].Points[1].Y);
                    //    if (item.Segments != null && item.Segments.Any())
                    //    {
                    //        item.Segments[item.Segments.Count - 1].Points[0] = new Point(lastChild.Points[1].X - 25, lastChild.Points[1].Y);
                    //    }
                    //    else
                    //    {
                    //        item.Points[0] = new Point(lastChild.Points[1].X - 25, lastChild.Points[1].Y);
                    //    }
                    //}
                    //item.XTreme = item.Points[0].X;
                    //item.YxTreme = item.Points[0].Y;
                    //}
                }
            }
        }

        public static void DetectarMiPosicionActualInMain(RiskPolyLine linea, Point mousePosition, DataSet Ds)
        {
            RiskPolyLine hermanoMenor = new RiskPolyLine();
            bool hayHermanoMenor = false;
            if (linea.Father.Children.Where(r => r.ID != linea.ID && r.StartDrawPoint.X < mousePosition.X && r.FromTop == linea.FromTop).OrderByDescending(x => x.StartDrawPoint.X).Any())
            {
                hermanoMenor = linea.Father.Children.Where(r => r.ID != linea.ID && r.StartDrawPoint.X < mousePosition.X && r.FromTop == linea.FromTop).OrderByDescending(x => x.StartDrawPoint.X).First();
                hayHermanoMenor = true;
            }

            if (hayHermanoMenor)
            {
                int oldindex = linea.Father.Children.FindIndex(l => l.ID == linea.ID);
                int newIdex = linea.Father.Children.FindIndex(l => l.ID == hermanoMenor.ID);
                if (newIdex > oldindex)
                {
                    newIdex -= 1;
                }
                ShiftElementMain(linea.Father.Children, oldindex, newIdex);
                for (int i = 0; i < linea.Father.Children.Count; i++)
                {
                    if (linea.Father.Children[i].IsCM)
                    {
                        Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(linea.Father.Children[i].ID)[DT_CounterM.POSITION] = i;
                    }
                    else
                    {
                        Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(linea.Father.Children[i].ID)[DT_Risk.POSITION] = i;
                    }
                }
            }
        }

        public static string Spaces(int Level)
        {
            string retornar = "";
            for (int i = 0; i < Level; i++)
            {
                retornar += "    ";
            }
            return retornar;
        }

        private static void CalculateDepth(RiskPolyLine node, int depth)
        {
            node.MyLevel = depth;

            foreach (var child in node.Children)
                CalculateDepth(child, depth + 1);
        }

        public static void ShiftElement(List<RiskPolyLine> lines, int oldIndex, int newIndex)
        {
            RiskPolyLine[] array = lines.ToArray();
            if (oldIndex == newIndex)
            {
                return; // No-op
            }
            RiskPolyLine tmp = array[oldIndex];
            if (newIndex < oldIndex)
            {
                Array.Copy(array, newIndex, array, newIndex + 1, oldIndex - newIndex);
            }
            else
            {
                Array.Copy(array, oldIndex + 1, array, oldIndex, newIndex - oldIndex);
            }
            array[newIndex] = tmp;
            lines.Clear();
            lines.AddRange(array.ToList());
        }

        public static void ShiftElementMain(List<RiskPolyLine> lines, int oldIndex, int newIndex)
        {

            if (oldIndex == newIndex)
            {
                return; // No-op
            }
            RiskPolyLine[] array = new RiskPolyLine[lines.Count];
            //List<RiskPolyLine> other = new List<RiskPolyLine>();
            RiskPolyLine toMove = lines[oldIndex];
            MoveInList(lines, toMove, newIndex);
            int par = 0;
            int impar = 1;
            foreach (var item in lines)
            {
                if (item.FromTop)
                {
                    array[par] = item;
                    array[par].Position = par;
                    par += 2;
                }
                else
                {
                    array[impar] = item;
                    array[impar].Position = impar;
                    impar += 2;
                }
            }
            lines.Clear();
            lines.AddRange(array.ToList());
        }

        public static void MoveInList(List<RiskPolyLine> lines, RiskPolyLine element, int newPosition)
        {
            try
            {
                lines.Remove(element);
                lines.Insert(newPosition, element);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Build the hierarquical tree. Asign the child to a father and father to a child
        /// </summary>
        public static void Build_Tree(List<RiskPolyLine> data)
        {
            if (data.Count > 0)
            {
                var root = data.FirstOrDefault(p => p.IsRoot);

                data.FirstOrDefault(p => p.IsRoot).Children = GetChildNodes(root, data);
                foreach (var item in data.FirstOrDefault(p => p.IsRoot).Children)
                {
                    item.Father = data.FirstOrDefault(p => p.IsRoot);
                }
                // add tree node children recursively
                CalculateDepth(root, 0);
            }
        }

        public static void Build_Tree(List<RiskPolyLine> data, RiskPolyLine root)
        {
            //var root = data.FirstOrDefault(p => p.IdRiskFather == 0);
            if (data.Count > 0)
            {
                root.Children = GetChildNodes(root, data);
                foreach (var item in root.Children)
                {
                    item.Father = root;
                }
                // add tree node children recursively
                CalculateDepth(root, 0);
            }
        }
        /// <summary>
        /// Seek the children of the father and viceversa
        /// </summary>
        private static List<RiskPolyLine> GetChildNodes(RiskPolyLine padre, List<RiskPolyLine> Lista)
        {
            var nodes = new List<RiskPolyLine>();
            if (!(padre.IsCM))
            {
                foreach (var item in Lista.Where(p => p.IdRiskFather == padre.ID).OrderBy(p => p.Position))
                {

                    item.Children = GetChildNodes(item, Lista);

                    foreach (var itemi in item.Children.OrderBy(p => p.Position))
                    {
                        itemi.Father = item;
                    }
                    nodes.Add(item);
                }

            }
            return nodes;
        }

        /// <summary>
        /// Return the entire descendence of the father giving the list , excluding the Father
        /// </summary>
        public static List<RiskPolyLine> GetOnlyMyAllChildsWithCM(RiskPolyLine fatherLine)
        {
            try
            {
                //OBTIENE solo LA DECENCENDIA DE drRoot SIN CONTRALO A EL MISMO
                List<RiskPolyLine> returnList = new List<RiskPolyLine>();
                foreach (RiskPolyLine item in fatherLine.Children)
                {
                    if (item.IsLeaf())
                    {
                        returnList.Add(item);
                    }
                    else
                    {
                        returnList.AddRange(GetMeAndAllChildsWithCM(item));
                    }
                }
                return returnList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Return the entire descendence of the father giving the list , including the Father
        /// </summary>
        public static List<RiskPolyLine> GetMeAndAllChildsWithCM(RiskPolyLine lineFather)
        {
            try
            {
                //GET padre ENTIRE DESCENDENCE INCLUDING padre
                List<RiskPolyLine> returnList = new List<RiskPolyLine>
                {
                    lineFather
                };

                foreach (var item in lineFather.Children)
                {
                    if (item.IsLeaf())
                    {
                        returnList.Add(item);
                    }
                    else
                    {
                        returnList.AddRange(GetMeAndAllChildsWithCM(item));
                    }
                }
                return returnList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Set the values of the DataRow Risk to RiskLine
        /// </summary>
        public static void SetRiskLineValues(RiskPolyLine rl, DataRow RiskRow)
        {
            try
            {
                //SET PRIMARY DATA TO A RISK
                rl.ShortName = RiskRow[DT_Risk.NAMESHORT].ToString();
                rl.Position = (Int32)RiskRow[DT_Risk.POSITION];
                rl.Probability = (Decimal)RiskRow[DT_Risk.PROBABILITY] / 100;

                if (rl.IsCM)
                {
                    rl.Collapsed = false;
                }
                else
                {
                    rl.Collapsed = (Boolean)RiskRow[DT_Risk.ISCOLLAPSED];
                    if (rl.Collapsed)
                    {
                        rl.Expand.Source = new BitmapImage(new Uri(General.CONTRAIDO));
                    }
                    else
                    {
                        rl.Expand.Source = new BitmapImage(new Uri(General.EXPANDIDO));
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //private static int Recursivo(RiskPolyLine line, bool IsDiagonal)
        //{
        //    return 1;
        //}

        private static int NivelesVerticalesTotal(RiskPolyLine line, bool IsDiagonal)
        {
            int result = 0;
            try
            {
                foreach (var item in line.Children)
                {
                    if (item.Children.Count > 0)
                    {
                        result += NivelesVerticalesTotal(item, !IsDiagonal);
                    }
                    if (IsDiagonal == true)
                    {
                        result += 1;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Clasifica todas los RiskPolyLine que son directamentes hijos de MainLine.
        /// El objetivo es saber que ramas se dibujan por encima y debajo de MainLine
        /// </summary>
        /// <param name="Lines"></param>
        private static void LineClassify(List<RiskPolyLine> Lines)
        {
            int countChild = 0;
            foreach (var child in Lines.OrderBy(x => x.Position))
            {
                if (countChild % 2 == 0)//the first insertion wil be always from top
                {
                    LinesUp.Add(child);
                    child.FromTop = true;
                }
                else
                {
                    LinesDown.Add(child);
                    child.FromTop = false;
                }
                countChild++;
            }
        }

        /// <summary>
        /// Dibuja los RiskPolyLine dados siempre por encima de MainLine
        /// </summary>
        /// <param name="Lines"></param>
        /// <param name="StartPoint"></param>
        private static void DrawDiagramAsFishBone(List<RiskPolyLine> Lines, Point StartPoint)
        {
            try
            {
                IEnumerable<RiskPolyLine> orderedLines = Lines.OrderBy(x => x.Position);

                for (int i = 0; i < Lines.Count; i++)
                {
                    var line = orderedLines.ElementAt(i);

                    if (line.Father.Father == null)
                    {
                        line.IsDiagonal = true;
                    }
                    else
                    {
                        line.IsDiagonal = !line.Father.IsDiagonal;
                        line.FromTop = line.Father.FromTop;
                    }

                    //if (i == 0 && line.Father.IsRoot)
                    //{
                    //    if (line.Father.Segments.Any())
                    //    {
                    //        line.Father.ExtendHorizontal(StartPoint.X + RiskPolyLine.horizontalShiftX);
                    //        line.NewDrawAtPoint(new Point(line.Father.XTreme, line.Father.YxTreme), line.ShortName);
                    //    }
                    //    else
                    //    {
                    //        // Esto solo ocurre cuando MainLine no tiene segmentos
                    //        line.NewDrawAtPoint(new Point(StartPoint.X, StartPoint.Y), line.ShortName);
                    //    }
                    //}
                    //else
                    //{
                    if (i == 0)
                    {
                        line.NewDrawAtPoint(new Point(StartPoint.X, StartPoint.Y), line.ShortName);
                    }
                    else
                    {
                        if (line.IsDiagonal)
                        {
                            //Si la linea es diagonal el padre es horizontal por lo tanto el padre se extiende a la izquierda en el eje de las X
                            //line.Father.Points[0] = new Point(orderedLines.ElementAt(i - 1).XTremee() - RiskPolyLine.horizontalShiftX, line.Father.Points[0].Y);
                            //if (line.Father.Father != null)
                            //{
                            //    line.Father.ExtendHorizontal(orderedLines.ElementAt(i - 1).XTremee());
                            //}
                            line.Father.ExtendHorizontal(orderedLines.ElementAt(i - 1).XTremee());

                            //Se dibuja el hijo al final del padre
                            //line.NewDrawAtPoint(new Point(line.Father.Points[0].X, line.Father.Points[0].Y), line.ShortName);
                            line.NewDrawAtPoint(new Point(line.Father.XTreme, line.Father.YxTreme), line.ShortName);
                        }
                        else
                        {
                            //Si la linea es horizontal el padre es diagonal por lo tanto el padre se extiende aumentando en el eje de las Y
                            //line.Father.Points[0] = new Point(line.Father.Points[0].X, orderedLines.ElementAt(i - 1).AbsoluteYxTremee() - RiskPolyLine.diagonalShiftY);
                            line.Father.ExtendVertical(orderedLines.ElementAt(i - 1).AbsoluteYxTremee());
                            //Se dibuja el hijo al final del padre
                            //line.NewDrawAtPoint(new Point(line.Father.Points[0].X, line.Father.Points[0].Y), line.ShortName);
                            line.NewDrawAtPoint(new Point(line.Father.XTreme, line.Father.YxTreme), line.ShortName);
                        }
                    }
                    //}
                    if (!(line.Collapsed) && line.Children.Count > 0)
                    {
                        DrawDiagramAsFishBone(line.Children, new Point(line.Points[0].X, line.Points[0].Y));
                    }

                    //TODO hmax = 101, altura que el diagrama nunca alcanza
                    //TODO: ROBERTO: QUITE EL DESPLAZAR A LA DERECHA PUES NO FUNCIONA CON L POSICIONAMIENTO
                    //A CONTACTAR CON LUCAS A VER QUE DESEA, O LO UNO O LO OTRO
                    //Este fragmento es el correcto
                    if (line.Father.Father == null)
                    {
                        //if (i > 0 && line.FromTop == true)
                        if (i > 0)
                        {
                            MoveRight(Lines, line, hmax);
                        }
                    }

                    //Console.WriteLine(line.ShortName + " : " + i);
                    //if (line.Father.Father == null && i > 0 && line.FromTop == true)
                    //{
                    //    line.ShortName += "-referencePoint-";
                    //    // hacer validaciones aqui para ver si puede moverse a la derecha
                    //    // en caso positivo poner punto inicial y repintar
                    //    line.Father.Points[0] = new Point(orderedLines.ElementAt(i - 1).XTremee() + RiskPolyLine.horizontalShiftX, line.Father.Points[0].Y);
                    //    line.NewDrawAtPoint(new Point(line.Father.Points[0].X, line.Father.Points[0].Y), line.ShortName);
                    //}
                }
            }
            catch (Exception ex)
            {
                throw new Exception("TreeOperation.DrawDiagramAsFishBone(): " + ex.Message);
            }
        }

        /// <summary>
        /// Revisa mover una sola, y hacer recursvio el metodo para calcular la posicion
        /// este metodo mueve un paso en cada iteracion, asi si hay que moverlo 3 lugares
        /// se mueve tres veces, es ineficiente en ese sentido
        /// </summary>
        /// <param name="Lines"></param>
        /// <param name="pLine"></param>
        /// <param name="altura"></param>
        private static void MoveRight(List<RiskPolyLine> Lines, RiskPolyLine pLine, double altura)
        {
            try
            {
                PointToMoveRight referencePoint = GetPointMoveRight(Lines, pLine, altura);
                if (!referencePoint.terminar)
                {
                    TranslateDirectChildrenTemporal(pLine, referencePoint.punto);
                    MoveRight(Lines, pLine, referencePoint.punto.Y);
                }
                else
                {
                    TranslateDirectChildrenTemporal(pLine, referencePoint.punto);
                }
                //Console.Out.WriteLine(" -- paso por aqui --");
            }
            catch (Exception e)
            {
                throw new Exception("TreeOperation.MoveRight: " + e.Message);
            }
        }

        /// <summary>
        /// Dado una rama y el arbol busca si es posible mover a la derecha la rama.
        /// </summary>
        /// <param name="Lines"></param>
        /// <param name="line"></param>
        //private static Point? GetPointMoveRight(List<RiskPolyLine> Lines, RiskPolyLine pLine)
        //{
        //    try
        //    {
        //        Point p = GetMaxPointBrothers(Lines, pLine);
        //        if (p.Y < pLine.YxTremee())
        //        {
        //            return p;
        //        }
        //        return null;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception("TreeOperation.GetPointMoveRight: " + e.Message);
        //    }
        //}

        private static PointToMoveRight GetPointMoveRight(List<RiskPolyLine> Lines, RiskPolyLine pLine, double altura)
        {
            try
            {
                PointToMoveRight result = new PointToMoveRight();
                Point p = GetMaxPointBrothers(Lines, pLine, altura);

                if (p.Y < pLine.YxTremee())
                {
                    result.punto = p;
                    result.terminar = false;
                    return result;
                }
                else
                {
                    // revisa como usar "horizontalShiftX" de la clase pline
                    result.punto = new Point(p.X - RiskPolyLine.horizontalShiftX, p.Y);
                    result.terminar = true;
                    return result;
                }
            }
            catch (Exception e)
            {
                throw new Exception("TreeOperation.GetPointMoveRight: " + e.Message);
            }
        }

        /// <summary>
        /// dada un riesgo(line) devuelve de todos su hermanos anteriores el punto mas a la derecha
        /// para luego revisar si cabe debajo de ese punto
        /// </summary>
        /// <param name="Lines"></param>
        /// <param name="polyLine"></param>
        /// <returns></returns>
        private static Point GetMaxPointBrothers(List<RiskPolyLine> Lines, RiskPolyLine pLine, double altura)
        {
            try
            {
                List<RiskPolyLine> orderedLines = Lines.OrderBy(x => x.Position).ToList();
                Point pmax = new Point(0, 0);

                //Point xtremo = TreeOperation.GetMeAndAllChildsWithCM(pLine.Father).Where(x => (x.Points[0].Y < pLine.Father.YxTremee()) && (x.Position < pLine.Father.Position)).OrderBy(x => x.Points[0].X).Last().Points[0];
                //Point pp = xtremo;

                foreach (var line in orderedLines)
                {
                    if (line.ID == pLine.ID)
                    {
                        break;
                    }
                    else
                    {
                        //Point horizontal = line.HorizontalMaxXTremee(pLine.YxTremee());
                        Point horizontal = line.HorizontalMaxXTremee(altura);
                        if (horizontal.X < pmax.X)
                        {
                            pmax.X = horizontal.X;
                            pmax.Y = horizontal.Y;
                        }
                    }
                }

                //for (int i = 0; i < Lines.Count; i++)
                //{
                //    var line = orderedLines.ElementAt(i);
                //    if (line.ID == pLine.ID)
                //    {
                //        break;
                //    }
                //    else
                //    {
                //        //Point horizontal = line.HorizontalMaxXTremee(pLine.YxTremee());
                //        Point horizontal = line.HorizontalMaxXTremee(altura);
                //        if (horizontal.X < pmax.X)
                //        {
                //            pmax.X = horizontal.X;
                //            pmax.Y = horizontal.Y;
                //        }
                //    }
                //}
                return pmax;
            }
            catch (Exception e)
            {
                throw new Exception("TreeOperation.GetMaxPointBrothers: " + e.Message);
            }
        }

        /// <summary>
        /// Repleja los RiskPolyLine que tienen que estar debajo de MainLine (Recuerdese que se dibujaron encima de MainLine)
        /// </summary>
        /// <param name="Lines"></param>
        /// <param name="ReflectAxesY"></param>
        private static void ReflectLines(List<RiskPolyLine> Lines, double ReflectAxesY)
        {
            foreach (var line in Lines)
            {
                line.Points[0] = new Point(line.Points[0].X, ReflectAxesY + (ReflectAxesY - line.Points[0].Y));
                line.Points[1] = new Point(line.Points[1].X, ReflectAxesY + (ReflectAxesY - line.Points[1].Y));
                if (line.Segments.Any())
                {
                    ReflectSegmentLines(line.Segments, ReflectAxesY);
                }
                if (!(line.Collapsed) && line.Children != null && line.Children.Count > 0)
                {
                    ReflectLines(line.Children, ReflectAxesY);
                }
            }
        }
        private static void ReflectSegmentLines(List<SegmentPolyLine> Lines, double ReflectAxesY)
        {
            foreach (var line in Lines)
            {
                line.Points[0] = new Point(line.Points[0].X, ReflectAxesY + (ReflectAxesY - line.Points[0].Y));
                line.Points[1] = new Point(line.Points[1].X, ReflectAxesY + (ReflectAxesY - line.Points[1].Y));
            }
        }

        /// <summary>
        /// Realiza traslación al RiskPolyLine dado y a toda su descendencia
        /// </summary>
        /// <param name="Line"></param>
        /// <param name="AngleX"></param>
        /// <param name="SkewAxesY"></param>
        private static void TranslateDirectChildren(List<RiskPolyLine> Lines, double AngleX, double SkewAxesY)
        {
            foreach (var line in Lines)
            {
                double XOffSet = (SkewAxesY - line.Points[1].Y) * Math.Tan(AngleX);
                //line.Points[0] = new Point(line.Points[0].X - XOffSet, line.Points[0].Y);
                //line.Points[1] = new Point(line.Points[1].X - XOffSet, line.Points[1].Y);
                line.Move(-(int)XOffSet, 0);
                if (!(line.Collapsed) && line.Children.Count > 0)
                {
                    TranslateLines(line.Children, XOffSet);
                }
            }
        }

        /// <summary>
        /// Realiza traslación al RiskPolyLine dado y a toda su descendencia 322222222222222
        /// </summary>
        /// <param name="Line"></param>
        /// <param name="AngleX"></param>
        /// <param name="SkewAxesY"></param>
        private static void TranslateDirectChildrenTemporal(RiskPolyLine Line, Point referencePoint)
        {
            double XOffSet = Line.Points[1].X - referencePoint.X;
            //Line.Points[0] = new Point(Line.Points[0].X - XOffSet, Line.Points[0].Y);
            //Line.Points[1] = new Point(Line.Points[1].X - XOffSet, Line.Points[1].Y);
            Line.Move((int)-XOffSet, 0);

            foreach (var line in Line.Children)
            {
                //line.Points[0] = new Point(line.Points[0].X - XOffSet, line.Points[0].Y);
                //line.Points[1] = new Point(line.Points[1].X - XOffSet, line.Points[1].Y);
                line.Move((int)-XOffSet, 0);
                if (!(line.Collapsed) && line.Children.Count > 0)
                {
                    TranslateLines(line.Children, XOffSet);
                }
            }
        }

        /// <summary>
        /// Ejecuta traslación a todos los RiskPolyLine dados incluyendo a su descendencia
        /// </summary>
        /// <param name="Lines"></param>
        /// <param name="XOffSet"></param>
        private static void TranslateLines(List<RiskPolyLine> Lines, double XOffSet)
        {
            foreach (var line in Lines)
            {
                //line.Points[0] = new Point(line.Points[0].X - XOffSet, line.Points[0].Y);
                //line.Points[1] = new Point(line.Points[1].X - XOffSet, line.Points[1].Y);
                line.Move(-(int)XOffSet, 0);
                if (!(line.Collapsed) && line.Children.Count > 0)
                {
                    TranslateLines(line.Children, XOffSet);
                }
            }
        }

        /// <summary>
        /// Aplica una traslación a un extremo de todos los RiskPolyLine verticales para lograr se muestren diagonales.
        /// </summary>
        /// <param name="Lines"></param>
        /// <param name="AngleX"></param>
        /// <param name="SkewAxesY"></param>
        private static void SkewLines(List<RiskPolyLine> Lines, double AngleX, double SkewAxesY)
        {
            RiskPolyLine line;
            for (int i = 0; i < Lines.Count; i++)
            {
                line = Lines[i];

                //foreach (var line in Lines)
                //{
                if (line.IsDiagonal)
                {
                    //Si es diagonal
                    //Se trasladan todos los hijos horizontales y toda su decendencia
                    if (line.Children != null)
                    {
                        TranslateDirectChildren(line.Children, AngleX, line.Points[1].Y);
                    }
                    double XOffSetSkew;
                    if (line.IsCM)
                    {
                        XOffSetSkew = (SkewAxesY - line.Points[0].Y) * Math.Tan(AngleX);
                        //Se coloca la linea en su posición diagonal
                        line.Points[0] = new Point(line.Points[0].X - XOffSetSkew, line.Points[0].Y);
                    }
                    else
                    {
                        XOffSetSkew = (SkewAxesY - line.Points[0].Y) * Math.Tan(AngleX);
                        //Se coloca la linea en su posición diagonal
                        line.Points[0] = new Point(line.Points[0].X - XOffSetSkew, line.Points[0].Y);
                        //if (line.Father.Father != null)
                        //{
                        //    //Si no es mainline
                        //    if (i > 0 && line.Father.Segments.Any())
                        //    {
                        //        RiskPolyLine segment = line.Father.Segments[i - 1];
                        //        segment.Points[0] = new Point(line.Points[1].X, line.Points[1].Y);
                        //        segment.Points[1] = new Point(Lines[i - 1].Points[1].X, Lines[i - 1].Points[1].Y);
                        //    }

                        //}
                        //if (line.Segments.Any())
                        //{
                        //    line.MoveSegments((int)(-XOffSetSkew), 0);
                        //    SkewLines(line.Segments, AngleX, line.Points[0].Y);
                        //}
                    }
                }
                else
                {
                    if (line.Father.Father != null)
                    {
                        //Si no es mainline
                        if (i > 0 && line.Father.Segments.Any())
                        {
                            SegmentPolyLine segment = line.Father.Segments[i - 1];
                            segment.Points[0] = new Point(line.Points[1].X, line.Points[1].Y);
                            segment.Points[1] = new Point(Lines[i - 1].Points[1].X, Lines[i - 1].Points[1].Y);
                        }

                    }
                }
                if (!(line.Collapsed) && line.Children != null && line.Children.Count > 0)
                {
                    //Se repite el proceso para sus hijos
                    SkewLines(line.Children, AngleX, line.Points[1].Y);
                }
                //}
            }
        }

        public static void BalancearDiagrama(RiskPolyLine Line)
        {
            double MaxUp = LinesUp.Min(x => x.HorizontalMaxXTremee(hmax).X); double MaxDown = LinesDown.Min(x => x.HorizontalMaxXTremee(hmax).X);
            RiskPolyLine t;
            if (MaxUp < MaxDown)
            {
                while (MaxUp < MaxDown && LinesUp.Count() > 1)
                {
                    t = LinesUp.Last();
                    t.FromTop = false;
                    LinesDown.Add(t);
                    LinesUp.Remove(t);

                    Line.AllSegmentClear();
                    DrawDiagramAsFishBone(LinesUp, new Point(LinesUp[0].Points[1].X, LinesUp[0].Points[1].Y));
                    DrawDiagramAsFishBone(LinesDown, new Point(LinesUp[0].Points[1].X - 30, LinesUp[0].Points[1].Y));

                    MaxUp = LinesUp.Min(x => x.HorizontalMaxXTremee(hmax).X);
                    MaxDown = LinesDown.Min(x => x.HorizontalMaxXTremee(hmax).X);
                }
            }
            else
            {
                while (MaxDown < MaxUp && LinesDown.Count() > 1)
                {
                    t = LinesDown.Last();
                    t.FromTop = true;
                    LinesUp.Add(t);
                    LinesDown.Remove(t);

                    Line.AllSegmentClear();
                    DrawDiagramAsFishBone(LinesUp, new Point(LinesUp[0].Points[1].X, LinesUp[0].Points[1].Y));
                    DrawDiagramAsFishBone(LinesDown, new Point(LinesUp[0].Points[1].X - 30, LinesUp[0].Points[1].Y));

                    MaxUp = LinesUp.Min(x => x.HorizontalMaxXTremee(hmax).X);
                    MaxDown = LinesDown.Min(x => x.HorizontalMaxXTremee(hmax).X);
                }
            }
        }

        /// <summary>
        /// Draw as FishBone diagram the line and its children
        /// </summary>
        /// <param name="Line">Line and its children to draw</param>
        public static void DrawEntireDiagramAsFishBone(RiskPolyLine Line)
        {
            LinesUp.Clear();
            LinesDown.Clear();
            Line.AllSegmentClear();
            LineClassify(Line.Children);
            if (LinesUp.Any())
            {
                DrawDiagramAsFishBone(LinesUp, new Point(Line.Points[0].X, Line.Points[0].Y));
            }
            if (LinesDown.Any())
            {
                DrawDiagramAsFishBone(LinesDown, new Point(LinesUp[0].Points[1].X - 30, LinesUp[0].Points[1].Y));
            }
            if (Line.Children.Count > 2)
            {
                BalancearDiagrama(Line);
                BalancearDiagrama(Line);
            }
            if (LinesUp.Any())
            {
                SkewLines(LinesUp, skewAngle, Line.Points[0].Y);
                FixCounterMesure(LinesUp, true);
                FixRisk(LinesUp);
            }
            if (LinesDown.Count > 0 && LinesUp.Count > 0)
            {
                SkewLines(LinesDown, skewAngle, Line.Points[0].Y);
                ReflectLines(LinesDown, LinesUp[0].Points[1].Y);
                FixCounterMesure(LinesDown, false);
                FixRisk(LinesDown);
            }

            //if (Line.Segments.Any())
            //{
            //    ReorderVisualSegments(Line);
            //}
            //else
            //{
            //    Line.Points[0] = new Point(Line.Children.Last().XTremee(), Line.Points[0].Y);
            //}
            ReorderMainLineSegments(Line);
        }

        private static void ReorderMainLineSegments(RiskPolyLine Line)
        {
            int i = 0;
            IEnumerable<RiskPolyLine> orderedLines = Line.Children.OrderByDescending(r => r.Points[1].X);

            Line.SegmentClear();

            IEnumerator<RiskPolyLine> lineEnumerator = orderedLines.GetEnumerator();

            RiskPolyLine rLine;
            RiskPolyLine lLine;
            SegmentPolyLine segment;
            if (lineEnumerator.MoveNext())
            {
                rLine = lineEnumerator.Current;
                while (lineEnumerator.MoveNext())
                {
                    lLine = lineEnumerator.Current;

                    Line.ExtendHorizontal(rLine.Points[1].X);
                    segment = Line.Segments[i];
                    segment.StartDrawPoint = new Point(lLine.Points[1].X, lLine.Points[1].Y);
                    segment.EndDrawPoint = new Point(rLine.Points[1].X, rLine.Points[1].Y);

                    rLine = lLine;
                    i++;
                }
                //RiskPolyLine lineaMasX = MainLine.Children.Where(rl => rl.Collapsed == false).OrderBy(rl => rl.XTremee()).First();
                double XMax = Line.XTremee();
                Line.ExtendHorizontal(XMax);
                segment = Line.Segments[i];
                segment.StartDrawPoint = new Point(XMax + 5, Line.Points[1].Y);
                segment.EndDrawPoint = new Point(rLine.Points[1].X, Line.Points[1].Y);
            }
        }

        public static List<RiskPolyLine> LoadLines(DataSet ds, int IdTreeRisk)
        {
            List<RiskPolyLine> lista = new List<RiskPolyLine>();
            //System.Drawing.Color drawingCColor = System.Drawing.Color.FromArgb(int.Parse(ds.Tables[DT_RiskTree_Damages.TABLENAME].Select(DT_RiskTree_Damages.ID_RISKTREE + " = " + IdTreeRisk)[0][DT_RiskTree_Damages.COLOR].ToString()));
            foreach (DataRow item in ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_DIAGRAM + " = " + IdTreeRisk))
            {
                if ((Boolean)item[DT_Risk.IS_ROOT])
                {
                    RiskPolyLine MainLine = new RiskPolyLine()
                    {
                        //Stroke = new SolidColorBrush(Color.FromArgb(lnColor.A, lnColor.R, lnColor.G, lnColor.B)),
                        IsRoot = true,
                        IsCM = false,
                        FromTop = (Boolean)item[DT_Risk.FROM_TOP],
                        StrokeThickness = 6,
                        ID = (Int32)item[DT_Risk.ID],
                        Probability = (Decimal)item[DT_Risk.PROBABILITY],
                        ShortName = item[DT_Risk.NAMESHORT].ToString(),
                        MyLevel = 0
                    };
                    MainLine.Group = new LineGroup()
                    {
                        IdGroup = null,
                        GroupName = "None"
                    };
                    lista.Add(MainLine);
                }
                else
                {
                    RiskPolyLine riskLine = new RiskPolyLine()
                    {
                        ShortName = item[DT_Risk.NAMESHORT].ToString(),
                        ID = (Int32)item[DT_Risk.ID],
                        Position = (Int32)item[DT_Risk.POSITION],
                        Collapsed = (Boolean)item[DT_Risk.ISCOLLAPSED],
                        Probability = (Decimal)item[DT_Risk.PROBABILITY],
                        IsActivated = (Boolean)item[DT_Risk.ENABLED],
                        StrokeThickness = 2,
                        IsCM = false,
                        IdRiskFather = (Int32)item[DT_Risk.IDRISK_FATHER]
                    };
                    if (item[DT_Risk.ID_GROUPE] != DBNull.Value)
                    {
                        riskLine.Group = new LineGroup()
                        {
                            IdGroup = (Int32)item[DT_Risk.ID_GROUPE],
                            GroupName = item[DT_Risk.GROUPE_NAME].ToString()
                        };
                    }
                    else
                    {
                        riskLine.Group = new LineGroup()
                        {
                            IdGroup = null,
                            GroupName = item[DT_Risk.GROUPE_NAME].ToString()
                        };
                    }

                    lista.Add(riskLine);
                }
            }
            foreach (DataRow item in ds.Tables[DT_CounterM.TABLE_NAME].Select(DT_CounterM.ID_RISK_TREE + " = " + IdTreeRisk))
            {
                RiskPolyLine cmline = new RiskPolyLine()
                {
                    IsCM = true,
                    Position = (Int32)item[DT_CounterM.POSITION],
                    ShortName = item[DT_CounterM.NAMESHORT].ToString(),
                    IdRiskFather = (Int32)item[DT_CounterM.ID_RISK],
                    ID = (Int32)item[DT_CounterM.ID],
                    Probability = (Decimal)item[DT_CounterM.PROBABILITY],
                    IsActivated = (Boolean)item[DT_CounterM.ENABLED]
                };
                if (item[DT_Risk.ID_GROUPE] != DBNull.Value)
                {
                    cmline.Group = new LineGroup()
                    {
                        IdGroup = (Int32)item[DT_Risk.ID_GROUPE],
                        GroupName = item[DT_Risk.GROUPE_NAME].ToString()
                    };
                }
                else
                {
                    cmline.Group = new LineGroup()
                    {
                        IdGroup = null,
                        GroupName = item[DT_Risk.GROUPE_NAME].ToString()
                    };
                }

                lista.Add(cmline);
            }

            Build_Tree(lista);
            return lista;
        }

        public static int LastCounterMeasurePosition(List<RiskPolyLine> polyLines)
        {
            var lastPolyLine = polyLines.Where(polyLine => polyLine.IsCM).OrderBy(polyLine => polyLine.Position).LastOrDefault();
            return (lastPolyLine == null) ? -1 : lastPolyLine.Position;
        }
    }
}