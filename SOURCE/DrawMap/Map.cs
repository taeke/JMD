namespace DrawMap
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The geomerty of a map.
    /// </summary>
    public class Map
    {
        /// <summary>
        /// Backing filed for BorderEndpoints.
        /// </summary>
        private List<BorderEndPoint> borderEndPoints = new List<BorderEndPoint>();

        /// <summary>
        /// parameter less constructor is needed for serializing.
        /// </summary>
        public Map()
        {
        }

        /// <summary>
        /// The endpoints of all the borders.
        /// </summary>
        public List<BorderEndPoint> BorderEndpoints
        {
            get 
            { 
                return this.borderEndPoints; 
            }

            set
            {
                this.borderEndPoints = value;
            }
        }
    }
}
