namespace DrawMap
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Serialization;
    using DrawMap.Interface;

    /// <summary>
    /// <inheritDoc/>
    /// </summary>
    public class MapFiles : IMapFiles
    {
        /// <summary>
        /// Backing field for FileName.
        /// </summary>
        private string fileName;

        /// <summary>
        /// Backing field for MapChanged.
        /// </summary>
        private bool mapChanged = false;

        /// <summary>
        /// Backing field for MayOverwriteExcisting.
        /// </summary>
        private bool mayOverwriteExcisting = false;

        /// <summary>
        /// Backing field for IgnoreChanges.
        /// </summary>
        private bool ignoreChanges = false;

        /// <summary>
        /// The map instance.
        /// </summary>
        private Map map = new Map();

        /// <summary>
        /// Create the MapFiles instance.
        /// </summary>
        public MapFiles()
        {
        }

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

            using (TextWriter textWriter = new StreamWriter(this.fileName))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Map));
                xmlSerializer.Serialize(textWriter, this.map);
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
            if (this.mapChanged && !this.ignoreChanges)
            {
                throw new InvalidOperationException(Strings.CHANGES_BUT_NOT_IGNORE);
            }

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

            using (TextReader textReader = new StreamReader(fileName))
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Map));
                try
                {
                    this.map = (Map)xmlSerializer.Deserialize(textReader);
                }
                catch (InvalidOperationException) 
                {
                    throw new InvalidOperationException(Strings.MALFORMED_XML);
                }
            }

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

            this.map.BorderEndpoints.Clear();
            this.mapChanged = false;
            this.mayOverwriteExcisting = false;
            this.ignoreChanges = false;
            this.fileName = string.Empty;
        }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public int AddBorderEndPoint(double x, double y)
        {
            this.mapChanged = true;
            int maxNumber = this.map.BorderEndpoints.Count == 0 ? 0 : this.map.BorderEndpoints.OrderByDescending(b => b.Number).First().Number;
            BorderEndPoint borderEndPoint = new BorderEndPoint(x, y, maxNumber + 1);
            this.map.BorderEndpoints.Add(borderEndPoint);
            return borderEndPoint.Number;
        }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public void RemoveEndPoint(int number)
        {
            BorderEndPoint borderEndPoint = this.map.BorderEndpoints.Find(b => b.Number == number);
            if (borderEndPoint == null)
            {
                throw new ArgumentOutOfRangeException(Strings.BORDERENDPOINT_DOES_NOT_EXCIST);
            }

            this.mapChanged = true;
            this.map.BorderEndpoints.Remove(borderEndPoint);
        }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public List<BorderEndPoint> GetBorderEndPoints()
        {
            return this.map.BorderEndpoints;
        }
    }
}
