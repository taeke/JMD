//-------------------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="MapFiles.cs">
// Taeke van der Veen juni 2013
// </copyright>
// Visual Studio Express 2012 for Windows Desktop
//-------------------------------------------------------------------------------------------------------------------------------------------------
namespace DrawMap
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
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
        /// The diameter for the Ellipses which are a visual representation of the BorderEndPoints.
        /// The other <see cref="BorderPoint"/> are also drawn but the visibility is set to Hidden. This
        /// way it van be used in testing for intersection but does not cluther the drawing.
        /// </summary>
        public const int PointDiameter = 4;

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
        public bool IsMapChanged
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
        public static bool IsIntersectingLine(Point a, Point b, Point c, Point d)
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
        /// Check if a line intersects with a circle.
        /// </summary>
        /// <param name="linePoint1"> First point of the line. </param>
        /// <param name="linePoint2"> Second point of the line. </param>
        /// <param name="centerPoint"> Centerpoint of the circle. </param>
        /// <param name="radius"> The radius of the circle. </param>
        /// <returns></returns>
        public static bool IsIntersectingCircle(Point linePoint1, Point linePoint2, Point centerPoint, double radius)
        {
            if ((centerPoint.X + radius < linePoint1.X && centerPoint.X + radius < linePoint2.X) ||
                (centerPoint.X - radius > linePoint1.X && centerPoint.X - radius > linePoint2.X) ||
                (centerPoint.Y + radius < linePoint1.Y && centerPoint.Y + radius < linePoint2.Y) ||
                (centerPoint.Y - radius > linePoint1.Y && centerPoint.Y - radius > linePoint2.Y))
            {
                return false;
            }

            double dx, dy, a, b, c, det;

            dx = linePoint2.X - linePoint1.X;
            dy = linePoint2.Y - linePoint1.Y;

            a = (dx * dx) + (dy * dy);
            b = 2 * ((dx * (linePoint1.X - centerPoint.X)) + (dy * (linePoint1.Y - centerPoint.Y)));
            c = ((linePoint1.X - centerPoint.X) * (linePoint1.X - centerPoint.X)) + ((linePoint1.Y - centerPoint.Y) * (linePoint1.Y - centerPoint.Y)) - (radius * radius);

            det = (b * b) - (4 * a * c);
            if ((a <= 0.0000001) || (det < 0))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Update de list with the total count of endpoints with the provided number.
        /// </summary>
        /// <param name="endPointNumberCount"> the list</param>
        /// <param name="number"> the number</param>
        public static void CountEndPointNumbers(Dictionary<int, int> endPointNumberCount, int number)
        {
            if (endPointNumberCount.ContainsKey(number))
            {
                endPointNumberCount[number] = endPointNumberCount[number] + 1;
            }
            else
            {
                endPointNumberCount.Add(number, 1);
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

            this.CreateXMLFile();
            this.CreateJSFile();
            this.CreateHTMLFile();
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
            this.mapChanged = false;
            this.ignoreChanges = false;
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

            this.map.BorderPoints.Clear();
            this.map.CountryBorders.Clear();
            this.mapChanged = false;
            this.mayOverwriteExcisting = false;
            this.ignoreChanges = false;
            this.fileName = string.Empty;
        }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public int AddBorderPoint(double x, double y, bool isEndPoint)
        {
            this.mapChanged = true;
            int maxNumber = this.map.BorderPoints.Count == 0 ? 0 : this.map.BorderPoints.OrderByDescending(b => b.Number).First().Number;
            BorderPoint borderPoint = new BorderPoint(x, y, maxNumber + 1, isEndPoint);
            this.map.BorderPoints.Add(borderPoint);
            return borderPoint.Number;
        }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public void AddCountryBorder(int[] borderEndPointNumbers)
        {
            if (borderEndPointNumbers[0] == borderEndPointNumbers[1])
            {
                throw new ArgumentException(Strings.NUMBERS_ARE_THE_SAME);
            }

            if (borderEndPointNumbers[0] > borderEndPointNumbers[1])
            {
                throw new ArgumentException(Strings.NUMBERS_NOT_IN_RIGHT_ORDER);
            }

            if (this.map.CountryBorders.Find(b => b.BorderEndPointNumbers[0] == borderEndPointNumbers[0] && b.BorderEndPointNumbers[1] == borderEndPointNumbers[1]) != null)
            {
                throw new ArgumentException(Strings.BORDER_ALLREADY_EXCISTS);
            }

            BorderPoint borderEndPoint1 = this.map.BorderPoints.Find(b => b.Number == borderEndPointNumbers[0]);
            BorderPoint borderEndPoint2 = this.map.BorderPoints.Find(b => b.Number == borderEndPointNumbers[1]);
            if (borderEndPoint1 == null || borderEndPoint2 == null)
            {
                throw new ArgumentException(Strings.BORDERENDPOINT_DOES_NOT_EXCIST);
            }

            this.CheckIntersection(borderEndPoint1, borderEndPoint2);

            this.mapChanged = true;
            this.map.CountryBorders.Add(new CountryBorder(borderEndPointNumbers));
        }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public void AddCountry(string name, List<int[]> countriesBorderEndPointNumbers)
        {
            if (name == string.Empty)
            {
                throw new ArgumentException(Strings.NAME_EMPTY);
            }

            if (this.map.Countries.Find(c => c.Name == name) != null)
            {
                throw new ArgumentException(Strings.NAMES_ARE_THE_SAME);
            }

            if (!countriesBorderEndPointNumbers.All(c => this.map.CountryBorders.Find(b => b.BorderEndPointNumbers[0] == c[0] && b.BorderEndPointNumbers[1] == c[1]) != null))
            {
                throw new ArgumentException(Strings.BORDER_DOES_NOT_EXCIST);
            }

            if (!this.AreBordersValidCountry(countriesBorderEndPointNumbers))
            {
                throw new ArgumentException(Strings.BORDERS_ARE_NOT_COUNTRY);
            }

            if (countriesBorderEndPointNumbers.All(b => this.map.Countries.Find(c => c.CountriesBorderEndPointNumbers.Find(e => e[0] == b[0] && e[1] == b[1]) != null) != null))
            {
                throw new ArgumentException(Strings.BORDERS_ARE_NOT_COUNTRY);
            }

            this.mapChanged = true;
            this.map.Countries.Add(new Country(name, countriesBorderEndPointNumbers));
        }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        /// <param name="location"><inheritDoc/></param>
        public void AddOriginalMap(string location)
        {
            if (location == string.Empty)
            {
                throw new ArgumentException(Strings.LOCATION_EMPTY);
            }

            if (Path.GetExtension(location).ToUpper() != ".JPG")
            {
                throw new ArgumentException(Strings.EXTENSION_NOT_JPG);
            }

            if (!File.Exists(location))
            {
                throw new ArgumentException(Strings.FILE__DOES_NOT_EXCIST);
            }

            this.map.OriginalMap = location;
            this.mapChanged = true;
        }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        /// <param name="borderPointNumbers"><inheritDoc/></param>
        /// <param name="borderPointNumber"><inheritDoc/></param>
        public void InsertBorderPoint(int[] borderPointNumbers, int borderPointNumber)
        {
            if (borderPointNumbers[0] == borderPointNumbers[1])
            {
                throw new ArgumentException(Strings.NUMBERS_ARE_THE_SAME);
            }

            if (borderPointNumbers[0] > borderPointNumbers[1])
            {
                throw new ArgumentException(Strings.NUMBERS_NOT_IN_RIGHT_ORDER);
            }

            if (borderPointNumbers[1] > borderPointNumber)
            {
                throw new ArgumentException(Strings.NUMBERS_NOT_IN_RIGHT_ORDER);
            }

            BorderPoint borderEndPoint1 = this.map.BorderPoints.Find(b => b.Number == borderPointNumbers[0]);
            BorderPoint borderEndPoint2 = this.map.BorderPoints.Find(b => b.Number == borderPointNumbers[1]);
            BorderPoint borderEndPoint3 = this.map.BorderPoints.Find(b => b.Number == borderPointNumber);
            if (borderEndPoint1 == null || borderEndPoint2 == null || borderEndPoint3 == null)
            {
                throw new ArgumentException(Strings.BORDERENDPOINT_DOES_NOT_EXCIST);
            }

            CountryBorder countryBorder = this.map.CountryBorders.Find(
                b => b.BorderParts.Find(
                    p => p.BorderPointNumbers[0] == borderPointNumbers[0] &&
                         p.BorderPointNumbers[1] == borderPointNumbers[1])
                        != null);
            if (countryBorder == null)
            {
                throw new ArgumentException(Strings.BORDER_DOES_NOT_EXCIST);
            }

            if (countryBorder.BorderParts.Find(p => p.BorderPointNumbers.Contains(borderPointNumber)) != null)
            {
                throw new ArgumentException(Strings.NUMBER_ALLREADY_IN_PART);
            }

            this.CheckIntersection(borderEndPoint1, borderEndPoint3);
            this.CheckIntersection(borderEndPoint2, borderEndPoint3);

            this.mapChanged = true;
            BorderPart borderPartNew = new BorderPart(new int[2] { borderEndPoint2.Number, borderEndPoint3.Number });
            countryBorder.BorderParts.Add(borderPartNew);
            BorderPart borderPartExcist = countryBorder.BorderParts.Find(p => p.BorderPointNumbers[0] == borderPointNumbers[0] && p.BorderPointNumbers[1] == borderPointNumbers[1]);
            borderPartExcist.BorderPointNumbers[1] = borderEndPoint3.Number;
        }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public void RemoveBorderPoint(int number)
        {
            BorderPoint borderEndPoint = this.map.BorderPoints.Find(b => b.Number == number);
            if (borderEndPoint == null)
            {
                throw new ArgumentOutOfRangeException(Strings.BORDERENDPOINT_DOES_NOT_EXCIST);
            }

            if (this.map.CountryBorders.Find(c => c.BorderEndPointNumbers.Contains(number)) != null)
            {
                throw new ArgumentException(Strings.CANNOT_DELETE_ENDPOINT_BORDER);
            }

            this.mapChanged = true;
            this.map.BorderPoints.Remove(borderEndPoint);
        }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public void RemoveCountryBorder(int[] numbers)
        {
            CountryBorder countryBorder = this.map.CountryBorders.Find(b => b.BorderEndPointNumbers[0] == numbers[0] && b.BorderEndPointNumbers[1] == numbers[1]);
            if (countryBorder == null)
            {
                throw new ArgumentOutOfRangeException(Strings.BORDER_DOES_NOT_EXCIST);
            }

            foreach (BorderPart borderPart in countryBorder.BorderParts)
            {
                foreach (int number in borderPart.BorderPointNumbers)
                {
                    if (this.map.BorderPoints.Find(p => p.Number == number) == null)
                    {
                        throw new ArgumentOutOfRangeException(Strings.BORDERENDPOINT_DOES_NOT_EXCIST);
                    }
                }
            }

            if (this.map.Countries.Find(c => c.CountriesBorderEndPointNumbers.Find(b => b[0] == numbers[0] && b[1] == numbers[1]) != null) != null)
            {
                throw new ArgumentException(Strings.CANNOT_DELETE_BORDER_COUNTRY);
            }

            this.mapChanged = true;
            foreach (BorderPart borderPart in countryBorder.BorderParts)
            {
                foreach (int number in borderPart.BorderPointNumbers)
                {
                    if (!countryBorder.BorderEndPointNumbers.Contains(number))
                    {
                        this.map.BorderPoints.Remove(this.map.BorderPoints.Find(p => p.Number == number));
                    }
                }
            }

            this.map.CountryBorders.Remove(countryBorder);
        }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public void RemoveCountry(string name)
        {
            if (name == string.Empty)
            {
                throw new ArgumentException(Strings.NAME_EMPTY);
            }

            Country country = this.map.Countries.Find(c => c.Name == name);
            if (country == null)
            {
                throw new ArgumentException(Strings.NAME_DOES_NOT_EXICST);
            }

            this.map.Countries.Remove(country);
        }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public List<BorderPoint> GetBorderPoints()
        {
            return this.map.BorderPoints;
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
        public List<Country> GetCountries()
        {
            return this.map.Countries;
        }

        /// <summary>
        /// <inheritDoc/>
        /// </summary>
        public string GetOriginalMap()
        {
            return this.map.OriginalMap;
        }

        /// <summary>
        /// Checks if the two provided endpoints form a <see cref="CountryBorder"/> which intersects with on of the other 
        /// <see cref="CountryBorder"/> instances.
        /// </summary>
        /// <param name="borderEndPoint1"> The first <see cref="BorderPoint"/> of the new <see cref="CountryBorder"/></param>
        /// <param name="borderEndPoint2"> The second <see cref="BorderPoint"/> of the new <see cref="CountryBorder"/></param>
        private void CheckIntersection(BorderPoint borderEndPoint1, BorderPoint borderEndPoint2)
        {
            foreach (CountryBorder countryBorder in this.map.CountryBorders)
            {
                foreach (var borderPart in countryBorder.BorderParts)
                {
                    BorderPoint borderEndPoint3 = this.map.BorderPoints.Find(b => b.Number == borderPart.BorderPointNumbers[0]);
                    BorderPoint borderEndPoint4 = this.map.BorderPoints.Find(b => b.Number == borderPart.BorderPointNumbers[1]);
                    if (borderEndPoint1 != borderEndPoint3 &&
                        borderEndPoint1 != borderEndPoint4 &&
                        borderEndPoint2 != borderEndPoint3 &&
                        borderEndPoint2 != borderEndPoint4 &&
                        IsIntersectingLine(
                          new Point(borderEndPoint1.X, borderEndPoint1.Y),
                          new Point(borderEndPoint2.X, borderEndPoint2.Y),
                          new Point(borderEndPoint3.X, borderEndPoint3.Y),
                          new Point(borderEndPoint4.X, borderEndPoint4.Y)))
                    {
                        throw new ArgumentException(Strings.COUNTRYBORDERS_INTERSECT);
                    }
                }
            }

            foreach (BorderPoint borderPoint in this.map.BorderPoints)
            {
                if (borderEndPoint1 != borderPoint && borderEndPoint2 != borderPoint)
                {
                    if (IsIntersectingCircle(new Point(borderEndPoint1.X, borderEndPoint1.Y), new Point(borderEndPoint2.X, borderEndPoint2.Y), new Point(borderPoint.X, borderPoint.Y), 2))
                    {
                        throw new ArgumentException(Strings.COUNTRYBORDERS_INTERSECT);
                    }
                }
            }
        }

        /// <summary>
        /// A country is valid if alle de borders form a closed shape this means that every endpoint should be exactly 2 times in the total list.
        /// </summary>
        /// <returns></returns>
        private bool AreBordersValidCountry(List<int[]> countriesBorderEndPointNumbers)
        {
            Dictionary<int, int> endPointNumberCount = new Dictionary<int, int>();
            foreach (int[] numbers in countriesBorderEndPointNumbers)
            {
                CountEndPointNumbers(endPointNumberCount, numbers[0]);
                CountEndPointNumbers(endPointNumberCount, numbers[1]);
            }

            foreach (int count in endPointNumberCount.Values)
            {
                if (count != 2)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Creates a ordert list of unique indentifing numbers for <see cref="BorderPoint"/>. It orders them
        /// by starting at a random posision and follow each borderpart to the next point until we have had all 
        /// the point for the provided <see cref="Country"/>.
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        private List<int> CreateListOfPoints(Country country)
        {
            List<int> result = new List<int>();
            List<BorderPart> allBorderParts = this.GetAllBorderPartsForCountry(country);
            int firstNumber = allBorderParts[0].BorderPointNumbers[0];
            result.Add(firstNumber);
            int nextNumber = allBorderParts[0].BorderPointNumbers[1];
            allBorderParts.Remove(allBorderParts[0]);
            while (allBorderParts.Count > 0)
            {
                result.Add(nextNumber);
                BorderPart borderPart = allBorderParts.Find(b => b.BorderPointNumbers.Contains(nextNumber));
                nextNumber = borderPart.BorderPointNumbers[0] == nextNumber ? borderPart.BorderPointNumbers[1] : borderPart.BorderPointNumbers[0];
                allBorderParts.Remove(borderPart);
            }

            return result;
        }

        /// <summary>
        /// Convert the list of points for a <see cref="Country"/> in a string with a javascript array of points.
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        private string CreateJavaScriptArrayOfPoints(Country country)
        {
            List<int> points = this.CreateListOfPoints(country);
            StringBuilder result = new StringBuilder();
            foreach (var point in points)
            {
                result.Append("[");
                result.Append(((int)this.map.BorderPoints.Find(b => b.Number == point).X).ToString());
                result.Append(",");
                result.Append(((int)this.map.BorderPoints.Find(b => b.Number == point).Y).ToString());
                result.Append("]");
                if (point != points.Last())
                {
                    result.Append(",");
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Create a list of All the <see cref="BorderPart"/> instances of all the <see cref="CountryBorder"/> instances for a <see cref="Country"/>
        /// </summary>
        /// <param name="country">The <see cref="Country"/> for which to find all the <see cref="BorderPart"/></param>
        /// <returns> A list with all the <see cref="BorderPart"/> instances for a <see cref="Country"/>.</returns>
        private List<BorderPart> GetAllBorderPartsForCountry(Country country)
        {
            List<CountryBorder> allBorders = country.CountriesBorderEndPointNumbers.Select(b => this.map.CountryBorders.Find(c => c.BorderEndPointNumbers[0] == b[0] && c.BorderEndPointNumbers[1] == b[1])).ToList();
            return allBorders.SelectMany(b => b.BorderParts).ToList();
        }

        /// <summary>
        /// Creates the JavaScript file.
        /// </summary>
        private void CreateJSFile()
        {
            using (TextWriter textWriterJS = new StreamWriter(this.fileName.Replace(".xml", ".js")))
            {
                StringBuilder oWorldData = new StringBuilder("{");
                StringBuilder countryNames = new StringBuilder("\"");
                foreach (var country in this.map.Countries)
                {
                    oWorldData.Append("\"");
                    oWorldData.Append(country.Name);
                    oWorldData.Append("\":");
                    oWorldData.Append("[[");
                    oWorldData.Append(this.CreateJavaScriptArrayOfPoints(country));
                    oWorldData.Append("]]");
                    countryNames.Append(country.Name);
                    if (country != this.map.Countries.Last())
                    {
                        countryNames.Append(",");
                        oWorldData.Append(",");
                    }
                }

                oWorldData.Append("}");
                countryNames.Append("\"");
                string js = this.GetDefaultJS();
                js = js.Replace("#PlaceHolderOMapData#", oWorldData.ToString());
                js = js.Replace("#PlaceHolderCountryNames#", countryNames.ToString());
                textWriterJS.Write(js);
            }
        }

        /// <summary>
        /// Creates the HTML File
        /// </summary>
        private void CreateHTMLFile()
        {
            using (TextWriter textWriterHTML = new StreamWriter(this.fileName.Replace(".xml", ".html")))
            {
                string html = this.GetDefaultHTML();
                html = html.Replace("#PlaceHolderJSFileName#", Path.GetFileName(this.fileName.Replace(".xml", ".js")));
                StringBuilder countryNames = new StringBuilder();
                var random = new Random();
                foreach (var country in this.map.Countries)
                {
                    countryNames.Append("\"");
                    countryNames.Append(country.Name);
                    countryNames.Append("\": \"");
                    countryNames.Append(string.Format("#{0:X6}", random.Next(0x1000000)));
                    countryNames.Append("\",");
                }

                html = html.Replace("#PlaceHolderDetail#", countryNames.ToString());
                textWriterHTML.Write(html);
            }
        }

        /// <summary>
        /// The base HTML file is an emmbedded resource.
        /// </summary>
        /// <returns> A string with the content of the base HTML file.</returns>
        private string GetDefaultHTML()
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DrawMap." + "map-example-jmd.html"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// The base JS file is an emmbedded resource.
        /// </summary>
        /// <returns> A string with the content of the base JS file.</returns>
        private string GetDefaultJS()
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DrawMap." + "map-jmd.js"))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Create the XML file.
        /// </summary>
        private void CreateXMLFile()
        {
            using (TextWriter textWriterXML = new StreamWriter(this.fileName))
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Map));
                xmlSerializer.Serialize(textWriterXML, this.map);
            }
        }
    }
}
