using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnsureRisk
{
    public enum EnumOperations
    {
        CanEditUserList = 1,
        CanEditRoleList = 2,
        CanAddDiagram = 3,
        CanUpdateDiagram = 4,
        CanDeleteDiagram = 5,
        DiagRisk_Ops = 6,
        CanEditProjectList = 7,
        CanEditDefltRiskList = 8,
        DiagCM_Ops = 9,
        CanEditDamageList = 10,
        CanCreateWBS = 11,
        CanEditWBS = 12
    }

    public enum EnumExcelValue
    {
        IdRisk = 1,
        RiskShortName = 2,
        RiskComments = 3,
        RiskEnabled = 4,
        IdParentRisk = 5,
        Id_CM = 7,
        CMShortName = 8,
        CM_Comments = 9,
        RiskDamage = 10,
        RiskProbability = 11,
        CM_RiskReduction = 12,
        CM_Damage = 13,
        CM_Status = 14,
        WBSColumn = 15
    }
}
