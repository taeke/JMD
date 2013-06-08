//-------------------------------------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs">
// Taeke van der Veen juni 2013
// </copyright>
// Visual Studio Express 2012 for Windows Desktop
//-------------------------------------------------------------------------------------------------------------------------------------------------

namespace JMD
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using DrawMap;
    using DrawMap.Interface;
    using Microsoft.Win32;

    /// <summary>
    /// Code behind for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The <see cref="IMapFiles"/> instance.
        /// </summary>
        private IMapFiles mapFiles;

        /// <summary>
        /// Objects found on the DrawingSurface Canvas within a radius around the point where the user clicks with the mouse.
        /// </summary>
        private List<DependencyObject> hitResultsList = new List<DependencyObject>();

        /// <summary>
        /// The list of <see cref="BorderItem"/> instances. 
        /// </summary>
        private ObservableCollection<BorderItem> borderItems = new ObservableCollection<BorderItem>();

        /// <summary>
        /// The list of <see cref="BorderPointItem"/> instances. 
        /// </summary>
        private List<BorderPointItem> borderPointItems = new List<BorderPointItem>();

        /// <summary>
        /// The ObservableCollection of <see cref="BorderEndPointItem"/> instances. 
        /// </summary>
        private ObservableCollection<BorderEndPointItem> borderEndPointItems = new ObservableCollection<BorderEndPointItem>();

        /// <summary>
        /// The observableCollection of <see cref="CountryItem"/> instances.
        /// </summary>
        private ObservableCollection<CountryItem> countryItems = new ObservableCollection<CountryItem>();

        /// <summary>
        /// If the user his hold his mouse down on the selectedBorderItem, when Borders is selected in "What to draw", the line on
        /// which he clicked is split in two. This holds a reference to both lines. 
        /// </summary>
        private List<Line> dragingLines = new List<Line>();

        /// <summary>
        /// Is the user holding his mouse down and is draging to a new location and Borders is selected in "What to draw".
        /// </summary>
        private bool isDragingNewBorderPoint = false;

        /// <summary>
        /// Initializes the window.
        /// </summary>
        /// <param name="mapFiles"> The IMapFiles instance. </param>
        public MainWindow(IMapFiles mapFiles)
        {
            this.mapFiles = mapFiles;
            this.InitializeComponent();
            this.BorderEndPoints.ItemsSource = this.borderEndPointItems;
            this.BorderEndPoint1.ItemsSource = this.borderEndPointItems;
            this.BorderEndPoint2.ItemsSource = this.borderEndPointItems;
            this.Borders.ItemsSource = this.borderItems;
            this.Countries.ItemsSource = this.countryItems;
            this.BorderEndPointsGrid.Visibility = Visibility.Collapsed;
            this.BordersGrid.Visibility = Visibility.Collapsed;
            this.CountriesGrid.Visibility = Visibility.Collapsed;
            ((INotifyCollectionChanged)this.borderItems).CollectionChanged += this.BorderItems_CollectionChanged;
            ((INotifyCollectionChanged)this.BordersForCountry.Items).CollectionChanged += this.BordersForCountry_CollectionChanged;
        }

        /// <summary>
        /// The selected <see cref="BorderItem"/> when Borders is selected in "What to draw".
        /// </summary>
        private BorderItem SelectedBorderItem
        {
            get
            {
                if (this.BorderEndPoint1.SelectedItem == null)
                {
                    return null;
                }

                if (this.BorderEndPoint2.SelectedItem == null)
                {
                    return null;
                }

                int[] numbers = this.GetNumbers();
                return this.borderItems.ToList().Find(b => b.EndPointNumbers[0] == numbers[0] && b.EndPointNumbers[1] == numbers[1]);
            }
        }

        /// <summary>
        /// The BorderItems collection of <see cref="BorderItem"/> instances is changed. The buttons
        /// for creating or deleting a borderitem must also change.
        /// </summary>
        /// <param name="sender"> The collection of <see cref="BorderItem"/> instances.</param>
        /// <param name="e"> The NotifyCollectionChangedEventArgs instance. </param>
        public void BorderItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.SetBorderItemsButtons();
        }

        /// <summary>
        /// The BordersForCountry collection of <see cref="BorderItem"/> instances for the country is changed. The list of visible 
        /// <see cref="BorderItem"/> instances in de Borders ComboBox must also change.
        /// </summary>
        /// <param name="sender"> The Listbox. The BordersForCountry collectien is set as Source for this Listbox. </param>
        /// <param name="e"> The NotifyCollectionChangedEventArgs instance. </param>
        public void BordersForCountry_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var item in this.Borders.Items)
            {
                ((BorderItem)item).VisibleInComboBox = Visibility.Visible;
            }

            foreach (var item in this.BordersForCountry.Items)
            {
                ((BorderItem)item).VisibleInComboBox = Visibility.Collapsed;
            }

            this.Borders.Items.Refresh();
        }

        /// <summary>
        /// The user clicked on the MenuItem for opening an excisting file.
        /// </summary>
        /// <param name="sender"> The MenuItem clicked. </param>
        /// <param name="e"> The <see cref="RoutedEventArges"/> instance. </param>
        private void MenuItem_Open_Click(object sender, RoutedEventArgs e)
        {
            this.CheckChanges();
            if (!this.mapFiles.IsMapChanged || this.mapFiles.IgnoreChanges)
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
        /// The user clicked on the MenuItem for creating an new file.
        /// </summary>
        /// <param name="sender"> The MenuItem clicked. </param>
        /// <param name="e"> The <see cref="RoutedEventArges"/> instance. </param>
        private void MenuItem_New_Click(object sender, RoutedEventArgs e)
        {
            this.CheckChanges();
            if (!this.mapFiles.IsMapChanged || this.mapFiles.IgnoreChanges)
            {
                this.ClearDrawing();
                this.mapFiles.New();
                this.ShowOriginalMapTools.IsChecked = true;
            }
        }

        /// <summary>
        /// The user clicked on the MenuItem for Saving an file.
        /// </summary>
        /// <param name="sender"> The MenuItem clicked. </param>
        /// <param name="e"> The <see cref="RoutedEventArges"/> instance. </param>
        private void MenuItem_Save_Click(object sender, RoutedEventArgs e)
        {
            bool isCanceled = false;
            if (this.mapFiles.FileName == string.Empty || this.mapFiles.FileName == null)
            {
                isCanceled = !this.SetSaveFileName();
            }

            if (!isCanceled)
            {
                isCanceled = !this.CheckOverwrite();
            }

            if (!isCanceled)
            {
                this.mapFiles.Save();
            }
        }

        /// <summary>
        /// The user clicked on the MenuItem for saving an file under another name.
        /// </summary>
        /// <param name="sender"> The MenuItem clicked. </param>
        /// <param name="e"> The <see cref="RoutedEventArges"/> instance. </param>
        private void MenuItem_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            bool isCanceled = !this.SetSaveFileName();
            if (!isCanceled)
            {
                isCanceled = !this.CheckOverwrite();
            }

            if (!isCanceled)
            {
                this.mapFiles.Save();
            }
        }

        /// <summary>
        /// The user clicked somewhere on the DrawingSurface Canvas.
        /// </summary>
        /// <param name="sender"> The clicked Canvas. </param>
        /// <param name="e"> The <see cref="MouseButtonEventArgs"/> instance. </param>
        private void DrawingSurface_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point clickedPoint = e.GetPosition((UIElement)sender);
            this.GetHitResultsList(clickedPoint);
            if (this.ShowBorderEndPointTools != null && (bool)this.ShowBorderEndPointTools.IsChecked)
            {
                this.DrawOrSelectBorderEndPoint(clickedPoint);
            }

            if (this.ShowBorderTools != null && (bool)this.ShowBorderTools.IsChecked)
            {
                this.CreateTempLinesOrSelectBorderItem(clickedPoint);
            }

            if (this.ShowCountryTools != null && (bool)this.ShowCountryTools.IsChecked)
            {
                this.AddBorderItemToCountry();
            }
        }

        /// <summary>
        /// The user lifted the mouse button on the DrawingSurface Canvas. 
        /// </summary>
        /// <param name="sender"> The Canvas. </param>
        /// <param name="e"> The <see cref="MouseButtonEventArgs"/> instance. </param>
        private void DrawingSurface_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.isDragingNewBorderPoint)
            {
                Point newPoint = new Point(e.GetPosition(this.DrawingSurface).X, e.GetPosition(this.DrawingSurface).Y);
                this.MoveDragingLines(newPoint);
                if (!this.IsIntersectingWithOtherLinesOrPoints(new Point(this.dragingLines[0].X1, this.dragingLines[0].Y1), newPoint) && 
                    !this.IsIntersectingWithOtherLinesOrPoints(new Point(this.dragingLines[1].X1, this.dragingLines[1].Y1), newPoint))
                {
                    this.CreateNewBorderPoint(newPoint);
                }

                this.CancelDragging();
            }
        }

        /// <summary>
        /// The user move the mouse away from the DrawingSurface. He could be draging a point on the border to
        /// a new location and we have to cancel that operation.
        /// </summary>
        /// <param name="sender"> The DrawingSurface</param>
        /// <param name="e"> The <see cref="MouseEventArgs"/> instance. </param>
        private void DrawingSurface_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.isDragingNewBorderPoint)
            {
                this.RemoveNewDragingLineAndRestoreOriginal();
                this.CancelDragging();
            }
        }

        /// <summary>
        /// The user moves the mouse over the DrawingSurface Canvas.
        /// </summary>
        /// <param name="sender"> The DrawingSurface, </param>
        /// <param name="e"> The <see cref="MouseEventArgs"/> instance. </param>
        private void DrawingSurface_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.isDragingNewBorderPoint)
            {
                this.MoveDragingLines(new Point(e.GetPosition(this.DrawingSurface).X, e.GetPosition(this.DrawingSurface).Y));
            }
        }

        /// <summary>
        /// The user clicked the Create CountryBorder Button.
        /// </summary>
        /// <param name="sender"> The clicked button. </param>
        /// <param name="e"> The <see cref="RoutedEventArgs"/> instance. </param>
        private void CreateCountryBorder_Click(object sender, RoutedEventArgs e)
        {
            if (!this.IsIntersectingWithOtherLinesOrPoints(
                ((BorderEndPointItem)this.BorderEndPoint1.SelectedItem).ClickedPoint,
                ((BorderEndPointItem)this.BorderEndPoint2.SelectedItem).ClickedPoint))
            {
                this.CreateNewBorder();
                this.ReDraw();
            }
        }

        /// <summary>
        /// The user selected another BorderEndpoint when Borders is selected in "What to draw".
        /// </summary>
        /// <param name="sender"> The combobox containing the borderendpoints. </param>
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
            BorderEndPointItem borderEndPointItem = (BorderEndPointItem)this.BorderEndPoints.SelectedItem;
            if (this.borderItems.ToList().Find(b => b.EndPointNumbers.Contains(borderEndPointItem.Number)) == null)
            {
                this.mapFiles.RemoveBorderPoint(borderEndPointItem.Number);
                this.DrawingSurface.Children.Remove(borderEndPointItem.Ellipse);
                this.borderPointItems.Remove(this.borderPointItems.Find(p => p.Number == borderEndPointItem.Number));
                this.borderEndPointItems.Remove(borderEndPointItem);
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
            BorderItem borderItem = this.SelectedBorderItem;
            if (this.countryItems.ToList().Find(c => c.BorderItems.Contains(borderItem)) == null)
            {
                this.mapFiles.RemoveCountryBorder(borderItem.EndPointNumbers);
                this.RemoveBorderItem(borderItem);
            }
            else
            {
                MessageBox.Show("Can't delete a CountryBorder which is part of a Country");
            }
        }

        /// <summary>
        /// The user tries to close the application.
        /// </summary>
        /// <param name="sender"> The controle which was clicked to close the application. </param>
        /// <param name="e"> The <see cref="CancelEventArgs"/> instance. </param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            this.CheckChanges();
            if (this.mapFiles.IsMapChanged && !this.mapFiles.IgnoreChanges)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// The user choose another drawing tool. 
        /// </summary>
        /// <param name="sender"> The radiobutton clicked. </param>
        /// <param name="e"> The <see cref="RoutedEventArgs"/> instance. </param>
        private void DrawingTools_Checked(object sender, RoutedEventArgs e)
        {
            this.SetShowOriginalMapIfOriginalMapNotSelected();
            this.ShowAnToolsGrid();
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
        /// The user clicked the AddBorder button.
        /// </summary>
        /// <param name="sender"> The clicked button. </param>
        /// <param name="e"> The RoutedEventArgs instance. </param>
        private void AddBorder_Click(object sender, RoutedEventArgs e)
        {
            this.BordersForCountry.Items.Add(this.Borders.SelectedItem);
            this.Borders.SelectedItem = null;
        }

        /// <summary>
        /// The selection of borders is changed.
        /// </summary>
        /// <param name="sender"> The combobox. </param>
        /// <param name="e"> The SelectionChangedEventArgs instance. </param>
        private void Borders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SetIsEnabledCountryControls();
            this.ReDraw();
        }

        /// <summary>
        /// The selection of borders for a country is changed.
        /// </summary>
        /// <param name="sender"> The listbox. </param>
        /// <param name="e"> The SelectionChangedEventArgs instance. </param>
        private void BordersForCountry_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SetIsEnabledCountryControls();
            this.ReDraw();
        }

        /// <summary>
        /// The selection of countries is changed.
        /// </summary>
        /// <param name="sender"> The combobox. </param>
        /// <param name="e"> The SelectionChangedEventArgs instance. </param>
        private void Countries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SetIsEnabledCountryControls();
            this.ClearCountryControls();
            if (this.Countries.SelectedItem != null)
            {
                this.CountryName.Text = ((CountryItem)this.Countries.SelectedItem).Name;
                foreach (var item in ((CountryItem)this.Countries.SelectedItem).BorderItems)
                {
                    this.BordersForCountry.Items.Add(item);
                }
            }

            this.ReDraw();
        }

        /// <summary>
        /// The user clicked the  DeleteBorderFromCountry button.
        /// </summary>
        /// <param name="sender"> The clicked button. </param>
        /// <param name="e"> The RoutedEventArgs instance. </param>
        private void DeleteBorderFromCountry_Click(object sender, RoutedEventArgs e)
        {
            ((BorderItem)this.BordersForCountry.SelectedItem).VisibleInComboBox = Visibility.Visible;
            this.Borders.Items.Refresh();
            this.BordersForCountry.Items.Remove(this.BordersForCountry.SelectedItem);
        }

        /// <summary>
        /// The user clicked the AddCountry button.
        /// </summary>
        /// <param name="sender"> The button. </param>
        /// <param name="e"> The RoutedEventArgs instance. </param>
        private void AddCountry_Click(object sender, RoutedEventArgs e)
        {
            bool isOk = true;
            if (this.CountryName.Text == string.Empty)
            {
                MessageBox.Show("The name of the country can not be empty");
                isOk = false;
            }

            if (isOk && (this.BordersForCountry.Items.Count == 0))
            {
                MessageBox.Show("The borders do not form a valid country.");
                isOk = false;
            }

            if (isOk && !this.AreBordersValidCountry())
            {
                MessageBox.Show("The borders do not form a valid country.");
                isOk = false;
            }

            CountryItem countryItem = this.CreateCountryItem();
            if (isOk)
            {
                isOk = !this.CountryExcists(countryItem);
            }

            if (isOk)
            {
                this.countryItems.Add(countryItem);
                this.Countries.SelectedItem = countryItem;
                this.mapFiles.AddCountry(countryItem.Name, countryItem.BorderItems.Select(b => b.EndPointNumbers).ToList());
            }
        }

        /// <summary>
        /// The user clicked the DeleteCountry button.
        /// </summary>
        /// <param name="sender"> The button</param>
        /// <param name="e"> The RoutedEventArgs instance. </param>
        private void DeleteCountry_Click(object sender, RoutedEventArgs e)
        {
            this.mapFiles.RemoveCountry(((CountryItem)this.Countries.SelectedItem).Name);
            this.countryItems.Remove((CountryItem)this.Countries.SelectedItem);
            this.ClearCountryControls();
        }

        /// <summary>
        /// The user clicked the NewCountry button.
        /// </summary>
        /// <param name="sender"> The button. </param>
        /// <param name="e"> The RoutedEventArgs instance. </param>
        private void NewCountry_Click(object sender, RoutedEventArgs e)
        {
            this.Countries.SelectedItem = null;
            this.ClearCountryControls();
        }

        /// <summary>
        /// The user clicked the SelectJPG button.
        /// </summary>
        /// <param name="sender"> The button. </param>
        /// <param name="e"> The RoutedEventArgs instance. </param>
        private void SelectJPGFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".jpg";
            openFileDialog.Filter = "JPG documents (.jpg)|*.jpg";
            bool? resultOpen = openFileDialog.ShowDialog();
            if (resultOpen == true)
            {
                this.JPGFile.Text = openFileDialog.FileName;
                this.mapFiles.AddOriginalMap(this.JPGFile.Text);
                this.HideOriginal.IsChecked = false;
                this.OriginalMapJPG.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// The jpg file name is changed. The user can't changes this directly but by using the select button.
        /// </summary>
        /// <param name="sender"> The textbox. </param>
        /// <param name="e"> The TextChangedEventArgs instance. </param>
        private void JPGFile_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.JPGFile.Text != string.Empty && File.Exists(this.JPGFile.Text))
            {
                this.OriginalMapJPG.Source = new BitmapImage(new Uri(this.JPGFile.Text));
                this.DrawingSurfaceBorder.Width = this.OriginalMapJPG.Source.Width + 2;
                this.DrawingSurfaceBorder.Height = this.OriginalMapJPG.Source.Height + 2;
            }
        }

        /// <summary>
        /// De user clicked on HideOriginal
        /// </summary>
        /// <param name="sender"> The checkbox. </param>
        /// <param name="e"> The RoutedEventArgs instance. </param>
        private void HideOriginal_Click(object sender, RoutedEventArgs e)
        {
            this.OriginalMapJPG.Visibility = (bool)this.HideOriginal.IsChecked ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// The user choose another endpoint for the <see cref="CountryBorder"/>.
        /// </summary>
        /// <param name="changed"> The combobox which changed. </param>
        /// <param name="other"> The combobox for the other endpoint for the <see cref="CountryBorder"/>. </param>
        private void EndPointsForBorderSelectionChanged(ComboBox changed, ComboBox other)
        {
            this.SetVisibilityForBorderEndPointItems(changed, other);
            this.SetBorderItemsButtons();
            this.ReDraw();
        }

        /// <summary>
        /// When a user selects a borderendpoint for a countryborder it may not choose this borderendpoint int the other
        /// control. This method will hide the borderendpoint in the other control.
        /// </summary>
        /// <param name="changed"> The combobox which changed. </param>
        /// <param name="other"> The combobox for the other endpoint for the <see cref="CountryBorder"/>. </param>
        private void SetVisibilityForBorderEndPointItems(ComboBox changed, ComboBox other)
        {
            foreach (BorderEndPointItem borderEndPointItem in this.BorderEndPoint2.Items)
            {
                borderEndPointItem.VisibleIn1 = changed == this.BorderEndPoint1 ? borderEndPointItem.VisibleIn1 : Visibility.Visible;
                borderEndPointItem.VisibleIn2 = changed == this.BorderEndPoint2 ? borderEndPointItem.VisibleIn2 : Visibility.Visible;
            }

            if (changed.SelectedItem != null)
            {
                BorderEndPointItem borderEndPointItem = (BorderEndPointItem)changed.SelectedItem;
                borderEndPointItem.VisibleIn1 = changed == this.BorderEndPoint1 ? borderEndPointItem.VisibleIn1 : Visibility.Collapsed;
                borderEndPointItem.VisibleIn2 = changed == this.BorderEndPoint2 ? borderEndPointItem.VisibleIn2 : Visibility.Collapsed;
            }

            other.Items.Refresh();
        }

        /// <summary>
        /// This selects or draws a new <see cref="BorderPoint"/>. It can also be that the user clicked on a 
        /// Line in that case nothing will happen.
        /// </summary>
        /// <param name="clickedPoint"> The point where the user clicked. </param>
        private void DrawOrSelectBorderEndPoint(Point clickedPoint)
        {
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
        /// This creates a new temperarly borderpoint. Once the user lifts the mouse button it will stay permanant.
        /// When the user click on an excisting borderpoint nothing will happen.
        /// When the user did not click on a line nothing will happen.
        /// When the user did click on a line but the line is not part of the selected border. The border where the 
        /// line is part of will get selected and NO new borderpoint will get created.
        /// </summary>
        /// <param name="clickedPoint"> The point where the user clicked. </param>
        private void CreateTempLinesOrSelectBorderItem(Point clickedPoint)
        {
            if (this.hitResultsList.Count > 0 && this.hitResultsList.Find(r => r is Ellipse) == null)
            {
                Line line = (Line)this.hitResultsList.Find(r => r is Line);
                if (line != null)
                {
                    BorderItem borderItem = this.borderItems.ToList().Find(b => b.Lines.Keys.Contains(line));
                    if (borderItem != null)
                    {
                        if (this.SelectedBorderItem != null && this.SelectedBorderItem == borderItem)
                        {
                            this.CreateTempLinesForNewBorderPoint(clickedPoint, line);
                        }
                        else
                        {
                            this.SetBorderEndPointItems(borderItem);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// There are two combobox controls for selecting both endpoints of en border. This will set
        /// the selecteditems for those controle based on a borderItem.
        /// </summary>
        /// <param name="borderItem"> The borderitem for which to set the controls. </param>
        private void SetBorderEndPointItems(BorderItem borderItem)
        {
            foreach (var item in this.BorderEndPoint1.Items)
            {
                if (((BorderEndPointItem)item).Number == borderItem.EndPointNumbers[0])
                {
                    this.BorderEndPoint1.SelectedItem = item;
                }

                if (((BorderEndPointItem)item).Number == borderItem.EndPointNumbers[1])
                {
                    this.BorderEndPoint2.SelectedItem = item;
                }
            }
        }

        /// <summary>
        /// When a user clicks on a line. The line gets split in two parts and the user can drag the new point to
        /// the locations he wants it to be. On the releas of the mouse button the new line and new point become
        /// permanent. This method will split the line.
        /// </summary>
        /// <param name="clickedPoint"> The point to split the line on.</param>
        /// <param name="line"> The line to split. Will get a new endpoint and the new line will get the old end point as start point. </param>
        private void CreateTempLinesForNewBorderPoint(Point clickedPoint, Line line)
        {
            this.isDragingNewBorderPoint = true;
            double x1 = this.borderPointItems.Find(b => b.Number == this.SelectedBorderItem.Lines.First(l => l.Key == line).Value[1]).ClickedPoint.X;
            double y1 = this.borderPointItems.Find(b => b.Number == this.SelectedBorderItem.Lines.First(l => l.Key == line).Value[1]).ClickedPoint.Y;
            this.dragingLines.Add(this.AddLineToDrawingSurface(new Point(x1, y1), clickedPoint, true));
            line.X2 = clickedPoint.X;
            line.Y2 = clickedPoint.Y;
            this.dragingLines.Add(line);
        }

        /// <summary>
        /// Fills the list with HitResults for a clicked point.
        /// </summary>
        /// <param name="clickedPoint"> The clicked point. </param>
        private void GetHitResultsList(Point clickedPoint)
        {
            EllipseGeometry expandedHitTestArea = new EllipseGeometry(clickedPoint, MapFiles.PointDiameter / 2, MapFiles.PointDiameter / 2);
            this.hitResultsList.Clear();
            VisualTreeHelper.HitTest(
                this.DrawingSurface,
                null,
                new HitTestResultCallback(this.HitTestResultCallback),
                new GeometryHitTestParameters(expandedHitTestArea));
        }

        /// <summary>
        /// Get an array with the numbers for both the selected <see cref="BorderPoint"/> for a <see cref="CountryBorder"/>.
        /// </summary>
        /// <returns> An array of int with a length of 2 with both numbers. </returns>
        private int[] GetNumbers()
        {
            int[] numbers = new int[2];
            BorderEndPointItem[] borderEndPointItemsLine = this.GetBorderEndPointItemsOrdered();
            if (borderEndPointItemsLine[0] != null && borderEndPointItemsLine[1] != null)
            {
                numbers[0] = borderEndPointItemsLine[0].Number;
                numbers[1] = borderEndPointItemsLine[1].Number;
            }

            return numbers;
        }

        /// <summary>
        /// Check if a file would be overwriteen and, if so, ask the user if he wants to overwrite the file if the MayOverwriteExcisting
        /// bool is not set yet.
        /// </summary>
        /// <returns> false if the filename would be overwritten but the user chooses NOT to overwrite. </returns>
        private bool CheckOverwrite()
        {
            if (this.mapFiles.FileName != string.Empty && 
                this.mapFiles.FileName != null && 
                this.mapFiles.FilenameExcists && 
                !this.mapFiles.MayOverwriteExcisting)
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
        /// Ask the user to provide a XML filename for the mapFiles instance.
        /// </summary>
        /// <returns> false if the user canceled the dialog. </returns>
        private bool SetSaveFileName()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".xml";
            saveFileDialog.Filter = "XML documents (.xml)|*.xml";
            saveFileDialog.OverwritePrompt = false;
            bool saveResult = (bool)saveFileDialog.ShowDialog();
            if (saveResult == true)
            {
                this.mapFiles.FileName = saveFileDialog.FileName;
            }

            return saveResult;
        }

        /// <summary>
        /// Adds a visual representation of the <see cref="BorderPoint"/> to the drawing surface.
        /// </summary>
        /// <param name="clickedPoint"> The point where to create the new ellipse for the <see cref="BorderPoint"/>. </param>
        /// <param name="visible"> Should the new ellipse be visible or hidden. </param>
        /// <returns> The created Ellipse. </returns>
        private Ellipse AddEllipseToDrawingSurface(Point clickedPoint, bool visible)
        {
            Ellipse ellipse = this.CreateEllipse();
            ellipse.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
            Canvas.SetLeft(ellipse, clickedPoint.X);
            Canvas.SetTop(ellipse, clickedPoint.Y);
            this.DrawingSurface.Children.Add(ellipse);
            return ellipse;
        }

        /// <summary>
        /// Adds a visual representation of the <see cref="CountryPart"/> to the drawing surface.
        /// </summary>
        /// <param name="point1"> The coordinates of the first <see cref="BorderPoint"/> for the CountryBorder. </param>
        /// <param name="point2"> The coordinates of the second <see cref="BorderPoint"/> for the CountryBorder. </param>
        /// <param name="isSelected"> Is the new line also the new selected line. </param>
        /// <returns> The created Line.</returns>
        private Line AddLineToDrawingSurface(Point point1, Point point2, bool isSelected)
        {
            Line line = this.CreateLine(point1, point2, isSelected);
            this.DrawingSurface.Children.Add(line);
            return line;
        }

        /// <summary>
        /// Something changed in the selection of items in the drawing so we need to give them different colors 
        /// they will automatacilly get redrawn.
        /// </summary>
        private void ReDraw()
        {
            List<Ellipse> ellipsesSelected = this.GetSelectedEllipsesForReDraw();
            List<Ellipse> ellipsesCountry = this.GetEllipsesCountryForReDraw();
            List<Line> linesSelected = this.GetSelectedLineForReDraw();
            List<Line> linesCountry = this.GetCountryLinesForRedraw();
            foreach (var item in this.DrawingSurface.Children)
            {
                if (item is Ellipse)
                {
                    ((Ellipse)item).Fill = ellipsesSelected != null && ellipsesSelected.Contains(item) ? this.SelectedBrush() : this.UnSelectedBrush();
                    ((Ellipse)item).Stroke = ellipsesSelected != null && ellipsesSelected.Contains(item) ? this.SelectedBrush() : this.UnSelectedBrush();
                    ((Ellipse)item).Fill = ellipsesCountry != null && ellipsesCountry.Contains(item) ? this.CountryBrush() : ((Ellipse)item).Fill;
                    ((Ellipse)item).Stroke = ellipsesCountry != null && ellipsesCountry.Contains(item) ? this.CountryBrush() : ((Ellipse)item).Stroke;
                }

                if (item is Line)
                {
                    ((Line)item).Stroke = linesSelected != null && linesSelected.Contains(item) ? this.SelectedBrush() : this.UnSelectedBrush();
                    ((Line)item).Stroke = linesCountry != null && linesCountry.Contains(item) ? this.CountryBrush() : ((Line)item).Stroke;
                }
            }
        }

        /// <summary>
        /// If there are one or more lines in a selected country return them.
        /// </summary>
        /// <returns> A list of lines which are part of a selected country. </returns>
        private List<Line> GetCountryLinesForRedraw()
        {
            if (this.ShowCountryTools != null &&
                (bool)this.ShowCountryTools.IsChecked &&
                this.BordersForCountry != null)
            {
                List<Line> result = new List<Line>();
                foreach (var item in this.BordersForCountry.Items)
                {
                    result.AddRange(((BorderItem)item).Lines.Keys.ToList());
                }

                return result;
            }

            return null;
        }

        /// <summary>
        /// If there are one or more ellipses in a selected country return them.
        /// </summary>
        /// <returns> A list of Ellipse which are part of a selected Country. </returns>
        private List<Ellipse> GetEllipsesCountryForReDraw()
        {
            List<Ellipse> result = new List<Ellipse>();
            if (this.ShowCountryTools != null &&
                (bool)this.ShowCountryTools.IsChecked &&
                this.BordersForCountry != null)
            {
                foreach (var item in this.BordersForCountry.Items)
                {
                    this.AddEllipseForRedraw(this.borderEndPointItems.ToList().Find(b => b.Number == ((BorderItem)item).EndPointNumbers[0]).Ellipse, result);
                    this.AddEllipseForRedraw(this.borderEndPointItems.ToList().Find(b => b.Number == ((BorderItem)item).EndPointNumbers[1]).Ellipse, result);
                }
            }

            return result;
        }

        /// <summary>
        /// If there is a selected line return it otherwise null.
        /// </summary>
        /// <returns> The line. </returns>
        private List<Line> GetSelectedLineForReDraw()
        {
            if (this.ShowBorderTools != null &&
                (bool)this.ShowBorderTools.IsChecked)
            {
                int[] numbers = this.GetNumbers();
                BorderItem borderItem = this.borderItems.ToList().Find(b => b.EndPointNumbers[0] == numbers[0] && b.EndPointNumbers[1] == numbers[1]);
                return borderItem == null ? null : borderItem.Lines.Keys.ToList();
            }

            if (this.ShowCountryTools != null &&
                (bool)this.ShowCountryTools.IsChecked &&
                this.Borders != null &&
                this.Borders.SelectedItem != null)
            {
                return ((BorderItem)this.Borders.SelectedItem).Lines.Keys.ToList();
            }

            return null;
        }

        /// <summary>
        /// If there are one or more selected ellipses return them.
        /// </summary>
        /// <returns> The ellipses. </returns>
        private List<Ellipse> GetSelectedEllipsesForReDraw()
        {
            List<Ellipse> result = new List<Ellipse>();
            if (this.ShowBorderEndPointTools != null &&
                (bool)this.ShowBorderEndPointTools.IsChecked &&
                this.BorderEndPoints != null)
            {
                this.AddEllipseForRedraw(
                    this.BorderEndPoints.SelectedItem != null ? ((BorderEndPointItem)this.BorderEndPoints.SelectedItem).Ellipse : null, 
                    result);
            }

            if (this.ShowBorderTools != null &&
                (bool)this.ShowBorderTools.IsChecked)
            {
                this.AddEllipseForRedraw(
                    this.BorderEndPoint1.SelectedItem != null ? ((BorderEndPointItem)this.BorderEndPoint1.SelectedItem).Ellipse : null, 
                    result);
                this.AddEllipseForRedraw(
                    this.BorderEndPoint2.SelectedItem != null ? ((BorderEndPointItem)this.BorderEndPoint2.SelectedItem).Ellipse : null, 
                    result);
            }

            if (this.ShowCountryTools != null &&
                (bool)this.ShowCountryTools.IsChecked &&
                this.Borders != null &&
                this.Borders.SelectedItem != null)
            {
                this.AddEllipseForRedraw(
                    this.borderEndPointItems.ToList().Find(b => b.Number == ((BorderItem)this.Borders.SelectedItem).EndPointNumbers[0]).Ellipse, 
                    result);
                this.AddEllipseForRedraw(
                    this.borderEndPointItems.ToList().Find(b => b.Number == ((BorderItem)this.Borders.SelectedItem).EndPointNumbers[1]).Ellipse, 
                    result);
            }

            return result;
        }

        /// <summary>
        /// Add a ellipse to the list of ellipses but only if the ellipse instance not is null.
        /// </summary>
        /// <param name="ellipse"> The ellipse to add or null. </param>
        /// <param name="ellipses"> The list of ellipses. </param>
        private void AddEllipseForRedraw(Ellipse ellipse, List<Ellipse> ellipses)
        {
            if (ellipse != null)
            {
                ellipses.Add(ellipse);
            }
        }

        /// <summary>
        /// A visual representation of a <see cref="BorderPoint"/>.
        /// </summary>
        /// <returns> The Ellipse representing the <see cref="BorderPoint"/>. </returns>
        private Ellipse CreateEllipse()
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Height = MapFiles.PointDiameter;
            ellipse.Width = MapFiles.PointDiameter;
            SolidColorBrush selectedBrush = this.UnSelectedBrush();
            ellipse.Stroke = selectedBrush;
            ellipse.Fill = selectedBrush;
            TranslateTransform myTranslate = new TranslateTransform();
            myTranslate.X = -MapFiles.PointDiameter / 2;
            myTranslate.Y = -MapFiles.PointDiameter / 2;
            ellipse.RenderTransform = myTranslate;
            return ellipse;
        }

        /// <summary>
        /// A visual representation of a <see cref="BorderPart"/>.
        /// </summary>
        /// <param name="point1"> The first endpoint of the line. </param>
        /// <param name="point2"> The second endpoint of the line. </param>
        /// <param name="isSelected"> Is the new line also the new selected line. </param>
        /// <returns> The Line representing the <see cref="BorderPart"/>. </returns>
        private Line CreateLine(Point point1, Point point2, bool isSelected)
        {
            Line line = new Line();
            line.X1 = point1.X;
            line.Y1 = point1.Y;
            line.X2 = point2.X;
            line.Y2 = point2.Y;
            SolidColorBrush selectedBrush = isSelected ? this.SelectedBrush() : this.UnSelectedBrush();
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
        /// The brush for drawing country items.
        /// </summary>
        /// <returns> The Brush</returns>
        private SolidColorBrush CountryBrush()
        {
            SolidColorBrush result = new SolidColorBrush();
            result.Color = Colors.Blue;
            return result;
        }

        /// <summary>
        /// Callback for testing if the user selected an allready drawn item.
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
        /// Create a new <see cref="BorderPoint"/> which is an endpoint for a <see cref="CountryBorder"/> add it to the list 
        /// and create a visual representation of it.
        /// </summary>
        /// <param name="clickedPoint"> Point where we want to create the new <see cref="BorderPoint"/></param>
        private void CreateNewBorderEndPoint(Point clickedPoint)
        {
            Ellipse ellipse = this.AddEllipseToDrawingSurface(clickedPoint, true);
            BorderEndPointItem borderEndPointItem = new BorderEndPointItem(this.mapFiles.AddBorderPoint(clickedPoint.X, clickedPoint.Y, true), ellipse, clickedPoint);
            BorderPointItem borderPointItem = new BorderPointItem(borderEndPointItem.Number, ellipse, clickedPoint);
            this.borderPointItems.Add(borderPointItem);
            this.borderEndPointItems.Add(borderEndPointItem);
            this.BorderEndPoints.SelectedIndex = this.BorderEndPoints.Items.IndexOf(borderEndPointItem);
            this.ReDraw();
        }

        /// <summary>
        /// Create a new <see cref="CountryBorder"/> add it to the list and create a visual representation of it.
        /// </summary>
        private void CreateNewBorder()
        {
            BorderEndPointItem[] borderEndPointItemsLine = this.GetBorderEndPointItemsOrdered();
            Line line = this.AddLineToDrawingSurface(borderEndPointItemsLine[0].ClickedPoint, borderEndPointItemsLine[1].ClickedPoint, true);
            int[] numbers = this.GetNumbers();
            this.mapFiles.AddCountryBorder(numbers);
            BorderItem borderItem = new BorderItem(numbers, line, new int[2] { numbers[0], numbers[1] });
            this.borderItems.Add(borderItem);
            this.ReDraw();
        }

        /// <summary>
        /// The user clicked on a allready created <see cref="BorderPoint"/> select it.
        /// </summary>
        private void SelectBorderEndPoint()
        {
            Ellipse ellipse = (Ellipse)this.hitResultsList.Find(r => r is Ellipse);
            if (ellipse != null)
            {
                foreach (var item in this.BorderEndPoints.Items)
                {
                    if (((BorderEndPointItem)item).Ellipse == ellipse)
                    {
                        this.BorderEndPoints.SelectedIndex = this.BorderEndPoints.Items.IndexOf(item);
                    }
                }
            }
        }

        /// <summary>
        /// Open an excisting file.
        /// </summary>
        /// <param name="fileName"> The filename to open. </param>
        private void OpenFile(string fileName)
        {
            try
            {
                this.ClearDrawing();
                this.mapFiles.Open(fileName);
                this.JPGFile.Text = this.mapFiles.GetOriginalMap();
                List<BorderPoint> borderPoints = this.mapFiles.GetBorderPoints();
                List<CountryBorder> countryBorders = this.mapFiles.GetCountryBorders();
                List<Country> countries = this.mapFiles.GetCountries();
                this.RecreateDrawing(borderPoints, countryBorders, countries);
            }
            catch (InvalidOperationException er)
            {
                MessageBox.Show(er.Message);
            }
        }

        /// <summary>
        /// Check if there are changes and, if so, ask the user if he wants to ignore them.
        /// </summary>
        private void CheckChanges()
        {
            if (this.mapFiles.IsMapChanged)
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

        /// <summary>
        /// Draw the visual representions of the <see cref="CountryBorder"/> and <see cref="BorderPoint"/> on the drawingsurface 
        /// after reopening a file.
        /// </summary>
        /// <param name="borderPoints"> The list with <see cref="BorderPoint"/>. </param>
        /// <param name="countryBorders"> The list with <see cref="CountryBorder"/>. </param>
        /// <param name="countries"> The list of <see cref="Country"/>. </param>
        private void RecreateDrawing(List<BorderPoint> borderPoints, List<CountryBorder> countryBorders, List<Country> countries)
        {
            foreach (var item in borderPoints)
            {
                this.RecreateBorderPoint(item);
            }

            foreach (var countryBorder in countryBorders)
            {
                this.RecreateCountryBorder(countryBorder, borderPoints);
            }

            this.RecreateCountries(countries);
        }

        /// <summary>
        /// Draws a visual representation of all the <see cref="BorderPart"/> and <see cref="BorderPoint"/> for a <see cref="CountryBorder"/>
        /// </summary>
        /// <param name="countryBorder"> The list with <see cref="CountryBorder"/></param>
        /// <param name="borderPoints"> The list with <see cref="BorderPoint"/></param>
        private void RecreateCountryBorder(CountryBorder countryBorder, List<BorderPoint> borderPoints)
        {
            Line line = this.CreateLineFromBorderPoints(countryBorder, countryBorder.BorderParts[0], borderPoints);
            BorderItem borderItem = new BorderItem(countryBorder.BorderEndPointNumbers, line, countryBorder.BorderParts[0].BorderPointNumbers);
            for (int i = 1; i < countryBorder.BorderParts.Count; i++)
            {
                Line line2 = this.CreateLineFromBorderPoints(countryBorder, countryBorder.BorderParts[i], borderPoints);
                borderItem.Lines.Add(line2, countryBorder.BorderParts[i].BorderPointNumbers);
            }

            this.borderItems.Add(borderItem);
        }

        /// <summary>
        /// Fill the list with Countries for the countries dropdown.
        /// </summary>
        /// <param name="countries"> The list with <see cref="Country"/></param>
        private void RecreateCountries(List<Country> countries)
        {
            foreach (var country in countries)
            {
                List<BorderItem> borderItems = new List<BorderItem>();
                foreach (var numbers in country.CountriesBorderEndPointNumbers)
                {
                    borderItems.Add(this.borderItems.ToList().Find(b => b.EndPointNumbers[0] == numbers[0] && b.EndPointNumbers[1] == numbers[1]));
                }

                CountryItem countryItem = new CountryItem(country.Name, borderItems);
                this.countryItems.Add(countryItem);
            }

            this.Countries.Items.Refresh();
        }

        /// <summary>
        /// Recreates the visuals representation of a <see cref="BorderPart"/>.
        /// </summary>
        /// <param name="countryBorder"> The <see cref="CountryBorder"/> for which to create the <see cref="BorderPart"/>. </param>
        /// <param name="borderPart"> The <see cref="BorderPart"/></param>
        /// <param name="borderPoints"> All the <see cref="BorderPoint"/> for the map. </param>
        /// <returns> The created Line. </returns>
        private Line CreateLineFromBorderPoints(CountryBorder countryBorder, BorderPart borderPart, List<BorderPoint> borderPoints)
        {
            Point point1 = this.borderPointItems.Find(b => b.Number == borderPart.BorderPointNumbers[0]).ClickedPoint;
            Point point2 = this.borderPointItems.Find(b => b.Number == borderPart.BorderPointNumbers[1]).ClickedPoint;
            Line line = this.AddLineToDrawingSurface(point1, point2, false);
            return line;
        }

        /// <summary>
        /// Recreates the visual representation of a <see cref="BorderPoint"/>.
        /// </summary>
        /// <param name="borderPoint"> The <see cref="BorderPoint"/> to recreate in the drawing. </param>
        private void RecreateBorderPoint(BorderPoint borderPoint)
        {
            Point point = new Point(borderPoint.X, borderPoint.Y);
            Ellipse ellipse = this.AddEllipseToDrawingSurface(point, borderPoint.IsEndPoint);
            BorderPointItem borderPointItem = new BorderPointItem(borderPoint.Number, ellipse, point);
            this.borderPointItems.Add(borderPointItem);
            if (borderPoint.IsEndPoint)
            {
                BorderEndPointItem boderEndPointItem = new BorderEndPointItem(borderPoint.Number, ellipse, point);
                this.borderEndPointItems.Add(boderEndPointItem);
            }
        }

        /// <summary>
        /// Checks if the two new lines which where created by draging an new borderpoint don't intersect whith other borderparts or
        /// borderpoints
        /// </summary>
        /// <param name="c"> first point on first line.</param>
        /// <param name="d"> second point on both lines.</param>
        /// <param name="e"> first point on second line.</param>
        /// <returns> True if one of the lines or both are intersecting wih one of the other lines or ellipses. </returns>
        private bool DragingLinesAreIntersectingWithOtherLinesOrPoints(Point c, Point d, Point e)
        {
            return this.IsIntersectingWithOtherLinesOrPoints(c, d) || this.IsIntersectingWithOtherLinesOrPoints(d, e);
        }

        /// <summary>
        /// Check if the line representent by both endpoints is intersecting with any other lines or points.
        /// </summary>
        /// <param name="c"> first endpoint of the line. </param>
        /// <param name="d"> second endpoint of the line. </param>
        /// <returns>true if the line is intersecting wih one of the other lines or ellipses.</returns>
        private bool IsIntersectingWithOtherLinesOrPoints(Point c, Point d)
        {
            bool areIntersecting = false;
            foreach (BorderItem borderItem in this.borderItems)
            {
                foreach (Line item in borderItem.Lines.Keys)
                {
                    Point a = new Point(item.X1, item.Y1);
                    Point b = new Point(item.X2, item.Y2);
                    if (!areIntersecting && a != c && a != d && b != c && b != d && MapFiles.IsIntersectingLine(a, b, c, d))
                    {
                        MessageBox.Show("Lines may not intersect.");
                        areIntersecting = true;
                    }
                }
            }

            foreach (BorderPointItem item in this.borderPointItems)
            {
                if (!areIntersecting && item.ClickedPoint != c && item.ClickedPoint != d && MapFiles.IsIntersectingCircle(c, d, item.ClickedPoint, 4))
                {
                    MessageBox.Show("Lines may not intersect.");
                    areIntersecting = true;
                }
            }

            return areIntersecting;
        }

        /// <summary>
        /// Cancel the draging of the new borderpoint.
        /// </summary>
        private void CancelDragging()
        {
            this.isDragingNewBorderPoint = false;
            this.dragingLines.Clear();
            this.ReDraw();
        }

        /// <summary>
        /// The user is draging the new borderpoint update the position of the new point.
        /// </summary>
        /// <param name="point"> The new point. </param>
        private void MoveDragingLines(Point point)
        {
            this.dragingLines[0].X2 = point.X;
            this.dragingLines[0].Y2 = point.Y;
            this.dragingLines[1].X2 = point.X;
            this.dragingLines[1].Y2 = point.Y;
        }

        /// <summary>
        /// A line is split in two parts now the CountryBorder has to be updated with this new and changed borderpart.
        /// And the list of borderpointitems and borderitems have to be updated.
        /// </summary>
        /// <param name="point"> The point where to split the selectedBorderItem. </param>
        private void CreateNewBorderPoint(Point point)
        {
            if (this.dragingLines.Count == 2 && this.SelectedBorderItem != null)
            {
                Ellipse ellipse = this.AddEllipseToDrawingSurface(point, false);
                BorderPointItem borderPointItem = new BorderPointItem(this.mapFiles.AddBorderPoint(point.X, point.Y, false), ellipse, point);
                this.borderPointItems.Add(borderPointItem);
                KeyValuePair<Line, int[]> lineNumbers = this.SelectedBorderItem.Lines.First(l => l.Key == this.dragingLines[1]);
                int[] numbers = new int[2];
                numbers[0] = lineNumbers.Value[1];
                numbers[1] = borderPointItem.Number;
                this.mapFiles.InsertBorderPoint(lineNumbers.Value, borderPointItem.Number);
                lineNumbers.Value[1] = borderPointItem.Number;
                this.SelectedBorderItem.Lines.Add(this.dragingLines[0], numbers);
            }
        }

        /// <summary>
        /// Clears the visual representation of the map.
        /// </summary>
        private void ClearDrawing()
        {
            this.DrawingSurface.Children.RemoveRange(1, this.DrawingSurface.Children.Count);
            this.OriginalMapJPG.Source = null;
            this.JPGFile.Text = string.Empty;
            this.borderEndPointItems.Clear();
            this.borderItems.Clear();
            this.borderPointItems.Clear();
            this.countryItems.Clear();
            this.BordersForCountry.Items.Clear();
            this.CountryName.Text = string.Empty;
            this.DrawingSurfaceBorder.Height = double.NaN;
            this.DrawingSurfaceBorder.Width = double.NaN;
            this.ShowOriginalMapTools.IsChecked = true;
            this.HideOriginal.IsChecked = false;
            this.OriginalMapJPG.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// We aspect the endpointitems always to be the smallest number first and then the bigger one. But the user can select them the
        /// other way around. Here we return them in the order we want them.
        /// </summary>
        /// <returns> The borderEndPointItems order from smallest number to biggest. </returns>
        private BorderEndPointItem[] GetBorderEndPointItemsOrdered()
        {
            BorderEndPointItem[] result = new BorderEndPointItem[2];
            if (this.BorderEndPoint1 != null &&
                this.BorderEndPoint1.SelectedItem != null &&
                this.BorderEndPoint2 != null &&
                this.BorderEndPoint2.SelectedItem != null)
            {
                if (((BorderEndPointItem)this.BorderEndPoint1.SelectedItem).Number < ((BorderEndPointItem)this.BorderEndPoint2.SelectedItem).Number)
                {
                    result[0] = (BorderEndPointItem)this.BorderEndPoint1.SelectedItem;
                    result[1] = (BorderEndPointItem)this.BorderEndPoint2.SelectedItem;
                }
                else
                {
                    result[1] = (BorderEndPointItem)this.BorderEndPoint1.SelectedItem;
                    result[0] = (BorderEndPointItem)this.BorderEndPoint2.SelectedItem;
                }
            }

            return result;
        }

        /// <summary>
        /// Removes the borders and name for a country in the controls for creating a new country.
        /// </summary>
        private void ClearCountryControls()
        {
            this.BordersForCountry.Items.Clear();
            this.Borders.Items.Refresh();
            this.CountryName.Text = string.Empty;
        }

        /// <summary>
        /// Setting the IsEnabled for all the controls for creating a border based on het selections.
        /// </summary>
        private void SetIsEnabledCountryControls()
        {
            this.NewCountry.IsEnabled = this.Countries.SelectedItem != null;
            this.DeleteCountry.IsEnabled = this.Countries.SelectedItem != null;
            this.AddCountry.IsEnabled = this.Countries.SelectedItem == null;
            this.Borders.IsEnabled = this.Countries.SelectedItem == null;
            this.CountryName.IsEnabled = this.Countries.SelectedItem == null;
            this.AddBorder.IsEnabled = this.Countries.SelectedItem == null && this.Borders.SelectedItem != null;
            this.DeleteBorderFromCountry.IsEnabled = this.Countries.SelectedItem == null && this.BordersForCountry.SelectedItem != null;
        }

        /// <summary>
        /// A country is valid if alle de borders form a closed shape this means that every endpoint should be exactly 2 times in the total list.
        /// </summary>
        /// <returns> True if the <see cref="Borders"/> are a closed loop. </returns>
        private bool AreBordersValidCountry()
        {
            Dictionary<int, int> endPointNumberCount = new Dictionary<int, int>();
            foreach (BorderItem item in this.BordersForCountry.Items)
            {
                MapFiles.CountEndPointNumbers(endPointNumberCount, item.EndPointNumbers[0]);
                MapFiles.CountEndPointNumbers(endPointNumberCount, item.EndPointNumbers[1]);
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
        /// Set the SelectedItem on the Borders control. 
        /// </summary>
        private void AddBorderItemToCountry()
        {
            if (this.hitResultsList.Count > 0 && this.Countries.SelectedItem == null)
            {
                Line line = (Line)this.hitResultsList.Find(r => r is Line);
                if (line != null)
                {
                    BorderItem borderItem = this.borderItems.ToList().Find(b => b.Lines.Keys.Contains(line));
                    if (borderItem != null && !this.BordersForCountry.Items.Contains(borderItem))
                    {
                        this.BordersForCountry.Items.Add(borderItem);
                        if (borderItem == this.Borders.SelectedItem)
                        {
                            this.Borders.SelectedItem = null;
                        }

                        this.ReDraw();
                    }
                }
            }
        }

        /// <summary>
        /// Remove the new line which was created by draging en restore the endpoint of the original line.
        /// </summary>
        private void RemoveNewDragingLineAndRestoreOriginal()
        {
            this.dragingLines[1].X2 = this.dragingLines[0].X1;
            this.dragingLines[1].Y2 = this.dragingLines[0].Y1;
            this.DrawingSurface.Children.Remove(this.dragingLines[0]);
        }

        /// <summary>
        /// The create or the delete button is enabled for a <see cref="BorderItem"/> depending on wheter there
        /// is allready a created <see cref="BorderItem"/> for the selected endpoints or not.
        /// </summary>
        private void SetBorderItemsButtons()
        {
            this.CreateCountryBorder.IsEnabled = this.SelectedBorderItem == null &&
                                                 this.BorderEndPoint1.SelectedItem != null &&
                                                 this.BorderEndPoint2.SelectedItem != null;
            this.DeleteCountryBorder.IsEnabled = this.SelectedBorderItem != null &&
                                                 this.BorderEndPoint1.SelectedItem != null &&
                                                 this.BorderEndPoint2.SelectedItem != null;
        }

        /// <summary>
        /// The user must first select an original map JPG before doing any other drawing.
        /// </summary>
        private void SetShowOriginalMapIfOriginalMapNotSelected()
        {
            if (this.ShowOriginalMapTools != null &&
                this.JPGFile != null &&
                this.JPGFile.Text == string.Empty &&
                !(bool)this.ShowOriginalMapTools.IsChecked)
            {
                MessageBox.Show("Select first an original Map JPG");
                this.ShowOriginalMapTools.IsChecked = true;
            }
        }

        /// <summary>
        /// Show the right grid based on which checkbox is selected in the DrawingTools group.
        /// </summary>
        private void ShowAnToolsGrid()
        {
            if (this.ShowOriginalMapTools != null && this.OriginalMapGrid != null)
            {
                this.OriginalMapGrid.Visibility = (bool)this.ShowOriginalMapTools.IsChecked ? Visibility.Visible : Visibility.Collapsed;
            }

            if (this.ShowBorderEndPointTools != null && this.BorderEndPointsGrid != null)
            {
                this.BorderEndPointsGrid.Visibility = (bool)this.ShowBorderEndPointTools.IsChecked ? Visibility.Visible : Visibility.Collapsed;
            }

            if (this.ShowBorderTools != null && this.BordersGrid != null)
            {
                this.BordersGrid.Visibility = (bool)this.ShowBorderTools.IsChecked ? Visibility.Visible : Visibility.Collapsed;
            }

            if (this.ShowCountryTools != null && this.CountriesGrid != null)
            {
                this.CountriesGrid.Visibility = (bool)this.ShowCountryTools.IsChecked ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Check if the given <see cref="CountryItem"/> allready excists in the countryItems list and warn the user.
        /// </summary>
        /// <param name="countryItem"> The <see cref="CountryItem"/> instance. </param>
        /// <returns></returns>
        private bool CountryExcists(CountryItem countryItem)
        {
            if (this.countryItems.ToList().Find(c => c.Name == countryItem.Name) != null)
            {
                MessageBox.Show("There is allready a country with this name in the list.");
                return true;
            }

            if (this.countryItems.ToList().Find(delegate(CountryItem i)
            {
                return countryItem.BorderItems
                    .OrderBy(b1 => b1.EndPointNumbers[0])
                    .ThenBy(b2 => b2.EndPointNumbers[1])
                    .SequenceEqual(i.BorderItems
                        .OrderBy(b1 => b1.EndPointNumbers[0])
                        .ThenBy(b2 => b2.EndPointNumbers[1]));
            }) != null)
            {
                MessageBox.Show("There is allready a country with these borders in the list");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Create a new <see cref="CountryItem"/> based on the current selections in the controls in the CountriesGrid.
        /// </summary>
        /// <returns> The new <see cref="CountryItem"/>. </returns>
        private CountryItem CreateCountryItem()
        {
            List<BorderItem> borderItemsForCountry = this.BordersForCountry.Items.Cast<BorderItem>().ToList();
            CountryItem countryItem = new CountryItem(this.CountryName.Text, borderItemsForCountry);
            return countryItem;
        }

        /// <summary>
        /// Remove the given <see cref="BorderItem"/> from the DrawingSurface Canvas   
        /// </summary>
        /// <param name="borderItem"> The <see cref="BorderItem"/> to remove. </param>
        private void RemoveBorderItem(BorderItem borderItem)
        {
            foreach (KeyValuePair<Line, int[]> lineNumbers in borderItem.Lines)
            {
                foreach (int number in lineNumbers.Value)
                {
                    if (!borderItem.EndPointNumbers.Contains(number))
                    {
                        this.borderPointItems.Remove(this.borderPointItems.Find(b => b.Number == number));
                    }
                }
            }

            foreach (Line line in borderItem.Lines.Keys)
            {
                this.DrawingSurface.Children.Remove(line);
            }

            this.borderItems.Remove(borderItem);
            this.ReDraw();
        }
    }
}
