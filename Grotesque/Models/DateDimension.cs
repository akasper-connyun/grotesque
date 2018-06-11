namespace Grotesque.Models
{
    public class DateDimension
    {
        public DateDimension(string _builtInProperty, string _size)
        {
            dateHistogram = new DateHistogram(_builtInProperty, _size);
        }
        public DateHistogram dateHistogram { get; set; }
    }

    public class DateHistogram
    {
        public DateHistogram(string _builtInProperty, string _size)
        {
            input = new BuiltInInput { builtInProperty = _builtInProperty };
            breaks = new Breaks { size = _size };
        }
        public BuiltInInput input { get; set; }
        public Breaks breaks { get; set; }
    }

    public class BuiltInInput
    {
        public string builtInProperty { get; set; }
    }

    public class Breaks
    {
        public string size { get; set; }
    }
}
