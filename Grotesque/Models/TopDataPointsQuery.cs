using Grotesque.Util;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Grotesque.Models
{
    public class TopDataPointsQuery
    {
        [JsonConverter(typeof(CustomDateTimeFormatConverter), "yyyy-MM-ddTHH:mm:ss.fffZ"), JsonRequired]
        public DateTime from;
        [JsonConverter(typeof(CustomDateTimeFormatConverter), "yyyy-MM-ddTHH:mm:ss.fffZ"), JsonRequired]
        public DateTime to;
        [DefaultValue(10)]
        public int count = 10;
    }
}
