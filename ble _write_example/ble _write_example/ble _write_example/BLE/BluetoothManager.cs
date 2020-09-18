using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace ble__write_example.BLE
{
    public class BluetoothManager
    {
        #region Singleton
        private static readonly Lazy<BluetoothManager> lazyBluetoothManager = new Lazy<BluetoothManager>(() => new BluetoothManager());
        public static BluetoothManager Instance
        {
            get { return lazyBluetoothManager.Value; }

        }
        #endregion

        private static readonly Guid uuid_service = new Guid("fa4a4bfc-1f1b-4ddd-98bf-7d84129af27b");
        private static readonly Guid uuid_characteristic = new Guid("4f0c6064-49c2-4d07-ae60-0133fd80722c");

        public IBluetoothLE IBLE;
        public IAdapter AdapterBLE { get; set; }
        public IDevice BLEDevice { get; set; }
        public ICharacteristic Charateristic { get; set; }
        public int MTU { get; private set; }

        public ObservableCollection<IDevice> DeviceList { get; set; }

        public BluetoothManager()
        {
            IBLE = CrossBluetoothLE.Current;
            AdapterBLE = CrossBluetoothLE.Current.Adapter;
            DeviceList = new ObservableCollection<IDevice>();

            AdapterBLE.DeviceDiscovered += Adapter_DeviceDiscovered;
            AdapterBLE.DeviceConnected += Adapter_DeviceConnected;
            AdapterBLE.DeviceDisconnected += Adapter_DeviceDisconnected;
            AdapterBLE.ScanTimeoutElapsed += Adapter_ScanTimeoutElapsed;
        }

        public async Task<bool> CheckServiceAndCharateristics()
        {
            if (this.BLEDevice == null || this.BLEDevice.State != Plugin.BLE.Abstractions.DeviceState.Connected)
                return false;

            this.MTU = await BLEDevice.RequestMtuAsync(517);
            var service = await BLEDevice.GetServiceAsync(uuid_service);
            if (service == null)
                return false;

            Charateristic = await service.GetCharacteristicAsync(uuid_characteristic);

            if (Charateristic == null)
                return false;

            return true;
        }

        public void StartScanning()
        {
            StartScanning(Guid.Empty);
        }

        void StartScanning(Guid forService)
        {
            if (AdapterBLE.IsScanning)
            {
                AdapterBLE.StopScanningForDevicesAsync();
                Debug.WriteLine("adapter.StopScanningForDevices()");
            }
            else
            {
                DeviceList.Clear();
                AdapterBLE.StartScanningForDevicesAsync();
                Debug.WriteLine("adapter.StartScanningForDevices(" + forService + ")");
            }
        }

        public async void StopScanning()
        {
            if (AdapterBLE.IsScanning)
            {
                Debug.WriteLine("Still scanning, stopping the scan");
                await AdapterBLE.StopScanningForDevicesAsync();
            }
        }

        void Adapter_DeviceDiscovered(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            if (DeviceList.Contains(e.Device) == false && e.Device.Name?.Contains("esp32") == true)
                DeviceList.Add(e.Device);
        }

        void Adapter_DeviceConnected(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            Debug.WriteLine("Device already connected");
        }

        void Adapter_DeviceDisconnected(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            //DeviceDisconnectedEvent?.Invoke(sender,e);
            Debug.WriteLine("Device already disconnected");
        }

        void Adapter_ScanTimeoutElapsed(object sender, EventArgs e)
        {
            AdapterBLE.StopScanningForDevicesAsync();
            Debug.WriteLine("Timeout", "Bluetooth scan timeout elapsed");
        }

        public void DisconnectDevice()
        {
            if (BLEDevice != null)
            {
                AdapterBLE.DisconnectDeviceAsync(BLEDevice);
                BLEDevice.Dispose();
                BLEDevice = null;

                Charateristic = null;
            }
        }
    }
}
