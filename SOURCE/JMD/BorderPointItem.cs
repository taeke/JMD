//-------------------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="BorderPointItem.cs">
// Taeke van der Veen juni 2013
// </copyright>
// Visual Studio Express 2012 for Windows Desktop
//-------------------------------------------------------------------------------------------------------------------------------------------------
namespace JMD
{
    using System.Windows;
    using System.Windows.Shapes;

    /// <summary>
    /// Storing a <see cref="BorderPoint"/>.
    /// </summary>
    public class BorderPointItem
    {
        /// <summary>
        /// Creating a BorderPointItem
        /// </summary>
        /// <param name="number"> The number for this instance. </param>
        /// <param name="ellipse"> The ellipse for this instance. </param>
        /// <param name="clickedPoint"> The clicked point where the EndPoint is created. </param>
        public BorderPointItem(int number, Ellipse ellipse, Point clickedPoint)
        {
            this.Number = number;
            this.Ellipse = ellipse;
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
    }
}
