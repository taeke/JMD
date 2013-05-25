namespace JMD
{
    using System.Collections.Generic;

    /// <summary>
    /// Storing a <see cref="Country"/> which is a collection of <see cref="CountryBorder"/> represented by its
    /// two unique indetifying numbers. 
    /// </summary>
    public class CountryItem
    {
        /// <summary>
        /// Creating a CountryItem
        /// </summary>
        /// <param name="name"> The name for the country. </param>
        /// <param name="countryBorderNumbers"> The list of <see cref="BorderItem"/> which form the Country. </param>
        public CountryItem(string name, List<BorderItem> countryBorderNumbers)
        {
            this.Name = name;
            this.BorderItems = countryBorderNumbers;
        }

        /// <summary>
        /// The name for the country.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// A list of <see cref="BorderItem"/> for the <see cref="CountryBorder"/> list for this Country.
        /// </summary>
        public List<BorderItem> BorderItems { get; private set; }
    }
}
