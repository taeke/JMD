//-------------------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="CountryBorder.cs">
// Taeke van der Veen juni 2013
// </copyright>
// Visual Studio Express 2012 for Windows Desktop
//-------------------------------------------------------------------------------------------------------------------------------------------------
namespace DrawMap
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///  A border for one or more countries,
    /// </summary>
    [Serializable]
    public class CountryBorder
    {
        /// <summary>
        /// Parameter less constructor is needed for serializing.
        /// </summary>
        public CountryBorder()
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="number1"> The unique inditifying numbers of the <see cref="BorderPoint"/> instances for this instance</param>
        public CountryBorder(int[] borderEndPointNumbers)
        {
            this.BorderEndPointNumbers = borderEndPointNumbers;
            BorderPart borderPart = new BorderPart(new int[2] { borderEndPointNumbers[0], borderEndPointNumbers[1] });
            this.BorderParts = new List<BorderPart>();
            this.BorderParts.Add(borderPart);
        }

        /// <summary>
        /// The unique indentifying numbers of the <see cref="BorderPoint"/> instances for this instance which form the endPoints of this border.
        /// </summary>
        public int[] BorderEndPointNumbers { get; set; }

        /// <summary>
        /// The <see cref="BorderPart"/> instances which form the CountryBorder.
        /// </summary>
        public List<BorderPart> BorderParts { get; set; }
    }
}
