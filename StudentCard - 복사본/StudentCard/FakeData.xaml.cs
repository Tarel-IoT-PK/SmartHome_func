using Bogus;
using StudentCard.Module;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows;
using uPLibrary.Networking.M2Mqtt;

namespace StudentCard
{
    /// <summary>
    /// FakeData.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FakeData : MetroWindow
    {
        Faker<SensorInfo> FakeHomeSensor { get; set; } = null;            // 가짜 스마트홈 센서값 변수

        int MaxCount { get; set; } = 10;
        MqttClient Client { get; set; }
        Thread MqttThread = null;

        public FakeData()
        {
            InitializeComponent();
            InitFakeData();
        }

        private void InitFakeData()
        {
            FakeHomeSensor = new Faker<SensorInfo>()
                .RuleFor(s => s.Home_Id, f => f.Random.Int(101, 104))                    // 임의로 픽스된 홈아이디 101동 703호
                .RuleFor(s => s.Sensing_DateTime, f => f.Date.Past(0))           // 현재시각이 생성
                .RuleFor(s => s.Temp, f => f.Random.Float(20.0f, 30.0f))         // 20 ~ 30도 사이 실수값 생성
                .RuleFor(s => s.Humid, f => f.Random.Float(40.0f, 64.0f));       // 40 ~ 64% 사이의 습도값
        }

        private async void Btn_Connect_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Tbx_MqttBrokerIp.Text))
            {
                await this.ShowMessageAsync("오류", "브로커 아이피를 입력하세요");
                return;
            }
            // 브로커아이피로 접속
            ConnectMqttBroker();

            // 하위의 로직을 무한반복
            StartPublish();
        }

        private void StartPublish()
        {
            // 센서값 MQTT
            // var info = FakeHomeSensor.Generate();
            // RtbLog에 출력
            MqttThread = new Thread(() =>
            {
                while (true)
                {
                    SensorInfo info = FakeHomeSensor.Generate();
                    Debug.WriteLine($"{info.Home_Id} / {info.Sensing_DateTime} / {info.Temp}");
                    var jsonValue = JsonConvert.SerializeObject(info, Formatting.Indented);
                    Client.Publish("Campus/IoTData/", Encoding.Default.GetBytes(jsonValue));
                    this.Invoke(new Action(() => {
                        Rtb_Log.AppendText($"{jsonValue}\n");
                        Rtb_Log.ScrollToEnd();


                        if (MaxCount <= 0)
                        {
                            Rtb_Log.Document.Blocks.Clear();
                            Rtb_Log.AppendText(">>> 문서 건수가 많아져서 초기화! \n");
                            Rtb_Log.ScrollToEnd();
                            MaxCount = 10;  // 테스트할땐 10, 운영시는 50, 위에 선언부분도 수정
                        }

                        Rtb_Log.ScrollToEnd();
                        MaxCount--;

                    }));
                    Thread.Sleep(1000);
                }

            });
            MqttThread.Start();
        }

        private void ConnectMqttBroker()
        {
            Client = new MqttClient(Tbx_MqttBrokerIp.Text);
            Client.Connect("Campus");
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Client != null && Client.IsConnected == true)
            {
                Client.Disconnect();        // 접속을 안끊으면 메모리 상에 계속 남아 있다.
            }

            if (MqttThread != null)
            {
                MqttThread.Abort();         // 여기가 없으면 프로그램 종료 후에도 메모리에 남아있다.
            }
        }

    }
}
