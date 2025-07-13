/* Testing dance machine buttons to communicate to Processing */
const int buttonPin1 = 1;     // the number of the pushbutton pin
const int buttonPin2 = 2;
const int buttonPin3 = 3;
const int buttonPin4 = 4; 
const int buttonPin5 = 5;
const int buttonPin6 = 6;
const int buttonPin7 = 7;
const int buttonPin8 = 8;
const int buttonPin9 = 9;


int buttonState1 = LOW; 
int buttonState2 = LOW;         // variable for reading the pushbutton status
int buttonState3 = LOW;
int buttonState4 = LOW;
int buttonState5 = LOW;
int buttonState6 = LOW;
int buttonState7 = LOW;
int buttonState8 = LOW;
int buttonState9 = LOW;
char buttonsPressed[9];



void setup() {
  Serial.begin(9600);
  
  pinMode(buttonPin1, INPUT);
  pinMode(buttonPin2, INPUT);
  pinMode(buttonPin3, INPUT);
  pinMode(buttonPin4, INPUT);
  pinMode(buttonPin5, INPUT);
  pinMode(buttonPin6, INPUT);
  pinMode(buttonPin7, INPUT);
  pinMode(buttonPin8, INPUT);
  pinMode(buttonPin9, INPUT);
}

void loop() {
  for (int i=0; i < sizeof(buttonsPressed); i++) {
    buttonsPressed[i] = '-';
  }

  buttonState1 = digitalRead(buttonPin1);
  buttonState2 = digitalRead(buttonPin2);
  buttonState3 = digitalRead(buttonPin3);
  buttonState4 = digitalRead(buttonPin4);
  buttonState5 = digitalRead(buttonPin5);
  buttonState6 = digitalRead(buttonPin6);
  buttonState7 = digitalRead(buttonPin7);
  buttonState8 = digitalRead(buttonPin8);
  buttonState9 = digitalRead(buttonPin9);
  
  //The button numbers relate to the keyboard number pad
  if (buttonState1 == HIGH) {  
    buttonsPressed[0] = 'a';
  } 
  if (buttonState2 == HIGH) {
    buttonsPressed[1] = 'f';
    //Serial.println('s');
  } 
  if (buttonState3 == HIGH) {
    buttonsPressed[2] = 'l';
    //Serial.println('d');
  } 
  if (buttonState4 == HIGH) {
    buttonsPressed[3] = 'd';
    //Serial.println('f');
  } 
  if (buttonState5 == HIGH) {
    buttonsPressed[4] = 'g';
    //Serial.println('g');
  } 
  if (buttonState6 == HIGH) {
    buttonsPressed[5] = 'j';
    //Serial.println('h');
  } 
  if (buttonState7 == HIGH) {
    buttonsPressed[6] = 's';
    //Serial.println('j');
  } 
  if (buttonState8 == HIGH) {
    buttonsPressed[7] = 'h';
    //Serial.println('k');
  } 
  if (buttonState9 == HIGH) {
    buttonsPressed[8] = 'k';
    //Serial.println('l');
  } 
  // if(buttonState2 == LOW && buttonState5 == LOW && buttonState8 == LOW){
  //   Serial.println("");
  // }
  /*
  if(buttonState1 == LOW && buttonState2 == LOW && buttonState3 == LOW && buttonState4 == LOW && buttonState5 == LOW &&
    buttonState6 == LOW && buttonState7 == LOW && buttonState8 == LOW && buttonState9 == LOW){
      Serial.println("");
  }*/
  Serial.println(buttonsPressed);

  Serial.flush();
  delay(80);
}