/* Sweep
 and distance sensor  HC-SR04
 */

/*WiFi*/
#include <ESP8266WiFi.h>
#include <WiFiClientSecure.h>
const char* ssid = "TechPublic";
const char* password = "";
const char* host = "prayeroom.azure-mobile.net";
const int httpsPort = 80;
WiFiClient client;

/**Distance**/
#define echoPin D2  // Echo Pin
#define trigPin D1 // Trigger Pin
int ImamGirlsDis = 111; // This is the max distance ::Imam Girls Distance
int minimumRange = 0; // Minimum range needed
int maxDistance = ImamGirlsDis;
long duration, distance; // Duration used to calculate distance

/**Servo**/
#include <Servo.h>
Servo myservo;  // create servo object to control a servo
int pos = 30;    // variable to store the servo position

/**Flags**/
int EmptyFlag = 1; //it is empty
int GirlsImamFlag = 0;
int BoysImamFlag = 0;
String Inside = "Empty";

void setup() {
  Serial.begin(115200);
  pinMode(trigPin, OUTPUT);
  pinMode(echoPin, INPUT);
  myservo.attach(D4);  // attaches the servo on pin 9 to the servo object
  myservo.write(pos);     // tell servo to go to position in variable 'pos'
  wifi_connect();
}
void loop() {
  /**The Room Is EMPTY**/
  delay(1000);
  int whileFlag = 0;
  int BoysImamFlag = 0;
  while (EmptyFlag ) {
    isEmpty(1); //changing emptyFlag 
    if (whileFlag == 0) {
      http_request("/api/gender/choosegender?gender=Empty");
    }
    whileFlag = 1;
  }
  /**The Room Is NOT EMPTY**/
  if (!EmptyFlag && !BoysImamFlag) {
    http_request("/api/gender/choosegender?gender=Suspended"); //send Suspended
    Serial.println("Suspended");
    changeInside();
    checkInside();
  }
}

/*****************************changeInside*****************************/
/*****************************changeInside*****************************/
/*****************************changeInside*****************************/
void changeInside() { //if you are here then you are Suspended
  isEmpty(1);
  delay(3000); // wait until the pressure sensor change

  if (GirlsImamFlag) { //boys automatic change from the pressure sensors
    http_request("/api/gender/choosegender?gender=Girls");
    GirlsImamFlag = 0;
  }
}
/*****************************checkInside*****************************/
/*****************************checkInside*****************************/
/*****************************checkInside*****************************/
void checkInside() { // if you are here then you are "Girls" or "Boys"
    http_request("/api/gender/gender");
  while (Inside != "Finished") { //Inside == "Girls" || Inside=="Boys"
    http_request("/api/gender/gender");
  }
 swapChecker();
}

void swapChecker(){
  EmptyFlag=0;
    waitToBeEmpty();
    myservo.write(50); 
    delay(15);
    isEmpty(0);
    maxDistance=140;
    EmptyFlag=0;
    waitToBeEmpty();
    maxDistance=ImamGirlsDis;
    http_request("/api/gender/choosegender?gender=Empty");
    myservo.write(pos); // tell servo to go to position in variable 'pos'
}

void waitToBeEmpty(){
      while (EmptyFlag == 0) {
      isEmpty(0);
    }
}

/**********************Helper Functions**************************/

void checkDistance() {
  digitalWrite(trigPin, LOW);
  delayMicroseconds(2);
  digitalWrite(trigPin, HIGH);
  delayMicroseconds(10);
  digitalWrite(trigPin, LOW);
  duration = pulseIn(echoPin, HIGH);
  //Calculate the distance (in cm) based on the speed of sound.
  distance = duration / 58.2;
}
/*****************isEmpty Function*****************/

void isEmpty(int flagBoyS) {
  GirlsImamFlag=0;
  if(flagBoyS){
  BoysImamFlag=0;
  http_request("/api/gender/gender");
  if (Inside == "Boys") { //Inside == "Girls" || Inside=="Boys"
    BoysImamFlag=1;
    EmptyFlag=0;
    checkInside();
    return;
   }
  }
  checkDistance();
  int counterDelay = 0; //to make sure 

  for (int i = 0; i < 20; i++) {
    delay(200);
    /* to indicate "out of range" */
    checkDistance();
    if (distance > maxDistance || distance <= minimumRange) {
      counterDelay++;
    } else if (distance <= maxDistance - 3 && distance > minimumRange) {
      if (distance > 90) {
        GirlsImamFlag++;
      }
      counterDelay--;
    }
    Serial.println(distance);
    Serial.println(counterDelay);
  }
  /**
  if (counterDelay >= 46) {
    Serial.println("empty");
    EmptyFlag = 1; //empty
    GirlsImamFlag = 0;
  } else {
    Serial.println("not empty: ");
    Serial.println(distance);
    EmptyFlag = 0; //not empty
    if (GirlsImamFlag >= 7) {
      GirlsImamFlag = 1;
    }
  }
  ***/
  if ( GirlsImamFlag >= 10 ) {
    Serial.println("not empty: ");
    Serial.println(distance);
    EmptyFlag = 0; //not empty
    GirlsImamFlag = 1;
  }
  else {
    if (counterDelay >= 10) {
    Serial.println("empty");
    EmptyFlag = 1; //empty
    GirlsImamFlag = 0;
    }
  }
}

/*********************WiFi Connect****************************/
void wifi_connect() {
  WiFi.persistent(false);
  Serial.begin(115200);
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
/*******************http request*******************/
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
 
void dataPut(String line) {
  if (line.indexOf("Suspended") != -1) {
    Inside = "Suspended";
  }
  if (line.indexOf("Girls") != -1) {
    Inside = "Girls";
  }
  if (line.indexOf("Boys") != -1) {
    Inside = "Boys";
  }
  if (line.indexOf("Empty") != -1) {
    Inside = "Empty";
  }
  if (line.indexOf("Finished") != -1) {
    Inside = "Finished";
  }
}



