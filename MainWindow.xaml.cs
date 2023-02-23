using ImageHandlerEXIFWPF.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;


namespace ImageScaleEXIFWPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private EXIFImage ?exifImage;

        public MainWindow() {
            InitializeComponent();
        }

        private void pickFile_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true) {
                FileNameTextBox.Text = openFileDialog.FileName;
                exifImage = new EXIFImage(openFileDialog.FileName);
                List<MetadataExtractor.Tag> tags = new();
                foreach (MetadataExtractor.Directory dir in exifImage.MetaDirectories) {
                    tags.AddRange(dir.Tags);
                }
                exifView.ItemsSource = tags;
            } else {
                FileNameTextBox.Text = String.Empty;
                exifView.SelectedIndex = 0;
                exifView.ItemsSource = null;
                exifImage = null;
            }
        }
        private void scaleFile_Click(object sender, RoutedEventArgs e) {
            if (exifImage == null) {
                throw new Exception("There is nothing to scale. Please load an image first.");
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JPEG (*.jpg)|*.jpg";
            if (saveFileDialog.ShowDialog() == true) {
                Bitmap imageToSave = exifImage.AddExif(exifImage.ScaleMaintainAspect(700, 500));
                imageToSave.Save(saveFileDialog.FileName, ImageFormat.Jpeg);
            }
        }

    }
}
