namespace JMD
{
    using System.Collections.Generic;
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
            this.Numbers = numbers;
            this.Lines = new Dictionary<Line, int[]>();
            this.Lines.Add(line, lineNumbers);
        }

        /// <summary>
        /// The unique indentifying numbers for the BorderEndPoints.
        /// </summary>
        public int[] Numbers { get; private set; }

        /// <summary>
        /// The Line used for showing the <see cref="CountryBorder"/> on the drawingSurface.
        /// </summary>
        public Dictionary<Line, int[]> Lines { get; private set; }
    }
}
