namespace Grotesque.Models
{
    public class PropertyWithType
    {
        public PropertyWithType(string _property, string _type)
        {
            property = _property;
            type = _type;
        }

        public string property { get; set; }
        public string type { get; set; }
    }
}
