using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ble__write_example
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BLE.BluetoothManager.Instance.BLEDevice == null)
                await Navigation.PushAsync(new Pages.DevicePage());

            lbMTU.Text = $"MTU: {BLE.BluetoothManager.Instance.MTU}";
        }

        private async void btnSendShort_Clicked(object sender, EventArgs e)
        {
            await SendToBLE("Less Bytes");
        }

        private async void btnSendLong_Clicked(object sender, EventArgs e)
        {
            await SendToBLE("This is a very long text, at least longer than 20 bytes");
        }

        private async Task<bool> SendToBLE(string toSend)
        {
            byte[] data = Encoding.ASCII.GetBytes(toSend);
            Debug.WriteLine($"Sending {data.Count()} bytes");
            return await BLE.BluetoothManager.Instance.Charateristic.WriteAsync(data);
        }
    }
}
