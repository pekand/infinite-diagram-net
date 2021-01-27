namespace Diagram
{
    /// <summary>
    /// diagram layer</summary>
    public class Layer //UID1099250588
    {
        public long id = 0; // parent node id, layer owner

        /*************************************************************************************************************************/
        // PARENTS

        public Node parentNode = null;
        public Layer parentLayer = null; // up layer in layer hierarchy

        /*************************************************************************************************************************/
        // LAYER ITEMS

        public Nodes nodes = new Nodes();               // all layer nodes
        public Lines lines = new Lines();               // all layer lines

        /*************************************************************************************************************************/
        // CONSTRUCTORS

        /// <summary>
        /// LAYER construct
        /// </summary>
        public Layer(Nodes nodes = null, Lines lines = null)
        {
            this.nodes = nodes;
            this.lines = lines;
        }

        /// <summary>
        /// LAYER construct  
        /// </summary>
        /// <param name="parentNode">is node in upper layer</param>   
        /// <param name="parentLayer">is layer whitch has parentNode</param> 
        public Layer(Node parentNode = null, Layer parentLayer = null)
        {
            if (parentNode != null)
            {
                this.id = parentNode.id;
                this.parentNode = parentNode;
            }

            this.parentLayer = parentLayer;
        }
    }
}
