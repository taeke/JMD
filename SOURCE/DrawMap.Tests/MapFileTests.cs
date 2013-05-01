namespace DrawMap.Tests
{
    using System;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The tests for the Map class.
    /// </summary>
    [TestClass]
    public class MapFileTests
    {
        /// <summary>
        /// The instance of the class under test.
        /// </summary>
        private MapFiles map;

        /// <summary>
        /// Initializing for every test.
        /// </summary>
        [TestInitialize]
        public void MapInitialize()
        {
            this.map = new MapFiles();
        }

        /// <summary>
        /// Tests for the FileName property.
        /// </summary>
        [TestClass]
        public class TheFileNameProperty : MapFileTests
        {
            /// <summary>
            /// Test if setting FileName sets MapChanged to True.
            /// </summary>
            [TestMethod]
            public void FillFileNameShouldSetMapChangedTrue()
            {
                // Arange

                // Act
                this.map.FileName = "test.xml";

                // Assert
                Assert.IsTrue(this.map.MapChanged);
            }
        }

        /// <summary>
        /// The tests for the Save Method.
        /// </summary>
        [TestClass]
        public class TheSaveMethod : MapFileTests
        {
            /// <summary>
            /// The Save Method should throw an InvalidOperationException if MayOverwriteExcisting is false but the file excists.
            /// </summary>
            [TestMethod, ExpectedException(typeof(InvalidOperationException)), TestCategory("Integration")]
            public void ShouldThrowExceptionIfMayOverwriteExcistingIsFalseButFileExcists()
            {
                // Arange
                this.map.MayOverwriteExcisting = false;
                this.map.FileName = "test.xml";

                // Act
                this.map.Save();

                // Assert
                // Assertion is done bij ExpectedException attribute.
            }

            /// <summary>
            /// The Save Method should throw an InvalidOperationException if the FileName not is filled.
            /// </summary>
            [TestMethod, ExpectedException(typeof(InvalidOperationException))]
            public void ShouldThrowExceptionIfFilnameIsNotFilled()
            {
                // Arange

                // Act
                this.map.Save();

                // Assert
                // Assertion is done bij ExpectedException attribute.
            }
            
            /// <summary>
            /// Test if the Save method sets MapChanged to false.
            /// </summary>
            [TestMethod, TestCategory("Integration")]
            public void ShouldSetMapChangedFalse()
            {
                // Arange
                this.map.FileName = "test2.xml";
                this.map.MayOverwriteExcisting = true;

                // Act
                this.map.Save();

                // Assert
                Assert.IsFalse(this.map.MapChanged);
            }

            /// <summary>
            /// Test if the Save method sets MayOverwriteExcisting to false.
            /// </summary>
            [TestMethod, TestCategory("Integration")]
            public void ShouldSetIgnoreChangesFalse()
            {
                // Arange
                this.map.FileName = "test2.xml";
                this.map.MayOverwriteExcisting = true;
                this.map.IgnoreChanges = true;

                // Act
                this.map.Save();

                // Assert
                Assert.IsFalse(this.map.IgnoreChanges);
            }

            /// <summary>
            /// Test if saving set MayOverwriteExcisting True.
            /// </summary>
            [TestMethod, TestCategory("Integration")]
            public void ShouldSetMayOverwriteExcistingTrue()
            {
                // Arange
                File.Delete("test2.xml");
                this.map.FileName = "test2.xml";
                this.map.MayOverwriteExcisting = false;
                this.map.IgnoreChanges = true;

                // Act
                this.map.Save();

                // Assert
                Assert.IsTrue(this.map.MayOverwriteExcisting);
            }

            /// <summary>
            /// Test if Save really creates a xml file.
            /// </summary>
            [TestMethod, TestCategory("Integration")]
            public void ShouldCreateXMLFile()
            {
                // Arange
                const string FileName = "test6.xml";
                this.map.FileName = FileName;
                this.map.MayOverwriteExcisting = true;

                // Act
                this.map.Save();

                // Assert
                File.Exists(FileName);
            }
        }

        /// <summary>
        /// The tests for the New Method.
        /// </summary>
        [TestClass]
        public class TheNewMethod : MapFileTests
        {
            /// <summary>
            /// Test if calling New sets MapChanged false.
            /// </summary>
            [TestMethod]
            public void ShouldSetMapChangedFalse()
            {
                // Arrange
                this.map.FileName = "test.xml";
                this.map.IgnoreChanges = true;

                // Act
                this.map.New();

                // Assert
                Assert.IsFalse(this.map.MapChanged);
            }

            /// <summary>
            /// Test if calling New sets MayOverwriteExcisting false.
            /// </summary>
            [TestMethod]
            public void ShouldSetMayOverwriteExcistingFalse()
            {
                // Arrange
                this.map.FileName = "test.xml";
                this.map.IgnoreChanges = true;
                this.map.MayOverwriteExcisting = true;

                // Act
                this.map.New();

                // Assert
                Assert.IsFalse(this.map.MapChanged);
            }

            /// <summary>
            /// Test if calling New sets IgnoreChanges false.
            /// </summary>
            [TestMethod]
            public void ShouldSetIgnoreChangesFalse()
            {
                // Arrange
                this.map.FileName = "test.xml";
                this.map.IgnoreChanges = true;

                // Act
                this.map.New();

                // Assert
                Assert.IsFalse(this.map.MapChanged);
            }

            /// <summary>
            /// Calling New should empty the FileName
            /// </summary>
            [TestMethod]
            public void ShouldSetFileNameEmpty()
            {
                // Arrange
                this.map.FileName = "test.xml";
                this.map.IgnoreChanges = true;

                // Act
                this.map.New();
                
                // Assert
                Assert.AreEqual(string.Empty, this.map.FileName);
            }

            /// <summary>
            /// If IgnoreChanges is NOT true and there are changes then calling New Should throw an InvalidOperationException.
            /// </summary>
            [TestMethod, ExpectedException(typeof(InvalidOperationException))]
            public void ShouldThrowExceptionIfIgnoreChangesFalseAndThereAreChanges()
            {
                // Arrange
                this.map.FileName = "test.xml";

                // Act
                this.map.New();

                // Assert
                // Assertion is done bij ExpectedException attribute.
            }
        }
        
        /// <summary>
        /// The tests for the Open Method.
        /// </summary>
        [TestClass]
        public class TheOpenMethod : MapFileTests
        {
            /// <summary>
            /// Test if opening a file sets the MayOverWriteExcisting.
            /// </summary>
            [TestMethod, TestCategory("Integration")]
            public void ShouldSetMayOverWriteExcistingSetTrue()
            {
                // Arange

                // Act
                this.map.Open("test.xml");

                // Assert
                Assert.IsTrue(this.map.MayOverwriteExcisting);
            }

            /// <summary>
            /// Test if Open throws an InvalidOperationException als MapChanged is true;
            /// </summary>
            [TestMethod, ExpectedException(typeof(InvalidOperationException))]
            public void ShouldThrowExceptionIfMapChanged()
            {
                // Arange
                this.map.FileName = "test.xml";

                // Act
                this.map.Open("test.xml");

                // Assert
                // Assertion is done bij ExpectedException attribute.
            }

            /// <summary>
            /// Test if Open throws an InvalidOperarionException if the FileName does not excist.
            /// </summary>
            [TestMethod, ExpectedException(typeof(InvalidOperationException)), TestCategory("Integration")]
            public void ShouldThrowExceptionIfFileNameDoesNotExcist()
            {
                // Arrange

                // Act
                this.map.Open("test3.xml");

                // Assert
                // Assertion is done bij ExpectedException attribute.
            }

            /// <summary>
            /// Test if Open sets the FileName propery.
            /// </summary>
            [TestMethod, TestCategory("Integration")]
            public void ShouldSetFileName()
            {
                // Arange
                const string FileName = "test.xml";

                // Act
                this.map.Open(FileName);

                // Assert
                Assert.AreEqual(FileName, this.map.FileName);
            }

            /// <summary>
            /// Test if calling Open with a null fileName throws an ArgumentNullException.
            /// </summary>
            [TestMethod, ExpectedException(typeof(ArgumentNullException))]
            public void ShouldThrowExceptionIfFileNameIsNull()
            {
                // Arrange
                
                // Act
                this.map.Open(null);

                // Assert
                // Assertion is done bij ExpectedException attribute.
            }

            /// <summary>
            /// Test if calling Open with a empty string as fileName throws an ArgumentException.
            /// </summary>
            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowExceptionIfFileNameIsEmpty()
            {
                // Arrange

                // Act
                this.map.Open(string.Empty);

                // Assert
                // Assertion is done bij ExpectedException attribute.
            }
        }
    }
}
