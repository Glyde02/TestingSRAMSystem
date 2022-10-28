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

        public MainWindow()
        {
            InitializeComponent();
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
            InputData = ComPort.ReadExisting();
                        
            index++;
            Dispatcher.Invoke(() =>
                {
                    txtBox_recieve.AppendText(InputData);
                    txtBox_recieve.ScrollToEnd();
                });


        }

        private void btn_send_Click(object sender, RoutedEventArgs e)
        {
            ComPort.Write("Hello xyi\0");
        }
    }
}
