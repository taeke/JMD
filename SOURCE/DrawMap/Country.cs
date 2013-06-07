//-------------------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="Country.cs">
// Taeke van der Veen juni 2013
// </copyright>
// Visual Studio Express 2012 for Windows Desktop
//-------------------------------------------------------------------------------------------------------------------------------------------------
namespace DrawMap
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A complete country.
    /// </summary>
    [Serializable]
    public class Country
    {
        /// <summary>
        /// Parameter less constructor is needed for serializing.
        /// </summary>
        public Country()
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="name"> The name for this Country. </param>
        /// <param name="countriesBorderEndPointNumbers"> The unique inditifying numbers of the <see cref="BorderPoint"/> in pairs 
        /// for all the <see cref="CountryBorder"/> instances for this Country. </param>
        public Country(string name, List<int[]> countriesBorderEndPointNumbers)
        {
            this.Name = name;
            this.CountriesBorderEndPointNumbers = countriesBorderEndPointNumbers;
        }

        /// <summary>
        /// The <see cref="CountryBorder"/> instances which form the Country.
        /// </summary>
        public List<int[]> CountriesBorderEndPointNumbers { get; set; }

        /// <summary>
        /// The name for the Country.
        /// </summary>
        public string Name { get; set; }
    }
}
