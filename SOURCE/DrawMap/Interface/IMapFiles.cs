namespace DrawMap.Interface
{
    using System.Collections.Generic;

    /// <summary>
    /// The files and operation on them for the map.
    /// </summary>
    public interface IMapFiles
    {
        /// <summary>
        /// The filename with the extension xml.
        /// </summary>
        string FileName { get; set; }

        /// <summary>
        /// If there excists a file with the same name as FileName Save will overwrite it if this is true.
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
        /// <returns> the unique indentifying number of this <see cref="BorderEndPoint"/>. </returns>
        int AddBorderEndPoint(double x, double y);

        /// <summary>
        /// Removes a <see cref="BorderEndPoint"/>.
        /// </summary>
        /// <param name="number"> The unique number of the <see cref="BorderEndPoint"/> to remove. </param>
        void RemoveEndPoint(int number);

        /// <summary>
        /// Get the list with <see cref="BorderEndPoints"/>.
        /// </summary>
        /// <returns> The list with <see cref="BorderEndPoints"/>. </returns>
        List<BorderEndPoint> GetBorderEndPoints();
    }
}
