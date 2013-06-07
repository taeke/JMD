//-------------------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="BorderPoint.cs">
// Taeke van der Veen juni 2013
// </copyright>
// Visual Studio Express 2012 for Windows Desktop
//-------------------------------------------------------------------------------------------------------------------------------------------------
namespace DrawMap
{
    using System;
    using DrawMap.Interface;

    /// <summary>
    /// An endpoint on a <see cref="CountryBorder"/>.
    /// </summary>
    [Serializable]
    public class BorderPoint
    {
        /// <summary>
        /// Backing field for X.
        /// </summary>
        private double x;

        /// <summary>
        /// Backing field for Y.
        /// </summary>
        private double y;

        /// <summary>
        /// Backing field for Number.
        /// </summary>
        private int number;

        /// <summary>
        /// Backing field for IsEndPoint.
        /// </summary>
        private bool isEndPoint;

        /// <summary>
        /// Parameter less constructor is needed for serializing.
        /// </summary>
        public BorderPoint()
        {
        }

        /// <summary>
        /// Create an instance of BorderEndPoint
        /// </summary>
        /// <param name="x"> the X coordinant </param>
        /// <param name="y"> the Y coordinant </param>
        /// <param name="number"> The unique indentifying number for this instance. </param>
        public BorderPoint(double x, double y, int number, bool isEndPoint)
        {
            this.x = x;
            this.y = y;
            this.number = number;
            this.isEndPoint = isEndPoint;
        }

        /// <summary>
        /// X Coordinant.
        /// </summary>
        public double X
        {
            get
            {
                return this.x;
            }

            set
            {
                this.x = value;
            }
        }

        /// <summary>
        /// Y Coordinant
        /// </summary>
        public double Y
        {
            get
            {
                return this.y;
            }

            set
            {
                this.y = value;
            }
        }

        /// <summary>
        /// Unique indentifying number for this instance.
        /// </summary>
        public int Number
        {
            get
            {
                return this.number;
            }

            set
            {
                this.number = value;
            }
        }

        /// <summary>
        /// Is this BorderPoint a endpoint.
        /// </summary>
        public bool IsEndPoint
        {
            get
            {
                return this.isEndPoint;
            }

            set
            {
                this.isEndPoint = value;
            }
        }
    }
}
