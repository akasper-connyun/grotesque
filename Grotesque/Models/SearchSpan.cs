using Grotesque.Util;
using Newtonsoft.Json;
using System;

namespace Grotesque.Models
{
    public class SearchSpan
    {
        public SearchSpan(DateTime f, DateTime t)
        {
            from = new SpanDateTime(f);
            to = new SpanDateTime(t);
        }

        public SpanDateTime from { get; set; }
        public SpanDateTime to { get; set; }
    }

    public class SpanDateTime
    {
        public SpanDateTime(DateTime dt) => dateTime = dt;

        [JsonConverter(typeof(CustomDateTimeFormatConverter), "yyyy-MM-ddTHH:mm:ss.fffZ")]
        public DateTime dateTime { get; set; }
    }
}