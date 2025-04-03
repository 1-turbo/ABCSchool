namespace Infrastructure.OpenApi
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple =false)]
    public class SwaggerHearderAttribute : Attribute
    {
        public string HeaderName { get; }
        public string Description { get; }
        public string DefaultName { get; }
        public bool IsRequired { get; }
        public string DefaultValue { get; }  

        public SwaggerHearderAttribute(
            string headerName,
            string description,
            string defaultName = "",
            bool isRequired = false,
            string defaultValue = "")
        {
            HeaderName = headerName;
            Description = description;
            DefaultName = defaultName;
            IsRequired = isRequired;
            DefaultValue = defaultValue;
        }
    }
}
