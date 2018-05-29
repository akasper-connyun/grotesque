using Newtonsoft.Json.Converters;

namespace Grotesque.Util
{
    public class CustomDateTimeFormatConverter : IsoDateTimeConverter
    {
        public CustomDateTimeFormatConverter(string format)
        {
            DateTimeFormat = format;
        }
    }
}
