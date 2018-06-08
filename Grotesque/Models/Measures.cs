using System;

namespace Grotesque.Models
{
    public class Measures
    {
        public class Avg
        {
            public Avg(string _property, string _type)
            {
                input = new PropertyWithType(_property, _type);
            }

            public PropertyWithType input { get; set; }
        }

        public class Last
        {
            public Last(string _inputProperty, string _inputPropertyType, string _orderProperty, string _orderPropertyType)
            {
                input = new PropertyWithType(_inputProperty, _inputPropertyType);
                orderBy = new PropertyWithType(_orderProperty, _orderPropertyType);
            }

            public PropertyWithType input { get; set; }
            public PropertyWithType orderBy { get; set; }
        }
    }

    public class AvgMeasure
    {
        public AvgMeasure(string _property, string _type)
        {
            avg = new Measures.Avg(_property, _type);
        }

        public Measures.Avg avg { get; set; }
    }

    public class LastMeasure
    {
        public LastMeasure(string _property, string _propertyType, string _orderProperty, string _orderPropertyType)
        {
            last = new Measures.Last(_property, _propertyType, _orderProperty, _orderPropertyType);
        }

        public Measures.Last last { get; set; }
    }

    public class CountMeasure
    {
        public Object count { get; set; }
    }
}
