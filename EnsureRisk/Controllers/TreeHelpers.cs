﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnsureRisk
{
    public static class TreeHelpers<T>
    where T : class
    {
        private static int nodeSize = 1;
        private static double siblingDistance = 0.0F;
        private static double treeDistance = 0.0F;

        public static void CalculateNodePositions(TreeNodeModel<T> rootNode)
        {
            // initialize node x, y, and mod values
            InitializeNodes(rootNode, 0);

            // assign initial X and Mod values for nodes
            CalculateInitialX(rootNode);

            // ensure no node is being drawn off screen
            CheckAllChildrenOnScreen(rootNode);

            // assign final X values to nodes
            CalculateFinalPositions(rootNode, 0);
        }

        public static void CalculateNodePositionsV2(TreeNodeModel<T> rootNode)
        {
            // initialize node x, y, and mod values
            InitializeNodes(rootNode, 0);

            // assign initial X and Mod values for nodes
            CalculateInitialX(rootNode);

            // ensure no node is being drawn off screen
            CheckAllChildrenOnScreen(rootNode);

            // assign final X values to nodes
            CalculateFinalPositions(rootNode, 0);
        }

        // recusrively initialize x, y, and mod values of nodes
        private static void InitializeNodes(TreeNodeModel<T> node, int depth)
        {
            node.X = -1;
            node.Y = depth;
            node.Mod = 0;

            foreach (var child in node.Children)
                InitializeNodes(child, depth + 1);
        }

        private static void CalculateFinalPositions(TreeNodeModel<T> node, double modSum)
        {
            node.X += modSum;
            modSum += node.Mod;

            foreach (var child in node.Children)
                CalculateFinalPositions(child, modSum);

            if (node.Children.Count == 0)
            {
                node.Width = node.X;
                node.Height = node.Y;
            }
            else
            {
                node.Width = node.Children.OrderByDescending(p => p.Width).First().Width;
                node.Height = node.Children.OrderByDescending(p => p.Height).First().Height;
            }
        }

        private static void CalculateInitialX(TreeNodeModel<T> node)
        {
            foreach (var child in node.Children)
                CalculateInitialX(child);

            // if no children
            if (node.Is_Leaf())
            {
                // if there is a previous sibling in this set, set X to prevous sibling + designated distance
                if (!node.FirstInserted())
                    node.X = node.GetHermanoInsertadoAntesQueYo().X + nodeSize + siblingDistance;
                else
                    // if this is the first node in a set, set X to 0
                    node.X = 0;
            }
            // if there is only one child
            else if (node.Children.Count == 1)
            {
                // if this is the first node in a set, set it's X value equal to it's child's X value
                if (node.FirstInserted())
                {
                    node.X = node.Children[0].X;
                }
                else
                {
                    node.X = node.GetHermanoInsertadoAntesQueYo().X + nodeSize + siblingDistance;
                    node.Mod = node.X - node.Children[0].X;
                }
            }
            else
            {
                var leftChild = node.GetPrimerHijoInsertado();
                var rightChild = node.GetUltimoHijoInsertado();
                var mid = (leftChild.X + rightChild.X) / 2;

                if (node.FirstInserted())
                {
                    node.X = mid;
                }
                else
                {
                    node.X = node.GetHermanoInsertadoAntesQueYo().X + nodeSize + siblingDistance;
                    node.Mod = node.X - mid;
                }
            }

            if (node.Children.Count > 0 && !node.FirstInserted())
            {
                // Since subtrees can overlap, check for conflicts and shift tree right if needed
                CheckForConflicts(node);
            }
        }

        private static void CheckForConflicts(TreeNodeModel<T> node)
        {
            var minDistance = treeDistance + nodeSize;
            double shiftValue = 0;

            var nodeContour = new Dictionary<int, double>();
            GetLeftContour(node, 0, ref nodeContour);

            var sibling = node.GetHermanoPrimeroInsertado();
            while (sibling != null && sibling != node)
            {
                var siblingContour = new Dictionary<int, double>();
                GetRightContour(sibling, 0, ref siblingContour);

                for (int level = node.Y + 1; level <= Math.Min(siblingContour.Keys.Max(), nodeContour.Keys.Max()); level++)
                {
                    var distance = nodeContour[level] - siblingContour[level];
                    if (distance + shiftValue < minDistance)
                    {
                        shiftValue = minDistance - distance;
                    }
                }

                if (shiftValue > 0)
                {
                    node.X += shiftValue;
                    node.Mod += shiftValue;

                    CenterNodesBetween(node, sibling);

                    shiftValue = 0;
                }

                sibling = sibling.GetHermanoInsertadoDespuesDeMi();
            }
        }

        private static void CenterNodesBetween(TreeNodeModel<T> leftNode, TreeNodeModel<T> rightNode)
        {
            var leftIndex = leftNode.Father.Children.IndexOf(rightNode);
            var rightIndex = leftNode.Father.Children.IndexOf(leftNode);

            var numNodesBetween = (rightIndex - leftIndex) - 1;

            if (numNodesBetween > 0)
            {
                var distanceBetweenNodes = (leftNode.X - rightNode.X) / (numNodesBetween + 1);

                int count = 1;
                for (int i = leftIndex + 1; i < rightIndex; i++)
                {
                    var middleNode = leftNode.Father.Children[i];

                    var desiredX = rightNode.X + (distanceBetweenNodes * count);
                    var offset = desiredX - middleNode.X;
                    middleNode.X += offset;
                    middleNode.Mod += offset;

                    count++;
                }

                CheckForConflicts(leftNode);
            }
        }

        private static void CheckAllChildrenOnScreen(TreeNodeModel<T> node)
        {
            var nodeContour = new Dictionary<int, double>();
            GetLeftContour(node, 0, ref nodeContour);

            double shiftAmount = 0;
            foreach (var y in nodeContour.Keys)
            {
                if (nodeContour[y] + shiftAmount < 0)
                    shiftAmount = (nodeContour[y] * -1);
            }

            if (shiftAmount > 0)
            {
                node.X += shiftAmount;
                node.Mod += shiftAmount;
            }
        }

        private static void GetLeftContour(TreeNodeModel<T> node, double modSum, ref Dictionary<int, double> values)
        {
            if (!values.ContainsKey(node.Y))
                values.Add(node.Y, node.X + modSum);
            else
                values[node.Y] = Math.Min(values[node.Y], node.X + modSum);

            modSum += node.Mod;
            foreach (var child in node.Children)
            {
                GetLeftContour(child, modSum, ref values);
            }
        }

        private static void GetRightContour(TreeNodeModel<T> node, double modSum, ref Dictionary<int, double> values)
        {
            if (!values.ContainsKey(node.Y))
                values.Add(node.Y, node.X + modSum);
            else
                values[node.Y] = Math.Max(values[node.Y], node.X + modSum);

            modSum += node.Mod;
            foreach (var child in node.Children)
            {
                GetRightContour(child, modSum, ref values);
            }
        }
    }

}
