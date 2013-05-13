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
        /// Backing filed for Borders.
        /// </summary>
        private List<CountryBorder> countryBorders = new List<CountryBorder>();

        /// <summary>
        /// Parameter less constructor is needed for serializing.
        /// </summary>
        public Map()
        {
        }

        /// <summary>
        /// The <see cref="BorderEndPoint"/> instances of all the <see cref="CountryBorder"/> instances.
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

        /// <summary>
        /// The <see cref="CountryBorder"/> instances.
        /// </summary>
        public List<CountryBorder> CountryBorders
        {
            get
            {
                return this.countryBorders;
            }

            set
            {
                this.countryBorders = value;
            }
        }
    }
}
