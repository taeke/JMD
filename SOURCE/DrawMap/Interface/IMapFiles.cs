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
        /// Adds a new <see cref="BorderEndPoint"/>.
        /// </summary>
        /// <param name="X"> the X coördinant. </param>
        /// <param name="Y"> the Y coördinant. </param>
        /// <returns> The unique indentifying number of the <see cref="BorderEndPoint"/>. </returns>
        int AddBorderEndPoint(double x, double y);

        /// <summary>
        /// Adds a new <see cref="CountryBorder"/>
        /// </summary>
        /// <param name="Number1"> Unique indentifying number of the first <see cref="BorderEndPoint"/></param>
        /// <param name="Number2"> Unique indentifying number of the second <see cref="BorderEndPoint"/></param>
        void AddCountryBorder(int[] numbers);

        /// <summary>
        /// Removes a <see cref="BorderEndPoint"/>.
        /// </summary>
        /// <param name="number"> The unique indentifying number of the <see cref="BorderEndPoint"/> to remove. </param>
        void RemoveEndPoint(int number);

        /// <summary>
        /// Get the list with <see cref="BorderEndPoint"/>.
        /// </summary>
        /// <returns> The list with <see cref="BorderEndPoint"/>. </returns>
        List<BorderEndPoint> GetBorderEndPoints();

        /// <summary>
        /// Get the list with <see cref="CountryBorder"/>.
        /// </summary>
        /// <returns> The list with <see cref="CountryBorder"/>. </returns>
        List<CountryBorder> GetCountryBorders();

        /// <summary>
        /// Remove a <see cref="CountryBorder"/> from the list.
        /// </summary>
        /// <param name="numbers"> The numbers of the two <see cref="BorderEndPoint"/> for the <see cref="CountryBorder"/> to remove. </param>
        void RemoveCountryBorder(int[] numbers);
    }
}
