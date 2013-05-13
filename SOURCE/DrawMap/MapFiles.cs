namespace DrawMap
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows;
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
        /// The <see cref="Map"/> instance.
        /// </summary>
        private Map map = new Map();

        /// <summary>
        /// Create an instance.
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
        /// Check of two lines are intersecting.
        /// </summary>
        /// <param name="a"> First endpoint of first line. </param>
        /// <param name="b"> Second endpoint of first line.</param>
        /// <param name="c"> First endpoint of second line. </param>
        /// <param name="d"> Second endpoint of second line.</param>
        /// <returns></returns>
        public static bool IsIntersecting(Point a, Point b, Point c, Point d)
        {
            double denominator = ((b.X - a.X) * (d.Y - c.Y)) - ((b.Y - a.Y) * (d.X - c.X));
            double numerator1 = ((a.Y - c.Y) * (d.X - c.X)) - ((a.X - c.X) * (d.Y - c.Y));
            double numerator2 = ((a.Y - c.Y) * (b.X - a.X)) - ((a.X - c.X) * (b.Y - a.Y));

            // Detect coincident lines (has a problem if two lines are the same. Make sure no two lines with same endpoints can be created). 
            if (denominator == 0)
            {
                return numerator1 == 0 && numerator2 == 0;
            }

            double r = numerator1 / denominator;
            double s = numerator2 / denominator;

            return (r >= 0 && r <= 1) && (s >= 0 && s <= 1);
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
            this.map.CountryBorders.Clear();
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
        public void AddCountryBorder(int[] numbers)
        {
            if (numbers[0] == numbers[1])
            {
                throw new ArgumentException(Strings.NUMBERS_ARE_THE_SAME);
            }

            if (numbers[0] > numbers[1])
            {
                throw new ArgumentException(Strings.NUMBER1_GREATER_NUMBER2);
            }

            if (this.map.CountryBorders.Find(b => b.Numbers[0] == numbers[0] && b.Numbers[1] == numbers[1]) != null)
            {
                throw new ArgumentException(Strings.BORDER_ALLREADY_EXCISTS);
            }

            BorderEndPoint borderEndPoint1 = this.map.BorderEndpoints.Find(b => b.Number == numbers[0]);
            BorderEndPoint borderEndPoint2 = this.map.BorderEndpoints.Find(b => b.Number == numbers[1]);
            
            if (borderEndPoint1 == null || borderEndPoint2 == null)
            {
                throw new ArgumentException(Strings.BORDERENDPOINT_DOES_NOT_EXCIST);
            }

            this.CheckIntersection(borderEndPoint1, borderEndPoint2);

            this.mapChanged = true;
            CountryBorder countryBorder = new CountryBorder(numbers);
            this.map.CountryBorders.Add(countryBorder);
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

            if (this.map.CountryBorders.Find(c => c.Numbers.Contains(number)) != null)
            {
                throw new ArgumentException(Strings.CANNOT_DELETE_ENDPOINT_BORDER);
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

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public List<CountryBorder> GetCountryBorders()
        {
            return this.map.CountryBorders;
        }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public void RemoveCountryBorder(int[] numbers)
        {
            CountryBorder countryBorder = this.map.CountryBorders.Find(b => b.Numbers[0] == numbers[0] && b.Numbers[1] == numbers[1]);
            if (countryBorder == null)
            {
                throw new ArgumentOutOfRangeException(Strings.BORDER_DOES_NOT_EXCIST);
            }

            this.mapChanged = true;
            this.map.CountryBorders.Remove(countryBorder);
        }

        /// <summary>
        /// Checks if the two provided endpoints form a <see cref="CountryBorder"/> which intersects with on of the other 
        /// <see cref="CountryBorder"/> instances.
        /// </summary>
        /// <param name="borderEndPoint1"> The first <see cref="BorderEndPoint"/> of the new <see cref="CountryBorder"/></param>
        /// <param name="borderEndPoint2"> The second <see cref="BorderEndPoint"/> of the new <see cref="CountryBorder"/></param>
        private void CheckIntersection(BorderEndPoint borderEndPoint1, BorderEndPoint borderEndPoint2)
        {
            foreach (CountryBorder item in this.map.CountryBorders)
            {
                BorderEndPoint borderEndPoint3 = this.map.BorderEndpoints.Find(b => b.Number == item.Numbers[0]);
                BorderEndPoint borderEndPoint4 = this.map.BorderEndpoints.Find(b => b.Number == item.Numbers[1]);
                if (borderEndPoint1 != borderEndPoint3 &&
                    borderEndPoint1 != borderEndPoint4 &&
                    borderEndPoint2 != borderEndPoint3 &&
                    borderEndPoint2 != borderEndPoint4 &&
                    IsIntersecting(
                      new Point(borderEndPoint1.X, borderEndPoint1.Y),
                      new Point(borderEndPoint2.X, borderEndPoint2.Y),
                      new Point(borderEndPoint3.X, borderEndPoint3.Y),
                      new Point(borderEndPoint4.X, borderEndPoint4.Y)))
                {
                    throw new ArgumentException(Strings.COUNTRYBORDERS_INTERSECT);
                }
            }
        }
    }
}
