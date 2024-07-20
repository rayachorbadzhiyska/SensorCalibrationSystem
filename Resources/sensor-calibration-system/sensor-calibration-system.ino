#include "Arduino.h"
#include "Arduino_BHY2.h"
#include "bosch/common/common.h"

SensorQuaternion rotation(SENSOR_ID_RV);

#define MAX_WRITE_BYTES 64

const byte numChars = 64;
char receivedChars[numChars];  

void setup() {
  // Initialize BHI260AP
  BHY2.begin();

  // Initialize serial
  Serial.begin(115200);
  while(!Serial);
}

bool areQuaternionValuesStreaming = false;
const int updateInterval = 200; 
int previousPrintTime = 0;

void startStreamingQuaternionValues() {
  areQuaternionValuesStreaming = true;
  rotation.begin();
  previousPrintTime = millis(); // Initialize the time
}

void printQuaternionValues() {
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

void stopQuaternionValuesStreaming() {
  areQuaternionValuesStreaming = false;
  rotation.end();
}

void writeRegister(uint8_t reg, uint8_t *value, uint32_t length) {
  if (bhy2_spi_write(reg & 0x7F, value, length, nullptr) == 0) {
    Serial.print("OK Register WRITE ");
    Serial.print("0x");
    Serial.print(reg, HEX);
    Serial.print(": ");
    for (uint32_t i = 0; i < length; i++) {
        Serial.print(value[i]);
      }
    Serial.println();
  } else {
    Serial.print("ERROR: Could not write register ");
    Serial.print("0x");
    Serial.print(reg, HEX);
    Serial.print(" with value ");
    for (uint32_t i = 0; i < length; i++) {
        Serial.print(value[i]);
      }
    Serial.println();
  }
}

void readRegister(uint8_t startReg, uint32_t length) {
    uint8_t *data = new uint8_t[length]; // Dynamically allocate memory for the data array

    if (bhy2_spi_read(startReg | 0x80, data, length, nullptr) == 0) {
        Serial.print("OK Register READ ");
        Serial.print("0x");
        Serial.print(startReg, HEX);
        Serial.print(": ");

        for (uint32_t i = 0; i < length; i++) {
            Serial.print(data[i]);
        }

        Serial.println();
    } else {
        Serial.print("ERROR: Could not read register ");
        Serial.print("0x");
        Serial.println(startReg, HEX);
    }

    delete[] data; // Free the allocated memory
}

bool parseRegisterReadCommand(const char input[], uint8_t &startReg, uint32_t &length) {
    char buffer[256];
    strncpy(buffer, input, sizeof(buffer) - 1);
    buffer[sizeof(buffer) - 1] = '\0'; // Ensure null termination

    char *command;
    char *regStr;
    char *lengthStr;

    // Split the input string by spaces
    command = strtok(buffer, " ");
    regStr = strtok(NULL, " ");
    lengthStr = strtok(NULL, " ");

    // Check if we have valid command, register, and length
    if (command != NULL && regStr != NULL && lengthStr != NULL) {
        startReg = (uint8_t)strtol(regStr, NULL, 16);
        length = (uint32_t)strtol(lengthStr, NULL, 10) / 8; // Convert length from bits to bytes

        return true;
    } else {
        Serial.println("ERROR: Invalid command format.");
        return false;
    }
}

bool parseRegisterWriteCommand(const char input[], uint8_t &startReg, uint8_t *&data, uint32_t &length) {
    char buffer[256];
    strncpy(buffer, input, sizeof(buffer) - 1);
    buffer[sizeof(buffer) - 1] = '\0'; // Ensure null termination

    char *command;
    char *regStr;
    char *valueStr;
    char *lengthStr;

    // Split the input string by spaces
    command = strtok(buffer, " ");
    regStr = strtok(NULL, " ");
    valueStr = strtok(NULL, " ");
    lengthStr = strtok(NULL, " ");

    // Check if we have valid command, register, value, and length
    if (command != NULL && regStr != NULL && valueStr != NULL && lengthStr != NULL) {
        startReg = (uint8_t)strtol(regStr, NULL, 16);
        length = (uint32_t)strtol(lengthStr, NULL, 10) / 8; // Convert length from bits to bytes

        uint32_t value = (uint32_t)strtol(valueStr, NULL, 10);

        data = new uint8_t[length];
        for (uint32_t i = 0; i < length; i++) {
            data[length - 1 - i] = (uint8_t)(value >> (i * 8));
        }

        return true;
    } else {
        Serial.println("ERROR: Invalid command format.");
        return false;
    }
}

void readSerialData() {
    static byte ndx = 0;
    char endMarker = '\n';
    char rc;

    // Read data as long as it's available
    while (Serial.available() > 0) {
        rc = Serial.read();

        // Check for the end marker
        if (rc != endMarker) {
            receivedChars[ndx] = rc;
            ndx++;
            if (ndx >= numChars) {
                ndx = numChars - 1;
            }
        }
        else {
            receivedChars[ndx] = '\0'; // terminate the string
            ndx = 0;

            if (strcmp(receivedChars, "START_QUATERNION_VALUES_STREAMING") == 0) {
              startStreamingQuaternionValues();
            } else if (strcmp(receivedChars, "STOP_QUATERNION_VALUES_STREAMING") == 0) {
              stopQuaternionValuesStreaming();
            } else if (strncmp (receivedChars, "READ", 4) == 0) {
                uint8_t startReg;
                uint32_t length;

                if (parseRegisterReadCommand(receivedChars, startReg, length)) {
                  readRegister(startReg, length);
                }
            } else if (strncmp (receivedChars, "WRITE", 5) == 0) {
                uint8_t startReg;
                uint8_t *data;
                uint32_t length;

                if (parseRegisterWriteCommand(receivedChars, startReg, data, length)) {
                  writeRegister(startReg, data, length);
                  delete[] data; // Don't forget to free the allocated memory
                }
            }
        }
    }
}

void loop() {
  readSerialData();

  if (areQuaternionValuesStreaming) {
    printQuaternionValues();
  }
}
