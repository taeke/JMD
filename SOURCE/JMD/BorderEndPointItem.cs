namespace JMD
{
    using System.Windows;
    using System.Windows.Shapes;

    /// <summary>
    /// Storing a <see cref="BorderPoint"/> which is an endpoint and its visual representation.
    /// </summary>
    public class BorderEndPointItem
    {
        /// <summary>
        /// Creating a BorderEndPointItem
        /// </summary>
        /// <param name="number"> The number for this instance. </param>
        /// <param name="ellipse"> The ellipse for this instance. </param>
        /// <param name="clickedPoint"> The clicked point where the EndPoint is created. </param>
        public BorderEndPointItem(int number, Ellipse ellipse, Point clickedPoint)
        {
            this.Number = number;
            this.Ellipse = ellipse;
            this.VisibleIn1 = Visibility.Visible;
            this.VisibleIn2 = Visibility.Visible;
            this.ClickedPoint = clickedPoint;
        }

        /// <summary>
        /// The unique indetifying number given to this point by the AddBorderPoint
        /// </summary>
        public int Number { get; private set; }

        /// <summary>
        /// The Ellipse used for showing the BorderEndPoint on the drawingSurface.
        /// </summary>
        public Ellipse Ellipse { get; private set; }

        /// <summary>
        /// The point where the <see cref="BorderEndPoint"/> is created.
        /// </summary>
        public Point ClickedPoint { get; private set; }

        /// <summary>
        /// There are two comboboxs for selecting the endpoints of a <see cref="CountryBorder"/> if a <see cref="BorderEndPoint"/>
        /// is selected in one comboboc it must not be visible in the other. This is false if this <see cref="BorderEndPoint"/> in 
        /// the second ComboBox.
        /// </summary>
        public Visibility VisibleIn1 { get; set; }

        /// <summary>
        /// There are two comboboxs for selecting the endpoints of a <see cref="CountryBorder"/> if a <see cref="BorderEndPoint"/>
        /// is selected in one comboboc it must not be visible in the other. This is false if this <see cref="BorderEndPoint"/> in 
        /// the first ComboBox.
        /// </summary>
        public Visibility VisibleIn2 { get; set; }
    }
}
