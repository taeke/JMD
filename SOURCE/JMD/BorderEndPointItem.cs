namespace JMD
{
    using System.Windows.Shapes;

    /// <summary>
    /// Storing and showing a <see cref="BorderEndPoint"/> in a combobox.
    /// </summary>
    public class BorderEndPointItem
    {
        /// <summary>
        /// Creating a BorderEndPointItem
        /// </summary>
        /// <param name="number"> The number for this instance. </param>
        /// <param name="ellipse"> The ellipse for this instance. </param>
        public BorderEndPointItem(int number, Ellipse ellipse)
        {
            this.Number = number;
            this.Ellipse = ellipse;
        }

        /// <summary>
        /// The unique indetifying number given to this point by the AddBorderEndPoint
        /// </summary>
        public int Number { get; private set; }

        /// <summary>
        /// The Ellipse used for showing the BorderEndPoint on the drawingSurface.
        /// </summary>
        public Ellipse Ellipse { get; private set; }
    }
}
