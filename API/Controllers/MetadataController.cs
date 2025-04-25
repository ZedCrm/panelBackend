using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using MyFrameWork.AppTool;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetadataController : ControllerBase
    {
        [HttpGet("GetModelMetadata/{modelName}")]
        public IActionResult GetModelMetadata(string modelName)
        {
            var assembly = Assembly.Load("App.Contracts");

            var type = assembly.GetTypes()
                .FirstOrDefault(t =>
                    t.IsClass &&
                    t.Namespace != null &&
                    t.Namespace.StartsWith("App.Contracts") &&
                    t.Name.Equals(modelName, StringComparison.OrdinalIgnoreCase));

            if (type == null)
                return NotFound($"Model '{modelName}' not found.");

            var propertyMetadata = new List<object>();

            foreach (var prop in type.GetProperties())
            {
                var displayAttr = prop.GetCustomAttribute<DisplayAttribute>();
                var requiredAttr = prop.GetCustomAttribute<RequiredAttribute>();
                var stringLengthAttr = prop.GetCustomAttribute<StringLengthAttribute>();
                var minLengthAttr = prop.GetCustomAttribute<MinLengthAttribute>();
                var maxLengthAttr = prop.GetCustomAttribute<MaxLengthAttribute>();
                var selectSourceAttr = prop.GetCustomAttribute<SelectSourceAttribute>();

                var inputType = prop.PropertyType == typeof(bool) ? "boolean" :
                                prop.PropertyType == typeof(int) || prop.PropertyType == typeof(long) ? "number" :
                                "text";

                var metadata = new
                {
                    name = ToCamelCase(prop.Name),
                    displayName = displayAttr?.Name ?? prop.Name,
                    inputType,
                    controlType = selectSourceAttr != null ? "select" : null,
                    selectSource = selectSourceAttr?.SourceName,
                    required = requiredAttr != null,
                    maxLength = maxLengthAttr?.Length ?? stringLengthAttr?.MaximumLength,
                    minLength = minLengthAttr?.Length ?? stringLengthAttr?.MinimumLength
                };

                propertyMetadata.Add(metadata);
            }

            return Ok(new Dictionary<string, List<object>> { { type.Name, propertyMetadata } });
        }

        private static string ToCamelCase(string input)
        {
            if (string.IsNullOrEmpty(input) || char.IsLower(input[0]))
                return input;

            return char.ToLower(input[0]) + input.Substring(1);
        }
    }
}
