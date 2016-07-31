#include <ESP8266WiFi.h>
#include <WiFiClientSecure.h>
#include<String.h>
#include <LiquidCrystal.h>
#define FINISHED 1000000000000000000000

LiquidCrystal lcd( D6, D5, D4, D3, D1, D2);
WiFiClient client;
const char* ssid = "TechPublic";
const char* password = "";
const char* host = "prayeroom.azure-mobile.net";
const int httpsPort = 80;
String salah = "";
int counterSojods = 0;
int sojodNum = 0;
int rakat = 0;
int FSRanalog = 0; //fsr is the pressure sensor
int pressure = 0;
void(* resetFunc) (void) = 0;//declare reset function at address 0

/*********************setup**************************/
/*********************setup**************************/
/*********************setup**************************/
void setup() {
  Serial.begin(9600); //debugging on screen
  lcd.begin(16, 2);
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("Alslamu Alikum");
  wifi_connect();
  salahGet(); //getting the the prayerTime
  delay(500);
  lcd.clear();
  lcd.print("Time For:");
  lcd.setCursor(0, 1);
  lcd.print(salah);
  rakatSend();
  delay(1000);
  //using A0 for reseting
}
/*********************loop**************************/
/*********************loop**************************/
/*********************loop**************************/
void loop() {
  int flag = 0;
  //pressure= map(analogRead(FSRanalog), 0, 1023, 0, 255);
  if (analogRead(FSRanalog) > 400) {
    delay(1000);
    while (analogRead(FSRanalog) > 400) {
      Serial.println(analogRead(FSRanalog)); //debug
      flag = 1;
      delay(10);
      continue;
    }
  }
  if (flag == 1) {
    counterSojods++;
    sojodNum++;
//     Serial.println("sojds Num"); //debug
//     Serial.println(counterSojods); //debug
    if (!(sojodNum % 2)) {
      rakatPrint();
      rakatSend();
      delay(1000);
    }
  }
  delay(100);
}

/*********************rakatSend**************************/
/*********************rakatSend**************************/
/*********************rakatSend**************************/
void rakatSend() {
  // rakat global variable
  String temp = "/api/rakaat/chooserakaat?rakaat=";
  temp += counterSojods/2;
//    Serial.println( rakat); //debug
//    Serial.println( temp);  //debug
  http_request(temp);
}
/*********************salahGet**************************/
/*********************salahGet**************************/
/*********************salahGet**************************/
void salahGet() {
  //salah global variable
  http_request("/api/prayertime/prayer"); //changing global variable salah
}
/******************rakatPrint**********************************/
/******************rakatPrint**********************************/
/******************rakatPrint**********************************/

void printFinished() {
  http_request("/api/gender/choosegender?gender=Finished");  //Finished
  rakatSend();
  lcd.clear();
  lcd.setCursor(2, 0);
  lcd.print("Takabl Allah");
  lcd.setCursor(1, 1);
  lcd.print("Turn me OFF :) ");
  delay(FINISHED); //BusyWait
  while (1);
}

void screenResetRow2() {
  lcd.setCursor(0, 1);
  lcd.print("                    ");
}

void rakatPrint() {
  if (counterSojods == 2) {
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print("Raka Num: ");
  }
  rakat = sojodNum / 2;
  lcd.setCursor(11, 0);
  lcd.print(rakat);
  if (salah == "Salah Fajr") {
    Serial.print("rak3at number is  = "); //debug
    Serial.println(sojodNum / 2); // debug
    if (counterSojods == 4) {
//counterSojods = 0;
      sojodNum = 0;
      printFinished();
    }
  }
  if (salah == "Salah Dhuhor") {
    Serial.print("rak3at number is  = "); //debug
    Serial.println(sojodNum / 2); // debug
    if (counterSojods == 4) {
      sojodNum = 0;
      screenResetRow2();
      lcd.setCursor(0, 1);
      lcd.print("Finished 2 Sonah");
      delay(1000);
      screenResetRow2();
    }
    if (counterSojods == 12) {
      sojodNum = 0;
      screenResetRow2();
      lcd.setCursor(0, 1);
      lcd.print("Finished 4 Fard");
      delay(1000);
      screenResetRow2();
    }
    if (counterSojods == 16) {
      sojodNum = 0;
     // counterSojods = 0;
      printFinished();
    }
  }
  if (salah == "Salah Asr") {
    Serial.print("rak3at number is  = "); //debug
    Serial.println(sojodNum / 2); // debug
    if (counterSojods == 8) {
     // counterSojods = 0;
      sojodNum = 0;
      printFinished();
    }
  }
  if (salah == "Salah Maghrib") {
    Serial.print("rak3at number is  = "); //debug
    Serial.println(sojodNum / 2); // debug
    if (counterSojods == 6) {
      sojodNum = 0;
      screenResetRow2();
      lcd.setCursor(0, 1);
      lcd.print("Finished 3 Fard");
      delay(1000);
      screenResetRow2();
    }
    if (counterSojods == 10) {
      sojodNum = 0;
      //counterSojods = 0;
      printFinished();
    }
  }
  if (salah == "Salah Ishaa") {
    Serial.print("rak3at number is  = "); //debug
    Serial.println(sojodNum / 2); // debug
    if (counterSojods == 8) {
      sojodNum = 0;
      screenResetRow2();
      lcd.setCursor(0, 1);
      lcd.print("Finished 4 Fard");
      delay(1000);
      screenResetRow2();
    }
    if (counterSojods == 12) {
      sojodNum = 0;
      //counterSojods = 0;
      printFinished();
    }
  }
}
/******************wifi_connect**********************************/
/******************wifi_connect**********************************/
/******************wifi_connect**********************************/
void wifi_connect() {
  WiFi.persistent(false);
  Serial.println();
  Serial.print("connecting to ");
  Serial.println(ssid);
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("");
  Serial.println("WiFi connected");
  Serial.println("IP address: ");
  Serial.println(WiFi.localIP());
  // Use WiFiClientSecure class to create TLS connection
  Serial.print("connecting to ");
  Serial.println(host);
  if (!client.connect(host, httpsPort)) {
    Serial.println("connection failed");
    return;
  }
}
/******************http_request**********************************/
/******************http_request**********************************/
/******************http_request**********************************/
void http_request(String url) {
  Serial.print("requesting URL: ");
  Serial.println(url);
  client.print(
      String("GET ") + url + " HTTP/1.1\r\n"
          + "X-ZUMO-APPLICATION: YuRCAmFNVXnELKXSjVAFAmKtpvVcMV58\r\n"
          + "X-ZUMO-INSTALLATION-ID: b3022ffd-4f21-46a7-b8eb-2507f161f1e5\r\n"
          + "Host: prayeroom.azure-mobile.net\r\n"
          + "Connection: Keep-Alive\r\n\r\n");
  Serial.println("request sent");
  http_response();
}
/******************http_response**********************************/
/******************http_response**********************************/
/******************http_response**********************************/
void dataPut(String line) {

  if (line.indexOf("Fajr") != -1) {
    salah = "Salah Fajr";
  }
  if (line.indexOf("Dhuhor") != -1) {
    salah = "Salah Dhuhor";
  }
  if (line.indexOf("Asr") != -1) {
    salah = "Salah Asr";
  }
  if (line.indexOf("Maghrib") != -1) {
    salah = "Salah Maghrib";
  }
  if (line.indexOf("Ishaa") != -1) {
    salah = "Salah Ishaa";
  }
}

void http_response() {
  unsigned long timeout = millis();
  while (client.available() == 0) {
    if (millis() - timeout > 5000) {
      Serial.println(">>> Client Timeout !");
      client.stop();
      return;
    }
  }
  // Read all the lines of the reply from server and print them to Serial
  String line;
  while (client.available()) {
    line = client.readStringUntil('\r');
    Serial.print(line);
    dataPut(line);
  }
  Serial.println();
  Serial.println("closing connection");
}



