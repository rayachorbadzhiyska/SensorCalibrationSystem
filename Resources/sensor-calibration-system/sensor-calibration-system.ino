#include "Arduino.h"
#include "Arduino_BHY2.h"
#include "bosch/common/common.h"

SensorQuaternion rotation(SENSOR_ID_RV);

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

void writeRegister(uint8_t reg, uint8_t value) {
  if (bhy2_spi_write(reg & 0x7F, &value, 1, nullptr) == 0) {
    Serial.print("OK Register WRITE ");
    Serial.print("0x");
    Serial.print(reg, HEX);
    Serial.print(": ");
    Serial.println(value);
  } else {
    Serial.print("ERROR Register WRITE ");
    Serial.print("0x");
    Serial.print(reg, HEX);
    Serial.print(": ");
    Serial.println(value);
  }
}

void readRegister(uint8_t reg) {
  uint8_t data = 0;

  if (bhy2_spi_read(reg | 0x80, &data, 1, nullptr) == 0) {
    Serial.print("OK Register READ ");
    Serial.print("0x");
    Serial.print(reg, HEX);
    Serial.print(": ");
    Serial.println(data);
  } else {
    Serial.print("ERROR Register READ ");
    Serial.print("0x");
    Serial.println(reg, HEX);
  }
}

uint8_t getRegister(const char input[], bool &isValid) {
    char buffer[64];
    strncpy(buffer, input, sizeof(buffer) - 1);
    buffer[sizeof(buffer) - 1] = '\0'; // Ensure null termination

    char *command;
    char *value;
    uint8_t regAddress = 0;

    // Split the receivedChars string by spaces
    command = strtok(buffer, " ");
    value = strtok(NULL, " ");

    // Check if we have a valid command and value
    if (command != NULL && value != NULL) {
      regAddress = (uint8_t)strtol(value, NULL, 16);
      isValid = true;
    } else {
      Serial.println("ERROR: Invalid command or value received.");
      isValid = false;
    }

    return regAddress;
}

uint8_t getRegisterWriteValue(const char input[], bool &isValid) {
    char buffer[64];
    strncpy(buffer, input, sizeof(buffer) - 1);
    buffer[sizeof(buffer) - 1] = '\0'; // Ensure null termination

    char *command;
    char *reg;
    char *writeValue;
    uint8_t writeRegValue = 0;

    // Split the receivedChars string by spaces
    command = strtok(buffer, " ");
    reg = strtok(NULL, " ");
    writeValue = strtok(NULL, " ");

    // Check if we have a valid command and value
    if (command != NULL && reg != NULL && writeValue != NULL) {
      writeRegValue = (uint8_t)strtol(writeValue, NULL, 10);
      isValid = true;
    } else {
      Serial.println("ERROR: Invalid command, register or write value received.");
      isValid = false;
    }

    return writeRegValue;
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
                bool isValid;
                uint8_t regAddress = getRegister(receivedChars, isValid);

                if (isValid) {
                  readRegister(regAddress);
                }
            } else if (strncmp (receivedChars, "WRITE", 5) == 0) {
                bool isValid;
  
                uint8_t regAddress = getRegister(receivedChars, isValid);
                uint8_t regWriteValue = getRegisterWriteValue(receivedChars, isValid);

                if (isValid) {
                  writeRegister(regAddress, regWriteValue);
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
