using ImageHandlerEXIFWPF.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;


namespace ImageScaleEXIFWPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private EXIFImage ?exifImage;
        public int requestedWidth { get; set; } = 768;
        public int requestedHeight { get; set; } = 768;

        public MainWindow() {
            InitializeComponent();
            DataContext = this;
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
                Bitmap imageToSave = exifImage.AddExif(exifImage.ScaleMaintainAspect(requestedWidth, requestedHeight));
                imageToSave.Save(saveFileDialog.FileName, ImageFormat.Jpeg);
            }
        }

    }
}
