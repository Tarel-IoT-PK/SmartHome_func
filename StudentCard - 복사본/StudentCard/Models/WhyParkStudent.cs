using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentCard.Models
{
    public class WhyParkStudent
    {
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string Reason { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
