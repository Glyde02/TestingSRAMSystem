using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography;
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
        enum Commands
        {
            Read = 0b00000001,
            Write0 = 0b00000010,
            Write1 = 0b00000011,
            SavePosition = 0b00001000,
            LoadPosition = 0b00001100,
            ClearPosition = 0b00001110,
            Marching = 0b10000000
        }

        Commands command = Commands.Read;

        SerialPort ComPort = new SerialPort();
        string InputData = String.Empty;
        int index = 0;
        bool isContinue = false;
        bool isFirst = true;

        static int packSize = 64;
        byte[] bytes = new byte[packSize];
        byte[] bytes0 = new byte[packSize];
        byte[] bytes1 = new byte[packSize];
        int offset;

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
        public void DrawArrayMarching(int index, int size, string memoryArray)
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
                        else if (memoryArray[j] == '2')
                        {
                            color_data = 255 << 16; // R
                            color_data |= 0 << 8;   // G
                            color_data |= 0 << 0;   // B
                        }
                        else if(memoryArray[j] == '3')
                        {
                            color_data = 0 << 16; // R
                            color_data |= 0 << 8;   // G
                            color_data |= 255 << 0;   // B
                        }
                        else if (memoryArray[j] == '4')
                        {
                            color_data = 255 << 16; // R
                            color_data |= 0 << 8;   // G
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
            int totalBytes = 0;
            byte[] gettingBytes = new byte[packSize];


            switch (command)
            {
                case Commands.Read:

                    while (totalBytes != packSize)
                    {
                        int bufBytes = ComPort.BytesToRead;
                        totalBytes += bufBytes;
                        ComPort.Read(gettingBytes, totalBytes - bufBytes, bufBytes);
                    }

                    foreach (byte b in gettingBytes)
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
                        ComPort.Write(new byte[] { (byte)command }, 0, 1);

                    break;
                case Commands.Write0:
                    while (totalBytes != packSize)
                    {
                        int bufBytes = ComPort.BytesToRead;
                        totalBytes += bufBytes;
                        ComPort.Read(gettingBytes, totalBytes - bufBytes, bufBytes);
                    }

                    foreach (byte b in gettingBytes)
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
                        ComPort.Write(new byte[] { (byte)command }, 0, 1);

                    break;
                case Commands.Write1:

                    while (totalBytes != packSize)
                    {
                        int bufBytes = ComPort.BytesToRead;
                        totalBytes += bufBytes;
                        ComPort.Read(gettingBytes, totalBytes - bufBytes, bufBytes);
                    }

                    foreach (byte b in gettingBytes)
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
                        ComPort.Write(new byte[] { (byte)command }, 0, 1);
                    break;
                case Commands.Marching:

                    var getBytes = new byte[packSize * 2];
                    while (totalBytes != packSize*2)
                    {
                        int bufBytes = ComPort.BytesToRead;
                        totalBytes += bufBytes;
                        ComPort.Read(getBytes, totalBytes - bufBytes, bufBytes);
                    }
                    Array.Copy(getBytes, 0, bytes0, 0, packSize);
                    Array.Copy(getBytes, packSize, bytes1, 0, packSize);

                    var resultBytes = CompareResults();
                    Dispatcher.Invoke(() =>
                    {
                        DrawArrayMarching(index, 8*packSize, resultBytes);
                    });
                    index++;
                    if (index >= 512 * 512 / (8*packSize))
                    {
                        index = 0;
                        isContinue = false;
                    }
                    Dispatcher.Invoke(() =>
                    {
                        txtBox_recieve.AppendText(resultBytes + "\r\n");
                        txtBox_recieve.ScrollToEnd();
                    });

                    if (isContinue)
                        ComPort.Write(new byte[] { (byte)command }, 0, 1);

                    break;
            }

        }

        private string CompareResults()
        {
            var resultBytes = new byte[packSize];
            string buf = "";
            for(int i = 0; i < packSize; i++)
            {
                string bytes0Str = Convert.ToString(bytes0[i], 2).PadLeft(8, '0');
                string bytes1Str = Convert.ToString(bytes1[i], 2).PadLeft(8, '0');
                for (int j = 0; j < 8; j++)
                {
                    if (bytes0Str[j] == '0' && bytes1Str[j] == '1') //good
                    {
                        buf += '1';
                    }
                    else if (bytes0Str[j] == '0' && bytes1Str[j] == '0') //bad on zero
                    {
                        buf += '2';
                    }
                    else if (bytes0Str[j] == '1' && bytes1Str[j] == '1') //bad on one
                    {
                        buf += '3';
                    }
                    else //bad on invertor
                    {
                        buf += '4';
                    }
                }

            }
            return buf;

        }

        private void btn_send_Click(object sender, RoutedEventArgs e)
        {
            index = 0;

            command = Commands.Read;
            ClearBTMP();
            ComPort.Write(new byte[] { (byte)Commands.ClearPosition }, 0, 1);
            ComPort.Write(new byte[] { (byte)command }, 0, 1);

            //ComPort.Write(new byte[] { 0b10000000 }, 0, 1);
            //command = Commands.Write1;
            isContinue = true;

        }

        private void btn_Write0_Click(object sender, RoutedEventArgs e)
        {
            command = Commands.Write0;
            index = 0;
            ClearBTMP();
            ComPort.Write(new byte[] { (byte)Commands.ClearPosition }, 0, 1);
            ComPort.Write(new byte[] { (byte)command }, 0, 1);
            //ComPort.Write(new byte[] { 0b10000000 }, 0, 1);
            //command = Commands.Write1;
            isContinue = true;
        }

        private void ClearBTMP()
        {
            byte[] colorData = new byte[4 * 512 * 512];
            for (int i = 0; i < colorData.Length; i++)
            {
                colorData[i] = 0;
            }

            Int32Rect rect = new Int32Rect(0, 0, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight);
            int stride = (writeableBitmap.PixelWidth * writeableBitmap.Format.BitsPerPixel) / 8;
            writeableBitmap.WritePixels(rect, colorData, stride, 0);
        }

        private void btn_Write1_Click(object sender, RoutedEventArgs e)
        {
            command = Commands.Write1;
            index = 0;
            ClearBTMP();
            ComPort.Write(new byte[] { (byte)Commands.ClearPosition }, 0, 1);
            ComPort.Write(new byte[] { (byte)command }, 0, 1);
            //ComPort.Write(new byte[] { 0b10000000 }, 0, 1);
            //command = Commands.Write1;
            isContinue = true;
        }

        private void btn_Marching_Click(object sender, RoutedEventArgs e)
        {
            command = Commands.Marching;
            index = 0;
            ClearBTMP();
            ComPort.Write(new byte[] { (byte)Commands.ClearPosition }, 0, 1);
            ComPort.Write(new byte[] { (byte)Commands.Marching }, 0, 1);
            
            //ComPort.Write(new byte[] { 0b10000000 }, 0, 1);
            //command = Commands.Write1;
            isContinue = true;
        }
    }
}
