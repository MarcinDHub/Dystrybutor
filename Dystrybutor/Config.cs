using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dystrybutor
{
    class Config
    {
        // public static double converter = 0.2937500;    // Stacja - 01
        public static double converter = 0.28371;         // Bennett
        public static List<Refueling> refuelingList = new List<Refueling>();


        public class Refueling
        {
            public DateTime startTime { get; set; }
            public string dateFormated { get { return startTime.ToString("dd.MM.yyy hh:mm"); } set { } }
            public double fuelAmount { get; set; }
            public string fuelAmountFormated { get { return Math.Round(fuelAmount, 2).ToString() + " L"; } set { } }
            public string dallas1 { get; set; }
        }


        public class Frame
        {
            public DateTime dateTime { get; set; }
            public int l7 { get; set; }
            public int dallas1 { get; set; }
            public int dallas2 { get; set; }
        }
    }
}
