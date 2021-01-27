using System.Collections.Generic;

namespace Diagram
{
    /// <summary>
    /// collection of nodes</summary>
    public class Nodes : List<Node> //UID2727152862
    {
        /*************************************************************************************************************************/
        // CONSTRUCTORS

        public Nodes()
        {
        }

        public Nodes(int capacity) : base(capacity)
        {
        }

        public Nodes(List<Node> collection) : base(collection)
        {
        }

        /*************************************************************************************************************************/
        // SETTERS AND GETTERS

        public void Copy(Nodes nodes)
        {
            this.Clear();

            foreach (Node node in nodes)
            {
                this.Add(node.Clone());
            }

        }

        public void Set(Nodes nodes)
        {
            this.Clear();

            foreach (Node node in nodes)
            {
                this.Add(node);
            }

        }

        /*************************************************************************************************************************/
        // SORT

        public void OrderByIdAsc()
        {
            this.Sort((x, y) => x.id.CompareTo(y.id));
        }

        public void OrderByNameAsc()
        {
            this.Sort((x, y) => string.Compare(x.name, y.name));
        }

        public void OrderByNameDesc()
        {
            this.Sort((x, y) => string.Compare(y.name, x.name));
        }

        public void OrderByLink()
        {
            this.Sort((x, y) => string.Compare(x.link, y.link));
        }

        public void OrderByPositionY()
        {
            this.Sort((a, b) => a.position.y.CompareTo(b.position.y));
        }

        public void OrderByPositionX()
        {
            this.Sort((a, b) => a.position.x.CompareTo(b.position.x));
        }
        
        public Node Find(long id)
        {
            return this.Find(n => n.id == id);
        }
        
    }
}
