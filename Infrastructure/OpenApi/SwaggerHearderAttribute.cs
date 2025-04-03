using Newtonsoft.Json.Linq;

namespace Infrastructure.OpenApi
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple =false)]
    public class SwaggerHearderAttribute(string headerName, string description, string defaultName, bool isRequired , string defaultValue) : Attribute
    {
        public string HeaderName { get; set; } = headerName;
        public string Description { get; set; } = description;
        public string DefaultName { get; set; } = defaultName;
        public string DefaultValue { get; set; } = defaultValue;
        public bool IsRequired { get; set; } = isRequired;
    }
}
