namespace JMD
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using DrawMap;
    using DrawMap.Interface;
    using Microsoft.Win32;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The diameter for the Ellipses which are a visual representation of the BorderEndPoints.
        /// </summary>
        private const int PointDiameter = 6;

        /// <summary>
        /// The diameter for the selection. If a item falls whitin this diameter there is not a new item drawn but this item is selected.
        /// </summary>
        private const int SelectionDiameter = 6;

        /// <summary>
        /// The <see cref="MapFiles"/> instance.
        /// </summary>
        private IMapFiles mapFiles;

        /// <summary>
        /// Resuls for the mouseClick on the drawing surface.
        /// </summary>
        private List<DependencyObject> hitResultsList = new List<DependencyObject>();

        /// <summary>
        /// The list of <see cref="BorderItem"/>.
        /// </summary>
        private List<BorderItem> borderItems = new List<BorderItem>();

        /// <summary>
        /// Initializes the window.
        /// </summary>
        /// <param name="mapFiles"> the IMapFiles instance. </param>
        public MainWindow(IMapFiles mapFiles)
        {
            this.mapFiles = mapFiles;
            this.InitializeComponent();
            this.DeleteBorderEndPoint.IsEnabled = false;
            this.BordersGrid.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Opening an excisting file.
        /// </summary>
        /// <param name="sender"> The menu clicked. </param>
        /// <param name="e"> The <see cref="RoutedEventArges"/> instance. </param>
        private void MenuItem_Open_Click(object sender, RoutedEventArgs e)
        {
            this.CheckChanges();
            if (!this.mapFiles.MapChanged || this.mapFiles.IgnoreChanges)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.DefaultExt = ".xml";
                openFileDialog.Filter = "XML documents (.xml)|*.xml";
                bool? resultOpen = openFileDialog.ShowDialog();
                if (resultOpen == true)
                {
                    this.OpenFile(openFileDialog.FileName);
                }
            }
        }

        /// <summary>
        /// Creating an new file.
        /// </summary>
        /// <param name="sender"> The menu clicked. </param>
        /// <param name="e"> The <see cref="RoutedEventArges"/> instance. </param>
        private void MenuItem_New_Click(object sender, RoutedEventArgs e)
        {
            this.CheckChanges();
            if (!this.mapFiles.MapChanged || this.mapFiles.IgnoreChanges)
            {
                this.DrawingSurface.Children.Clear();
                this.BorderEndPoints.Items.Clear();
                this.BorderEndPoint1.Items.Clear();
                this.BorderEndPoint2.Items.Clear();
                this.borderItems.Clear();
                this.mapFiles.New();
                this.ShowBorderEndPointTools.IsChecked = true;
            }
        }

        /// <summary>
        /// Saving an file.
        /// </summary>
        /// <param name="sender"> The menu clicked. </param>
        /// <param name="e"> The <see cref="RoutedEventArges"/> instance. </param>
        private void MenuItem_Save_Click(object sender, RoutedEventArgs e)
        {
            bool isCanceld = false;
            if (this.mapFiles.FileName == string.Empty || this.mapFiles.FileName == null)
            {
                isCanceld = !this.CheckFileName();
            }

            if (!isCanceld)
            {
                isCanceld = !this.CheckOverwrite();
            }

            if (!isCanceld)
            {
                this.mapFiles.Save();
            }
        }

        /// <summary>
        /// Saving an file under another name.
        /// </summary>
        /// <param name="sender"> The menu clicked. </param>
        /// <param name="e"> The <see cref="RoutedEventArges"/> instance. </param>
        private void MenuItem_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            bool isCanceld = !this.CheckFileName();
            if (!isCanceld)
            {
                isCanceld = !this.CheckOverwrite();
            }

            if (!isCanceld)
            {
                this.mapFiles.Save();
            }
        }

        /// <summary>
        /// The user clicked somewhere on the drawing surface and wants to draw something.
        /// </summary>
        /// <param name="sender"> The clicked drawing surface. </param>
        /// <param name="e"> The <see cref="MouseButtonEventArgs"/> instance. </param>
        private void DrawingSurface_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.ShowBorderEndPointTools != null && (bool)this.ShowBorderEndPointTools.IsChecked)
            {
                this.DrawOrSelectBorderEndPoint(e.GetPosition((UIElement)sender));
            }
        }

        /// <summary>
        /// The user clicked the Create CountryBorder Button.
        /// </summary>
        /// <param name="sender"> The clicked button. </param>
        /// <param name="e"> The <see cref="RoutedEventArgs"/> instance. </param>
        private void CreateCountryBorder_Click(object sender, RoutedEventArgs e)
        {
            if (!this.CheckIntersection())
            {
                this.CreateNewBorder();
                this.CreateCountryBorder.IsEnabled = false;
                this.DeleteCountryBorder.IsEnabled = true;
                this.ReDraw();
            }
        }

        /// <summary>
        /// The user selected another BorderEndpoint.
        /// </summary>
        /// <param name="sender"> The combobox containing the borderendpoints </param>
        /// <param name="e"> The <see cref="SelectionChangedEventArgs"/> instance. </param>
        private void BorderEndPoints_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.DeleteBorderEndPoint.IsEnabled = this.BorderEndPoints.SelectedItem != null;
            this.ReDraw();
        }

        /// <summary>
        /// The user clicked the delete BorderEndPoint button.
        /// </summary>
        /// <param name="sender"> The button. </param>
        /// <param name="e"> The <see cref="RoutedEventArgs"/> instance. </param>
        private void DeleteBorderEndPoint_Click(object sender, RoutedEventArgs e)
        {
            if (this.borderItems.Find(b => b.Numbers.Contains(((BorderEndPointItem)this.BorderEndPoints.SelectedItem).Number)) == null)
            {
                this.mapFiles.RemoveEndPoint(((BorderEndPointItem)this.BorderEndPoints.SelectedItem).Number);
                this.DrawingSurface.Children.Remove(((BorderEndPointItem)this.BorderEndPoints.SelectedItem).Ellipse);
                BorderEndPointItem borderEndPointItem = (BorderEndPointItem)this.BorderEndPoints.SelectedItem;
                this.BorderEndPoints.Items.Remove(borderEndPointItem);
                this.BorderEndPoint1.Items.Remove(borderEndPointItem);
                this.BorderEndPoint2.Items.Remove(borderEndPointItem);
            }
            else
            {
                MessageBox.Show("Can't delete a BorderEndPoint which is part of a CountryBorder");
            }
        }

        /// <summary>
        /// The user clicked the delete CountryBorder button.
        /// </summary>
        /// <param name="sender"> The button. </param>
        /// <param name="e"> The <see cref="RoutedEventArgs"/> instance. </param>
        private void DeleteCountryBorder_Click(object sender, RoutedEventArgs e)
        {
            int[] numbers = this.GetNumbers();
            this.mapFiles.RemoveCountryBorder(numbers);
            BorderItem borderItem = this.borderItems.Find(b => b.Numbers[0] == numbers[0] && b.Numbers[1] == numbers[1]);
            this.DrawingSurface.Children.Remove(borderItem.Line);
            this.borderItems.Remove(borderItem);
            this.CreateCountryBorder.IsEnabled = true;
            this.DeleteCountryBorder.IsEnabled = false;
            this.ReDraw();
        }

        /// <summary>
        /// The user tries to close the application.
        /// </summary>
        /// <param name="sender"> The controle which was clicked to close the application. </param>
        /// <param name="e"> The <see cref="CancelEventArgs"/> instance. </param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            this.CheckChanges();
            if (this.mapFiles.MapChanged && !this.mapFiles.IgnoreChanges)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// The user choose another drawing tool. Make the right grid visible.
        /// </summary>
        /// <param name="sender"> The radiobutton clicked. </param>
        /// <param name="e"> The <see cref="RoutedEventArgs"/> instance. </param>
        private void DrawingTools_Checked(object sender, RoutedEventArgs e)
        {
            if (this.ShowBorderTools != null && this.BordersGrid != null)
            {
                this.BordersGrid.Visibility = (bool)this.ShowBorderTools.IsChecked ? Visibility.Visible : Visibility.Collapsed;
            }

            if (this.ShowBorderEndPointTools != null && this.BorderEndPointsGrid != null)
            {
                this.BorderEndPointsGrid.Visibility = (bool)this.ShowBorderEndPointTools.IsChecked ? Visibility.Visible : Visibility.Collapsed;
            }

            this.ReDraw();
        }

        /// <summary>
        /// The user choose another endpoint for the <see cref="CountryBorder"/>.
        /// </summary>
        /// <param name="sender"> The combobox with the endpoints. </param>
        /// <param name="e"> The <see cref="SelectionChangedEventArgs"/> instance. </param>
        private void BorderEndPoint1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.EndPointsForBorderSelectionChanged(this.BorderEndPoint1, this.BorderEndPoint2);
        }

        /// <summary>
        /// The user choose another endpoint for the <see cref="CountryBorder"/>.
        /// </summary>
        /// <param name="sender"> The combobox with the endpoints. </param>
        /// <param name="e"> The <see cref="SelectionChangedEventArgs"/> instance. </param>
        private void BorderEndPoint2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.EndPointsForBorderSelectionChanged(this.BorderEndPoint2, this.BorderEndPoint1);
        }

        /// <summary>
        /// The user choose another endpoint for the <see cref="CountryBorder"/>.
        /// </summary>
        /// <param name="changed"> The combobox which changed. </param>
        /// <param name="other"> The combobox for the other endpoint for the <see cref="CountryBorder"/>. </param>
        private void EndPointsForBorderSelectionChanged(ComboBox changed, ComboBox other)
        {
            foreach (BorderEndPointItem item in this.BorderEndPoint2.Items)
            {
                item.VisibleIn1 = changed == this.BorderEndPoint1 ? item.VisibleIn1 : Visibility.Visible;
                item.VisibleIn2 = changed == this.BorderEndPoint2 ? item.VisibleIn2 : Visibility.Visible;
            }

            if (changed.SelectedItem != null)
            {
                BorderEndPointItem item = (BorderEndPointItem)changed.SelectedItem;
                item.VisibleIn1 = changed == this.BorderEndPoint1 ? item.VisibleIn1 : Visibility.Collapsed;
                item.VisibleIn2 = changed == this.BorderEndPoint2 ? item.VisibleIn2 : Visibility.Collapsed;
            }

            other.Items.Refresh();
            this.CreateCountryBorder.IsEnabled = this.NotCreatedCountryBorderSelected();
            this.DeleteCountryBorder.IsEnabled = !this.CreateCountryBorder.IsEnabled && 
                                                 this.BorderEndPoint1.SelectedItem != null && 
                                                 this.BorderEndPoint2.SelectedItem != null;
            this.ReDraw();
        }

        /// <summary>
        /// This selects or draws a new <see cref="BorderEndPoint"/>
        /// </summary>
        /// <param name="clickedPoint"> The point where the user clicked. </param>
        private void DrawOrSelectBorderEndPoint(Point clickedPoint)
        {
            EllipseGeometry expandedHitTestArea = new EllipseGeometry(clickedPoint, 10, 10);
            this.hitResultsList.Clear();
            VisualTreeHelper.HitTest(
                this.DrawingSurface,
                null,
                new HitTestResultCallback(this.HitTestResultCallback),
                new GeometryHitTestParameters(expandedHitTestArea));
            if (this.hitResultsList.Count > 0)
            {
                this.SelectBorderEndPoint();
            }
            else
            {
                this.CreateNewBorderEndPoint(clickedPoint);
            }
        }

        /// <summary>
        /// Check if the <see cref="BorderEndPoint"/> for a <see cref="CountryBorder"/> both are selected and there is NOT
        /// allready created a <see cref="CountryBorder"/> for this combination.
        /// </summary>
        /// <returns> True if a combination of <see cref="BorderEndPoint"/> is chosen that isn't allready a <see cref="CountryBorder"/>. </returns>
        private bool NotCreatedCountryBorderSelected()
        {
            if (this.BorderEndPoint1.SelectedItem == null)
            {
                return false;
            }

            if (this.BorderEndPoint2.SelectedItem == null)
            {
                return false;
            }

            int[] numbers = this.GetNumbers();
            if (this.borderItems.Find(b => b.Numbers[0] == numbers[0] && b.Numbers[1] == numbers[1]) != null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get an array with the numbers for both the selected <see cref="BorderEndPoint"/> for a <see cref="CountryBorder"/>.
        /// </summary>
        /// <returns> An array of int with a length of 2 with both numbers. </returns>
        private int[] GetNumbers()
        {
            int[] numbers = new int[2];
            if (this.BorderEndPoint1.SelectedItem != null && this.BorderEndPoint2.SelectedItem != null)
            {
                numbers[0] = ((BorderEndPointItem)this.BorderEndPoint1.SelectedItem).Number;
                numbers[1] = ((BorderEndPointItem)this.BorderEndPoint2.SelectedItem).Number;
                this.Swap(numbers);
            }

            return numbers;
        }

        /// <summary>
        /// The numbers for the <see cref="CountryBorder"/> are always stored with the lowest number first.
        /// </summary>
        /// <param name="numbers"> The array with the two ints to swap. </param>
        private void Swap(int[] numbers)
        {
            if (numbers[0] > numbers[1])
            {
                int swap = numbers[0];
                numbers[0] = numbers[1];
                numbers[1] = swap;
            }
        }

        /// <summary>
        /// Check if we must ask the user if he wants to overwrite the file and set the bool.
        /// </summary>
        /// <returns> false if the filename would be overwritten but the user presses No in the dialog. </returns>
        private bool CheckOverwrite()
        {
            if (this.mapFiles.FileName != string.Empty && this.mapFiles.FileName != null && this.mapFiles.FilenameExcists && !this.mapFiles.MayOverwriteExcisting)
            {
                if (MessageBox.Show(
                    "Overwrite excisting file?",
                    "Excisting file",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    this.mapFiles.MayOverwriteExcisting = true;
                    return true;
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Check if we must ask the user for a file name and set the filename.
        /// </summary>
        /// <returns> false if the user must provide a filename but cancels the dialog. </returns>
        private bool CheckFileName()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".xml";
            saveFileDialog.Filter = "XML documents (.xml)|*.xml";
            saveFileDialog.OverwritePrompt = false;
            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                this.mapFiles.FileName = saveFileDialog.FileName;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds a visual representation of the <see cref="BorderEndPoint"/> to the drawing surface.
        /// </summary>
        /// <param name="clickedPoint"> The point where to create the new ellipse for the <see cref="BorderEndPoint"/>. </param>
        /// <returns> The create Ellipse. </returns>
        private Ellipse AddEllipseToDrawingSurface(Point clickedPoint)
        {
            Ellipse ellipse = this.CreateEllipse();
            Canvas.SetLeft(ellipse, clickedPoint.X);
            Canvas.SetTop(ellipse, clickedPoint.Y);
            this.DrawingSurface.Children.Add(ellipse);
            return ellipse;
        }

        /// <summary>
        /// Adds a visual representation of the <see cref="CountryBorder"/> to the drawing surface.
        /// </summary>
        /// <param name="point1"> The coordinates of the first <see cref="BorderEndPoint"/> for the CountryBorder. </param>
        /// <param name="point2"> The coordinates of the second <see cref="BorderEndPoint"/> for the CountryBorder. </param>
        /// <returns> The created Line.</returns>
        private Line AddLineToDrawingSurface(Point point1, Point point2)
        {
            Line line = this.CreateLine(point1, point2);
            this.DrawingSurface.Children.Add(line);
            return line;
        }

        /// <summary>
        /// Something changed in the selection of items in the drawing so we need to give them different colors 
        /// they will automatacilly get redrawn.
        /// </summary>
        private void ReDraw()
        {
            List<Ellipse> ellipses = this.GetSelectedEllipsesForDraw();
            Line line = this.GetSelectedLineForDraw();
            foreach (var item in this.DrawingSurface.Children)
            {
                if (item is Ellipse)
                {
                    ((Ellipse)item).Fill = ellipses.Contains(item) ? this.SelectedBrush() : this.UnSelectedBrush();
                    ((Ellipse)item).Stroke = ellipses.Contains(item) ? this.SelectedBrush() : this.UnSelectedBrush();
                }

                if (item is Line)
                {
                    ((Line)item).Stroke = item == line ? this.SelectedBrush() : this.UnSelectedBrush();
                }
            }
        }

        /// <summary>
        /// If there is a selected line return it otherwise null.
        /// </summary>
        /// <returns> The line. </returns>
        private Line GetSelectedLineForDraw()
        {
            if (this.ShowBorderTools != null && (bool)this.ShowBorderTools.IsChecked)
            {
                int[] numbers = this.GetNumbers();
                BorderItem borderItem = this.borderItems.Find(b => b.Numbers[0] == numbers[0] && b.Numbers[1] == numbers[1]);
                return borderItem == null ? null : borderItem.Line;
            }

            return null;
        }

        /// <summary>
        /// If there are one or more selected ellipses return them.
        /// </summary>
        /// <returns> The ellipses. </returns>
        private List<Ellipse> GetSelectedEllipsesForDraw()
        {
            List<Ellipse> result = new List<Ellipse>();
            if (this.ShowBorderEndPointTools != null &&
                (bool)this.ShowBorderEndPointTools.IsChecked &&
                this.BorderEndPoints != null)
            {
                result.Add(this.BorderEndPoints.SelectedItem != null ? ((BorderEndPointItem)this.BorderEndPoints.SelectedItem).Ellipse : null);
            }

            if (this.ShowBorderTools != null &&
                (bool)this.ShowBorderTools.IsChecked &&
                this.BorderEndPoint1.SelectedItem != null &&
                this.BorderEndPoint2.SelectedItem != null)
            {
                result.Add(this.BorderEndPoint1.SelectedItem != null ? ((BorderEndPointItem)this.BorderEndPoint1.SelectedItem).Ellipse : null);
                result.Add(this.BorderEndPoint2.SelectedItem != null ? ((BorderEndPointItem)this.BorderEndPoint2.SelectedItem).Ellipse : null);
            }

            return result;
        }

        /// <summary>
        /// A visual representation of a <see cref="BorderEndPoint"/>.
        /// </summary>
        /// <returns> The Ellipse representing the <see cref="BorderEndPoint"/>. </returns>
        private Ellipse CreateEllipse()
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Height = PointDiameter;
            ellipse.Width = PointDiameter;
            SolidColorBrush selectedBrush = this.UnSelectedBrush();
            ellipse.Stroke = selectedBrush;
            ellipse.Fill = selectedBrush;
            TranslateTransform myTranslate = new TranslateTransform();
            myTranslate.X = -(int)PointDiameter / 2;
            myTranslate.Y = -(int)PointDiameter / 2;
            ellipse.RenderTransform = myTranslate;
            return ellipse;
        }

        /// <summary>
        /// A visual representation of a <see cref="CountryBorder"/>.
        /// </summary>
        /// <param name="point1"> The first endpoint of the line. </param>
        /// <param name="point2"> The second endpoint of the line. </param>
        /// <returns> The Line representing the <see cref="CountryBorder"/>. </returns>
        private Line CreateLine(Point point1, Point point2)
        {
            Line line = new Line();
            line.X1 = point1.X;
            line.Y1 = point1.Y;
            line.X2 = point2.X;
            line.Y2 = point2.Y;
            SolidColorBrush selectedBrush = this.UnSelectedBrush();
            line.Stroke = selectedBrush;
            return line;
        }

        /// <summary>
        /// The brush for drawing selected items.
        /// </summary>
        /// <returns> The Brush</returns>
        private SolidColorBrush SelectedBrush()
        {
            SolidColorBrush result = new SolidColorBrush();
            result.Color = Colors.Red;
            return result;
        }

        /// <summary>
        /// The brush for drawing unselected items.
        /// </summary>
        /// <returns> The Brush</returns>
        private SolidColorBrush UnSelectedBrush()
        {
            SolidColorBrush result = new SolidColorBrush();
            result.Color = Colors.Green;
            return result;
        }

        /// <summary>
        /// Callback for testing if the user selected an allready drawn part.
        /// </summary>
        /// <param name="result"> The result list. </param>
        /// <returns> The HitTestResultBehavior. </returns>
        private HitTestResultBehavior HitTestResultCallback(HitTestResult result)
        {
            IntersectionDetail intersectionDetail = ((GeometryHitTestResult)result).IntersectionDetail;
            switch (intersectionDetail)
            {
                case IntersectionDetail.FullyContains:
                    return HitTestResultBehavior.Continue;
                case IntersectionDetail.Intersects:
                    this.hitResultsList.Add(result.VisualHit);
                    return HitTestResultBehavior.Continue;
                case IntersectionDetail.FullyInside:
                    this.hitResultsList.Add(result.VisualHit);
                    return HitTestResultBehavior.Continue;
                default:
                    return HitTestResultBehavior.Stop;
            }
        }

        /// <summary>
        /// Create a new <see cref="BorderEndPoint"/> add it to the list and create a visual representation of it.
        /// </summary>
        /// <param name="clickedPoint"> Point where we want to create the new <see cref="BorderEndPoint"/></param>
        private void CreateNewBorderEndPoint(Point clickedPoint)
        {
            Ellipse ellipse = this.AddEllipseToDrawingSurface(clickedPoint);
            BorderEndPointItem boderEndPointItem = new BorderEndPointItem(this.mapFiles.AddBorderEndPoint(clickedPoint.X, clickedPoint.Y), ellipse, clickedPoint);
            this.BorderEndPoints.Items.Add(boderEndPointItem);
            this.BorderEndPoint1.Items.Add(boderEndPointItem);
            this.BorderEndPoint2.Items.Add(boderEndPointItem);
            this.BorderEndPoints.SelectedIndex = this.BorderEndPoints.Items.IndexOf(boderEndPointItem);
            this.ReDraw();
        }

        /// <summary>
        /// Create a new <see cref="CountryBorder"/> add it to the list and create a visual representation of it.
        /// </summary>
        private void CreateNewBorder()
        {
            Line line = this.AddLineToDrawingSurface(
                ((BorderEndPointItem)this.BorderEndPoint1.SelectedItem).ClickedPoint,
                ((BorderEndPointItem)this.BorderEndPoint2.SelectedItem).ClickedPoint);
            int[] numbers = this.GetNumbers();
            this.mapFiles.AddCountryBorder(numbers);
            BorderItem borderItem = new BorderItem(numbers, line);
            this.borderItems.Add(borderItem);
            this.ReDraw();
        }

        /// <summary>
        /// The user clicked on a allready created <see cref="BorderEndPoint"/> select it.
        /// </summary>
        private void SelectBorderEndPoint()
        {
            if (this.hitResultsList.First() is Ellipse)
            {
                foreach (var item in this.BorderEndPoints.Items)
                {
                    if (((BorderEndPointItem)item).Ellipse == this.hitResultsList.First())
                    {
                        this.BorderEndPoints.SelectedIndex = this.BorderEndPoints.Items.IndexOf(item);
                    }
                }
            }
        }

        /// <summary>
        /// Open an excisting file with the definition of a map.
        /// </summary>
        /// <param name="fileName"> The filename to open. </param>
        private void OpenFile(string fileName)
        {
            try
            {
                this.mapFiles.Open(fileName);
                this.DrawingSurface.Children.Clear();
                this.BorderEndPoints.Items.Clear();
                List<BorderEndPoint> borderEndPoints = this.mapFiles.GetBorderEndPoints();
                this.RecreateBorderEndPoints(borderEndPoints);
                List<CountryBorder> countryBorders = this.mapFiles.GetCountryBorders();
                this.RecreateCountryBorders(borderEndPoints, countryBorders);
            }
            catch (InvalidOperationException er)
            {
                MessageBox.Show(er.Message);
            }
        }

        /// <summary>
        /// Draw the visual representions ot the <see cref="CountryBorder"/> on the drawingsurface after reopening a file.
        /// </summary>
        /// <param name="borderEndPoints"> The list with <see cref="BorderEndPoint"/>. </param>
        /// <param name="countryBorders"> The list with <see cref="CountryBorder"/>. </param>
        private void RecreateCountryBorders(List<BorderEndPoint> borderEndPoints, List<CountryBorder> countryBorders)
        {
            foreach (var countryBorder in countryBorders)
            {
                Point point1 = new Point();
                point1.X = borderEndPoints.Find(b => b.Number == countryBorder.Numbers[0]).X;
                point1.Y = borderEndPoints.Find(b => b.Number == countryBorder.Numbers[0]).Y;
                Point point2 = new Point();
                point2.X = borderEndPoints.Find(b => b.Number == countryBorder.Numbers[1]).X;
                point2.Y = borderEndPoints.Find(b => b.Number == countryBorder.Numbers[1]).Y;
                Line line = this.AddLineToDrawingSurface(point1, point2);
                BorderItem borderItem = new BorderItem(countryBorder.Numbers, line);
                this.borderItems.Add(borderItem);
            }
        }

        /// <summary>
        /// Draw the visual representions ot the <see cref="BorderEndPoint"/> on the drawingsurface after reopening a file.
        /// </summary>
        /// <param name="borderEndPoints"> The list with <see cref="BorderEndPoint"/>. </param>
        private void RecreateBorderEndPoints(List<BorderEndPoint> borderEndPoints)
        {
            BorderEndPointItem boderEndPointItem = null;
            foreach (var borderEndPoint in borderEndPoints)
            {
                Ellipse ellipse = this.AddEllipseToDrawingSurface(new Point(borderEndPoint.X, borderEndPoint.Y));
                boderEndPointItem = new BorderEndPointItem(borderEndPoint.Number, ellipse, new Point(borderEndPoint.X, borderEndPoint.Y));
                this.BorderEndPoints.Items.Add(boderEndPointItem);
                this.BorderEndPoint1.Items.Add(boderEndPointItem);
                this.BorderEndPoint2.Items.Add(boderEndPointItem);
            }

            if (boderEndPointItem != null)
            {
                this.BorderEndPoints.SelectedIndex = this.BorderEndPoints.Items.IndexOf(boderEndPointItem);
            }
        }

        /// <summary>
        /// Check if a new visual representation of a <see cref="CountryBorder"/> intersects with the allready drawn istances.
        /// </summary>
        /// <returns> True if the new <see cref="CountryBorder"/> intersects with an allready created instance. </returns>
        private bool CheckIntersection()
        {
            Point c = ((BorderEndPointItem)this.BorderEndPoint1.SelectedItem).ClickedPoint;
            Point d = ((BorderEndPointItem)this.BorderEndPoint2.SelectedItem).ClickedPoint;
            bool areIntersecting = false;
            foreach (BorderItem item in this.borderItems)
            {
                Point a = new Point(item.Line.X1, item.Line.Y1);
                Point b = new Point(item.Line.X2, item.Line.Y2);
                if (!areIntersecting && a != c && a != d && b != c && b != d && MapFiles.IsIntersecting(a, b, c, d))
                {
                    MessageBox.Show("Lines may not intersect.");
                    areIntersecting = true;
                }
            }

            return areIntersecting;
        }

        /// <summary>
        /// Check if there are changes and ask the user if he wants to ignore them.
        /// </summary>
        private void CheckChanges()
        {
            if (this.mapFiles.MapChanged)
            {
                string msg = "There is unsafed data. Ignore without saving?";
                MessageBoxResult result =
                  MessageBox.Show(
                    msg,
                    "Unsaved",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                this.mapFiles.IgnoreChanges = result == MessageBoxResult.Yes;
            }
        }
    }
}
