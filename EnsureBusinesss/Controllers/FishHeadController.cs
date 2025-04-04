﻿using DataMapping.Data;
using EnsureBusinesss.Business;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnsureBusinesss
{
    public class FishHeadController
    {
        public static List<DataRow> GetChildss(DataRow drFather, DataTable dtRisk)
        {
            List<DataRow> returnList = new List<DataRow>();

            foreach (DataRow item in dtRisk.Select(DT_Risk.IDRISK_FATHER + " = " + drFather[DT_Risk.ID].ToString()))
            {
                returnList.Add(item);
            }
            return returnList;
        }

        public static bool IsLeaf(DataRow drChild, DataTable dtRiskStructure)
        {
            return !dtRiskStructure.Select(DT_Risk.IDRISK_FATHER + " = " + drChild[DT_Risk.ID]).Any();
        }

        /// <summary>
        /// Calculate and returns the value of the "Damage" of the "risk"
        /// </summary>
        private static decimal CalcSelectedRiskDamageValue(DataRow risk, DataTable Risk_Damage, decimal idDamage)
        {
            try
            {
                if ((bool)risk[DT_Risk.IS_ACTIVE])
                {
                    if (!(Risk_Damage.Select(DT_Risk_Damages.ID_RISK + " = " + risk[DT_Risk.ID] + " AND " +
                                DT_Risk_Damages.ID_DAMAGE + " = " + idDamage).Any()))
                    {
                        return 0;
                    }
                    else
                    {
                        return (decimal)Risk_Damage.Select(DT_Risk_Damages.ID_RISK + " = " + risk[DT_Risk.ID] + " AND " +
                            DT_Risk_Damages.ID_DAMAGE + " = " + idDamage).First()[DT_Risk_Damages.VALUE];
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
        /// Calculate and returns the value of the "Damage (TopRisk)" of the Counter measures 
        /// </summary>
        private static decimal CalcCM_DamageValue(DataRow CM, DataTable CM_Damage, decimal idDamage)
        {
            if (!(CM_Damage.Select(DT_Risk_Damages.ID_RISK + " = " + CM[DT_Risk.ID] + " AND " +
                DT_Risk_Damages.ID_DAMAGE + " = " + idDamage).Any()))
            {
                return 0;
            }
            else
            {
                return (decimal)CM_Damage.Select(DT_Risk_Damages.ID_RISK + " = " + CM[DT_Risk.ID] + " AND " +
                    DT_Risk_Damages.ID_DAMAGE + " = " + idDamage).First()[DT_Risk_Damages.VALUE];
            }
        }

        /// <summary>
        /// Return an Array with the CM of the Risk
        /// </summary>
        private static DataRow[] GetRiskCMs(DataRow risk, DataTable CM)
        {
            return CM.Select(DT_Risk.IDRISK_FATHER + " = " + risk[DT_Risk.ID] + " and " + DT_Risk.IS_ACTIVE + " = " + true);
        }

        /// <summary>
        /// Calculate and returns the value of the Damage (TopRisk) of the Risk
        /// </summary>
        public static decimal CalcDiagramDamageValue(DataRow drRoot, DataTable dtRisk, decimal idDamage, DataTable dtRisk_Damage)
        {
            //THIS FUNCTION CALCULATES THE VALUE OF A TOPRISK, 
            //THE CODING AND STRUCTURE TABLES, HANDLE THE RISK AND ITS POSITION WITHIN THE TREE TO CALCULATE THE VALUE OF ITS CHILDREN.
            decimal riskValue = CalcSelectedRiskDamageValue(drRoot, dtRisk_Damage, idDamage);//HERE IS CALCULATED THE VALUE OF THE FATHER ONLY 
            decimal cmValue = 0;

            //HERE WE SELECT ALL THE COUNTERMEASURE OF THE RISK AND SUM ALL HIS VALUES AND RESTAMOS
            foreach (DataRow item in GetRiskCMs(drRoot, dtRisk))
            {
                cmValue += CalcCM_DamageValue(item, dtRisk_Damage, idDamage);
            }
            riskValue += cmValue;//ORIGINALMENTE ERA MENOS, PERO POR LO QUE DIJO LUCAS CAMBIE A +
            //NOW, FOR EACH CHILDS OF 'drRoot' WE EVALUATE IF HAS CHILDREN
            foreach (DataRow item in GetChildss(drRoot, dtRisk)) //FOR EACH CHILDS OF 'drRoot'
            {
                if (IsLeaf(item, dtRisk))//HERE IS EVALUATED IF drRoot CHILDS HAVE CHILDREN
                {
                    //IF NOT HAVE CHILDREN AS THE VALUE IS ADDED TO THE VALUE OF THE FATHER
                    riskValue += CalcSelectedRiskDamageValue(item, dtRisk_Damage, idDamage);
                    foreach (DataRow item2 in GetRiskCMs(item, dtRisk))
                    {
                        cmValue = CalcCM_DamageValue(item2, dtRisk_Damage, idDamage);
                        riskValue += cmValue;//ORIGINALMENTE ERA MENOS, PERO POR LO QUE DIJO LUCAS CAMBIE A +
                    }
                }
                else
                {
                    //ELSE FOR EACH CHILDS EXECUTE THE FUNCTION 
                    if ((bool)item[DT_Risk.IS_ACTIVE])
                    {
                        riskValue += CalcDiagramDamageValue(item, dtRisk, idDamage, dtRisk_Damage);
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
            decimal AcumulatedLikelihood;//This will be the value to return
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
                            Probability_List.Add(FishHeadController.AcumulatedLikelihood(item));//else, call the function as recursive
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
                AcumulatedLikelihood = (LineFather.IsActivated ? LineFather.Probability : 1) * EL_Inclusion_Exclusion(Probability_List);
                foreach (var item in CM_Probabilities)
                {
                    AcumulatedLikelihood *= (1M - item);//adding to the return value the Risk Reduction Formula for each CounterMeasure
                }
            }
            else
            {
                AcumulatedLikelihood = LineFather.IsActivated ? LineFather.Probability : 1;//If don´t have child, Acum. Likelihood = its Probability
                foreach (var item in CM_Probabilities)
                {
                    AcumulatedLikelihood *= (1M - item);//adding to the return value the Risk Reduction Formula for each CounterMeasure
                }
            }
            if (AcumulatedLikelihood > 1)
            {
                return 1;
            }
            else
            {
                return AcumulatedLikelihood;
            }
        }

        public static decimal AcumulatedLikelihood(RiskPolyLine LineFather, List<RiskPolyLine> Children)
        {
            decimal AcumulatedLikelihood;//This will be the value to return
            bool hasChildren = false;//the flag ill be activated if the risk has children,
            List<decimal> Probability_List = new List<decimal>();
            List<decimal> CM_Probabilities = new List<decimal>();
            foreach (var item in Children)
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
                            Probability_List.Add(FishHeadController.AcumulatedLikelihood(item,item.Children));//else, call the function as recursive
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
                AcumulatedLikelihood = (LineFather.IsActivated ? LineFather.Probability : 1) * EL_Inclusion_Exclusion(Probability_List);
                foreach (var item in CM_Probabilities)
                {
                    AcumulatedLikelihood *= (1M - item);//adding to the return value the Risk Reduction Formula for each CounterMeasure
                }
            }
            else
            {
                AcumulatedLikelihood = LineFather.IsActivated ? LineFather.Probability : 1;//If don´t have child, Acum. Likelihood = its Probability
                foreach (var item in CM_Probabilities)
                {
                    AcumulatedLikelihood *= (1M - item);//adding to the return value the Risk Reduction Formula for each CounterMeasure
                }
            }
            if (AcumulatedLikelihood > 1)
            {
                return 1;
            }
            else
            {
                return AcumulatedLikelihood;
            }
        }
        
        public static decimal SectAcmLike(RiskPolyLine line)
        {
            decimal AcumulatedLikelihood;//This will be the value to return
            bool hasChildren = false;//the flag ill be activated if the risk has children,
            List<decimal> Probability_List = new List<decimal>();
            List<decimal> CM_Probabilities = new List<decimal>();
            List<RiskPolyLine> hermanos = line.Father.Children.Where(l => l.Points[1].X <= line.Points[1].X).ToList();
            foreach (var sibling in hermanos.OrderBy(l => l.Points[1].X))
            {
                if (sibling.IsCM)
                {
                    if (sibling.IsActivated)
                    {
                        CM_Probabilities.Add(sibling.Probability);
                    }
                }
                else
                {
                    hasChildren = true;
                    if (sibling.IsLeaf())
                    {
                        if (!(sibling.IsActivated))
                        {
                            Probability_List.Add(1);
                        }
                        else
                        {
                            Probability_List.Add(sibling.Probability);//if don´t have child, Acum. Likelihhod = its Probability
                        }
                    }
                    else
                    {
                        if (sibling.IsActivated)
                        {
                            Probability_List.Add(SectAcmLike(sibling));//else, call the function as recursive
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
                AcumulatedLikelihood = (line.Father.IsActivated ? line.Father.Probability : 1) * EL_Inclusion_Exclusion(Probability_List);
                foreach (var item in CM_Probabilities)
                {
                    AcumulatedLikelihood *= (1M - item);//adding to the return value the Risk Reduction Formula for each CounterMeasure
                }
            }
            else
            {
                AcumulatedLikelihood = line.Father.IsActivated ? line.Father.Probability : 1;//If don´t have child, Acum. Likelihood = its Probability
                foreach (var item in CM_Probabilities)
                {
                    AcumulatedLikelihood *= (1M - item);//adding to the return value the Risk Reduction Formula for each CounterMeasure
                }
            }
            if (AcumulatedLikelihood > 1)
            {
                return 1;
            }
            else
            {
                return AcumulatedLikelihood;
            }
        }


        /// <summary>
        /// Calculating Inclusion_Exclusion likelihood 
        /// </summary>
        public static decimal EL_Inclusion_Exclusion(List<decimal> probabilities)
        {
            if (probabilities.Count > 1)
            {
                decimal temp = 0;
                for (int i = 0; i < probabilities.Count - 1; i++)
                {
                    temp = ProbabilityOr(probabilities[i], probabilities[i + 1]);
                    probabilities[i + 1] = temp;
                }
                return temp;
            }
            else
            {
                return probabilities[0];
            }
        }

        /// <summary>
        /// Calculating the probability of A U B
        /// </summary>
        public static decimal ProbabilityOr(decimal pA, decimal pB)
        {
            return (pA + pB) - (pA * pB);
        }
    }
}
