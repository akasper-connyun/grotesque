using Grotesque.Util;
using Newtonsoft.Json;
using System;

namespace Grotesque.Models
{
    public class TopDataPointsQuery
    {
        [JsonConverter(typeof(CustomDateTimeFormatConverter), "yyyy-MM-ddTHH:mm:ss.fffZ")]
        public DateTime from { get; set; }
        [JsonConverter(typeof(CustomDateTimeFormatConverter), "yyyy-MM-ddTHH:mm:ss.fffZ")]
        public DateTime to { get; set; }
        public int count { get; set; } = 1000;
    }
}
