//-------------------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="BorderItem.cs">
// Taeke van der Veen juni 2013
// </copyright>
// Visual Studio Express 2012 for Windows Desktop
//-------------------------------------------------------------------------------------------------------------------------------------------------
namespace JMD
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Shapes;

    /// <summary>
    /// Storing a <see cref="CountryBorder"/> and its visual representations.
    /// </summary>
    public class BorderItem
    {
        /// <summary>
        /// Creating a BorderItem
        /// </summary>
        /// <param name="numbers"> The unique indentifying numbers of both <see cref="BorderPoint"/> which form the endpoints of this <see cref="CountryBorder"/>. </param>
        /// <param name="line"> The Line for this instance. </param>
        /// <param name="lineNumbers"> The unique indentifying numbers of both <see cref="BorderPoint"/> which form the line. When receating
        /// the BorderItem this may not be the same as numbers. </param>
        public BorderItem(int[] numbers, Line line, int[] lineNumbers)
        {
            this.EndPointNumbers = numbers;
            this.Lines = new Dictionary<Line, int[]>();
            this.Lines.Add(line, lineNumbers);
            this.VisibleInComboBox = Visibility.Visible;
        }

        /// <summary>
        /// The unique indentifying numbers for the BorderEndPoints.
        /// </summary>
        public int[] EndPointNumbers { get; private set; }

        /// <summary>
        /// The Line used for showing the <see cref="CountryBorder"/> on the drawingSurface.
        /// </summary>
        public Dictionary<Line, int[]> Lines { get; private set; }

        /// <summary>
        /// True if this BorderItem is not part of the currently selected country. So it can be 
        /// selected to add it to the country.
        /// </summary>
        public Visibility VisibleInComboBox { get; set; }
    }
}
