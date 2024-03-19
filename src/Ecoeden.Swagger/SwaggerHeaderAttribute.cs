namespace Ecoeden.Swagger
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SwaggerHeaderAttribute : Attribute
    {
        public SwaggerHeaderAttribute(string name, string type)
        {
            Name = name;
            Type = type;
            Description = string.Empty;
            Required = false;
        }

        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
    }
}
