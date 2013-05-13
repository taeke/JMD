namespace JMD
{
    using System.Windows.Shapes;

    /// <summary>
    /// Storing a <see cref="CountryBorder"/> and its visual representation.
    /// </summary>
    public class BorderItem
    {
        /// <summary>
        /// Creating a BorderItem
        /// </summary>
        /// <param name="numbers"> The unique indentifying numbers of both <see cref="BorderEndPoint"/>. </param>
        /// <param name="line"> The Line for this instance. </param>
        public BorderItem(int[] numbers, Line line)
        {
            this.Numbers = numbers;
            this.Line = line; 
        }

        /// <summary>
        /// The unique indentifying numbers for the BorderEndPoints.
        /// </summary>
        public int[] Numbers { get; private set; }

        /// <summary>
        /// The Line used for showing the <see cref="CountryBorder"/> on the drawingSurface.
        /// </summary>
        public Line Line { get; private set; }
    }
}
