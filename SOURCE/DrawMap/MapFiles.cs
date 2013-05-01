namespace DrawMap
{
    using System;
    using System.IO;
    using DrawMap.Interface;

    /// <summary>
    /// <inheritDoc/>
    /// </summary>
    public class MapFiles : IMapFiles
    {
        private string fileName;
        private bool mapChanged = false;
        private bool mayOverwriteExcisting = false;
        private bool ignoreChanges = false;

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public string FileName
        {
            get
            {
                return this.fileName;
            }

            set
            {
                this.fileName = value;
                this.mapChanged = true;
                this.mayOverwriteExcisting = false;
            }
        }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public bool MayOverwriteExcisting
        {
            get
            {
                return this.mayOverwriteExcisting;
            }

            set
            {
                this.mayOverwriteExcisting = value;
            }
        }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public bool MapChanged
        {
            get
            {
                return this.mapChanged;
            }
        }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public bool IgnoreChanges
        {
            get
            {
                return this.ignoreChanges;
            }

            set
            {
                this.ignoreChanges = value;
            }
        }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public bool FilenameExcists
        {
            get
            {
                return File.Exists(this.fileName);
            }
        }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public void Save()
        {
            if (this.fileName == null || this.fileName == string.Empty)
            {
                throw new InvalidOperationException(Strings.FILENAME_NOT_FILLED);
            }

            if (!this.mayOverwriteExcisting && File.Exists(this.fileName))
            {
                throw new InvalidOperationException(Strings.FILENAME_EXCIST_MAY_NOT_OVERWRITE);
            }

            using (TextWriter textWriter = File.AppendText(this.fileName))
            {
            }

            this.mapChanged = false;
            this.ignoreChanges = false;
            this.mayOverwriteExcisting = true;
        }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        /// <param name="fileName"> <inheritDoc/> </param>
        public void Open(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            if (fileName == string.Empty)
            {
                throw new ArgumentException(Strings.FILENAME_EMPTY);
            }

            if (this.mapChanged)
            {
                throw new InvalidOperationException(Strings.CANT_OPEN_IF_CHANGED);
            }

            if (!File.Exists(fileName))
            {
                throw new InvalidOperationException(Strings.FILE__DOES_NOT_EXCIST);
            }

            TextReader textReader = new StreamReader(fileName);
            this.mayOverwriteExcisting = true;
            this.fileName = fileName;
        }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public void New()
        {
            if (this.mapChanged && !this.ignoreChanges)
            {
                throw new InvalidOperationException(Strings.CHANGES_BUT_NOT_IGNORE);
            }

            this.mapChanged = false;
            this.mayOverwriteExcisting = false;
            this.ignoreChanges = false;
            this.fileName = string.Empty;
        }
    }
}
