namespace DrawMap
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The geomerty of a map.
    /// </summary>
    [Serializable]
    public class Map
    {
        /// <summary>
        /// Backing filed for BorderEndpoints.
        /// </summary>
        private List<BorderPoint> borderPoints = new List<BorderPoint>();

        /// <summary>
        /// Backing filed for Borders.
        /// </summary>
        private List<CountryBorder> countryBorders = new List<CountryBorder>();

        /// <summary>
        /// Backing field for Countries.
        /// </summary>
        private List<Country> countries = new List<Country>();

        /// <summary>
        /// Parameter less constructor is needed for serializing.
        /// </summary>
        public Map()
        {
        }

        /// <summary>
        /// The <see cref="BorderPoint"/> instances of all the <see cref="CountryBorder"/> instances.
        /// </summary>
        public List<BorderPoint> BorderPoints
        {
            get 
            { 
                return this.borderPoints; 
            }

            set
            {
                this.borderPoints = value;
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

        /// <summary>
        /// The <see cref="Country"/> instances.
        /// </summary>
        public List<Country> Countries
        {
            get
            {
                return this.countries;
            }

            set
            {
                this.countries = value;
            }
        }
    }
}
