using ImageMagick;
using Microsoft.Win32;
using System;
using System.Windows;


namespace ImageScaleEXIFWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public int requestedWidth { get; set; } = 768;
        public int requestedHeight { get; set; } = 768;
        private MagickImage? magicImage = null;


        public MainWindow() {
            InitializeComponent();
            DataContext = this;
        }

        private void pickFile_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true) {
                // Proper code examples here:
                // https://github.com/dlemstra/Magick.NET/tree/main/docs#documentation
                magicImage = new(openFileDialog.FileName);
                exifView.ItemsSource = magicImage.GetExifProfile().Values;
                FileNameTextBox.Text = openFileDialog.FileName;
            } else {
                FileNameTextBox.Text = String.Empty;
                exifView.SelectedIndex = 0;
                exifView.ItemsSource = null;
                magicImage = null;
            }
        }
        private void scaleFile_Click(object sender, RoutedEventArgs e) {
            if (magicImage == null) {
                throw new Exception("There is nothing to scale. Please load an image first.");
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JPEG (*.jpg)|*.jpg";
            if (saveFileDialog.ShowDialog() == true && magicImage != null) {

                double width = (double)requestedWidth;
                double height = (double)requestedHeight;
                double originalAspect = (double)magicImage.Width / (double)magicImage.Height;
                double requestedAspect = width / height;

                if (originalAspect > requestedAspect) {
                    height = Math.Round(width * originalAspect);
                }
                else {
                    width = Math.Round(height * originalAspect);
                }
                magicImage.Resize((int)width, (int)height);
                magicImage.Write(saveFileDialog.FileName, MagickFormat.Jpeg);
                var exif = magicImage.GetExifProfile();
                foreach(var exifItem in exif.Values)
                {
                    var dt = exifItem.DataType;
                    string s = exifItem.ToString();
                }

                // Use magicImage.Write(SomeStream) for more flexible handling 
            }
        }

    }
}
