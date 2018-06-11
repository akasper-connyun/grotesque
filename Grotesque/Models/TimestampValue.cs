using Grotesque.Util;
using Newtonsoft.Json;
using System;

namespace Grotesque.Models
{
    public class TimestampValue
    {
        [JsonConverter(typeof(CustomDateTimeFormatConverter), "yyyy-MM-ddTHH:mm:ss.fffZ")]
        public DateTime timestamp { get; set; }
        public Object value { get; set; }
    }
}
