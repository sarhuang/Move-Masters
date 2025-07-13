#include "Keyboard.h"

int nButton = 13; //North
int neButton = 12; //North East
int eButton = 11; //East
int seButton = 10; //South East
int sButton = 9; //South
int swButton = 8; //South West
int wButton = 7; //West
int nwButton = 6; //North West
int cButton = 5; //Center
int pressDelay = 50;

int pressed = 0; //Use this as a byte array for the buttons pressed
int  nPressed = B1;
int nePressed = B10;
int  ePressed = B100;
int sePressed = B1000;
int  sPressed = B10000;
int swPressed = B100000;
int  wPressed = B1000000;
int nwPressed = B10000000;
int  cPressed = 256;

int  nKey = 119; //w
int neKey = 101; //e
int  eKey = 100; //d
int seKey = 99; //c
int  sKey = 120; //x
int swKey = 122; //z
int  wKey = 97; //a
int nwKey = 113; //q
int  cKey = 115; //s


void setup() {
  // put your setup code here, to run once:
  pinMode(nButton, INPUT_PULLUP);
  pinMode(neButton, INPUT_PULLUP);
  pinMode(eButton, INPUT_PULLUP);
  pinMode(seButton, INPUT_PULLUP);
  pinMode(sButton, INPUT_PULLUP);
  pinMode(swButton, INPUT_PULLUP);
  pinMode(wButton, INPUT_PULLUP);
  pinMode(nwButton, INPUT_PULLUP);
  pinMode(cButton, INPUT_PULLUP);
}

void pressKey(int key) {
  //Keyboard.press(key);
  char bigBuffer[40];
  sprintf(buffer, "Key Pressed: %d", key);
  Serial.println(buffer);
}

void releaseKey(int key) {
  //Keyboard.release(key);
}

void interpretButton(int p) {
  if (p & nPressed > 1) pressKey(nKey); 
  else releaseKey(nKey);

  if (p & nePressed > 1) pressKey(neKey); 
  else releaseKey(neKey);

  if (p & ePressed > 1) pressKey(eKey); 
  else releaseKey(eKey);

  if (p & sePressed > 1) pressKey(seKey); 
  else releaseKey(seKey);

  if (p & sPressed > 1) pressKey(sKey); 
  else releaseKey(sKey);

  if (p & swPressed > 1) pressKey(swKey); 
  else releaseKey(swKey);

  if (p & wPressed > 1) pressKey(wKey); 
  else releaseKey(wKey);

  if (p & nwPressed > 1) pressKey(nwKey); 
  else releaseKey(nwKey);

  if (p & cPressed > 1) pressKey(cKey); 
  else releaseKey(cKey);
}

void loop() {
  // put your main code here, to run repeatedly:
  pressed = 0;
  //Serial.println("Loop");

  if (digitalRead(nButton) == HIGH)  pressed = pressed | nPressed;
  if (digitalRead(neButton) == HIGH) pressed = pressed | nePressed;
  if (digitalRead(eButton) == HIGH)  pressed = pressed | ePressed;
  if (digitalRead(seButton) == HIGH) pressed = pressed | sePressed;
  if (digitalRead(sButton) == HIGH)  pressed = pressed | nPressed;
  if (digitalRead(swButton) == HIGH) pressed = pressed | swPressed;
  if (digitalRead(wButton) == HIGH)  pressed = pressed | wPressed;
  if (digitalRead(nwButton) == HIGH) pressed = pressed | nwPressed;
  if (digitalRead(cButton) == HIGH)  pressed = pressed | cPressed;

  if (pressed > 0) {
    //Serial.println("Button Pressed!");
    interpretButton(pressed);
  } else {
    Serial.println("Waiting...");
  }
}
