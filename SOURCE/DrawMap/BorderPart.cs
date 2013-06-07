//-------------------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="BorderPart.cs">
// Taeke van der Veen juni 2013
// </copyright>
// Visual Studio Express 2012 for Windows Desktop
//-------------------------------------------------------------------------------------------------------------------------------------------------
namespace DrawMap
{
    using System;

    /// <summary>
    /// A part of a <see cref="CountryBorder"/>.
    /// </summary>
    [Serializable]
    public class BorderPart
    {
        /// <summary>
        /// Parameter less constructor is needed for serializing.
        /// </summary>
        public BorderPart()
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="number1"> The unique inditifying numbers of the <see cref="BorderPoint"/> instances for this instance</param>
        public BorderPart(int[] borderPointNumbers)
        {
            this.BorderPointNumbers = borderPointNumbers;
        }

        /// <summary>
        /// The unique indentifying numbers of the <see cref="BorderPoint"/> instances for this instance. 
        /// </summary>
        public int[] BorderPointNumbers { get; set; }
    }
}
