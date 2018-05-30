using Grotesque.Util;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Grotesque.Models
{
    public class TopDataPointsQuery
    {
        [JsonConverter(typeof(CustomDateTimeFormatConverter), "yyyy-MM-ddTHH:mm:ss.fffZ")]
        [Required]
        public DateTime from { get; set; }
        [JsonConverter(typeof(CustomDateTimeFormatConverter), "yyyy-MM-ddTHH:mm:ss.fffZ")]
        [Required]
        public DateTime to { get; set; }
        [DefaultValue(1000)]
        public int count { get; set; } = 1000;
    }
}
