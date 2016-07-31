// UTFT_ViewFont 
// Copyright (C)2015 Rinky-Dink Electronics, Henning Karlsen. All right reserved
// web: http://www.RinkyDinkElectronics.com/
//
// This program is a demo of the included fonts.
//
// This demo was made for modules with a screen resolution 
// of 320x240 pixels.
//
// This program requires the UTFT library.
//

#include <UTFT.h>
#include <UTouch.h>
#include <Wire.h>

int command = 1;
char command_t;
char waitingFlag = 'S';
int emptyFlag = 0;
char data1;
char data2;
int data3;
int data4;
extern uint8_t SmallFont[];
extern uint8_t BigFont[];
extern uint8_t LargeFont[];
extern uint8_t SevenSegNumFont[];
String minutes = "";
String sec = "";
String inside = "Musallah status is: ";
String salah = "Prayer Time: ";
// Remember to change the model parameter to suit your display module!
UTFT myGLCD(CTE50,38,39,40,41);
UTouch myTouch( 6, 5, 4, 3, 2);

void setup() {
  Serial.begin(9600);           // start serial for output
  Wire.begin(8);                // join i2c bus with address #8
  myGLCD.InitLCD();
  myGLCD.clrScr();
  myTouch.InitTouch();
  myTouch.setPrecision(PREC_MEDIUM);
  initilizeScreen();
  Wire.onReceive(receiveEvent); // register event
  Wire.onRequest(requestEvent); // register event
//for camera 
//EMPTY//
//updateSalah('D');
//updateInside('E');
//Girls//
//updateSalah('D');
//updateInside('B');
//updateMinutes(2);
//Boys//
//updateSalah('D');
//updateInside('E');
//updateMinutes(0);
}

int x, y;
void loop() {
  while (myTouch.dataAvailable()) {
    myTouch.read();
    x = myTouch.getX();
    y = myTouch.getY();
    if ((y >= 204) && (y <= 340))  // Upper row
        {
      if ((x >= 320) && (x <= 460))  // Button: 1
          {
//          Serial.print("x ");
//          Serial.println(x);
//          Serial.print("y ");
//          Serial.println(y);
//          Serial.print("center ");
//          Serial.println(CENTER);
        if (!emptyFlag) {
            if (waitingFlag == 'S'){ //for the start initilizing
            screen_waiting();
            waitingFlag='W';
          }
          waitForIt(CENTER - 140, 140, CENTER, 270);
        }
      }
    }
  }
  delay(100);
}

/******************************Get&UpdateData****************************/
/******************************Get&UpdateData****************************/
/******************************Get&UpdateData****************************/
void updateScreen() {
  myGLCD.clrScr();
  initilizeScreen();
}
void updateSalah(char c) {
  salah = " Prayer Time: ";
  if (c == 'D') {
    salah += "Dhuhor ";
  }
  if (c == 'A') {
    salah += "Asr    ";
  }
  if (c == 'M') {
    salah += "Maghrib";
  }
  if (c == 'F') {
    salah += "Fajr   ";
  }
  if (c == 'I') {
    salah += "Ishaa  ";
  }
  screen_salah();
}

void updateInside(char c) {
  emptyFlag = 0;
  inside = "Musallah status is: ";
  if (c == 'B') {
    inside += "Boys     ";
    if (!emptyFlag) {
      screen_Inside();
    }
  }
  if (c == 'G') {
    inside += "Girls    ";
    if (!emptyFlag) {
      screen_Inside();
    }
  }
  if (c == 'E') {
    emptyFlag = 1;
    inside += "Empty    ";
    screen_empty();
  }
  if (c == 'S') {
    emptyFlag = 1;
    inside += "LOADING..";
    screen_empty();
  }
  
  

}

void updateMinutes(int mint) {
  minutes = "";
  int temp2 = mint % 10;
  int temp1 = (mint / 10) % 10;
  minutes = "00:" + String(temp1) + String(temp2);
  if (!emptyFlag) {
    screen_Time();
  }
}

//void updateSec(int se){
//  sec="";
//  int temp2=se%10;
//  int temp1=(se/10)%10;
//  sec=String(temp1)+String(temp2);
//screen_Time();
//}
/******************************ScreenDisplay*****************************/
/******************************ScreenDisplay*****************************/
/******************************ScreenDisplay*****************************/

void initilizeScreen() {
  myGLCD.setBackColor(0, 0, 0);
  myGLCD.setColor(0, 255, 0);
  myGLCD.setFont(LargeFont);
  myGLCD.print("### Welcome To Musallah ###", CENTER, 20);
  myGLCD.setColor(255, 0, 0);
  myGLCD.setFont(SmallFont);
  myGLCD.print("Double-click RED button to REST ", RIGHT, 450);

}

void screen_waiting() {
  myGLCD.setBackColor(VGA_BLACK);
  myGLCD.setColor(255, 255, 255);
  myGLCD.setFont(LargeFont);
//TODO BUTTON
  myGLCD.setColor(0, 0, 255);
  myGLCD.setBackColor(0x001F);
  myGLCD.fillRoundRect(CENTER - 140, 140, CENTER, 270);

  myGLCD.setColor(255, 255, 255);
  myGLCD.drawRoundRect(CENTER - 140, 140, CENTER, 270);
  myGLCD.print("Waiting", CENTER + 15 - 140, 170);
  myGLCD.print("Outside", CENTER + 15 - 140, 220);
  /*******/
}

void screen_no_waiting() {
  myGLCD.setBackColor(VGA_BLACK);
  myGLCD.setColor(255, 255, 255);
  myGLCD.setFont(LargeFont);
//TODO BUTTON
  myGLCD.setColor(0, 0, 255);
  myGLCD.setBackColor(0x001F);
  myGLCD.fillRoundRect(CENTER - 140, 140, CENTER, 270);

  myGLCD.setColor(255, 255, 255);
  myGLCD.drawRoundRect(CENTER - 140, 140, CENTER, 270);
  myGLCD.print("  Not", CENTER + 15 - 140, 170);
  myGLCD.print("Waiting", CENTER + 15 - 140, 220);
  /*******/
}
void screen_Inside() {
  myGLCD.setBackColor(VGA_BLACK);
  myGLCD.setColor(255, 255, 255);
  myGLCD.setFont(LargeFont);
  myGLCD.print(inside, CENTER, 140);
  screen_waiting(); //return it to waiting 

}

void screen_empty() {
  myGLCD.setBackColor(VGA_BLACK);
  myGLCD.setColor(255, 255, 255);
  myGLCD.setFont(LargeFont);
  myGLCD.print(inside, CENTER, 140);
}

void screen_salah() {
  myGLCD.setBackColor(VGA_BLACK);
  myGLCD.setColor(255, 255, 255);
  myGLCD.setFont(LargeFont);
  myGLCD.print(salah, CENTER, 66);
}

String timee;
void screen_Time() {
  myGLCD.setBackColor(VGA_BLACK);
  myGLCD.setColor(255, 255, 255);
  myGLCD.setFont(BigFont);
  myGLCD.print("   Time[m] passed:", LEFT, 390);
  myGLCD.setColor(0, 255, 0);
  myGLCD.setFont(SevenSegNumFont);
  //timee= minutes +":"+ sec; //for adding secondes
  //myGLCD.print(timee, CENTER, 390);
  myGLCD.print(minutes, CENTER, 390);
}
/******************************RececingNewData*****************************/
/******************************RececingNewData*****************************/
/******************************RececingNewData*****************************/
char data;
//receving the command then the data
void receiveEvent(int howMany) {
  Serial.println("even has been recieved");

  int i = 0;
  if (command) {
    while (Wire.available() > 0) { // loop through all but the last
      command_t = Wire.read(); // receive byte as a character the first byte is the command type then the data
    }
    Serial.println(command_t);
    command = 0;
  } else {
    while (Wire.available() > 0) { // loop through all but the last
      data = Wire.read(); // receive byte as a character the first byte is the command type then the data
    }
    if (command_t == 'I') {  //Inside
      data1 = data;
      updateInside(data1);
      Serial.println(data1);

    }
    if (command_t == 'P') { //Salah
      data2 = data;
      updateSalah(data2);
      Serial.println(data2);

    }
    if (command_t == 'M') { //minutes
      data3 = data;
      updateMinutes(data3);
    }
//   if(command_t=='S'){ //secondes 
//     data4=data;
//    updateSec(data4);
//   }  

    if (command_t == 'R') { //reset screen
      updateScreen();

    }
    if (command_t == 'W') { //waiting button
      waitingFlag = 'W';
      screen_waiting();
    }
    if (command_t == 'N') { //not waiting button
      waitingFlag = 'N';
      screen_no_waiting();

    }
    command = 1;
  }
}

/*****************************  _Screeen*****************************/
/*****************************TOUCH_Screeen*****************************/
/*****************************TOUCH_Screeen*****************************/
// Draw a red frame while a button is touched
void waitForIt(int x1, int y1, int x2, int y2) 
{

  myGLCD.drawRoundRect(x1, y1, x2, y2);
  int count=0;
  while (myTouch.dataAvailable()) {
    myTouch.read();
      if(count<15){
      myGLCD.setColor(255, 0, 0);
      myGLCD.setBackColor(0x001F);
      myGLCD.setFont(SmallFont);
      myGLCD.print("*  HOLD!  *", CENTER, y2-60);
      }
      if(count>=15){
          myGLCD.setColor(255, 0, 0);
          myGLCD.setBackColor(0x001F);
          myGLCD.setFont(SmallFont);
          myGLCD.print("* RELEASE *", CENTER, y2-60);
        }
        count++;
      }


  myGLCD.drawRoundRect(x1, y1, x2, y2);
}

/*****************************SendingWaitingButton***********************/
/*****************************SendingWaitingButton***********************/
/*****************************SendingWaitingButton***********************/
int flag = 0;
void requestEvent() { //when master request
  if ((y >= 204) && (y <= 340))  // Upper row
      {
    if ((x >= 320) && (x <= 460))  // Button: 1
        {
      if (flag == 0) {
        flag = 1;
        Wire.write("W"); // waiting
      } else {
        flag = 0;
        Wire.write("N"); // not waiting
      }
    } else {
      Wire.write("1"); // respond with message of 6 bytes
    }
  } else {
    Wire.write("2"); // respond with message of 6 bytes

  }
  x = 0;
  y = 0;
}

