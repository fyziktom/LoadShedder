#include <M5Unified.h>
#include <WiFi.h>
//#include <Wire.h>
#include "HTTPClient.h"
#include "logo.h"
#include <algorithm>

#define CALIBRATION_MODE 1

#define M_SIZE 1  // Scale factor
#define VERTICAL_OFFSET 120
#define HORIZONTAL_OFFSET 160

#define NUMBER_OF_BITS 4
#define NUMBER_OF_ADC_MUX_INPUTS 16 //per IC
#define NUMBER_OF_ADC_MUX_IC 2 // number of ADC MUX ICs
#define NUMBER_OF_ADC_VIRTUAL_INTPUTS NUMBER_OF_ADC_MUX_INPUTS * NUMBER_OF_ADC_MUX_IC
// main control pins for switching transistors
#define BIT_0 16
#define BIT_1 17
#define BIT_2 2
#define BIT_3 5
#define ENABLE_MUX 26

// control of the simulators of PVE and WGT
#define LED_PWM 23
#define FAN_PWM 19

#define NUMBER_OF_ADCS 4
// ADC pins
#define ADC_0 35
#define ADC_1 36

#define ADC0_SCALE 1.0
#define ADC1_SCALE 1.0
#define STABILISATION_INTERVAL 2 //ms 1 - 5ms is ideal
#define READING_INTERVAL 15 //us 100us
#define ADC_AVG_COUNT 30

#define DATA_ARRAY_LENGTH NUMBER_OF_ADC_MUX_INPUTS * NUMBER_OF_ADC_MUX_IC

int dataToSend[DATA_ARRAY_LENGTH];

int old_analog = -999; // Value last time it was updated
float ltx = 0;        // Saved x coord of bottom of needle
uint16_t osx = HORIZONTAL_OFFSET, osy = VERTICAL_OFFSET;  // Center point of the semicircle at the top

String MeterLabel[5] = {"100", "50", "0", "-50", "-100"};


String moduleName = "test";
String GameBoardName = "testBoard";
String apiCommand = "NewDeviceData";

// please fill IP address
String DEFAULT_SRVIP = "192.168.1.117"; // "IP OF PC WHERE YOU RUN Load Shedder Server App"
String SERVER_PORT = "5000"; // "Port of LoadShedder Server App 5059 is default port in debug 5000 in full run"

// please fill own network parameters
//String ssid = "BocaSimon WIFI";//"YOUR SSID NAME";
String ssid = "Linksys00940";//"YOUR SSID NAME";
String password = "ubackqthyx";//"YOUR WIFI PASSWORD";

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
  //M5.Lcd.setRotation(0);
  ClearDisplay(false);
  M5.Lcd.setTextColor(WHITE);
  delay(500);
  ClearDisplay(false);
  
  Serial.println("Starting...");

  connectWifi();

  //inMode(M5_LED, OUTPUT);
  //digitalWrite(M5_LED, HIGH);

  //setup RFID reader
  //Wire.begin();                   // Initialize I2C
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
  pinMode(ENABLE_MUX,OUTPUT);
  digitalWrite(ENABLE_MUX, 0);

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
  
  M5.Lcd.fillScreen(TFT_BLACK);

  if (!CALIBRATION_MODE) {
    analogMeter();
  }
  
  M5.Lcd.setCursor(145, 180);
  M5.Lcd.setTextSize(3);
  M5.Lcd.print("MW");

}

void SetOneDrivingBit(int settedBit) {

  digitalWrite(BIT_0,0);
  digitalWrite(BIT_1,0);
  digitalWrite(BIT_2,0);
  digitalWrite(BIT_3,0);

  switch(settedBit) {
    case 0: digitalWrite(BIT_0,1);
      break;
    case 1: digitalWrite(BIT_0,1);
      break;
    case 2: digitalWrite(BIT_0,1);
      break;
    case 3: digitalWrite(BIT_0,1);
    default : break;
  }
}

void setValueToBits(int value)
{
  digitalWrite(BIT_0, (value >> 0) & 1);
  digitalWrite(BIT_1, (value >> 1) & 1);
  digitalWrite(BIT_2, (value >> 2) & 1);
  digitalWrite(BIT_3, (value >> 3) & 1);
}

void ReadAverageADCValues(float* resultADC_0, float* resultADC_1)
{
  float dataToAvg_0[ADC_AVG_COUNT];
  float dataToAvg_1[ADC_AVG_COUNT];

  for (int i = 0; i < ADC_AVG_COUNT; i++) {
    dataToAvg_0[0] = 0;
    dataToAvg_1[0] = 0;
  }
  
  for (int i = 0; i < ADC_AVG_COUNT; i++) {
    dataToAvg_0[i] = analogReadMilliVolts(ADC_0) * ADC0_SCALE;   
    dataToAvg_1[i] = analogReadMilliVolts(ADC_1) * ADC1_SCALE;   
    delayMicroseconds(READING_INTERVAL);
  }

  float res_0 = 0.0;
  float res_1 = 0.0;

  for (int i = 0; i < ADC_AVG_COUNT; i++) {
    res_0 += dataToAvg_0[i];   
    res_1 += dataToAvg_1[i];

    delayMicroseconds(READING_INTERVAL);
  }

  res_0 /= ADC_AVG_COUNT;
  res_1 /= ADC_AVG_COUNT;

  *resultADC_0 = res_0;
  *resultADC_1 = res_1;
}

void ReadMedianADCValues(float* resultADC_0, float* resultADC_1)
{
  float dataToAvg_0[ADC_AVG_COUNT];
  float dataToAvg_1[ADC_AVG_COUNT];

  for (int i = 0; i < ADC_AVG_COUNT; i++) {
      dataToAvg_0[0] = 0;
      dataToAvg_1[0] = 0;
  }

  for (int i = 0; i < ADC_AVG_COUNT; i++) {
    dataToAvg_0[i] = analogReadMilliVolts(ADC_0) * ADC0_SCALE;   
    dataToAvg_1[i] = analogReadMilliVolts(ADC_1) * ADC1_SCALE;   
    delayMicroseconds(READING_INTERVAL);
  }

  // Seřazení polí
  std::sort(dataToAvg_0, dataToAvg_0 + ADC_AVG_COUNT);
  std::sort(dataToAvg_1, dataToAvg_1 + ADC_AVG_COUNT);

  float median_0, median_1;

  // Výpočet mediánu pro první pole
  if (ADC_AVG_COUNT % 2 != 0)
    median_0 = dataToAvg_0[ADC_AVG_COUNT / 2];
  else
    median_0 = (dataToAvg_0[(ADC_AVG_COUNT - 1) / 2] + dataToAvg_0[ADC_AVG_COUNT / 2]) / 2.0;

  // Výpočet mediánu pro druhé pole
  if (ADC_AVG_COUNT % 2 != 0)
    median_1 = dataToAvg_1[ADC_AVG_COUNT / 2];
  else
    median_1 = (dataToAvg_1[(ADC_AVG_COUNT - 1) / 2] + dataToAvg_1[ADC_AVG_COUNT / 2]) / 2.0;

  *resultADC_0 = median_0;
  *resultADC_1 = median_1;
}


void ReadProcedure() {
  digitalWrite(ENABLE_MUX, 0);
  delay(1);
  for (int i = 0; i < NUMBER_OF_ADC_MUX_INPUTS; i++) {

    setValueToBits(i);
    delay(STABILISATION_INTERVAL);
    float result_ADC0 = 0.0;
    float result_ADC1 = 0.0;
    ReadMedianADCValues(&result_ADC0, &result_ADC1);
    //ReadAverageADCValues(&result_ADC0, &result_ADC1);
    setValueToBits(0);
    delay(STABILISATION_INTERVAL);
    dataToSend[i] = result_ADC0;
    dataToSend[NUMBER_OF_ADC_MUX_INPUTS + i] = result_ADC1;
    
    //delayMicroseconds(READING_INTERVAL);
  }
  digitalWrite(ENABLE_MUX, 1);
  delay(1);
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


void analogMeter() {
  M5.Lcd.fillRect(0, 0, 320, 240, TFT_BLACK);

  for (int i = 0; i <= 180; i += 5) {
    int len = 5;  
    if (i % 45 == 0) len = 15;
    float radian = (i - 180) * DEG_TO_RAD;  // Adjusted the offset
    int x1 = HORIZONTAL_OFFSET + 100 * cos(radian);
    int y1 = VERTICAL_OFFSET + 100 * sin(radian);  
    int x2 = HORIZONTAL_OFFSET + (100-len) * cos(radian);
    int y2 = VERTICAL_OFFSET + (100-len) * sin(radian);
    M5.Lcd.drawLine(x1, y1, x2, y2, TFT_WHITE);
    M5.Lcd.setTextSize(1);
    if (i % 45 == 0) {
      int val = map(i, 0, 180, -100, 100);
      char buf[5];
      itoa(val, buf, 10);
      M5.Lcd.drawCentreString(buf, HORIZONTAL_OFFSET + (115) * cos(radian), VERTICAL_OFFSET + (115) * sin(radian), 2);
    }
  }
}

void plotNeedle(int value, byte ms_delay) {
  static int16_t oldPos = -999;
  M5.Lcd.setTextColor(TFT_BLACK, TFT_WHITE);

  value = constrain(value, -100, 100);
  int16_t newPos = map(value, -100, 100, 180, 0);  // Adjusted the mapping

  if (oldPos != -999) {
    float radian = (oldPos - 180) * DEG_TO_RAD;
    int x = HORIZONTAL_OFFSET + 95 * cos(radian);
    int y = VERTICAL_OFFSET + 95 * sin(radian);
    M5.Lcd.drawLine(HORIZONTAL_OFFSET, VERTICAL_OFFSET, x, y, TFT_BLACK);
  }
  
  float radian = (newPos - 180) * DEG_TO_RAD;
  int x = HORIZONTAL_OFFSET + 95 * cos(radian);
  int y = VERTICAL_OFFSET + 95 * sin(radian);
  M5.Lcd.drawLine(HORIZONTAL_OFFSET, VERTICAL_OFFSET, x, y, TFT_RED);

  oldPos = newPos;
  delay(ms_delay);
}


void GetBilance() {

  String path = pathBase + "GetBoardBilance" + "/" + GameBoardName;

  Serial.println("Path");
  Serial.println((char*)path.c_str());

  http.begin((char*)path.c_str());
  http.addHeader("accept", "*/*");

  // send data in PUT request
  int httpResponseCode = http.GET();
    
  // parse output if code is not error
  if (httpResponseCode > 0 && httpResponseCode < 400) {
    String response = http.getString();
    Serial.println(httpResponseCode);
    Serial.println(response);
  
    Bilance_Request_Success(response);
  }
  else {
    Serial.print("Error on sending GET Request: ");
    Serial.println(httpResponseCode);
  }
}

void Play_A_Chord() {
    M5.Speaker.tone(880, 2000);
    delay(250);
    M5.Speaker.tone(1047, 1250);
    delay(250);
    M5.Speaker.tone(1319, 750);
    delay(250);
    M5.Speaker.tone(1760, 250);
    delay(1250);
}

void GameResponseAction_Request_Success(String response) {
  int action = response.toInt();

   if (action == 2 || action == 3) {
     Play_A_Chord();
   }
}

void GetGameResponseAction() {

  String path = pathBase + "GameResponseActionsByDeviceId" + "/" + moduleName;

  Serial.println("Path");
  Serial.println((char*)path.c_str());

  http.begin((char*)path.c_str());
  http.addHeader("accept", "*/*");

  // send data in PUT request
  int httpResponseCode = http.GET();
    
  // parse output if code is not error
  if (httpResponseCode > 0 && httpResponseCode < 400) {
    String response = http.getString();
    Serial.println(httpResponseCode);
    Serial.println(response);
  
    GameResponseAction_Request_Success(response);
  }
  else {
    Serial.print("Error on sending GET Request: ");
    Serial.println(httpResponseCode);
  }
}

void Bilance_Request_Success(String response) {
  float value = response.toFloat() / 1000;

    //plotNeedle(value, 5);
  plotNeedle(map((int)value, -100, 100, 100, -100), 5);
  
  //M5.Lcd.fillRect(0, M5.Lcd.height() - 40, M5.Lcd.width(), 40, TFT_BLACK);
  int start = 123;
  if (value >= 10 && value < 100) {
    start = 112;
  }
  else if (value >= 100 && value < 1000) {
    start = 105;
  }

  M5.Lcd.setCursor(start, M5.Lcd.height() - 90);
  M5.Lcd.fillRect(start - 30, M5.Lcd.height() - 90, 200, 30, TFT_BLACK);
  M5.Lcd.setTextColor(WHITE);
  M5.Lcd.setTextSize(3);
  M5.Lcd.print(value);
  
}

void DataReading() {
  //M5.Lcd.fillScreen(BLACK);
  //M5.Lcd.setTextSize(2);
  //M5.Lcd.setCursor(25, 5); M5.Lcd.print("Load Shedder");
  //M5.Lcd.setCursor(25, 30); M5.Lcd.print("Reading positions");
  Serial.println();

  ReadProcedure();
  SendData();
  delay(100);
  GetBilance();
  GetGameResponseAction();
}

void CalibrationMode() {
  ReadProcedure();  
  //SendData();
  
  M5.Lcd.fillScreen(BLACK);
  M5.Lcd.setTextSize(2);
  M5.Lcd.fillScreen(BLACK);
  M5.Lcd.setCursor(25, 25); M5.Lcd.print(dataToSend[0]);
  M5.Lcd.setCursor(25, 50); M5.Lcd.print(dataToSend[1]);
  M5.Lcd.setCursor(25, 75); M5.Lcd.print(dataToSend[2]);
  M5.Lcd.setCursor(25, 100); M5.Lcd.print(dataToSend[3]);
  M5.Lcd.setCursor(25, 125); M5.Lcd.print(dataToSend[4]);
  M5.Lcd.setCursor(25, 150); M5.Lcd.print(dataToSend[5]);
  M5.Lcd.setCursor(25, 175); M5.Lcd.print(dataToSend[6]);
  M5.Lcd.setCursor(25, 200); M5.Lcd.print(dataToSend[7]);

  M5.Lcd.setCursor(100, 25); M5.Lcd.print(dataToSend[8]);
  M5.Lcd.setCursor(100, 50); M5.Lcd.print(dataToSend[9]);
  M5.Lcd.setCursor(100, 75); M5.Lcd.print(dataToSend[10]);
  M5.Lcd.setCursor(100, 100); M5.Lcd.print(dataToSend[11]);
  M5.Lcd.setCursor(100, 125); M5.Lcd.print(dataToSend[12]);
  M5.Lcd.setCursor(100, 150); M5.Lcd.print(dataToSend[13]);
  M5.Lcd.setCursor(100, 175); M5.Lcd.print(dataToSend[14]);
  M5.Lcd.setCursor(100, 200); M5.Lcd.print(dataToSend[15]);

  M5.Lcd.setCursor(170, 25); M5.Lcd.print(dataToSend[16]);
  M5.Lcd.setCursor(170, 50); M5.Lcd.print(dataToSend[17]);
  M5.Lcd.setCursor(170, 75); M5.Lcd.print(dataToSend[18]);
  M5.Lcd.setCursor(170, 100); M5.Lcd.print(dataToSend[19]);
  M5.Lcd.setCursor(170, 125); M5.Lcd.print(dataToSend[20]);
  M5.Lcd.setCursor(170, 150); M5.Lcd.print(dataToSend[21]);
  M5.Lcd.setCursor(170, 175); M5.Lcd.print(dataToSend[22]);
  M5.Lcd.setCursor(170, 200); M5.Lcd.print(dataToSend[23]);

  M5.Lcd.setCursor(250, 25); M5.Lcd.print(dataToSend[24]);
  M5.Lcd.setCursor(250, 50); M5.Lcd.print(dataToSend[25]);
  M5.Lcd.setCursor(250, 75); M5.Lcd.print(dataToSend[26]);
  M5.Lcd.setCursor(250, 100); M5.Lcd.print(dataToSend[27]);
  M5.Lcd.setCursor(250, 125); M5.Lcd.print(dataToSend[28]);
  M5.Lcd.setCursor(250, 150); M5.Lcd.print(dataToSend[29]);
  M5.Lcd.setCursor(250, 175); M5.Lcd.print(dataToSend[30]);
  M5.Lcd.setCursor(250, 200); M5.Lcd.print(dataToSend[31]);
/*
  M5.Lcd.setCursor(100, 25); M5.Lcd.print("Diff");
  M5.Lcd.setCursor(100, 50); M5.Lcd.print(dataToSend[0] - dataToSend[1]);
  M5.Lcd.setCursor(100, 75); M5.Lcd.print(dataToSend[1] - dataToSend[2]);
  M5.Lcd.setCursor(100, 100); M5.Lcd.print(dataToSend[2] - dataToSend[3]);
  M5.Lcd.setCursor(100, 125); M5.Lcd.print(dataToSend[3] - dataToSend[4]);
  M5.Lcd.setCursor(100, 150); M5.Lcd.print(dataToSend[4] - dataToSend[5]);
  M5.Lcd.setCursor(100, 175); M5.Lcd.print(dataToSend[5] - dataToSend[6]);
  M5.Lcd.setCursor(100, 200); M5.Lcd.print(dataToSend[6] - dataToSend[7]);
*/
  delay(100);
}


void loop() {

  if (!CALIBRATION_MODE) {
    DataReading();
  }
  else {
    CalibrationMode();    
  }

  M5.update();
  // wait 10 seconds before allow new read
  delay(1000);
}
