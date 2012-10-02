using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.MobileServices;
using System.Runtime.Serialization;

namespace SpeedTrapDemo
{
    [DataTable(Name = "speedTrapLocations")]
    public class SpeedTrapLocation
    {

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "longitude")]
        public double Longitude { get; set; }

        [DataMember(Name = "createdAt")]
        public DateTime? CreatedAt { get; set; }

        /*NOTE:
         * For this to behave properly, you have to add the following code:
         *      item.createdAt = new Date();
         * To your Insert script on Azure Mobile Services. This will throw
         * an exception otherwise
         */

    }
}
