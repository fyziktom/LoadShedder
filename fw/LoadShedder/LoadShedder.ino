#include <M5Unified.h>
#include <WiFi.h>
#include <Wire.h>
#include "HTTPClient.h"
#include "logo.h"

#define NUMBER_OF_BITS 8
// main control pins for switching transistors
#define BIT_0 5
#define BIT_1 2
#define BIT_2 17
#define BIT_3 16
#define BIT_4 1
#define BIT_5 3
#define BIT_6 21
#define BIT_7 22

// control of the simulators of PVE and WGT
#define LED_PWM 23
#define FAN_PWM 19

#define RESERVE_PIN 18

#define NUMBER_OF_ADCS 4
// ADC pins
#define ADC_0 25
#define ADC_1 26
#define ADC_2 35
#define ADC_3 36 

#define STABILISATION_INTERVAL 20 //ms
#define READING_INTERVAL 50 //ms

#define DATA_ARRAY_LENGTH NUMBER_OF_BITS * NUMBER_OF_ADCS
int dataToSend[DATA_ARRAY_LENGTH];

String moduleName = "test";
String apiCommand = "NewDeviceData";

// please fill IP address
String DEFAULT_SRVIP = ""; // "IP OF PC WHERE YOU RUN Load Shedder Server App"
String SERVER_PORT = "5000"; // "Port of LoadShedder Server App 5059 is default port in debug 5000 in full run"

// please fill own network parameters
//String ssid = "";//"YOUR SSID NAME";
String ssid = "";//"YOUR SSID NAME";
String password = "";//"YOUR WIFI PASSWORD";

String pathBase = "http://" + DEFAULT_SRVIP + ":" + SERVER_PORT + "/api/";

byte mac[6];
String localMACAddr;
IPAddress ip;
WiFiClient espClient;
HTTPClient http;

const int numberPoints = 7;
float wifiStrength;

void ClearMainDataArray() {
  for (int i = 0; i < DATA_ARRAY_LENGTH; i++) {
    dataToSend[i] = 0;    
  }
}

void ClearDisplay(bool isBackToMain) {
  M5.Lcd.fillRect(0, 0, 320, 240, BLACK);
  M5.Lcd.setTextColor(BLUE);
  M5.Lcd.setCursor(120, 2); M5.Lcd.println("Load Shedder");
  /*
    if (isBackToMain){

    //M5.Lcd.setCursor(0,15); M5.Lcd.printf(" Unit Name ");
    M5.Lcd.setTextSize(2);
    M5.Lcd.setCursor(5,15); M5.Lcd.printf((char*)moduleName.c_str());
    M5.Lcd.setTextSize(1);
    M5.Lcd.setCursor(0,45); M5.Lcd.printf(" WiFi Signal ");
    M5.Lcd.fillRect(0,60,70,15,BLACK);
    M5.Lcd.setCursor(0,60); M5.Lcd.printf(" RSSI: %.1f", wifiStrength);
    }
  */
  M5.Lcd.setTextColor(WHITE);

}

void connectWifi() {
  ClearDisplay(false);
  M5.Lcd.setCursor(0, 15); M5.Lcd.print("Connecting WIFI");
  M5.Lcd.setCursor(0, 30); M5.Lcd.print(ssid);
  M5.Lcd.setCursor(0, 45);

  WiFi.mode(WIFI_STA);
  WiFi.disconnect();

  int attempts = 20;

  WiFi.begin((char*)ssid.c_str(), (char*)password.c_str());
  while (WiFi.status() != WL_CONNECTED) {
    M5.Lcd.setTextWrap(true);
    M5.Lcd.print(".");
    delay(500);

    attempts--;

    if (attempts <= 0) {
      return;
    }
  }
  M5.Lcd.setTextWrap(false);

  ClearDisplay(false);
  M5.Lcd.setCursor(0, 45);
  M5.Lcd.println("Connection done!");
  delay(1000);

  M5.Lcd.fillRect(0, 0, 160, 80, BLACK);

  M5.Lcd.setCursor(0, 15); M5.Lcd.printf("IP:");
  ip = WiFi.localIP();

  M5.Lcd.setCursor(20, 15); M5.Lcd.printf("%d.%d.%d.%d", ip[0], ip[1], ip[2], ip[3]);
  M5.Lcd.setCursor(0, 30); M5.Lcd.printf("MAC: %s", (char*)localMACAddr.c_str());
  delay(1000);
  ClearDisplay(true);
}

void setup() {
  // put your setup code here, to run once:
  M5.begin();
  M5.Power.begin();
  //M5.Lcd.setRotation(3);
  ClearDisplay(false);
  M5.Lcd.setTextColor(WHITE);
  delay(500);
  ClearDisplay(false);
  
  Serial.println("Starting...");

  connectWifi();

  //inMode(M5_LED, OUTPUT);
  //digitalWrite(M5_LED, HIGH);

  //setup RFID reader
  Wire.begin();                   // Initialize I2C
  //ShowReaderDetails();            // Show details of PCD - MFRC522 Card Reader details

  M5.Lcd.setCursor(0, 30); M5.Lcd.print("Starting...");
  delay(500);

  ClearDisplay(true);

  
  Serial.println("Init of the outputs.");

  analogReadResolution(12);

  pinMode(BIT_0,OUTPUT);
  pinMode(BIT_1,OUTPUT);
  pinMode(BIT_2,OUTPUT);
  pinMode(BIT_3,OUTPUT);
  //pinMode(BIT_4,OUTPUT);
  //pinMode(BIT_5,OUTPUT);
  pinMode(BIT_6,OUTPUT);
  pinMode(BIT_7,OUTPUT);
  //pinMode(LED_PWM,OUTPUT);
  //pinMode(FAN_PWM,OUTPUT);
  //pinMode(RESERVE_PIN,OUTPUT);

  ClearMainDataArray();

  delay(1000);

  M5.Lcd.fillScreen(BLACK);
  M5.Lcd.setTextSize(2);
  M5.Lcd.setCursor(25, 50); M5.Lcd.print("Load Shedder");
  delay(3000);
  M5.Lcd.fillScreen(BLACK);
  M5.Lcd.setCursor(55, 50); M5.Lcd.print("BY");
  M5.Lcd.setCursor(25, 100); M5.Lcd.print("AquaElectra");
  //M5.Lcd.drawXBitmap(25, 50, logo, logoWidth, logoHeight, TFT_WHITE);

  delay(2000);

  Serial.println("Started.");

}

void SetOneDrivingBit(int settedBit) {

  digitalWrite(BIT_0,0);
  digitalWrite(BIT_1,0);
  digitalWrite(BIT_2,0);
  digitalWrite(BIT_3,0);
  //digitalWrite(BIT_4,0);
  //digitalWrite(BIT_5,0);
  digitalWrite(BIT_6,0);
  digitalWrite(BIT_7,0);

  switch(settedBit) {
    case 0: digitalWrite(BIT_0,1);
      break;
    case 1: digitalWrite(BIT_0,1);
      break;
    case 2: digitalWrite(BIT_0,1);
      break;
    case 3: digitalWrite(BIT_0,1);
      break;
    case 4: //digitalWrite(BIT_0,1);
      break;
    case 5: //digitalWrite(BIT_0,1);
      break;
    case 6: digitalWrite(BIT_0,1);
      break;
    case 7: digitalWrite(BIT_0,1);
      break;
    default : break;
  }
}

void ReadProcedure() {
  for (int i = 0; i < 8; i++) {
    SetOneDrivingBit(i);
    delay(STABILISATION_INTERVAL);
    dataToSend[i] = analogReadMilliVolts(ADC_0) * 10;
    dataToSend[NUMBER_OF_BITS + i] = analogReadMilliVolts(ADC_1) * 10;
    dataToSend[2 * NUMBER_OF_BITS + i] = analogReadMilliVolts(ADC_2) * 10;
    dataToSend[3 * NUMBER_OF_BITS + i] = analogReadMilliVolts(ADC_3) * 10;
    
    delay(READING_INTERVAL);
  }
}

// when POST of tha data is finished
void Success() {


}

void SendData() {

  String path = pathBase + apiCommand;// + "/" + moduleName;

  Serial.println("Path");
  Serial.println((char*)path.c_str());

  http.begin((char*)path.c_str());
  http.addHeader("accept", "*/*");
  http.addHeader("Content-Type", "application/json;charset=utf-8");

  String data = "{\"id\":\"" + moduleName + "\",\"data\": [ ";
                 
  for (int i = 0; i < DATA_ARRAY_LENGTH; i++){
    data = data + String(dataToSend[i]);
    if (i < DATA_ARRAY_LENGTH - 1) {
      data = data + ", ";
    }
  }

  data = data + "] }";
  Serial.println("DATA to send: ");

  Serial.println((char*)data.c_str());

  // send data in PUT request
  int httpResponseCode = http.POST((char*)data.c_str());
    
  // parse output if code is not error
  if (httpResponseCode > 0 && httpResponseCode < 400) {
    String response = http.getString();
    Serial.println(httpResponseCode);
    Serial.println(response);
  
    Success();
  }
  else {
    Serial.print("Error on sending PUT Request: ");
    Serial.println(httpResponseCode);
  }
}

void DataReading() {
  M5.Lcd.fillScreen(BLACK);
  M5.Lcd.setTextSize(2);
  M5.Lcd.setCursor(25, 5); M5.Lcd.print("Load Shedder");
  M5.Lcd.setCursor(25, 30); M5.Lcd.print("Reading positions");
  Serial.println();

  ReadProcedure();
  SendData();
}


void loop() {
  DataReading();

  M5.update();
  // wait 10 seconds before allow new read
  delay(10000);
}
