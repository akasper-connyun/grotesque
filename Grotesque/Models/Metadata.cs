using Grotesque.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grotesque.Models
{
    public class Metadata
    {
        public SearchSpan searchSpan { get; set; }
    }

    public class SearchSpan
    {
        public From from { get; set; }
        public To to { get; set; }
    }

    public class From
    {
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-ddTHH:mm:ss.fffZ")]
        public DateTime dateTime { get; set; }
    }

    public class To
    {
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-ddTHH:mm:ss.fffZ")]
        public DateTime dateTime { get; set; }
    }
}
