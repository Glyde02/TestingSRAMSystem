using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace EEPROM_Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort ComPort = new SerialPort();
        string InputData = String.Empty;
        int index = 0;
        bool isContinue = false;

        static int packSize = 64;
        byte[] bytes = new byte[packSize];

        public string testStr1 = "";
        public string testStr2 = "";
        public string recvStr1 = "";

        private WriteableBitmap writeableBitmap;

        public MainWindow()
        {
            InitializeComponent();


            writeableBitmap = new WriteableBitmap(
                (int)imgMemoryArray.Width,
                (int)imgMemoryArray.Height,
                96,
                96,
                PixelFormats.Bgr32,
                null);

            imgMemoryArray.Source = writeableBitmap;

            imgMemoryArray.Stretch = Stretch.None;
            imgMemoryArray.HorizontalAlignment = HorizontalAlignment.Left;
            imgMemoryArray.VerticalAlignment = VerticalAlignment.Top;


            for (int i = 0; i < packSize; i++)
            {
                testStr1 += '1';
                testStr2 += '0';
                bytes[i] = 0xFF;
            }

            //for (int i = 0; i < 512; i++)
            //{
            //    if (i % 4 != 0)
            //        DrawArray(i, size, testStr2);
            //    else
            //        DrawArray(i, size, testStr1);
            //}



        }



        public void DrawArray(int index, int size, string memoryArray)
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
                        if (memoryArray[j] == '1')
                        {
                            // Compute the pixel's color.
                            color_data = 0 << 16; // R
                            color_data |= 255 << 8;   // G
                            color_data |= 0 << 0;   // B
                        }
                        else if (memoryArray[j] == '0')
                        {
                            color_data = 255 << 16; // R
                            color_data |= 0 << 8;   // G
                            color_data |= 0 << 0;   // B
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

        public void GetPorts()
        {

        }

        private void btn_getPorts_Click(object sender, RoutedEventArgs e)
        {
            string[] ArrayComPortsNames = null;
            int index = -1;
            string ComPortName = null;

            ArrayComPortsNames = SerialPort.GetPortNames();
            do
            {
                index += 1;
                cb_ports.Items.Add(ArrayComPortsNames[index]);
            }

            while (!((ArrayComPortsNames[index] == ComPortName)
              || (index == ArrayComPortsNames.GetUpperBound(0))));
            Array.Sort(ArrayComPortsNames);

            //want to get first out
            if (index == ArrayComPortsNames.GetUpperBound(0))
            {
                ComPortName = ArrayComPortsNames[0];
            }
            cb_ports.Text = ComPortName;
        }

        private void btn_start_stop_connection_Click(object sender, RoutedEventArgs e)
        {
            if (btn_start_stop_connection.Content.ToString() == "Start connection")
            {
                btn_start_stop_connection.Content = "Stop connection";
                ComPort.PortName = Convert.ToString(cb_ports.Text);
                ComPort.BaudRate = 115200;
                ComPort.DataBits = 8;
                ComPort.StopBits = StopBits.One;
                ComPort.Parity = Parity.None;
                ComPort.Handshake = Handshake.None;
                ComPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                ComPort.Open();

            }
            else if (btn_start_stop_connection.Content.ToString() == "Stop connection")
            {
                btn_start_stop_connection.Content = "Start connection";
                ComPort.Close();
            }


        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            //InputData = ComPort.ReadExisting();
            byte[] gettingBytes = new byte[packSize];
            int totalBytes = 0;
            while (totalBytes != packSize)
            {
                int bufBytes = ComPort.BytesToRead;
                totalBytes += bufBytes;
                ComPort.Read(gettingBytes, totalBytes - bufBytes, bufBytes);
            }
            foreach(byte b in gettingBytes)
            {

                var inputData = Convert.ToString(b, 2).PadLeft(8, '0');
                Dispatcher.Invoke(() =>
                {
                    DrawArray(index, 8, inputData);
                });
                index++;
                if (index >= 512 * 512 / (8))
                {
                    index = 0;
                    isContinue = false;
                }

                Dispatcher.Invoke(() =>
                {
                    txtBox_recieve.AppendText(inputData + "\r\n");
                    txtBox_recieve.ScrollToEnd();
                });
            }

            if (isContinue)
                ComPort.Write(bytes, 0, packSize);

            //if (buffer.Length == 0)
            //{
            //    if (InputData.Length < packSize)
            //    {
            //        isPartial = true;
            //        buffer += InputData;
            //    }
            //    else
            //    {
            //        isPartial = false;
            //        buffer = InputData;
            //    }
            //}
            //else
            //{
            //    buffer += InputData;
            //    if (buffer.Length == packSize)
            //        isPartial = false;
            //}

            //if (!isPartial)
            //{
            //    recvStr1 = InputData;
            //    Dispatcher.Invoke(() =>
            //    {

            //        DrawArray(index, packSize, buffer);
            //    });
            //    index++;
            //    if (index >= 512*512 / packSize)
            //    {
            //        index = 0;
            //        testStr1 = testStr2;
            //    }

            //    Dispatcher.Invoke(() =>
            //    {
            //        txtBox_recieve.AppendText(buffer + "\r\n");
            //        txtBox_recieve.ScrollToEnd();
            //    });
            //    buffer = "";


            //    //ComPort.Write(testStr1);
            //}

        }

        private void btn_send_Click(object sender, RoutedEventArgs e)
        {
            //ComPort.Write("Hello\0");
            //ComPort.Write(testStr1);
            
            ComPort.Write(bytes, 0, packSize);
            isContinue = true;

        }


    }
}
