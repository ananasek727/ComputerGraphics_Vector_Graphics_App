using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Point = System.Windows.Point;

namespace VectorGraphics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isDrawing = false;
        private Point start;
        private int idCouter = 0;

        private WriteableBitmap bitmap = new WriteableBitmap(1, 1, 96, 96, PixelFormats.Bgra32, null);
        private WriteableBitmap drawingBitmap = new WriteableBitmap(1, 1, 96, 96, PixelFormats.Bgra32, null);


        private Dictionary<int, List<Point>> idToPoints = new Dictionary<int, List<Point>>();
        private Dictionary<int, List<Point>> idToStartEnd = new Dictionary<int, List<Point>>();
        private Dictionary<Point, List<int>> pointToId = new Dictionary<Point, List<int>>();


        public MainWindow()
        {
            InitializeComponent();
        }


        //Initialize Image Control
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            int heigth = (int)imageControlGrid.ActualHeight;
            int width = (int)imageControlGrid.ActualWidth;

            //96 is the default DPI for WPF
            bitmap = new WriteableBitmap(width, heigth, 96, 96, PixelFormats.Bgra32, null);
            paintWhite(bitmap);
            imageControl.Source = bitmap;
            drawingBitmap = bitmap;
        }


        public void paintWhite(WriteableBitmap bitmap)
        {
            int stride = bitmap.PixelWidth * (bitmap.Format.BitsPerPixel / 8);
            byte[] pixels = new byte[bitmap.PixelHeight * stride];

            // Set all pixels to white (255, 255, 255, 255)
            for (int i = 0; i < pixels.Length; i += 4)
            {
                pixels[i] = 0;     // Blue component
                pixels[i + 1] = 255; // Green component
                pixels[i + 2] = 255; // Red component
                pixels[i + 3] = 255; // Alpha component
            }

            bitmap.WritePixels(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight), pixels, stride, 0);
        }


        private void imageControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDrawing = true;
            start = e.GetPosition(imageControl);
            //idToPoints.Add(idCouter, new List<Point>());
            //idToPoints[idCouter].Add(start);
            //idToStartEnd.Add(idCouter, new List<Point>());
            //idToStartEnd[idCouter].Add(start);
            //pointToId.Add(start, new List<int>());
            //pointToId[start].Add(idCouter);
        }


        private void imageControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing == false)
                return;

        }


        // Symmetric Midpoint Line Algorithm
        private void drawLine(Point point)
        {
            int x1 = (int)start.X;
            int y1 = (int)start.Y;
            int x2 = (int)point.X;
            int y2 = (int)point.Y;
            int dx = x2 - x1;
            int dy = y2 - y1;
            int d = 2 * dy - dx;
            int dE = 2 * dy;
            int dNE = 2 * (dy - dx);
            int xf = x1;
            int yf = y1;
            int xb = x2;
            int yb = y2;
            int sx = x1 < x2 ? 1 : -1;
            int sy = y1 < y2 ? 1 : -1;
            putPixel(xf, yf);
            putPixel(xb, yb);
            while (xf < xb)
            {
                xf += sx;
                xb -= sx;
                if (d < 0)
                    d += dE;
                else
                {
                    d += dNE;
                    yf += sy;
                    yb -= sx;
                }
                putPixel(xf, yf);
                putPixel(xb, yb);
            }
        }

        private void putPixel(int x, int y)
        {
            int width = (int)bitmap.PixelWidth;
            int height = (int)bitmap.PixelHeight;

            int stride = bitmap.PixelWidth * (bitmap.Format.BitsPerPixel / 8);
            byte[] pixels = new byte[bitmap.PixelHeight * stride];

            bitmap.CopyPixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);

            int offset = y * stride + x * (bitmap.Format.BitsPerPixel / 8);

            pixels[offset] = 0;     // Blue component
            pixels[offset + 1] = 0; // Green component
            pixels[offset + 2] = 0; // Red component
            pixels[offset + 3] = 255; // Alpha component (fully opaque)

            bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
        }

        private void imageControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //drawingBitmap = bitmap;
            drawLine(e.GetPosition(imageControl));
            //imageControl.Source = drawingBitmap;
            
            isDrawing = false;
            imageControl.UpdateLayout();
            //++idCouter;
        }
    }
}
