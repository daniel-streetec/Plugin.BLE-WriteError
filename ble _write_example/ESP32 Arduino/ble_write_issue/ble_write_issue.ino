#include <BLEDevice.h>
#include <BLEServer.h>
#include <BLEUtils.h>
#include <BLE2902.h>

#define UUID_SERVICE        "fa4a4bfc-1f1b-4ddd-98bf-7d84129af27b"
#define UUID_CHARACTERISTIC "4f0c6064-49c2-4d07-ae60-0133fd80722c"

class callbacks: public BLECharacteristicCallbacks {
  void onWrite(BLECharacteristic *pCharacteristic) {
        std::string rxValue = pCharacteristic->getValue();
        if (rxValue.length() > 0) {
            Serial.print("Received Value: ");
            for (int i = 0; i < rxValue.length(); i++)
                Serial.print(rxValue[i]);
            Serial.println();
        }
  }
};

BLEServer *pServer = NULL;

BLEService *pService = NULL;
BLECharacteristic * pCharacteristic = NULL;

void setup(){
    Serial.begin(115200);
    // Create the BLE Device
    BLEDevice::init("esp32");
    BLEDevice::setMTU(256);

    // Create the BLE Server
    pServer = BLEDevice::createServer();
    
    // Create the BLE Service
    pService = pServer->createService(UUID_SERVICE);
    // Create the BLE Charcteristic
    pCharacteristic = pService->createCharacteristic(
                                    UUID_CHARACTERISTIC,
                                    BLECharacteristic::PROPERTY_WRITE
                                );
    pCharacteristic->setCallbacks(new callbacks());

    pService->start();
    pServer->getAdvertising()->start();
}

void loop(){

}