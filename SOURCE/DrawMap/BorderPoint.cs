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
        public BorderPoint(double x, double y, int number)
        {
            this.x = x;
            this.y = y;
            this.number = number;
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
    }
}
