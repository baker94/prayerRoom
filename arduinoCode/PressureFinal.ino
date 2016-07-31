#include <ESP8266WiFi.h>
#include <WiFiClientSecure.h>
#include<String.h>
WiFiClient client;
const char* ssid = "TechPublic";
const char* password = "";
const char* host = "prayeroom.azure-mobile.net";
const int httpsPort = 80;
int FSRanalog = 0; //fsr is the pressure sensor
int pressure = 0;

/*********************setup**************************/
/*********************setup**************************/
/*********************setup**************************/
void setup() {
  Serial.begin(9600); //debugging on screen
  wifi_connect();
  delay(500);
  //using A0 for reseting
}
/*********************loop**************************/
/*********************loop**************************/
/*********************loop**************************/
String flagInside="Suspended";
void loop() {
    http_request("/api/gender/gender");
    Serial.println(flagInside); //debug
  //pressure= map(analogRead(FSRanalog), 0, 1023, 0, 255);
  if (analogRead(FSRanalog) > 400 && (flagInside=="Suspended" || flagInside=="Empty") ) {
    delay(6000); //wait four one minute for distance sensor to finish (now is suspended)
    if (analogRead(FSRanalog) > 400) { //still someboday stands more than one minute
      Serial.println(analogRead(FSRanalog)); //debug
         ImamAvailbleSend();
    }
  }


  delay(1000);
}

/*********************ImamAvailbleSend**************************/
/*********************ImamAvailbleSend**************************/
/*********************ImamAvailbleSend**************************/
void ImamAvailbleSend() {
  http_request("/api/gender/choosegender?gender=Boys");  //Boys
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
  if (line.indexOf("Suspended") != -1) {
  flagInside="Suspended";
  }
  if (line.indexOf("Girls") != -1) {
  flagInside="Girls";
  }
  if (line.indexOf("Boys") != -1) {
  flagInside="Boys";
  }
  if (line.indexOf("Empty") != -1) {
  flagInside="Empty";
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




