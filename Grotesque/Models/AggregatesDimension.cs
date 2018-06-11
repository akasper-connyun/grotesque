namespace Grotesque.Models
{
    public class AggregatesDimension
    {
        public AggregatesDimension(string _property, string _propertyType, int _take)
        {
            uniqueValues = new UniqueValues(_property, _propertyType, _take);
        }

        public UniqueValues uniqueValues { get; set; }
    }

    public class UniqueValues
    {
        public UniqueValues(string _property, string _propertyType, int _take = 100)
        {
            take = _take;
            input = new PropertyWithType(_property, _propertyType);
        }

        public PropertyWithType input { get; set; }
        public int take { get; set; }
    }
}
