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
        /// The <see cref="MapFiles"/> instances.
        /// </summary>
        private IMapFiles mapFiles;

        /// <summary>
        /// Resuls for the mouseClick on the drawing surface.
        /// </summary>
        private List<DependencyObject> hitResultsList = new List<DependencyObject>();

        /// <summary>
        /// Initializes the window.
        /// </summary>
        /// <param name="mapFiles"> the IMapFiles instance. </param>
        public MainWindow(IMapFiles mapFiles)
        {
            this.mapFiles = mapFiles;
            this.InitializeComponent();
            this.Delete.IsEnabled = false;
        }

        /// <summary>
        /// Opening an excisting file.
        /// </summary>
        /// <param name="sender"> The menu clicked. </param>
        /// <param name="e"> The <see cref="RoutedEventArges"/> instance. </param>
        private void MenuItem_Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".xml";
            openFileDialog.Filter = "XML documents (.xml)|*.xml";
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                this.OpenFile(openFileDialog.FileName);
            }
        }

        /// <summary>
        /// Creating an new file.
        /// </summary>
        /// <param name="sender"> The menu clicked. </param>
        /// <param name="e"> The <see cref="RoutedEventArges"/> instance. </param>
        private void MenuItem_New_Click(object sender, RoutedEventArgs e)
        {
            this.DrawingSurface.Children.Clear();
            this.BorderEndPoints.Items.Clear();
            this.mapFiles.New();
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
            Point clickedPoint = e.GetPosition((UIElement)sender);
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
        /// The user selected another BorderEndpoint.
        /// </summary>
        /// <param name="sender"> The combobox containing the borderendpoints </param>
        /// <param name="e"> The <see cref="SelectionChangedEventArgs"/> instance. </param>
        private void BorderEndPoints_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Delete.IsEnabled = this.BorderEndPoints.SelectedItem != null;
            this.ReDraw();
        }

        /// <summary>
        /// The user clicked the delete button.
        /// </summary>
        /// <param name="sender"> The button. </param>
        /// <param name="e"> The <see cref="RoutedEventArgs"/> instance. </param>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            this.mapFiles.RemoveEndPoint(((BorderEndPointItem)this.BorderEndPoints.SelectedItem).Number);
            this.DrawingSurface.Children.Remove(((BorderEndPointItem)this.BorderEndPoints.SelectedItem).Ellipse);
            this.BorderEndPoints.Items.Remove(this.BorderEndPoints.SelectedItem);
        }

        /// <summary>
        /// The user tries to close the application.
        /// </summary>
        /// <param name="sender"> The controle which was clicked to close the application. </param>
        /// <param name="e"> The <see cref="CancelEventArgs"/> instance. </param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (this.mapFiles.MapChanged)
            {
                string msg = "There is unsafed data. Close without saving?";
                MessageBoxResult result =
                  MessageBox.Show(
                    msg,
                    "Unsaved",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
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
        /// Something changed in the selection of items in the drawing so we need to give them different colors 
        /// they will automatacilly get redrawn.
        /// </summary>
        private void ReDraw()
        {
            foreach (var item in this.DrawingSurface.Children)
            {
                SolidColorBrush selectedBrush = this.SelectedBrush();
                SolidColorBrush unselectedBrush = this.UnSelectedBrush();
                Ellipse ellipse = this.BorderEndPoints.SelectedItem != null ? ((BorderEndPointItem)this.BorderEndPoints.SelectedItem).Ellipse : null;
                if (item is Ellipse)
                {
                    ((Ellipse)item).Fill = item == ellipse ? selectedBrush : unselectedBrush;
                    ((Ellipse)item).Stroke = item == ellipse ? selectedBrush : unselectedBrush;
                }
            }
        }

        /// <summary>
        /// A visual representation of a BorderEndPoint.
        /// </summary>
        /// <returns> The Ellipse representing the BorderEndPoint. </returns>
        private Ellipse CreateEllipse()
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Height = PointDiameter;
            ellipse.Width = PointDiameter;
            SolidColorBrush selectedBrush = this.SelectedBrush();
            ellipse.Stroke = selectedBrush;
            ellipse.Fill = selectedBrush;
            TranslateTransform myTranslate = new TranslateTransform();
            myTranslate.X = -(int)PointDiameter / 2;
            myTranslate.Y = -(int)PointDiameter / 2;
            ellipse.RenderTransform = myTranslate;
            return ellipse;
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
            BorderEndPointItem boderEndPointItem = new BorderEndPointItem(this.mapFiles.AddBorderEndPoint(clickedPoint.X, clickedPoint.Y), ellipse);
            this.BorderEndPoints.Items.Add(boderEndPointItem);
            this.BorderEndPoints.SelectedIndex = this.BorderEndPoints.Items.IndexOf(boderEndPointItem);
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
                BorderEndPointItem boderEndPointItem = null;
                foreach (var borderEndPoint in borderEndPoints)
                {
                    Ellipse ellipse = this.AddEllipseToDrawingSurface(new Point(borderEndPoint.X, borderEndPoint.Y));
                    boderEndPointItem = new BorderEndPointItem(borderEndPoint.Number, ellipse);
                    this.BorderEndPoints.Items.Add(boderEndPointItem);
                }

                if (boderEndPointItem != null)
                {
                    this.BorderEndPoints.SelectedIndex = this.BorderEndPoints.Items.IndexOf(boderEndPointItem);
                }
            }
            catch (InvalidOperationException er)
            {
                MessageBox.Show(er.Message);
            }
        }
    }
}
