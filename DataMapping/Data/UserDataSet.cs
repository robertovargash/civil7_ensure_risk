﻿using System.Data;

namespace DataMapping.Data
{
    public class UserDataSet : DataSet
    {
        #region DataRelations
        public const string PROJECT_DIAGRAM_RELATION = "PROJECT_DIAGRAM";
        public const string PROJECT_WBS_RELATION = "PROJECT_WBS";
        public const string RISK_TREE_RELATION = "RISK_TREE";
        public const string RISK_STRUCTURE_RELATION = "RISK_STRUCTURE";
        public const string STRUCTURE_RISK_RELATION = "STRUCTURE_RISK";
        public const string STRUCTURE_RISK_RISK_RELATION = "STRUCTURE_RISK_RISK";
        public const string RISK_GROUPE_RELATION = "RISK_GROUPE";
        public const string RISK_FATHER_RELATION = "RISK_FATHER_STRUCTURE";
        public const string RISK_TREE_TOP_RISK_RELATION = "RISK_TREE_TOP_RISK";
        public const string RISK_TREE_TOP_RISK_RELATION1 = "RISK_TREE_TOP_RISK_DAMAGE";
        public const string RISK_TOPRISK_RELATION = "RISK_TOPRISK";
        public const string RISK_TOPRISK_RELATION1 = "RISK_TOPRISK_DAMAGE";
        public const string ROLE_RISK_RELATION = "ROLE_RISK";
        public const string ROLE_OPERATION_RELATION = "ROLE_OPERATION";
        public const string WBS_FATHER_RELATION = "FATHER";
        //public const string WBS_CHILD_RELATION = "CHILD";
        public const string RISK_WBS_RELATION = "RISK_WBS";
        public const string RISK_WITH_WBS_RISK_DAMAGE = "WBS_RISK_DAMAGE";
        public const string DAMAGE_WITH_WBS_RISK_DAMAGE = "DAMAGE_WBS_RISK_DAMAGE";
        #endregion

        #region TablesAsProperties
        public DataTable Table_Project { get { return Tables[DT_Project.TABLE_NAME]; } }
        public DataTable Table_Group { get { return Tables[DT_Groupe.TABLE_NAME]; } }
        public DataTable Table_WBS { get { return Tables[DT_WBS.TABLE_NAME]; } }
        public DataTable Table_Diagram { get { return Tables[DT_Diagram.TABLE_NAME]; } }
        public DataTable Table_Risk { get { return Tables[DT_Risk.TABLE_NAME]; } }
        public DataTable Table_Riskstructure { get { return Tables[DT_RiskStructure.TABLE_NAME]; } }
        public DataTable Table_RiskDamage { get { return Tables[DT_Risk_Damages.TABLE_NAME]; } }
        public DataTable Table_RiskWBS { get { return Tables[DT_RISK_WBS.TABLE_NAME]; } }
        public DataTable Table_RiskRole { get { return Tables[DT_Role_Risk.TABLE_NAME]; } }
        public DataTable Table_User { get { return Tables[DT_User.TABLE_NAME]; } }
        public DataTable Table_UserRole { get { return Tables[DT_User_Role.TABLE_NAME]; } }
        public DataTable Table_Operation { get { return Tables[DT_Operation.TABLE_NAME]; } }
        public DataTable Table_OperationRole { get { return Tables[DT_OperationRole.TABLE_NAME]; } }
        public DataTable Table_UserOperation { get { return Tables[DT_User_Operation.TABLE_NAME]; } }
        public DataTable Table_Damage { get { return Tables[DT_Damage.TABLE_NAME]; } }
        public DataTable Table_DiagramDamage { get { return Tables[DT_Diagram_Damages.TABLE_NAME]; } }
        public DataTable Table_DefaultRisk { get { return Tables[DT_DefaulRisk.TABLE_NAME]; } }
        public DataTable Table_Clasification { get { return Tables[DT_Clasificator.TABLE_NAME]; } }
        public DataTable Table_WBS_RISK_Damage { get { return Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME]; } }
        #endregion


        public UserDataSet()
        {
            AddTable();
            CrearRelaciones();
            this.EnforceConstraints = false;
        }
        public void AddTable()
        {
            Tables.Add(new DT_Project());
            Tables.Add(new DT_Diagram());            
            Tables.Add(new DT_Risk());
            Tables.Add(new DT_RiskStructure());
            Tables.Add(new DT_Risk_Damages());
            Tables.Add(new DT_RISK_WBS());
            Tables.Add(new DT_WBS_RISK_DAMAGE());
            Tables.Add(new DT_Role_Risk());
            Tables.Add(new DT_Role());
            Tables.Add(new DT_User());
            Tables.Add(new DT_User_Role());
            Tables.Add(new DT_Operation());
            Tables.Add(new DT_OperationRole());
            Tables.Add(new DT_User_Operation());
            Tables.Add(new DT_Damage());
            Tables.Add(new DT_Diagram_Damages());
            Tables.Add(new DT_DefaulRisk());
            Tables.Add(new DT_Clasificator());
            Tables.Add(new DT_Groupe());
            Tables.Add(new DT_WBS());
            //Tables.Add(new DT_WBS_STRUCTURE());
        }

        public void CrearRelaciones()
        {
            _ = this.Relations.Add(PROJECT_DIAGRAM_RELATION, this.Tables[DT_Project.TABLE_NAME].Columns[DT_Project.ID_PROJECT],
                                       this.Tables[DT_Diagram.TABLE_NAME].Columns[DT_Diagram.ID_PROJECT]);

            DataRelation dr1 = Relations[PROJECT_DIAGRAM_RELATION];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            _ = this.Relations.Add(PROJECT_WBS_RELATION, this.Tables[DT_Project.TABLE_NAME].Columns[DT_Project.ID_PROJECT],
                           this.Tables[DT_WBS.TABLE_NAME].Columns[DT_WBS.IDPROJECT]);

            dr1 = Relations[PROJECT_WBS_RELATION];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;


            _ = this.Relations.Add(WBS_FATHER_RELATION, this.Tables[DT_WBS.TABLE_NAME].Columns[DT_WBS.ID_WBS],
                                       this.Tables[DT_WBS.TABLE_NAME].Columns[DT_WBS.ID_FATHER]);

            dr1 = this.Relations[WBS_FATHER_RELATION];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            _ = this.Relations.Add(RISK_TREE_RELATION, this.Tables[DT_Diagram.TABLE_NAME].Columns[DT_Diagram.ID_DIAGRAM],
                                       this.Tables[DT_Risk.TABLE_NAME].Columns[DT_Risk.ID_DIAGRAM]);

            dr1 = this.Relations[RISK_TREE_RELATION];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            _ = this.Relations.Add(RISK_GROUPE_RELATION, this.Tables[DT_Groupe.TABLE_NAME].Columns[DT_Groupe.ID_GROUPE],
                                      this.Tables[DT_Risk.TABLE_NAME].Columns[DT_Risk.ID_GROUPE]);

            dr1 = this.Relations[RISK_GROUPE_RELATION];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            //dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;//lo quito porque si elimino un grupo no tengo por que eliminar un riesgo

            _ = this.Relations.Add(RISK_TREE_TOP_RISK_RELATION, this.Tables[DT_Diagram.TABLE_NAME].Columns[DT_Diagram.ID_DIAGRAM],
                                        this.Tables[DT_Diagram_Damages.TABLE_NAME].Columns[DT_Diagram_Damages.ID_RISKTREE]);

            dr1 = this.Relations[RISK_TREE_TOP_RISK_RELATION];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            _ = this.Relations.Add(RISK_TREE_TOP_RISK_RELATION1, this.Tables[DT_Damage.TABLE_NAME].Columns[DT_Damage.ID_COLUMNA],
                                        this.Tables[DT_Diagram_Damages.TABLE_NAME].Columns[DT_Diagram_Damages.ID_DAMAGE]);

            dr1 = this.Relations[RISK_TREE_TOP_RISK_RELATION1];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            _ = this.Relations.Add(RISK_FATHER_RELATION, this.Tables[DT_Risk.TABLE_NAME].Columns[DT_Risk.ID],
                                        this.Tables[DT_RiskStructure.TABLE_NAME].Columns[DT_RiskStructure.IDRISK]);

            dr1 = this.Relations[RISK_FATHER_RELATION];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            _ = this.Relations.Add(RISK_STRUCTURE_RELATION, this.Tables[DT_Risk.TABLE_NAME].Columns[DT_Risk.ID],
                                        this.Tables[DT_RiskStructure.TABLE_NAME].Columns[DT_RiskStructure.IDRISK_FATHER]);

            dr1 = this.Relations[RISK_STRUCTURE_RELATION];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            _ = this.Relations.Add(RISK_TOPRISK_RELATION, this.Tables[DT_Risk.TABLE_NAME].Columns[DT_Risk.ID],
                                        this.Tables[DT_Risk_Damages.TABLE_NAME].Columns[DT_Risk_Damages.ID_RISK]);

            dr1 = this.Relations[RISK_TOPRISK_RELATION];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            _ = this.Relations.Add(RISK_WBS_RELATION, this.Tables[DT_Risk.TABLE_NAME].Columns[DT_Risk.ID],
                                        this.Tables[DT_RISK_WBS.TABLE_NAME].Columns[DT_RISK_WBS.ID_RISK]);

            dr1 = this.Relations[RISK_WBS_RELATION];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            _ = this.Relations.Add(RISK_WITH_WBS_RISK_DAMAGE, this.Tables[DT_Risk.TABLE_NAME].Columns[DT_Risk.ID],
                                        this.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Columns[DT_WBS_RISK_DAMAGE.ID_RISK]);

            dr1 = this.Relations[RISK_WITH_WBS_RISK_DAMAGE];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            _ = this.Relations.Add(DAMAGE_WITH_WBS_RISK_DAMAGE, this.Tables[DT_Damage.TABLE_NAME].Columns[DT_Damage.ID_COLUMNA],
                                        this.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Columns[DT_WBS_RISK_DAMAGE.ID_DAMAGE]);

            dr1 = this.Relations[DAMAGE_WITH_WBS_RISK_DAMAGE];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            _ = this.Relations.Add(RISK_TOPRISK_RELATION1, this.Tables[DT_Damage.TABLE_NAME].Columns[DT_Damage.ID_COLUMNA],
                                       this.Tables[DT_Risk_Damages.TABLE_NAME].Columns[DT_Risk_Damages.ID_DAMAGE]);

            dr1 = this.Relations[RISK_TOPRISK_RELATION1];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            _ = this.Relations.Add(ROLE_RISK_RELATION, this.Tables[DT_Risk.TABLE_NAME].Columns[DT_Risk.ID],
                                        this.Tables[DT_Role_Risk.TABLE_NAME].Columns[DT_Role_Risk.ID_RISK]);

            dr1 = this.Relations[ROLE_RISK_RELATION];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            _ = this.Relations.Add(ROLE_OPERATION_RELATION, this.Tables[DT_Role.ROLE_TABLE].Columns[DT_Role.IDROL_COLUMN],
                                        this.Tables[DT_OperationRole.TABLE_NAME].Columns[DT_OperationRole.IDROL_COLUMN]);

            dr1 = this.Relations[ROLE_OPERATION_RELATION];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;           
        }
    }
}
