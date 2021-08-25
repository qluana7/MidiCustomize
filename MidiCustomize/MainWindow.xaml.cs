using Midi.Devices;
using System;
using System.Collections.Generic;
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

namespace MidiCustomize
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Midi.Midi Midi { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            
            Midi = new Midi.Midi()
            {
                Logic = new Midi.PianoConverterLogic()
            };

            Midi.Sent += async (sender, e) =>
            {
                SentRadio.Dispatcher.Invoke(() => SentRadio.IsChecked = true);
                await Task.Delay(50);
                SentRadio.Dispatcher.Invoke(() => SentRadio.IsChecked = false);

            };

            ReloadBtn_Click(null, null);
        }

        private void Input_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Midi.Input = e.AddedItems.Cast<Devices>().FirstOrDefault().Device as IInputDevice;
        }

        private void Output_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Midi.Output = e.AddedItems.Cast<Devices>().FirstOrDefault().Device as IOutputDevice;
        }

        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            try { Midi.StartReceiving(new Clock(120)); }
            catch (Exception err) { MessageBox.Show(err.Message); }

            IsOpenLabel.Text = Midi.IsRunning.ToString();
            
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            try { Midi.CloseReceiving(); }
            catch (Exception err) { MessageBox.Show(err.Message); }

            IsOpenLabel.Text = Midi.IsRunning.ToString();
        }

        private void ReloadBtn_Click(object sender, RoutedEventArgs e)
        {
            Input.Items.Clear();
            Output.Items.Clear();

            foreach (var item in MidiCustomize.Midi.Midi.GetDevices<IInputDevice>())
                Input.Items.Add(new Devices() { Device = item });

            foreach (var item in MidiCustomize.Midi.Midi.GetDevices<IOutputDevice>())
                Output.Items.Add(new Devices() { Device = item });
        }
    }

    public struct Devices
    {
        public IDeviceBase Device { get; set; }

        public override string ToString() =>
            Device.Name;
    }
}
