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
        private bool firstClick = false;
        private bool isDrawingLine = false;

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
            if (!firstClick && isDrawingLine)
            {
                firstClick = true;
                start = e.GetPosition(imageControl);
                return;
            }
            if (firstClick && isDrawingLine)
            {
                drawLine(start, e.GetPosition(imageControl), bitmap);
                imageControl.Source = bitmap;
                firstClick = false;
                ++idCouter;
            }
            if (!isDrawingLine)
            {

            }
            //idToPoints.Add(idCouter, new List<Point>());
            //idToPoints[idCouter].Add(start);
            //idToStartEnd.Add(idCouter, new List<Point>());
            //idToStartEnd[idCouter].Add(start);
            //pointToId.Add(start, new List<int>());
            //pointToId[start].Add(idCouter);
        }


        private void imageControl_MouseMove(object sender, MouseEventArgs e)
        {
            //Animation
            //if (isDrawing == false)
            //    return;
            //drawingBitmap = bitmap.Clone();
            //drawLine(e.GetPosition(imageControl), drawingBitmap);
            //imageControl.Source = drawingBitmap;

            if (isDrawing == false)
                return;
            

        }
        // wikipedia
        private void drawLine(Point start,Point end, WriteableBitmap writeableBitmap)
        {
            if(end == start && end.X == start.X && end.Y == start.Y)
            {
                return;
            }
            int x0 = (int)start.X;
            int y0 = (int)start.Y;
            int x1 = (int)end.X;
            int y1 = (int)end.Y;
            int xf = x0, yf = y0;
            int xb = x1, yb = y1;
            int dx = Math.Abs(x1 - x0);
            int sx = (x0 < x1) ? 1 : -1;
            int dy = -Math.Abs(y1 - y0);
            int sy = (y0 < y1) ? 1 : -1;
            int error = dx + dy;

            putPixel(xf, yf, writeableBitmap);
            putPixel(xb, yb, writeableBitmap);
            while (true)
            {
                putPixel(xf, yf, writeableBitmap);
                putPixel(xb, yb, writeableBitmap);

                if ((xb == xf || xb + 1 == xf || xb - 1 == xf) && (yb == yf || yb + 1 == yf || yb - 1 == yf) )
                    break;

                int e2 = 2 * error;

                if (e2 >= dy)
                {
                    if (xb == xf)
                        break;

                    error += dy;
                    xf += sx;
                    xb -= sx;
                }

                if (e2 <= dx)
                {
                    if (yb == yf)
                        break;

                    error += dx;
                    yf += sy;
                    yb -= sy;
                }
            }
        }


        private void putPixel(int x, int y, WriteableBitmap writeableBitmap)
        {
            int width = (int)writeableBitmap.PixelWidth;
            int height = (int)writeableBitmap.PixelHeight;

            int stride = writeableBitmap.PixelWidth * (writeableBitmap.Format.BitsPerPixel / 8);
            byte[] pixels = new byte[writeableBitmap.PixelHeight * stride];

            writeableBitmap.CopyPixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);

            int offset = y * stride + x * (writeableBitmap.Format.BitsPerPixel / 8);

            pixels[offset] = 0;     // Blue component
            pixels[offset + 1] = 0; // Green component
            pixels[offset + 2] = 0; // Red component
            pixels[offset + 3] = 255; // Alpha component (fully opaque)

            writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);
        }

        private void imageControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //drawingBitmap = bitmap;

            //imageControl.Source = drawingBitmap;
            //drawLine(start, e.GetPosition(imageControl), bitmap);
            //isDrawing = false;
            //imageControl.UpdateLayout();
            //++idCouter;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            isDrawingLine = !isDrawingLine;
        }

    }
}
