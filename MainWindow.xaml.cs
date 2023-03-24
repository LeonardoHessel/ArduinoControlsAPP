using System;
using System.Collections.Generic;
using System.IO.Ports;
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

namespace ArduinoControlsAPP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort serialPort;
        string[] serialPorts;


        public MainWindow()
        {
            InitializeComponent();
            InitializeSerialPorts();
        }

        private void InitializeSerialPorts()
        {
            serialPorts = SerialPort.GetPortNames();
            if (serialPorts.Count() != 0)
            {
                foreach (string serial in  serialPorts)
                {
                    var serialItems = SerialPortNamesComboBox.Items;

                    if (!serialItems.Contains(serial))
                    {
                        SerialPortNamesComboBox.Items.Add(serial);
                    }
                }
                SerialPortNamesComboBox.SelectedIndex = 0;
            }
        }

        #region Conect to Arduino or Other
        bool isConnected = false;
        private void Connect()
        {
            try
            {
                string selectedSerialPort = 
                    SerialPortNamesComboBox.SelectedItem.ToString();

                serialPort = new SerialPort(selectedSerialPort, 9600);
                serialPort.Open();
                SerialPortConnectButton.Content = "Disconnect";
                LedControlPanel.IsEnabled = true;
                isConnected = true;

            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("The selected serial port is busy!", "Busy", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("There is no serial port!", "Empty Serial Port", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Disconnect()
        {
            SerialPortConnectButton.Content = "Connect";
            LedControlPanel.IsEnabled = false;
            isConnected = false;
            ResetControl();
            serialPort.Close();
        }

        private void ResetControl()
        {
            LedOffButton.Background = Brushes.DarkGray;
            LedOffButton.Background = Brushes.DarkGray;
            BrightnessSlider.Value = 0;
            BrightnessSliderLabel.Content = 0;
        }

        private void SerialPortConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isConnected)
            {
                Connect();
            }
            else
            {
                Disconnect();
            }
        }
        #endregion

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            InitializeSerialPorts();
        }

        #region Led Switch
        private void LedOffButton_Click(object sender, RoutedEventArgs e)
        {
            serialPort.Write("256\n");
            LedOnButton.Background = Brushes.DarkGray;
            LedOnButton.Background = Brushes.LightPink;
        }

        private void LedOnButton_Click(object sender, RoutedEventArgs e)
        {
            serialPort.Write("257\n");
            LedOnButton.Background = Brushes.LightPink;
            LedOnButton.Background = Brushes.DarkGray;
        }
        #endregion

        #region Led Brightness Control
        private void BrightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int sliderValue = (int)BrightnessSlider.Value;
            serialPort.Write(sliderValue.ToString() + "\n");
            BrightnessSliderLabel.Content = sliderValue.ToString();
        } 
        #endregion
    }
}
