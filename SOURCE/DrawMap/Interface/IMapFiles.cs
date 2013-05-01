namespace DrawMap.Interface
{
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
        /// If there excists a file with the same name as FileName Save will overwrite it.
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
        /// Opens a excisting map.
        /// </summary>
        /// <param name="fileName"> The name of the file to open. </param>
        void Open(string fileName);
    }
}
