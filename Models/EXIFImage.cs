using MetadataExtractor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace ImageHandlerEXIFWPF.Models {
    public class EXIFImage {

        public Bitmap Bitmap { get; }
        public PropertyItem[] PropertyArray { 
            get { 
                return Bitmap.PropertyItems; 
            }
        }
        // This is only here for debuggin and UI reasons.
        // Should be removed is code is used in production
        public IEnumerable<MetadataExtractor.Directory> MetaDirectories { get; set; }

        public EXIFImage(string filePath) 
        {
            // This byte array is not useful when moving to production.
            // The simpler Bitmap.FromFile(filePath) will do nicely for that.
            byte[] fileBytes = File.ReadAllBytes(filePath);
            Bitmap = new Bitmap(new MemoryStream(fileBytes));
            MetaDirectories = ImageMetadataReader.ReadMetadata(new MemoryStream(fileBytes));
            Bitmap.FromFile(filePath);
        }

        // Apply scaling in same aspect ratio as the original by either adjusting height or width of output.
        public Bitmap ScaleMaintainAspect(double width, double height) {
            double originalAspect = (double)Bitmap.Width / (double)Bitmap.Height;
            double requestedAspect = width / height;

            if (originalAspect > requestedAspect) {
                height = Math.Round(width * originalAspect);
            } else { 
                width = Math.Round(height * originalAspect);
            }
            return Scale((int)width, (int)height);
        }

        // Apply high quality scaling to the given pixel size.
        public Bitmap Scale(int width, int height) {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);
            destImage.SetResolution(Bitmap.HorizontalResolution, Bitmap.VerticalResolution);
            using (Graphics graphics = Graphics.FromImage(destImage)) {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                using (ImageAttributes wrapMode = new()) {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(Bitmap, destRect, 0, 0, Bitmap.Width, Bitmap.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }

        // Copy the original EXIF data into the bitmap given.
        public Bitmap AddExif(Bitmap bitmapToDecorate) {
            foreach (PropertyItem propertyItem in PropertyArray) {
                bitmapToDecorate.SetPropertyItem(propertyItem);
            }
            return bitmapToDecorate;
        }
    }
}
