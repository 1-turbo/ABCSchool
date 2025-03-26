namespace Infrastructure.OpenApi
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple =false)]
    public class SwaggerHearderAttribute(string headerName, string description, string defaultName, bool isRequired) : Attribute
    {
        public string HeaderName { get; set; } = headerName;
        public string Description { get; set; } = description;
        public string DefaultName { get; set; } = defaultName;
        public bool IsRequired { get; set; } = isRequired;
    }
}
