﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using DataMapping.Data;
using System.Windows;
using EnsureBusinesss.Business;
using System.Windows.Media.Imaging;

namespace EnsureBusinesss
{
    public class TreeOperation
    {
        private const double skewAngle = 22.375;
        private const int hmax = 101;
        private static readonly List<RiskPolyLine> LinesUp = new List<RiskPolyLine>();
        private static readonly List<RiskPolyLine> LinesDown = new List<RiskPolyLine>();
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
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Move&Copy

        /// <summary>
        /// Delete a specific Line with its descendants
        /// </summary>
        public static void DeleteLine(RiskPolyLine LineToDelete, DataSet DsMain)
        {
            try
            {
                List<DataRow> lista = new List<DataRow>
                {
                    DsMain.Tables[DT_Risk.TABLE_NAME].Rows.Find(LineToDelete.ID)
                };
                DeleteRiskAndCMFirst(lista, DsMain);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// The collection list is a descendants of Data to delete in a dataset.
        /// </summary>
        public static void DeleteRiskAndCMFirst(List<DataRow> collection, DataSet ds)
        {
            foreach (DataRow itemi in collection)
            {
                if (FishHeadController.IsLeaf(itemi, ds.Tables[DT_Risk.TABLE_NAME]))
                {
                    DataRow[] drs = ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID + " = " + itemi[DT_Risk.ID]);
                    foreach (DataRow item in drs)
                    {
                        int cantidad = ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.IDRISK_FATHER + " = " + item[DT_Risk.ID]).Count();
                        for (int i = 0; i < cantidad; i++)
                        {
                            ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.IDRISK_FATHER + " = " + item[DT_Risk.ID]).First().Delete();
                        }
                        ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(item[DT_Risk.ID]).Delete();
                    }
                }
                else
                {
                    DeleteRiskAndCMFirst(FishHeadController.GetChildss(itemi, ds.Tables[DT_Risk.TABLE_NAME]), ds);
                    int cantidad = ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.IDRISK_FATHER + " = " + itemi[DT_Risk.ID]).Count();
                    for (int i = 0; i < cantidad; i++)
                    {
                        ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.IDRISK_FATHER + " = " + itemi[DT_Risk.ID]).First().Delete();
                    }
                    ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(itemi[DT_Risk.ID]).Delete();
                }
            }
        }

        #endregion

        /// <summary>
        /// Set the line in a default position in the diagram. Takes how many siblings has in the branch
        /// </summary>
        public static void SetPositionInExcelDiagram(RiskPolyLine line, DataSet Ds)
        {
            int count = 0;
            foreach (var item in line.Children.OrderByDescending(x => x.IsCM))
            {
                Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(item.ID)[DT_Risk.POSITION] = count;

                count++;
                if (item.Children.Count > 0)
                {
                    SetPositionInExcelDiagram(item, Ds);
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

        /// <summary>
        /// Detect the position that user select and give it to the line
        /// </summary>
        public static void LinePositionByClick(RiskPolyLine linea, Point mousePosition, DataSet Ds)
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
            ShiftElement(linea.Father.Children, oldindex, newIdex);
            for (int i = 0; i < linea.Father.Children.Count; i++)
            {
                Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(linea.Father.Children[i].ID)[DT_Risk.POSITION] = i;
            }
        }

        /// <summary>
        /// Add a Tail to all Cms
        /// </summary>
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
        /// Add Tail to all Risks
        /// </summary>
        private static void FixRisk(List<RiskPolyLine> lineas)
        {
            foreach (RiskPolyLine item in lineas)
            {
                if (!item.IsRoot && !(item.Collapsed) && (item.Children.Count > 0))
                {
                    item.AddTail();
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
                    Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(linea.Father.Children[i].ID)[DT_Risk.POSITION] = i;
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

        /// <summary>
        /// Give to Line a new Position
        /// </summary>
        public static void ShiftElement(List<RiskPolyLine> lines, int oldPosition, int newPosition)
        {
            RiskPolyLine[] array = lines.ToArray();
            if (oldPosition == newPosition)
            {
                return;
            }
            RiskPolyLine tmp = array[oldPosition];
            if (newPosition < oldPosition)
            {
                Array.Copy(array, newPosition, array, newPosition + 1, oldPosition - newPosition);
            }
            else
            {
                Array.Copy(array, oldPosition + 1, array, oldPosition, newPosition - oldPosition);
            }
            array[newPosition] = tmp;
            lines.Clear();
            lines.AddRange(array.ToList());
        }

        /// <summary>
        /// Give to Line a new Position in Main Line
        /// </summary>
        public static void ShiftElementMain(List<RiskPolyLine> lines, int oldPosition, int newPosition)
        {

            if (oldPosition == newPosition)
            {
                return;
            }
            RiskPolyLine[] array = new RiskPolyLine[lines.Count];
            RiskPolyLine toMove = lines[oldPosition];
            MoveInList(lines, toMove, newPosition);
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

        /// <summary>
        /// Move an element to new position in the list
        /// </summary>
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
        /// <param name="data">Risk and counter measure list</param>
        public static void Build_Tree(List<RiskPolyLine> data)
        {
            if (data.Count > 0)
            {
                var root = data.FirstOrDefault(p => p.IsRoot);

                data.FirstOrDefault(p => p.IsRoot).Children = GetChildrenNodes(root, data);
                foreach (var item in data.FirstOrDefault(p => p.IsRoot).Children)
                {
                    item.Father = data.FirstOrDefault(p => p.IsRoot);
                }
                // add tree node children recursively
                CalculateDepth(root, 0);
            }
        }

        /// <summary>
        /// Build the hierarquical tree. Asign the child to a father and father to a child giving the root line
        /// </summary>
        public static void Build_Tree(List<RiskPolyLine> data, RiskPolyLine root)
        {
            if (data.Count > 0)
            {
                root.Children = GetChildrenNodes(root, data);
                foreach (var item in root.Children)
                {
                    item.Father = root;
                }
                CalculateDepth(root, 0);
            }
        }

        /// <summary>
        /// Seek the children of the father and viceversa
        /// </summary>
        private static List<RiskPolyLine> GetChildrenNodes(RiskPolyLine padre, List<RiskPolyLine> Lista)
        {
            var nodes = new List<RiskPolyLine>();
            if (!(padre.IsCM))
            {
                foreach (var item in Lista.Where(p => p.IdRiskFather == padre.ID).OrderBy(p => p.Position))
                {

                    item.Children = GetChildrenNodes(item, Lista);

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
        public static List<RiskPolyLine> GetOnlyMyChildrenWithCM(RiskPolyLine fatherLine)
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
                        returnList.AddRange(GetMeAndMyChildrenWithCM(item));
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
        public static List<RiskPolyLine> GetMeAndMyChildrenWithCM(RiskPolyLine lineFather)
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
                        returnList.AddRange(GetMeAndMyChildrenWithCM(item));
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
        /// Update the Risk Value according with the value updated by user.
        /// </summary>
        public static void SetRiskLineValues(RiskPolyLine rl, DataRow RiskRow)
        {
            try
            {
                rl.ShortName = RiskRow[DT_Risk.NAMESHORT].ToString();
                rl.Position = (int)RiskRow[DT_Risk.POSITION];
                rl.Probability = (decimal)RiskRow[DT_Risk.PROBABILITY] / 100;

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

        /// <summary>
        /// Update the CMs Value according with the value updated by user.
        /// </summary>
        public static void SetCMLineValues(RiskPolyLine rl, DataRow CMRow)
        {
            try
            {
                rl.ShortName = CMRow[DT_Risk.NAMESHORT].ToString();
                rl.Position = (int)CMRow[DT_Risk.POSITION];
                rl.Probability = (decimal)CMRow[DT_Risk.PROBABILITY] / 100;

                if (rl.IsCM)
                {
                    rl.Collapsed = false;
                }
                else
                {
                    rl.Collapsed = false;
                }

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
                if (countChild % 2 == 0)
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
        /// Draw RiskPolyLine upper MainLine
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
                        line.BaseFather = line;
                    }
                    else
                    {
                        line.IsDiagonal = !line.Father.IsDiagonal;
                        line.FromTop = line.Father.FromTop;
                        line.BaseFather = line.Father.BaseFather;
                    }
                    if (i == 0)
                    {
                        line.NewDrawAtPoint(new Point(StartPoint.X, StartPoint.Y), line.ShortName);
                    }
                    else
                    {
                        if (line.IsDiagonal)
                        {
                            line.Father.ExtendHorizontal(orderedLines.ElementAt(i - 1).XTremee());
                            line.NewDrawAtPoint(new Point(line.Father.XTreme, line.Father.YxTreme), line.ShortName);
                        }
                        else
                        {
                            line.Father.ExtendVertical(orderedLines.ElementAt(i - 1).AbsoluteYxTremee());
                            line.NewDrawAtPoint(new Point(line.Father.XTreme, line.Father.YxTreme), line.ShortName);
                        }
                    }
                    if (!(line.Collapsed) && line.Children.Count > 0)
                    {
                        DrawDiagramAsFishBone(line.Children, new Point(line.Points[0].X, line.Points[0].Y));
                    }

                    if (line.Father.Father == null)
                    {
                        if (i > 0)
                        {
                            MoveRight(Lines, line, hmax);
                        }
                    }
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
                if (!referencePoint.Terminar)
                {
                    TranslateDirectChildrenTemporal(pLine, referencePoint.Punto);
                    MoveRight(Lines, pLine, referencePoint.Punto.Y);
                }
                else
                {
                    TranslateDirectChildrenTemporal(pLine, referencePoint.Punto);
                }
                //Console.Out.WriteLine(" -- paso por aqui --");
            }
            catch (Exception e)
            {
                throw new Exception("TreeOperation.MoveRight: " + e.Message);
            }
        }

        private static PointToMoveRight GetPointMoveRight(List<RiskPolyLine> Lines, RiskPolyLine pLine, double altura)
        {
            try
            {
                PointToMoveRight result = new PointToMoveRight();
                Point p = GetMaxPointBrothers(Lines, pLine, altura);

                if (p.Y < pLine.YxTremee())
                {
                    result.Punto = p;
                    result.Terminar = false;
                    return result;
                }
                else
                {
                    // revisa como usar "horizontalShiftX" de la clase pline
                    result.Punto = new Point(p.X - RiskPolyLine.horizontalShiftX, p.Y);
                    result.Terminar = true;
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
            Line.Move((int)-XOffSet, 0);

            foreach (var line in Line.Children)
            {
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
                    SkewLines(line.Children, AngleX, line.Points[1].Y);
                }
                //}
            }
        }

        public static void BalanceDiagram(RiskPolyLine Line)
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
        /// Arrange and draw as FishBone diagram risk and counter measure
        /// </summary>
        /// <param name="Line">Risk and counter measure list to draw</param>
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
                BalanceDiagram(Line);
                BalanceDiagram(Line);
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
                double XMax = Line.XTremee();
                Line.ExtendHorizontal(XMax);
                segment = Line.Segments[i];
                segment.StartDrawPoint = new Point(XMax + 5, Line.Points[1].Y);
                segment.EndDrawPoint = new Point(rLine.Points[1].X, Line.Points[1].Y);
            }
        }

        public static void SetDiagramImportedPositions(DataSet ds, decimal idDiagram)
        {
            foreach (DataRow rowRisk in ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_DIAGRAM + " = " + idDiagram + " and " + DT_Risk.IS_CM + " = 0"))
            {
                if ((bool)rowRisk[DT_Risk.IS_ROOT])
                {
                    rowRisk[DT_Risk.POSITION] = 0;
                }
                SetPositionRiskChildren(rowRisk, ds, SetPositionCMChildren(rowRisk, ds));
            }
        }

        public static void SetPositionRiskChildren(DataRow drRiskFather, DataSet ds, int positionOfCM)
        {
            int riskPosition = positionOfCM;
            foreach (var rowRisk in ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.IDRISK_FATHER + " = " + drRiskFather[DT_Risk.ID] + " and " + DT_Risk.IS_CM + " = 0"))
            {
                rowRisk[DT_Risk.POSITION] = riskPosition;
                riskPosition++;
            }
        }

        public static int SetPositionCMChildren(DataRow drRiskFather, DataSet ds)
        {
            int cmposition = 0;
            foreach (var rowCM in ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.IDRISK_FATHER + " = " + drRiskFather[DT_Risk.ID] + " and " + DT_Risk.IS_CM + " = 1"))
            {
                rowCM[DT_Risk.POSITION] = cmposition;
                cmposition++;
            }
            return cmposition;
        }

        /// <summary>
        /// Get the last position of CM in the list. Take in count only CMs
        /// </summary>
        public static int LastCounterMeasurePosition(List<RiskPolyLine> polyLines)
        {
            var lastPolyLine = polyLines.Where(polyLine => polyLine.IsCM).OrderBy(polyLine => polyLine.Position).LastOrDefault();
            return (lastPolyLine == null) ? -1 : lastPolyLine.Position;
            //return polyLines.Where(polyLine => polyLine.IsCM).Count() - 1;
        }
    }
}