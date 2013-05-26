namespace DrawMap.Interface
{
    using System.Collections.Generic;

    /// <summary>
    /// The operation on the files for the map.
    /// </summary>
    public interface IMapFiles
    {
        /// <summary>
        /// The filename with the extension xml.
        /// </summary>
        string FileName { get; set; }

        /// <summary>
        /// If there excists a file with the same name as FileName. Save will overwrite it if this is true.
        /// </summary>
        bool MayOverwriteExcisting { get; set; }

        /// <summary>
        /// Is there a change after the last Save.
        /// </summary>
        bool MapChanged { get; }

        /// <summary>
        /// The FileName excists.
        /// </summary>
        bool FilenameExcists { get; }

        /// <summary>
        /// The changes may be ignored.
        /// </summary>
        bool IgnoreChanges { get; set; }

        /// <summary>
        /// Saves the current map.
        /// </summary>
        void Save();

        /// <summary>
        /// Resets everything to create a new map.
        /// </summary>
        void New();

        /// <summary>
        /// Resets everything and fills this instance with the information form the file.
        /// </summary>
        /// <param name="fileName"> The name of the file to open. </param>
        void Open(string fileName);

        /// <summary>
        /// Adds a new <see cref="BorderPoint"/>.
        /// </summary>
        /// <param name="X"> the X coördinant. </param>
        /// <param name="Y"> the Y coördinant. </param>
        /// <returns> The unique indentifying number of the <see cref="BorderPoint"/>. </returns>
        int AddBorderPoint(double x, double y);

        /// <summary>
        /// Adds a new <see cref="CountryBorder"/>
        /// </summary>
        /// <param name="borderEndPointNumbers"> Unique indentifying numbers of the <see cref="BorderPoint"/> for the endpoints. </param>
        void AddCountryBorder(int[] borderEndPointNumbers);

        /// <summary>
        /// Adds a new <see cref="Country"/>
        /// </summary>
        /// <param name="countriesBorderEndPointNumbers"> Unique indentifying numbers of the <see cref="BorderPoint"/> for the endpoints of all the 
        /// <see cref="CountryBorder"/> instances which form the Country. </param>
        void AddCountry(string name, List<int[]> countriesBorderEndPointNumbers);

        /// <summary>
        /// Adds the locaction of the JPG for the original map.
        /// </summary>
        /// <param name="location"></param>
        void AddOriginalMap(string location);

        /// <summary>
        /// Splits an excisting <see cref="BorderPart"/> up in two. Creates an new BorderPart and edits the other to give it the new BorderPoint.
        /// </summary>
        /// <param name="borderPointNumbers"> The original <see cref="BorderPoint"/> numbers for the original <see cref="BorderPart"/></param>
        /// <param name="borderPoint"> The new <see cref="BorderPoint"/> which is the split point. </param>
        void InsertBorderPoint(int[] borderPointNumbers, int borderPoint);

        /// <summary>
        /// Removes a <see cref="BorderPoint"/>.
        /// </summary>
        /// <param name="number"> The unique indentifying number of the <see cref="BorderPoint"/> to remove. </param>
        void RemoveBorderPoint(int number);

        /// <summary>
        /// Remove a <see cref="CountryBorder"/> from the list.
        /// </summary>
        /// <param name="numbers"> The numbers of the two <see cref="BorderPoint"/> for the <see cref="CountryBorder"/> to remove. </param>
        void RemoveCountryBorder(int[] numbers);

        /// <summary>
        /// Remove a <see cref="Country"/> from the list.
        /// </summary>
        /// <param name="name"> The name of the <see cref="Country"/> to remove. </param>
        void RemoveCountry(string name);

        /// <summary>
        /// Get the list with <see cref="BorderPoint"/>.
        /// </summary>
        /// <returns> The list with <see cref="BorderPoint"/>. </returns>
        List<BorderPoint> GetBorderPoints();

        /// <summary>
        /// Get the list with <see cref="CountryBorder"/>.
        /// </summary>
        /// <returns> The list with <see cref="CountryBorder"/>. </returns>
        List<CountryBorder> GetCountryBorders();

        /// <summary>
        /// Get the list with <see cref="Country"/>.
        /// </summary>
        /// <returns> The list with <see cref="Country"/>. </returns>
        List<Country> GetCountries();

        /// <summary>
        /// Gets the location of the original map JPG>
        /// </summary>
        /// <returns> The location of the JPG. </returns>
        string GetOriginalMap();
    }
}
