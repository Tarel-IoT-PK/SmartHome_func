using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentCard.Module
{
    public class SensorInfo
    {
        public int Home_Id { get; set; }                  // D101H101   -> 101동 101호?
        //public string Room_Name { get; set; }               // Living, Dinning, Bed, Bath
        public DateTime Sensing_DateTime { get; set; }      // 센싱되는 현재시각

        public float Temp { get; set; }                     // 온도

        public float Humid { get; set; }                    // 습도

        public int Gas { get; set; }                        // 가스

        public int Flame { get; set; }                      //화재

    }
}
