﻿namespace DrawMap.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using DrawMap.Interface;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// The tests for the Map class.
    /// </summary>
    [TestClass]
    public class MapFileTests
    {
        /// <summary>
        /// The instance of the class under test.
        /// </summary>
        private MapFiles mapFiles;

        /// <summary>
        /// Initializing for every test.
        /// </summary>
        [TestInitialize]
        public void MapInitialize()
        {
            this.mapFiles = new MapFiles();
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
                this.mapFiles.FileName = "test.xml";

                // Assert
                Assert.IsTrue(this.mapFiles.MapChanged);
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
                this.mapFiles.MayOverwriteExcisting = false;
                this.mapFiles.FileName = "JMD.xml";

                // Act
                this.mapFiles.Save();

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
                this.mapFiles.Save();

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
                this.mapFiles.FileName = "JMD2.xml";
                this.mapFiles.MayOverwriteExcisting = true;

                // Act
                this.mapFiles.Save();

                // Assert
                Assert.IsFalse(this.mapFiles.MapChanged);
            }

            /// <summary>
            /// Test if the Save method sets MayOverwriteExcisting to false.
            /// </summary>
            [TestMethod, TestCategory("Integration")]
            public void ShouldSetIgnoreChangesFalse()
            {
                // Arange
                this.mapFiles.FileName = "JMD2.xml";
                this.mapFiles.MayOverwriteExcisting = true;
                this.mapFiles.IgnoreChanges = true;

                // Act
                this.mapFiles.Save();

                // Assert
                Assert.IsFalse(this.mapFiles.IgnoreChanges);
            }

            /// <summary>
            /// Test if saving set MayOverwriteExcisting True.
            /// </summary>
            [TestMethod, TestCategory("Integration")]
            public void ShouldSetMayOverwriteExcistingTrue()
            {
                // Arange
                File.Delete("JMD2.xml");
                this.mapFiles.FileName = "JMD2.xml";
                this.mapFiles.MayOverwriteExcisting = false;
                this.mapFiles.IgnoreChanges = true;

                // Act
                this.mapFiles.Save();

                // Assert
                Assert.IsTrue(this.mapFiles.MayOverwriteExcisting);
            }

            /// <summary>
            /// Test if Save really creates a xml file.
            /// </summary>
            [TestMethod, TestCategory("Integration")]
            public void ShouldCreateXMLFile()
            {
                // Arange
                const string FileName = "JMD6.xml";
                this.mapFiles.FileName = FileName;
                this.mapFiles.MayOverwriteExcisting = true;

                // Act
                this.mapFiles.Save();

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
                this.mapFiles.FileName = "JMD.xml";
                this.mapFiles.IgnoreChanges = true;

                // Act
                this.mapFiles.New();

                // Assert
                Assert.IsFalse(this.mapFiles.MapChanged);
            }

            /// <summary>
            /// Test if calling New sets MayOverwriteExcisting false.
            /// </summary>
            [TestMethod]
            public void ShouldSetMayOverwriteExcistingFalse()
            {
                // Arrange
                this.mapFiles.FileName = "JMD.xml";
                this.mapFiles.IgnoreChanges = true;
                this.mapFiles.MayOverwriteExcisting = true;

                // Act
                this.mapFiles.New();

                // Assert
                Assert.IsFalse(this.mapFiles.MapChanged);
            }

            /// <summary>
            /// Test if calling New sets IgnoreChanges false.
            /// </summary>
            [TestMethod]
            public void ShouldSetIgnoreChangesFalse()
            {
                // Arrange
                this.mapFiles.FileName = "JMD.xml";
                this.mapFiles.IgnoreChanges = true;

                // Act
                this.mapFiles.New();

                // Assert
                Assert.IsFalse(this.mapFiles.MapChanged);
            }

            /// <summary>
            /// Calling New should empty the FileName
            /// </summary>
            [TestMethod]
            public void ShouldSetFileNameEmpty()
            {
                // Arrange
                this.mapFiles.FileName = "JMD.xml";
                this.mapFiles.IgnoreChanges = true;

                // Act
                this.mapFiles.New();

                // Assert
                Assert.AreEqual(string.Empty, this.mapFiles.FileName);
            }

            /// <summary>
            /// If IgnoreChanges is NOT true and there are changes then calling New Should throw an InvalidOperationException.
            /// </summary>
            [TestMethod, ExpectedException(typeof(InvalidOperationException))]
            public void ShouldThrowExceptionIfIgnoreChangesFalseAndThereAreChanges()
            {
                // Arrange
                this.mapFiles.FileName = "JMD.xml";

                // Act
                this.mapFiles.New();

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
                this.mapFiles.Open("JMD.xml");

                // Assert
                Assert.IsTrue(this.mapFiles.MayOverwriteExcisting);
            }

            /// <summary>
            /// Test if Open throws an InvalidOperationException als MapChanged is true;
            /// </summary>
            [TestMethod, ExpectedException(typeof(InvalidOperationException))]
            public void ShouldThrowExceptionIfMapChanged()
            {
                // Arange
                this.mapFiles.FileName = "JMD.xml";

                // Act
                this.mapFiles.Open("JMD.xml");

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
                this.mapFiles.Open("JMD3.xml");

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
                const string FileName = "JMD.xml";

                // Act
                this.mapFiles.Open(FileName);

                // Assert
                Assert.AreEqual(FileName, this.mapFiles.FileName);
            }

            /// <summary>
            /// Test if calling Open with a null fileName throws an ArgumentNullException.
            /// </summary>
            [TestMethod, ExpectedException(typeof(ArgumentNullException))]
            public void ShouldThrowExceptionIfFileNameIsNull()
            {
                // Arrange

                // Act
                this.mapFiles.Open(null);

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
                this.mapFiles.Open(string.Empty);

                // Assert
                // Assertion is done bij ExpectedException attribute.
            }

            /// <summary>
            /// If IgnoreChanges is NOT true and there are changes then calling Open Should throw an InvalidOperationException.
            /// </summary>
            [TestMethod, ExpectedException(typeof(InvalidOperationException))]
            public void ShouldThrowExceptionIfIgnoreChangesFalseAndThereAreChanges()
            {
                // Arange
                const string FileName = "JMD.xml";
                this.mapFiles.FileName = FileName;

                // Act
                this.mapFiles.Open(FileName);

                // Assert
                // Assertion is done bij ExpectedException attribute.
            }
        }

        /// <summary>
        /// Tests for the AddBorderEndPoint method.
        /// </summary>
        [TestClass]
        public class TheAddBorderEndPointMethod : MapFileTests
        {
            /// <summary>
            /// Test if calling AddBorderEndPoint sets MapChanged to true.
            /// </summary>
            [TestMethod]
            public void ShouldSetMapChangedTrue()
            {
                // Arange

                // Act
                int number = this.mapFiles.AddBorderPoint(1, 1);

                // Assert.
                Assert.AreEqual(true, this.mapFiles.MapChanged);
            }

            /// <summary>
            /// Test if calling AddBordEndPoint returns the number we configured in the mock.
            /// </summary>
            [TestMethod]
            public void ShouldReturnANumber()
            {
                // Arange

                // Act
                int number = this.mapFiles.AddBorderPoint(1, 1);

                // Assert the mock is configureted to return 1.
                Assert.AreEqual(1, number);
            }
        }

        /// <summary>
        /// Tests for the AddBorder method.
        /// </summary>
        [TestClass]
        public class TheAddCountryBorderMethod : MapFileTests
        {
            /// <summary>
            /// Test if calling AddBorder sets MapChanged to true.
            /// </summary>
            [TestMethod]
            public void ShouldSetMapChangedTrue()
            {
                // Arange
                int[] numbers = new int[2];
                numbers[0] = this.mapFiles.AddBorderPoint(1, 1);
                numbers[1] = this.mapFiles.AddBorderPoint(2, 2);

                // Act
                this.mapFiles.AddCountryBorder(numbers);

                // Assert the mock is configureted to return 1.
                Assert.AreEqual(true, this.mapFiles.MapChanged);
            }

            /// <summary>
            /// Test if calling AddCountryBorder throws an ArgumentException if both numbers are the same.
            /// </summary>
            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowExceptionIfBothNumbersAreSame()
            {
                // Arange
                int[] numbers = new int[2];
                numbers[0] = this.mapFiles.AddBorderPoint(1, 1);
                numbers[1] = numbers[0];

                // Act
                this.mapFiles.AddCountryBorder(numbers);

                // Assert
                // Assertion is done bij ExpectedException attribute.
            }

            /// <summary>
            /// Test if calling AddCountryBorder throws an ArgumentException if same border allready is added.
            /// </summary>
            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowExceptionIfSameBorderAllreadyAdded()
            {
                // Arange
                int[] numbers = new int[2];
                numbers[0] = this.mapFiles.AddBorderPoint(1, 1);
                numbers[1] = this.mapFiles.AddBorderPoint(2, 2);
                this.mapFiles.AddCountryBorder(numbers);

                // Act
                this.mapFiles.AddCountryBorder(numbers);

                // Assert
                // Assertion is done bij ExpectedException attribute.
            }

            /// <summary>
            /// Test if calling AddCountryBorder throws an ArgumentException if the first number is higher as the second.
            /// </summary>
            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowExceptionIfNumbersAreNotInTheRightOrder()
            {
                // Arange
                int number1 = this.mapFiles.AddBorderPoint(1, 1);
                int number2 = this.mapFiles.AddBorderPoint(2, 2);
                int[] numbers = new int[2];
                numbers[0] = number2;
                numbers[1] = number1;

                // Act
                this.mapFiles.AddCountryBorder(numbers);

                // Assert
                // Assertion is done bij ExpectedException attribute.
            }

            /// <summary>
            /// Test if calling AddCountryBorder throws an ArgumentException if one of the BorderEndPoints does not excist.
            /// </summary>
            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowExceptionIfOneOfTheBorderEndPointsDoNoExcist()
            {
                // Arange
                int[] numbers = new int[2];
                numbers[0] = this.mapFiles.AddBorderPoint(1, 1);
                numbers[1] = 2;

                // Act
                this.mapFiles.AddCountryBorder(numbers);

                // Assert
                // Assertion is done bij ExpectedException attribute.
            }

            /// <summary>
            /// Test if calling AddCountryBorder throws an ArgumentException if the new CountryBorder intersects with another one.
            /// </summary>
            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowExceptionIfBorderIntersectsWithOtherBorder()
            {
                // Arange
                int[] numbers1 = new int[2];
                numbers1[0] = this.mapFiles.AddBorderPoint(10, 10);
                numbers1[1] = this.mapFiles.AddBorderPoint(20, 20);
                this.mapFiles.AddCountryBorder(numbers1);

                int[] numbers2 = new int[2];
                numbers2[0] = this.mapFiles.AddBorderPoint(10, 20);
                numbers2[1] = this.mapFiles.AddBorderPoint(20, 10);

                // Act
                this.mapFiles.AddCountryBorder(numbers2);

                // Assert
                // Assertion is done bij ExpectedException attribute.
            }
        }

        /// <summary>
        /// The tests for the RemoveEndPoint method.
        /// </summary>
        [TestClass]
        public class TheRemoveEndPointMethod : MapFileTests
        {
            /// <summary>
            /// If RemoveEndPoint is calles with an invalid number it should throw an ArgumentOutOfRangeException
            /// </summary>
            [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
            public void ShouldThrowExceptionIfNumberDoesNotExicst()
            {
                // Arange

                // Act
                this.mapFiles.RemoveBorderPoint(5);

                // Assert
                // Assertion is done bij ExpectedException attribute.
            }

            /// <summary>
            /// Test if calling RemoveEndPoint while the BorderEndPoint is part of an excisting CountryBorder throws an ArgumentException.
            /// </summary>
            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowExceptionIfEndPointIsPartOfCountryBorder()
            {
                // Arange
                int pointToRemove = this.mapFiles.AddBorderPoint(10, 10);
                int secondPoint = this.mapFiles.AddBorderPoint(20, 20);
                this.mapFiles.AddCountryBorder(new int[] { pointToRemove, secondPoint });

                // Act
                this.mapFiles.RemoveBorderPoint(pointToRemove);

                // Assert
                // Assertion is done bij ExpectedException attribute.
            }
        }

        /// <summary>
        /// The tests for the RemoveEndPoint method.
        /// </summary>
        [TestClass]
        public class TheGetBorderEndPointsMethod : MapFileTests
        {
            /// <summary>
            /// A call to GetBorderEndPoints should return a list of <see cref="BorderPoint"/>.
            /// </summary>
            [TestMethod]
            public void ShouldReturnAListWithBordEndPoints()
            {
                // Arange
                this.mapFiles.AddBorderPoint(1, 1);

                // Act
                var endPoints = this.mapFiles.GetBorderPoints();

                // Assert
                Assert.IsNotNull(endPoints);
                Assert.AreEqual(1, endPoints.Count);
            }
        }

        /// <summary>
        /// The tests for the RemoveEndPoint method.
        /// </summary>
        [TestClass]
        public class TheInsertBorderPointMethod : MapFileTests
        {
            /// <summary>
            /// Test if calling InsertBorderPoint sets MapChanged to true.
            /// </summary>
            [TestMethod]
            public void ShouldSetMapChangedTrue()
            {
                // Arange
                int[] numbers = new int[2];
                numbers[0] = this.mapFiles.AddBorderPoint(10, 10);
                numbers[1] = this.mapFiles.AddBorderPoint(30, 30);
                this.mapFiles.AddCountryBorder(numbers);
                int number3 = this.mapFiles.AddBorderPoint(20, 20);

                // Act
                this.mapFiles.InsertBorderPoint(numbers, number3);

                // Assert.
                Assert.AreEqual(true, this.mapFiles.MapChanged);
            }

            /// <summary>
            /// Test if calling addBorder throws an exception if the two endpoints are the same.
            /// </summary>
            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowExceptionIfBothBorderEndPointNumbersAreSame()
            {
                // Arange
                int[] numbers = new int[2];
                numbers[0] = this.mapFiles.AddBorderPoint(1, 1);
                numbers[1] = numbers[0];
                this.mapFiles.AddCountryBorder(numbers);
                int number3 = this.mapFiles.AddBorderPoint(3, 3);

                // Act
                this.mapFiles.InsertBorderPoint(numbers, number3);

                // Assert
                // Assertion is done bij ExpectedException attribute.
            }

            /// <summary>
            /// Test if calling AddBorderPart throws an ArgumentException if the first number is higher as the second.
            /// </summary>
            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowExceptionIfBorderEndPointNumbersAreNotInTheRightOrder()
            {
                // Arange
                int[] numbers = new int[2];
                numbers[1] = this.mapFiles.AddBorderPoint(1, 1);
                numbers[0] = this.mapFiles.AddBorderPoint(2, 2);
                this.mapFiles.AddCountryBorder(numbers);
                int number3 = this.mapFiles.AddBorderPoint(3, 3);

                // Act
                this.mapFiles.InsertBorderPoint(numbers, number3);

                // Assert.
                // Assertion is done bij ExpectedException attribute.
            }

            /// <summary>
            /// Test if calling AddBorderPart throws an ArgumentException if the first number is higher as the second.
            /// </summary>
            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowExceptionIfBorderPointNumbersAreNotInTheRightOrder()
            {
                // Arange
                int[] numbers = new int[2];
                numbers[0] = this.mapFiles.AddBorderPoint(1, 1);
                int number2 = this.mapFiles.AddBorderPoint(2, 2);
                int number3 = this.mapFiles.AddBorderPoint(3, 3);
                numbers[1] = number3;
                this.mapFiles.AddCountryBorder(numbers);

                // Act
                this.mapFiles.InsertBorderPoint(numbers, number2);

                // Assert.
                // Assertion is done bij ExpectedException attribute.
            }

            /// <summary>
            /// Test if calling AddBorderPart throws an ArgumentException if the CountryBorder does not excist.
            /// </summary>
            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowExceptionIfCountryBorderDoesNotExcist()
            {
                // Arange
                int[] numbers = new int[2];
                numbers[0] = this.mapFiles.AddBorderPoint(1, 1);
                numbers[1] = this.mapFiles.AddBorderPoint(2, 2);
                int number3 = this.mapFiles.AddBorderPoint(3, 3);

                // Act
                this.mapFiles.InsertBorderPoint(numbers, number3);

                // Assert.
                // Assertion is done bij ExpectedException attribute.
            }

            /// <summary>
            /// Test if calling AddBorderPart throws an ArgumentException if the new number allready is part of a borderpart does not excist.
            /// </summary>
            [TestMethod, ExpectedException(typeof(ArgumentException))]
            public void ShouldThrowExceptionIfNumberAllreadyInBorder()
            {
                // Arange
                int[] numbers = new int[2];
                numbers[0] = this.mapFiles.AddBorderPoint(1, 1);
                numbers[1] = this.mapFiles.AddBorderPoint(2, 2);
                this.mapFiles.AddCountryBorder(numbers);

                // Act
                this.mapFiles.InsertBorderPoint(numbers, numbers[1]);

                // Assert.
                // Assertion is done bij ExpectedException attribute.
            }
        }
    }
}
