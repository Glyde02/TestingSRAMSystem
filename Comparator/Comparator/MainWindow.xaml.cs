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

namespace Comparator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private byte[] first_0 = new byte[512 * 512];
        private byte[] first_1 = new byte[512 * 512];
        private byte[] second_0 = new byte[512 * 512];
        private byte[] second_1 = new byte[512 * 512];

        private WriteableBitmap writeableBitmap0;
        private WriteableBitmap writeableBitmap1;
        public MainWindow()
        {
            InitializeComponent();

            writeableBitmap0 = new WriteableBitmap(
                (int)img_first.Width,
                (int)img_first.Height,
                96,
                96,
                PixelFormats.Bgr32,
                null);

            img_first.Source = writeableBitmap0;

            img_first.Stretch = Stretch.None;
            img_first.HorizontalAlignment = HorizontalAlignment.Left;
            img_first.VerticalAlignment = VerticalAlignment.Top;

            writeableBitmap1 = new WriteableBitmap(
                (int)img_second.Width,
                (int)img_second.Height,
                96,
                96,
                PixelFormats.Bgr32,
                null);

            img_second.Source = writeableBitmap1;

            img_second.Stretch = Stretch.None;
            img_second.HorizontalAlignment = HorizontalAlignment.Left;
            img_second.VerticalAlignment = VerticalAlignment.Top;
        }

        private void btn_Show_Click(object sender, RoutedEventArgs e)
        {
            DrawArray(0, 512 * 512, first_0, writeableBitmap0);
        }

        private void btn_OpenFirst_Click(object sender, RoutedEventArgs e)
        {
            first_0 = File.ReadAllBytes(txtBx_FileFirst.Text + "_0.byte");
            first_0 = File.ReadAllBytes(txtBx_FileFirst.Text + "_1.byte");
        }

        private void btn_OpenSecond_Click(object sender, RoutedEventArgs e)
        {
            second_0 = File.ReadAllBytes(txtBx_FileSecond.Text + "_0.byte");
            second_0 = File.ReadAllBytes(txtBx_FileSecond.Text + "_1.byte");
        }

        public void DrawArray(int index, int size, byte[] memoryArray, WriteableBitmap writeableBitmap)
        {
            int j = 0;
            for (int i = index * size; i < index * size + memoryArray.Length; i++)
            {

                int column = i % 512;
                int row = (int)(i / 512);

                try
                {
                    unsafe
                    {
                        writeableBitmap.Lock();

                        IntPtr pBackBuffer = writeableBitmap.BackBuffer;

                        // Find the address of the pixel to draw.
                        pBackBuffer += row * writeableBitmap.BackBufferStride;
                        pBackBuffer += column * 4;
                        int color_data = 0;
                        if (memoryArray[j] == 1)
                        {
                            // Compute the pixel's color.
                            color_data = 0 << 16; // R
                            color_data |= 255 << 8;   // G
                            color_data |= 0 << 0;   // B
                        }
                        else if (memoryArray[j] == 0)
                        {
                            color_data = 255 << 16; // R
                            color_data |= 0 << 8;   // G
                            color_data |= 0 << 0;   // B
                        }
                        else if (memoryArray[j] == 2)
                        {
                            color_data = 0 << 16; // R
                            color_data |= 0 << 8;   // G
                            color_data |= 255 << 0;   // B
                        }
                        else if (memoryArray[j] == 3)
                        {
                            color_data = 0 << 16; // R
                            color_data |= 0 << 8;   // G
                            color_data |= 255 << 0;   // B
                        }
                        else
                        {
                            color_data = 255 << 16; // R
                            color_data |= 255 << 8;   // G
                            color_data |= 255 << 0;   // B
                        }

                        *((int*)pBackBuffer) = color_data;
                    }
                    writeableBitmap.AddDirtyRect(new Int32Rect(column, row, 1, 1));
                }
                finally
                {
                    // Release the back buffer and make it available for display.
                    writeableBitmap.Unlock();
                }
                j++;
            }

        }
    }
}
