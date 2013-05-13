namespace DrawMap
{
    /// <summary>
    ///  A <see cref="CountryBorder"/> for one or more countries,
    /// </summary>
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
        /// <param name="number1"> The unique inditifying numbers of the <see cref="BorderEndPoint"/> instances for this instance</param>
        public CountryBorder(int[] numbers)
        {
            this.Numbers = numbers;
        }

        /// <summary>
        /// The first unique inditifying numbers of the <see cref="BorderEndPoint"/> instances for this instance.
        /// </summary>
        public int[] Numbers { get; set; }
    }
}
