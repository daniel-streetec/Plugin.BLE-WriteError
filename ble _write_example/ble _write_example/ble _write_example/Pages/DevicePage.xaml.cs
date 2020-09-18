using ble__write_example.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ble__write_example.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DevicePage : ContentPage
    {
        public DevicePage()
        {
            this.BindingContext = BluetoothManager.Instance.DeviceList;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BluetoothManager.Instance.StartScanning();
        }

        private async void lvDevices_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            BluetoothManager.Instance.StopScanning();
            try
            {
                BluetoothManager.Instance.BLEDevice = e.Item as IDevice;
                var device = e.Item as IDevice;
                if (BluetoothManager.Instance.AdapterBLE.ConnectedDevices.Count == 0)
                {
                    await BluetoothManager.Instance.AdapterBLE.ConnectToDeviceAsync(device);
                    if (await BluetoothManager.Instance.CheckServiceAndCharateristics())
                    {
                        await Navigation.PushAsync(new MainPage());
                    }
                }
                else
                {
                    await BluetoothManager.Instance.AdapterBLE.DisconnectDeviceAsync(device);
                }
            }
            catch (DeviceConnectionException ex)
            {
                await DisplayAlert("Error", "Could not connect to :" + ex.DeviceId, "OK");
            }
        }

        private void btnScan_Clicked(object sender, EventArgs e)
        {
            BluetoothManager.Instance.StartScanning();
        }
    }
}