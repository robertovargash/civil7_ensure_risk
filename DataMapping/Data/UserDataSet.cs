using System.Data;

namespace DataMapping.Data
{
    public class UserDataSet : DataSet
    {
        public const string PROJECT_DIAGRAM_RELATION = "PROJECT_DIAGRAM";
        public const string RISK_TREE_RELATION = "RISK_TREE";
        public const string RISK_STRUCTURE_RELATION = "RISK_STRUCTURE";
        public const string STRUCTURE_RISK_RELATION = "STRUCTURE_RISK";
        public const string RISK_GROUPE_RELATION = "RISK_GROUPE";
        public const string RISK_FATHER_RELATION = "RISK_FATHER_STRUCTURE";
        public const string RISK_TREE_COUNTERM_RELATION = "RISK_TREE_COUNTERM";
        public const string RISK_TREE_TOP_RISK_RELATION = "RISK_TREE_TOP_RISK";
        public const string RISK_TREE_TOP_RISK_RELATION1 = "RISK_TREE_TOP_RISK_DAMAGE";
        public const string RISK_TOPRISK_RELATION = "RISK_TOPRISK";
        public const string RISK_TOPRISK_RELATION1 = "RISK_TOPRISK_DAMAGE";
        public const string COUNTERM_TOPRISK_RELATION = "COUNTERM_TOP_RISK";
        public const string COUNTERM_GROUPE_RELATION = "COUNTERM_GROUPE";
        public const string COUNTERM_TOPRISK_RELATION1 = "COUNTERM_DAMAGE";
        public const string COUNTERM_ROLE_RELATION = "COUNTERM_ROLE";
        public const string RISK_COUNTERMEASURE_RELATION = "RISK_COUNTERMEASURE";
        public const string ROLE_RISK_RELATION = "ROLE_RISK";
        public const string ROLE_OPERATION_RELATION = "ROLE_OPERATION";
        public const string WBS_FATHER_RELATION = "FATHER";
        public const string WBS_CHILD_RELATION = "CHILD";
        public const string RISK_WBS_RELATION = "RISK_WBS";
        public const string CM_WBS_RELATION = "CM_WBS";

        public UserDataSet()
        {
            AddTable();
            CrearRelaciones();
            this.EnforceConstraints = false;
        }
        public void AddTable()
        {
            Tables.Add(new DT_Project());
            Tables.Add(new DT_Groupe());
            Tables.Add(new DT_WBS());
            Tables.Add(new DT_WBS_STRUCTURE());
            Tables.Add(new DT_RiskTree());            
            Tables.Add(new DT_Risk());
            Tables.Add(new DT_RiskStructure());
            Tables.Add(new DT_Risk_Damages());
            Tables.Add(new DT_RISK_WBS());
            Tables.Add(new DT_Role_Risk());
            Tables.Add(new DT_Role());
            Tables.Add(new DT_User());
            Tables.Add(new DT_User_Role());
            Tables.Add(new DT_Operation());
            Tables.Add(new DT_OperationRole());
            Tables.Add(new DT_User_Operation());
            Tables.Add(new DT_Damage());
            Tables.Add(new DT_RiskTree_Damages());
            Tables.Add(new DT_CounterM());
            Tables.Add(new DT_CounterM_Damage());
            Tables.Add(new DT_CM_WBS());
            Tables.Add(new DT_Role_CM());
            Tables.Add(new DT_DefaulRisk());
            Tables.Add(new DT_Clasificator());
        }

        public void CrearRelaciones()
        {

            //DataRelation dr1 = this.Relations.Add(PROJECT_DIAGRAM_RELATION, this.Tables[DT_Project.TABLE_NAME].Columns[DT_Project.ID_PROJECT],
            //                           this.Tables[DT_RiskTree.TABLE_NAME].Columns[DT_RiskTree.ID_WBS]);

            //dr1 = this.Relations[PROJECT_DIAGRAM_RELATION];
            //dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            //dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            DataRelation dr1 = this.Relations.Add(WBS_FATHER_RELATION, this.Tables[DT_WBS.TABLE_NAME].Columns[DT_WBS.ID_WBS],
                                       this.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Columns[DT_WBS_STRUCTURE.ID_FATHER]);

            dr1 = this.Relations[WBS_FATHER_RELATION];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            dr1 = this.Relations.Add(WBS_CHILD_RELATION, this.Tables[DT_WBS.TABLE_NAME].Columns[DT_WBS.ID_WBS],
                                       this.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Columns[DT_WBS_STRUCTURE.ID_CHILD]);

            dr1 = this.Relations[WBS_CHILD_RELATION];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            dr1 = this.Relations.Add(RISK_TREE_RELATION, this.Tables[DT_RiskTree.TABLE_NAME].Columns[DT_RiskTree.ID_RISK_TREE],
                                       this.Tables[DT_Risk.TABLE_NAME].Columns[DT_Risk.ID_RISK_TREE]);

            dr1 = this.Relations[RISK_TREE_RELATION];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            dr1 = this.Relations.Add(RISK_GROUPE_RELATION, this.Tables[DT_Groupe.TABLE_NAME].Columns[DT_Groupe.ID_GROUPE],
                                      this.Tables[DT_Risk.TABLE_NAME].Columns[DT_Risk.ID_GROUPE_COLUMN]);

            dr1 = this.Relations[RISK_GROUPE_RELATION];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            //dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;//lo quito porque si elimino un grupo no tengo por que eliminar un riesgo

            dr1 = this.Relations.Add(RISK_TREE_COUNTERM_RELATION, this.Tables[DT_RiskTree.TABLE_NAME].Columns[DT_RiskTree.ID_RISK_TREE],
                                       this.Tables[DT_CounterM.TABLE_NAME].Columns[DT_CounterM.ID_RISK_TREE]);

            dr1 = this.Relations[RISK_TREE_COUNTERM_RELATION];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            dr1 = this.Relations.Add(RISK_TREE_TOP_RISK_RELATION, this.Tables[DT_RiskTree.TABLE_NAME].Columns[DT_RiskTree.ID_RISK_TREE],
                                        this.Tables[DT_RiskTree_Damages.TABLENAME].Columns[DT_RiskTree_Damages.ID_RISKTREE]);

            dr1 = this.Relations[RISK_TREE_TOP_RISK_RELATION];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            dr1 = this.Relations.Add(RISK_TREE_TOP_RISK_RELATION1, this.Tables[DT_Damage.TopRisk_TABLA].Columns[DT_Damage.ID_COLUMNA],
                                        this.Tables[DT_RiskTree_Damages.TABLENAME].Columns[DT_RiskTree_Damages.ID_DAMAGE]);

            dr1 = this.Relations[RISK_TREE_TOP_RISK_RELATION1];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            dr1 = this.Relations.Add(RISK_FATHER_RELATION, this.Tables[DT_Risk.TABLE_NAME].Columns[DT_Risk.ID_COLUMNA],
                                        this.Tables[DT_RiskStructure.TABLE_NAME].Columns[DT_RiskStructure.IDRISK]);

            dr1 = this.Relations[RISK_FATHER_RELATION];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            dr1 = this.Relations.Add(RISK_STRUCTURE_RELATION, this.Tables[DT_Risk.TABLE_NAME].Columns[DT_Risk.ID_COLUMNA],
                                        this.Tables[DT_RiskStructure.TABLE_NAME].Columns[DT_RiskStructure.IDRISK_FATHER]);

            dr1 = this.Relations[RISK_STRUCTURE_RELATION];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            //dr1 = this.Relations.Add(STRUCTURE_RISK_RELATION, this.Tables[DT_RiskStructure.TABLE_NAME].Columns[DT_RiskStructure.IDRISK_FATHER],
            //                            this.Tables[DT_Risk.TABLE_NAME].Columns[DT_Risk.IDRISK_FATHER]);

            //dr1 = this.Relations[STRUCTURE_RISK_RELATION];
            //dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            //dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            dr1 = this.Relations.Add(RISK_TOPRISK_RELATION, this.Tables[DT_Risk.TABLE_NAME].Columns[DT_Risk.ID_COLUMNA],
                                        this.Tables[DT_Risk_Damages.TABLENAME].Columns[DT_Risk_Damages.ID_RISK]);

            dr1 = this.Relations[RISK_TOPRISK_RELATION];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            dr1 = this.Relations.Add(RISK_WBS_RELATION, this.Tables[DT_Risk.TABLE_NAME].Columns[DT_Risk.ID_COLUMNA],
                                        this.Tables[DT_RISK_WBS.TABLENAME].Columns[DT_RISK_WBS.ID_RISK]);

            dr1 = this.Relations[RISK_WBS_RELATION];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            dr1 = this.Relations.Add(RISK_TOPRISK_RELATION1, this.Tables[DT_Damage.TopRisk_TABLA].Columns[DT_Damage.ID_COLUMNA],
                                       this.Tables[DT_Risk_Damages.TABLENAME].Columns[DT_Risk_Damages.ID_DAMAGE]);

            dr1 = this.Relations[RISK_TOPRISK_RELATION1];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            dr1 = this.Relations.Add(ROLE_RISK_RELATION, this.Tables[DT_Risk.TABLE_NAME].Columns[DT_Risk.ID_COLUMNA],
                                        this.Tables[DT_Role_Risk.TABLENAME].Columns[DT_Role_Risk.ID_RISK]);

            dr1 = this.Relations[ROLE_RISK_RELATION];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            dr1 = this.Relations.Add(RISK_COUNTERMEASURE_RELATION, this.Tables[DT_Risk.TABLE_NAME].Columns[DT_Risk.ID_COLUMNA],
                                        this.Tables[DT_CounterM.TABLE_NAME].Columns[DT_CounterM.ID_RISK]);

            dr1 = this.Relations[RISK_COUNTERMEASURE_RELATION];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            dr1 = this.Relations.Add(COUNTERM_GROUPE_RELATION, this.Tables[DT_Groupe.TABLE_NAME].Columns[DT_Groupe.ID_GROUPE],
                                        this.Tables[DT_CounterM.TABLE_NAME].Columns[DT_CounterM.ID_GROUPE_COLUMN]);

            dr1 = this.Relations[COUNTERM_GROUPE_RELATION];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            //dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            dr1 = this.Relations.Add(COUNTERM_TOPRISK_RELATION, this.Tables[DT_CounterM.TABLE_NAME].Columns[DT_CounterM.ID_COLUMNA],
                                        this.Tables[DT_CounterM_Damage.TABLENAME].Columns[DT_CounterM_Damage.ID_COUNTERM]);

            dr1 = this.Relations[COUNTERM_TOPRISK_RELATION];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            dr1 = this.Relations.Add(CM_WBS_RELATION, this.Tables[DT_CounterM.TABLE_NAME].Columns[DT_CounterM.ID_COLUMNA],
                                       this.Tables[DT_CM_WBS.TABLENAME].Columns[DT_CM_WBS.ID_CM]);

            dr1 = this.Relations[CM_WBS_RELATION];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            dr1 = this.Relations.Add(COUNTERM_TOPRISK_RELATION1, this.Tables[DT_Damage.TopRisk_TABLA].Columns[DT_Damage.ID_COLUMNA],
                                        this.Tables[DT_CounterM_Damage.TABLENAME].Columns[DT_CounterM_Damage.ID_DAMAGE]);

            dr1 = this.Relations[COUNTERM_TOPRISK_RELATION1];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            dr1 = this.Relations.Add(COUNTERM_ROLE_RELATION, this.Tables[DT_CounterM.TABLE_NAME].Columns[DT_CounterM.ID_COLUMNA],
                                        this.Tables[DT_Role_CM.TABLENAME].Columns[DT_Role_CM.ID_CM]);

            dr1 = this.Relations[COUNTERM_ROLE_RELATION];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;

            dr1 = this.Relations.Add(ROLE_OPERATION_RELATION, this.Tables[DT_Role.ROLE_TABLE].Columns[DT_Role.IDROL_COLUMN],
                                        this.Tables[DT_OperationRole.OPERATION_ROLE_TABLE].Columns[DT_OperationRole.IDROL_COLUMN]);

            dr1 = this.Relations[ROLE_OPERATION_RELATION];
            dr1.ChildKeyConstraint.UpdateRule = Rule.Cascade;
            dr1.ChildKeyConstraint.DeleteRule = Rule.Cascade;
        }
    }
}
