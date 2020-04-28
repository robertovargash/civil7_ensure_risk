using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnsureRisk.Classess
{
    public enum MenuRiskItems
    {
        AddRisk=0,
        //EditRisk=1,
        //DelRisk=2,
        AddCM=1,
        Scope=2,
        Copy=3,
        Paste=4,
        Import=5,
        Enable=6
    }
    public enum MenuMain
    {
        AddRisk = 0,        
        Paste = 1,
        Import = 2
    }
    public enum MenuCMm
    {
        Edit = 0,
        Delete = 1,
        Move = 2,
        Enable = 3
    }
    public enum MenuGroupR
    {
        Enable = 0,
        AddCM = 1,
        Group = 2        
    }
    public enum MenuGroupCMm
    {
        Enable = 0,
        Delete = 1,
        Group = 2
    }
    public enum MenuGroupCMR
    {
        Enable = 0,
        Group = 1
    }
}
