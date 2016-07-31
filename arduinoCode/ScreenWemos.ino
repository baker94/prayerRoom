/*upload this code to wemos*/
/***************************/
/*sending data from server to touch screen*/
/*reading data from the touch screen (waiting button )and sending to server*/
/*reading and sending between wemos and TFT screen using I2C protocol*/
/*reading and sending data beween wemos and server by http requests data*/


#include <ESP8266WiFi.h>
#include <WiFiClientSecure.h>
#include <Wire.h>
const char* ssid = "TechPublic";
const char* password = "";
const char* host = "prayeroom.azure-mobile.net";
const int httpsPort = 80;
WiFiClient client;
/*WIFI*/
String data1_inside, data2_salah, data3_minutes, data4_sec;

String salah;
String inside;
String minutes;
String sec;

void setup() {
  Wire.begin();
  Serial.begin(9600);
  wifi_connect();
  sendReset(); //sending to screen by I2C
  http_request("/api/screen/datarequest/");
  delay(1000);
  sendSalah(data2_salah);
  salah = data2_salah;
  delay(500);
  sendInside(data1_inside);
  inside = data1_inside;
  delay(500);
  sendMinutes(data3_minutes);
  minutes = data3_minutes;
  delay(500);
//sendSec(data4_sec);
//sec=data4_sec;
//delay(500); 
  http_request("/api/newperson/issomeonewaiting?isWaiting=0"); //change data in service to Not waiting modee 
  delay(2000);
}
int waiting_flag = 0;
int counter = 0;
void loop() {

  Wire.requestFrom(8, 1);    // request 6 bytes from slave device #8
  Serial.println("sent request");         // print the character
  while (Wire.available() && data1_inside != "Empty") { // slave may send less than requested
    char c = Wire.read(); // receive a byte as character
    Serial.print(c);         // print the character
    if (c == 'W') {
      waiting_flag = 1;
      sendNotWaiting(); //change screen view
      delay(500);
      http_request("/api/newperson/issomeonewaiting?isWaiting=1"); //change data in service to waiting modee
    }
    if (c == 'N') {
      waiting_flag = 0;
      sendWaiting(); //change screen view
      delay(500);
      http_request("/api/newperson/issomeonewaiting?isWaiting=0"); //change data in service to Not waiting modee
    }
  }
  counter++;
  delay(1000);

  if (counter == 5) {
    http_request("/api/screen/datarequest/");
    delay(1000);

    if (inside != data1_inside) {
      if (data1_inside == "Empty"  || data1_inside == "Suspended") {
        sendReset(); //sending to screen by I2C
        sendSalah(data2_salah);
        delay(500);
      }
      minutes = 1000; //to make it change for the boys and girls
      sendInside(data1_inside);
      inside = data1_inside;
      delay(500);
      waiting_flag = 0;
    }

    if (salah != data2_salah) {
      sendSalah(data2_salah);
      salah = data2_salah;
    }

    if ((data1_inside != "Empty" && data1_inside != "Suspended" && minutes != data3_minutes)) {
      sendMinutes(data3_minutes);
      minutes = data3_minutes;
    }
    counter = 0;
    delay(1000);
  }
//if(inside!=data1_inside || salah!=data2_salah || (data1_inside!="Empty"&& minutes!=data3_minutes)){
//sendReset(); //sending to screen by I2C
//delay(500); 
//sendSalah(data2_salah);
//salah=data2_salah;
//sendInside(data1_inside);
//delay(500); 
//      if(data1_inside!="Empty" && inside==data1_inside && waiting_flag){//sendInside replace the not waiting to waiting , return it back 
//            sendNotWaiting(); //change screen view
//            delay(500);
//    }
//  inside=data1_inside;
//  delay(500);
//  if(data1_inside!="Empty"){
//  sendMinutes(data3_minutes);
//  minutes=data3_minutes; 
//  }
//}
//counter=0;
//delay(1000); 
//}
//getting timeSec
//if(sec!=data4_sec){
//sec=data4_sec;
//sendSec(sec);
//delay(1000); 
//}

}

/***************************SendNewData********************************/
/***************************SendNewData********************************/
/***************************SendNewData********************************/

void sendSalah(String s) {
  Wire.beginTransmission(8);
  Wire.write('P'); //Salah
  Wire.endTransmission();
  delay(1000);
  if (s == "Fajr") {
    Wire.beginTransmission(8);
    Wire.write('F');
    Wire.endTransmission();
    delay(1000);
  }
  if (s == "Dhuhor") {
    Wire.beginTransmission(8);
    Wire.write('D');
    Wire.endTransmission();
    delay(1000);
  }
  if (s == "Asr") {
    Wire.beginTransmission(8);
    Wire.write('A');
    Wire.endTransmission();
    delay(1000);
  }
  if (s == "Maghrib") {
    Wire.beginTransmission(8);
    Wire.write('M');
    Wire.endTransmission();
    delay(1000);
  }
  if (s == "Ishaa") {
    Wire.beginTransmission(8);
    Wire.write('I');
    Wire.endTransmission();
    delay(1000);
  }
}

void sendInside(String in) {
  Wire.beginTransmission(8);
  Wire.write('I'); //Inside
  Wire.endTransmission();
  delay(1000);
  if (in == "Girls") {
    Wire.beginTransmission(8);
    Wire.write('G');
    Wire.endTransmission();
    delay(1000);
  }
  if (in == "Boys") {
    Wire.beginTransmission(8);
    Wire.write('B');
    Wire.endTransmission();
    delay(1000);
  }
  if (in == "Empty") {
    Wire.beginTransmission(8);
    Wire.write('E');
    Wire.endTransmission();
    delay(1000);
  }
  if(in == "Suspended"){
    Wire.beginTransmission(8);
    Wire.write('S');
    Wire.endTransmission();
    delay(1000);
  }
}
void sendMinutes(String mints) {
  Wire.beginTransmission(8);
  Wire.write('M'); //time
  Wire.endTransmission();
  delay(1000);
  Wire.beginTransmission(8);
  Wire.write(mints.toInt());
  Wire.endTransmission();
  delay(1000);
}

//void sendSec(String sec) {
//  Wire.beginTransmission(8);
//  Wire.write('S'); //time
//  Wire.endTransmission();
//  delay(1000);
//  Wire.beginTransmission(8);
//  Wire.write(sec.toInt());
//  Wire.endTransmission();
//  delay(1000);
//}

void sendReset() {
  Wire.beginTransmission(8);
  Wire.write('R'); //Reset
  Wire.endTransmission();
  delay(1000);
  Wire.beginTransmission(8);
  Wire.write('R');
  Wire.endTransmission();
  delay(1000);
}

void sendWaiting() {
  Wire.beginTransmission(8);
  Wire.write('W'); //Reset
  Wire.endTransmission();
  delay(1000);
  Wire.beginTransmission(8);
  Wire.write('W');
  Wire.endTransmission();
  delay(1000);
}

void sendNotWaiting() {
  Wire.beginTransmission(8);
  Wire.write('N'); //Reset
  Wire.endTransmission();
  delay(1000);
  Wire.beginTransmission(8);
  Wire.write('N');
  Wire.endTransmission();
  delay(1000);
}

/*********************************WIFI*************************************/
/*********************************WIFI*************************************/
/*********************************WIFI*************************************/
/*********************************WIFI*************************************/

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
/*http request*/
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
    int index = line.indexOf("MusallahData");
    if (index != -1) {
      //INSIDE SALAH TIMEMintues TIMEsec
      dataPut(line);
    }
  }
  Serial.println();
  Serial.println("closing connection");
}
void dataPut(String line) {
  if (line.indexOf("Empty") != -1) {
    data1_inside = "Empty";
  }
  if (line.indexOf("Girls") != -1) {
    data1_inside = "Girls";
  }
  if (line.indexOf("Boys") != -1) {
    data1_inside = "Boys";
  }
  if (line.indexOf("Suspended") != -1) {
    data1_inside = "Suspended";
  }
  if (line.indexOf("Fajr") != -1) {
    data2_salah = "Fajr";
  }
  if (line.indexOf("Dhuhor") != -1) {
    data2_salah = "Dhuhor";
  }
  if (line.indexOf("Asr") != -1) {
    data2_salah = "Asr";
  }
  if (line.indexOf("Maghrib") != -1) {
    data2_salah = "Maghrib";
  }
  if (line.indexOf("Ishaa") != -1) {
    data2_salah = "Ishaa";
  }
  data3_minutes = String(line[line.indexOf("-") + 1])
      + String(line[line.indexOf("-") + 2]);
  Serial.println(data3_minutes);

  data4_sec = String(line[line.indexOf(":") + 1])
      + String(line[line.indexOf(":") + 2]);
  Serial.println(data4_sec);

}


