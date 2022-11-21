using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.NetworkInformation;
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
            Write0 = 0b00000001,
            Write1 = 0b00000010,
            Read = 0b00000011,
            SavePosition = 0b00000100,
            SetOldPosition = 0b00000101,
            SetNullPosition = 0b10000000
        }

        SerialPort ComPort = new SerialPort();
        string InputData = String.Empty;
        int index = 0;
        bool isContinue = false;
        volatile bool isReading = false;

        static int packSize = 64;
        byte[] bytes0 = new byte[packSize];
        byte[] bytes1 = new byte[packSize];
        byte[] gettingBytes = new byte[packSize];

        byte[] outFile0 = new byte[512 * 512];
        byte[] outFile1 = new byte[512 * 512];


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
                bytes0[i] = 0x00;
                bytes1[i] = 0xFF;
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
                        else if (memoryArray[j] == '2')
                        {
                            color_data = 0 << 16; // R
                            color_data |= 0 << 8;   // G
                            color_data |= 255 << 0;   // B
                        }
                        else if (memoryArray[j] == '3')
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
                //ComPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
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
            gettingBytes = new byte[packSize];
            int totalBytes = 0;
            while (totalBytes != packSize)
            {
                int bufBytes = ComPort.BytesToRead;
                if (bufBytes > 0)
                {
                    totalBytes += bufBytes;
                    ComPort.Read(gettingBytes, totalBytes - bufBytes, bufBytes);
                }
            }
            isReading = true;
            if (totalBytes < packSize)
            {
                throw new Exception();
            }

            /*

            //InputData = ComPort.ReadExisting();
            

                if (isContinue)
                    ComPort.Write(new byte[] { 0b00000010 }, 0, 1);

            }
            else
            {
                for (int i = 0; i < packSize; i++)
                {
                    var inputData1 = Convert.ToString(gettingBytes1[i], 2).PadLeft(8, '0');

                    string bufByteStr = "";
                    for (int j = 0; j < 8; j++)
                    {
                        if (inputData1[j] == '1')
                        {
                            bufByteStr += "1";
                            //write ok
                            //green
                        }
                        else if (inputData1[j] == '0')
                        {

                            bufByteStr += "0";
                            //damage on 1
                            //blue
                        }
                    }

                    Dispatcher.Invoke(() =>
                    {
                        DrawArray(index, 8, bufByteStr);
                    });
                    index++;
                    if (index >= 512 * 512 / (8))
                    {
                        index = 0;
                        isContinue = false;
                    }

                    Dispatcher.Invoke(() =>
                    {
                        txtBox_recieve.AppendText(bufByteStr + "\r\n");
                        txtBox_recieve.ScrollToEnd();
                    });

                }

                if (isContinue)
                    ComPort.Write(new byte[] { 0b00000011 }, 0, 1);

            }
            */

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

        private void btn_send_Click(object sender, RoutedEventArgs e)
        {
            ClearBTMP();

            ComPort.Write(new byte[] { (byte)Commands.SetNullPosition }, 0, 1);
            Thread myThread = new Thread(MarchingTest);
            myThread.Start();
            Task tsk = new Task(MarchingTest);
            tsk.Start();
            
            isContinue = true;
            

        }

        void MarchingTest()
        {
            for (int k = 0; k < 512 * 512 / (8 * packSize); k++)
            {

                isReading = false;
                ComPort.Write(new byte[] { (byte)Commands.SavePosition }, 0, 1);
                ComPort.Write(new byte[] { (byte)Commands.Write0 }, 0, 1);
                ComPort.Write(new byte[] { (byte)Commands.SetOldPosition }, 0, 1);
                ComPort.Write(new byte[] { (byte)Commands.Read }, 0, 1);
                while (!isReading)
                {
                }
                byte[] gettingBytes1 = new byte[packSize];
                gettingBytes.CopyTo(gettingBytes1, 0);


                isReading = false;
                ComPort.Write(new byte[] { (byte)Commands.SetOldPosition }, 0, 1);
                ComPort.Write(new byte[] { (byte)Commands.Write1 }, 0, 1);
                ComPort.Write(new byte[] { (byte)Commands.SetOldPosition }, 0, 1);
                ComPort.Write(new byte[] { (byte)Commands.Read }, 0, 1);
                while (!isReading)
                {
                }
                byte[] gettingBytes2 = new byte[packSize];
                gettingBytes.CopyTo(gettingBytes2, 0);


                for (int i = 0; i < packSize; i++)
                {
                    var inputData1 = Convert.ToString(gettingBytes1[i], 2).PadLeft(8, '0');
                    var inputData2 = Convert.ToString(gettingBytes2[i], 2).PadLeft(8, '0');

                    string bufByteStr = "";
                    for (int j = 0; j < 8; j++)
                    {
                        if (inputData1[j] == '0' && inputData2[j] == '1')
                        {
                            bufByteStr += "1";
                            //write ok
                            //green
                        }
                        else if (inputData1[j] == '1' && inputData2[j] == '1')
                        {

                            bufByteStr += "2";
                            //damage on 1
                            //blue
                        }
                        else if (inputData1[j] == '0' && inputData2[j] == '0')
                        {

                            bufByteStr += "3";
                            //damage on 0
                            //blue
                        }
                        else //if (inputData1[j] == '1' && inputData2[j] == '0')
                        {

                            bufByteStr += "0";
                            //damage on inverse
                            //red
                        }
                    }

                    Dispatcher.Invoke(() =>
                    {
                        DrawArray(index, 8, bufByteStr);
                    });
                    index++;
                    if (index >= 512 * 512 / (8))
                    {
                        index = 0;
                        isContinue = false;
                    }

                    Dispatcher.Invoke(() =>
                    {
                        txtBox_recieve.AppendText(bufByteStr + "\r\n");
                        txtBox_recieve.ScrollToEnd();
                    });

                }
            }

        }
        void Read(object command)
        {
            for (int i = 0; i < 512 * 512 / (packSize * 8); i++)
            {
                isReading = false;
                ComPort.Write((byte[])command, 0, 1);

                while (!isReading)
                {

                }

                for (int j = 0; j < packSize; j++)
                {
                    var inputData1 = Convert.ToString(gettingBytes[j], 2).PadLeft(8, '0');

                    string bufByteStr = "";
                    for (int k = 0; k < 8; k++)
                    {
                        if (inputData1[k] == '1')
                        {
                            bufByteStr += "1";
                            //reading 1
                            //green
                        }
                        else if (inputData1[k] == '0')
                        {

                            bufByteStr += "0";
                            //reading 0
                            //blue
                        }
                    }

                    Dispatcher.Invoke(() =>
                    {
                        DrawArray(index, 8, bufByteStr);
                    });
                    index++;
                    if (index >= 512 * 512 / (8))
                    {
                        index = 0;
                        isContinue = false;
                    }

                    Dispatcher.Invoke(() =>
                    {
                        txtBox_recieve.AppendText(bufByteStr + "\r\n");
                        txtBox_recieve.ScrollToEnd();
                    });

                }

            }


        }

        //Commands
        //0b00000001 - write all 1
        //0b00000010 - write all 0
        //0b00000011 - read EEPROM
        //

        private void btn_SaveLogs_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllBytes(txtBx_LogName.Text + "_0.byte", outFile0);
            File.WriteAllBytes(txtBx_LogName.Text + "_1.byte", outFile1);
        }

        private void btn_Read_Click(object sender, RoutedEventArgs e)
        {

            ClearBTMP();
            ComPort.Write(new byte[] { 0b10000000}, 0, 1);

            Thread thread = new Thread(Read);
            thread.Start(new byte[] { 0b00000011 });

            isContinue = true;
        }

        private void btn_Write1_Click(object sender, RoutedEventArgs e)
        {
            ComPort.Write(new byte[] { 0b10000000 }, 0, 1);

            //for (int i = 0; i < 512 * 512 / (packSize * 8); i++)
            //{
            //    isReading = false;
            //    ComPort.Write(new byte[] { 0b00000100 }, 0, 1);
            //    ComPort.Write(new byte[] { 0b00000010 }, 0, 1);
            //    ComPort.Write(new byte[] { 0b00000101 }, 0, 1);
            //    ComPort.Write(new byte[] { 0b00000011 }, 0, 1);
            //    //ComPort.Write((byte[])command, 0, 1);

            //    while (!isReading)
            //    {

            //    }

            //    for (int j = 0; j < packSize; j++)
            //    {
            //        var inputData1 = Convert.ToString(gettingBytes[j], 2).PadLeft(8, '0');

            //        string bufByteStr = "";
            //        for (int k = 0; k < 8; k++)
            //        {
            //            if (inputData1[k] == '1')
            //            {
            //                bufByteStr += "1";
            //                //reading 1
            //                //green
            //            }
            //            else if (inputData1[k] == '0')
            //            {

            //                bufByteStr += "0";
            //                //reading 0
            //                //blue
            //            }
            //        }

            //        Dispatcher.Invoke(() =>
            //        {
            //            DrawArray(index, 8, bufByteStr);
            //        });
            //        index++;
            //        if (index >= 512 * 512 / (8))
            //        {
            //            index = 0;
            //            isContinue = false;
            //        }

            //        Dispatcher.Invoke(() =>
            //        {
            //            txtBox_recieve.AppendText(bufByteStr + "\r\n");
            //            txtBox_recieve.ScrollToEnd();
            //        });

            //    }

            //}

            Task tsk = new Task(Send);
            tsk.Start();
            //Thread thread = new Thread(Send);
            //thread.Start();
            //thread.Join();

        }

        void Send()
        {
            for (int i = 0; i < 512 * 512 / (packSize * 8); i++)
            {
                //if(i > 10)
                //    throw new Exception();
                isReading = false;

                byte[] bt = new byte[1];
                ComPort.Write(new byte[] { 0b00000100 }, 0, 1);
                bt[0] = (byte)ComPort.ReadByte();
                ComPort.Write(new byte[] { 0b00000010 }, 0, 1);
                bt[0] = (byte)ComPort.ReadByte();
                ComPort.Write(new byte[] { 0b00000101 }, 0, 1);
                bt[0] = (byte)ComPort.ReadByte();
                ComPort.Write(new byte[] { 0b00000011 }, 0, 1);




                var buf = new byte[packSize];
                int totalBytes = 0;
                while (totalBytes != packSize)
                {
                    int bufBytes = ComPort.BytesToRead;
                    if (bufBytes > 0)
                    {
                        totalBytes += bufBytes;
                        ComPort.Read(buf, totalBytes - bufBytes, bufBytes);
                    }
                }

                
                for (int j = 0; j < packSize; j++)
                {
                    var inputData1 = Convert.ToString(buf[j], 2).PadLeft(8, '0');

                    string bufByteStr = "";
                    for (int k = 0; k < 8; k++)
                    {
                        if (inputData1[k] == '1')
                        {
                            bufByteStr += "1";
                            //reading 1
                            //green
                        }
                        else if (inputData1[k] == '0')
                        {

                            bufByteStr += "0";
                            //reading 0
                            //blue
                        }
                    }

                    Dispatcher.Invoke(() =>
                    {
                        DrawArray(index, 8, bufByteStr);
                    });
                    index++;
                    if (index >= 512 * 512 / (8))
                    {
                        index = 0;
                        isContinue = false;
                    }

                    Dispatcher.Invoke(() =>
                    {
                        txtBox_recieve.AppendText(bufByteStr + "\r\n");
                        txtBox_recieve.ScrollToEnd();
                    });

                }

            }

            throw new Exception();

        }
    }
}
