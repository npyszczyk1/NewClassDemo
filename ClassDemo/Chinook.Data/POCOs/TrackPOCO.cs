using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chinook.Data.POCOs
{
    public class TrackPOCO
    {
        public string song { get; set; }
        public int milliseconds { get; set; }
        public string length  //calculated value
        {
            get
            {
                int minutes = milliseconds / 60000;
                int seconds = (milliseconds % 60000) / 1000;
                return string.Format("{0}:{1:00}", minutes, seconds); //leading zero
            }
        }
    }
}
