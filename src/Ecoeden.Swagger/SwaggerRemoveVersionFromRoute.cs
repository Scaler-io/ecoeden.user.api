using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ecoeden.Swagger
{
    public sealed class SwaggerRemoveVersionFromRoute : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var modifiedPaths = new OpenApiPaths();
            foreach(var path in swaggerDoc.Paths)
            {
                string pathWithoutVersion = path.Key.Remove(0, 7);
                if (string.IsNullOrEmpty(pathWithoutVersion))
                {
                    pathWithoutVersion = "/";
                }
                modifiedPaths.Add(pathWithoutVersion, path.Value);
            }

            swaggerDoc.Paths = modifiedPaths;
        }
    }
}
