# SmartHome_func
스마트홈 센싱코드들 올려둘게요 

## 스마트홈 전체 센싱 코드
```python
#include "SoftI2CMaster.h"
#include "LiquidCrystal_SoftI2C.h"
#include "DHT.h"

LiquidCrystal_SoftI2C mylcd(0x27,16,2,A5,A4);

volatile int gas;
volatile int flame;

DHT dhtA3(A3, 11);

void humidfunc()
{
  Serial.println(String("T:")+String(dhtA3.readTemperature()));
  mylcd.setCursor(0, 0);
  mylcd.print(String("T:") + String(dhtA3.readTemperature()));
  mylcd.setCursor(0, 1);
  mylcd.print(String("H:") + String(dhtA3.readHumidity()));

  
  if(dhtA3.readTemperature()>30){
    digitalWrite(5,HIGH);
    digitalWrite(6,LOW);
    delay(2000);
  } else{
    digitalWrite(5,LOW);
    digitalWrite(6,LOW);
  }
}

void movebellfunc()
{
   if (digitalRead(2) == 1) {
    tone(3,247);
    delay(400);
    tone(3,294);
    delay(400);
    tone(3,370);
    delay(400);
    tone(3,294);
    delay(400);
    tone(3,233);
    delay(400);
    tone(3,277);
    delay(400);
    tone(3,330);
    delay(400);
    tone(3,233);
    delay(400);
    tone(3,247);
    delay(400);
    tone(3,294);
    delay(400);
    tone(3,370);
    delay(400);
    tone(3,294);
    delay(400);
    tone(3,233);
    delay(400);
    tone(3,277);
    delay(400);
    tone(3,330);
    delay(400);
    tone(3,466);
    delay(400);
    tone(3,392);
    delay(400);
    tone(3,370);
    delay(400);
    tone(3,330);
    delay(400);    
    tone(3,392);
    delay(400);   
    tone(3,370);
    delay(400);
    tone(3,330);
    delay(400);
    tone(3,294);
    delay(400);
    tone(3,277);
    delay(400);
    tone(3,494);
    delay(800);
    tone(3,392);
    delay(800);
  } else {
    noTone(3);
  }

  // 움직임
  if (digitalRead(4) == 1)
  {
    digitalWrite(3, HIGH);
  } else {
    digitalWrite(3, LOW);
  }
}

void gasflame()
{
  gas = analogRead(A0);
  flame = digitalRead(8);
  Serial.print(String("gas:") + String(gas));
  Serial.print(",");
  Serial.println(String("flame:") + String(flame));
  delay(200);

  if (gas > 100 || flame == 0) 
  {
    digitalWrite(3,LOW);
    digitalWrite(5,HIGH);
    digitalWrite(6,LOW);
    tone(3,131);
    delay(1000);
    tone(3,220);
    delay(1000);
  } 
  else {
    digitalWrite(3,LOW);
    digitalWrite(5,LOW);
    digitalWrite(6,LOW);
    noTone(3);
  }
}

void setup() {
  // 온습도 모터
  Serial.begin(9600);
  dhtA3.begin();
  pinMode(5,OUTPUT);
  pinMode(6,OUTPUT);
  // 온습도 LCD
  mylcd.init();
  mylcd.backlight();

  // 움직임 감지
  pinMode(4, INPUT);
  pinMode(3, OUTPUT);

  // 초인종
  pinMode(2, INPUT);
  pinMode(3, OUTPUT);

  // 가스, 화재감지
  Serial.begin(9600);
  gas = 0;
  flame = 0;
  pinMode(8, INPUT);
  pinMode(5, OUTPUT);
  pinMode(6, OUTPUT);
  pinMode(3, OUTPUT);

  
   
}

void loop() {
  // put your main code here, to run repeatedly:

  humidfunc();

  movebellfunc();

  gasflame();

}


```