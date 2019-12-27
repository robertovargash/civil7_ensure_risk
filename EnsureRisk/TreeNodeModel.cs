using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EnsureRisk
{
    public class TreeNodeModel<T>
    where T : class
    {
        public double X { get; set; }
        public int Y { get; set; }
        public double Mod { get; set; }
        public TreeNodeModel<T> Father { get; set; }
        public List<TreeNodeModel<T>> Children { get; set; }
        public double Width { get; set; }
        public int Height { get; set; }
        public T Item { get; set; }

        public TreeNodeModel(T item, TreeNodeModel<T> father)
        {
            this.Item = item;
            this.Father = father;
            this.Children = new List<TreeNodeModel<T>>();
        }

        public bool Is_Leaf()
        {
            return this.Children.Count == 0;
        }

        public bool FirstInserted()
        {
            if (this.Father == null)
                return true;

            return this.Father.Children[0] == this;
        }

        public bool UltimoInsertado()
        {
            if (this.Father == null)
                return true;

            return this.Father.Children[this.Father.Children.Count - 1] == this;
        }

        public TreeNodeModel<T> GetHermanoInsertadoAntesQueYo()
        {
            if (this.Father == null || this.FirstInserted())
                return null;

            return this.Father.Children[this.Father.Children.IndexOf(this) - 1];
        }

        public TreeNodeModel<T> GetHermanoInsertadoDespuesDeMi()
        {
            if (this.Father == null || this.UltimoInsertado())
                return null;

            return this.Father.Children[this.Father.Children.IndexOf(this) + 1];
        }

        public TreeNodeModel<T> GetHermanoPrimeroInsertado()
        {
            if (this.Father == null)
                return null;

            if (this.FirstInserted())
                return this;

            return this.Father.Children[0];
        }

        public TreeNodeModel<T> GetPrimerHijoInsertado()
        {
            if (this.Children.Count == 0)
                return null;

            return this.Children[0];
        }

        public TreeNodeModel<T> GetUltimoHijoInsertado()
        {
            if (this.Children.Count == 0)
                return null;

            return this.Children[Children.Count - 1];
        }

        public override string ToString()
        {
            return Item.ToString();
        }
    }

}
