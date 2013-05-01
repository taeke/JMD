namespace JMD
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using DrawMap;
    using Microsoft.Win32;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The <see cref="MapFiles"/> instances.
        /// </summary>
        private MapFiles map = new MapFiles();

        /// <summary>
        /// Initializes the window.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
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
                this.map.Open(openFileDialog.FileName);
            }
        }

        /// <summary>
        /// Creating an new file.
        /// </summary>
        /// <param name="sender"> The menu clicked. </param>
        /// <param name="e"> The <see cref="RoutedEventArges"/> instance. </param>
        private void MenuItem_New_Click(object sender, RoutedEventArgs e)
        {
            this.map.New();
        }

        /// <summary>
        /// Saving an file.
        /// </summary>
        /// <param name="sender"> The menu clicked. </param>
        /// <param name="e"> The <see cref="RoutedEventArges"/> instance. </param>
        private void MenuItem_Save_Click(object sender, RoutedEventArgs e)
        {
            bool isCanceld = false;
            if (this.map.FileName == string.Empty || this.map.FileName == null)
            {
                isCanceld = !this.CheckFileName();
            }

            if (!isCanceld)
            {
                isCanceld = !this.CheckOverwrite();
            }
            
            if (!isCanceld)
            {
                this.map.Save();
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
                this.map.Save();
            }
        }

        /// <summary>
        /// Check if we must ask the user if he wants to overwrite the file and set the bool.
        /// </summary>
        /// <returns> false if the filename would be overwritten but the user presses No in the dialog. </returns>
        private bool CheckOverwrite()
        {
            if (this.map.FileName != string.Empty && this.map.FileName != null && this.map.FilenameExcists && !this.map.MayOverwriteExcisting)
            {
                if (MessageBox.Show(
                    "Overwrite excisting file?",
                    "Excisting file",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    this.map.MayOverwriteExcisting = true;
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
                this.map.FileName = saveFileDialog.FileName;
                return true;
            }

            return false;
        }
    }
}
