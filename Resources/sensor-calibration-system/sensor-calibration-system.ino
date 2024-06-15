#include "Arduino.h"
#include "Arduino_BHY2.h"
#include <Wire.h>

SensorQuaternion rotation(SENSOR_ID_RV);

void setup() {
  Serial.begin(115200);
  while(!Serial);

  Wire.begin();
  BHY2.begin();
}

bool areQuaternionValuesStreaming = false;
const int updateInterval = 200; 
int previousPrintTime = 0;

void streamQuaternionValues() {
  areQuaternionValuesStreaming = true;

  rotation.begin();

  // Continuously update sensor data and stream quaternion values
  while (areQuaternionValuesStreaming) {
    // Get the current time
    unsigned long currentTime = millis();

    if (currentTime - previousPrintTime >= updateInterval) {
      previousPrintTime = currentTime;
      float x = rotation.x();
      float y = rotation.y();
      float z = rotation.z();
      float w = rotation.w();

      String data = "Quaternion: ";
      data += x;
      data += " ";
      data += y;
      data += " ";
      data += z;
      data += " ";
      data += w;
      data += "\n";

      Serial.print(data);
    }

    BHY2.update();
  }
}

void stopQuaternionValuesStreaming() {
  areQuaternionValuesStreaming = false;

  rotation.end();
}

void writeRegister(byte deviceAddress, byte registerAddress, byte data) {
  Wire.beginTransmission(deviceAddress);
  Wire.write(registerAddress);
  Wire.write(data);
  Wire.endTransmission();
}

byte readRegister(byte deviceAddress, byte registerAddress) {
  byte data = 0; // Variable to store the read data

  Wire.beginTransmission(deviceAddress);
  Wire.write(registerAddress);
  Wire.endTransmission();

  Wire.requestFrom(deviceAddress, 1); // Request 1 byte of data
  if (Wire.available()) {
    data = Wire.read(); // Read the data from the register
  }

  return data;
}

void loop() {

  if (Serial.available() > 0) {
    String command = Serial.readStringUntil('\n');
    
    if (command.equals("START_QUATERNION_VALUES_STREAMING")) {
      streamQuaternionValues();
    } else if (command.equals("STOP_QUATERNION_VALUES_STREAMING")) {
      stopQuaternionValuesStreaming();
    }
  }
}
