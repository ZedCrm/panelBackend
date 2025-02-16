using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetadataController : ControllerBase
    {
        [HttpGet("GetModelMetadata/{modelName}")]
       public IActionResult GetModelMetadata(string modelName)
{
    var assembly = Assembly.Load("App.Contracts"); // Or use Assembly.GetExecutingAssembly() if in the same project

    var type = assembly.GetTypes()
                        .FirstOrDefault(t => t.IsClass && t.Namespace != null && t.Namespace.StartsWith("App.Contracts") && t.Name.Equals(modelName, StringComparison.OrdinalIgnoreCase));

    if (type == null)
    {
        return NotFound($"Model '{modelName}' not found.");
    }

    var propertyMetadata = new List<object>();
    var properties = type.GetProperties();

    foreach (var prop in properties)
    {
         var displayAttr = prop.GetCustomAttribute<DisplayAttribute>(); 
        var metadata = new
        {
            Name = prop.Name,
            DisplayName = displayAttr?.Name ?? prop.Name, 
            Type = prop.PropertyType.Name,
            Required = prop.GetCustomAttribute<RequiredAttribute>() != null,
            MaxLength = prop.GetCustomAttribute<StringLengthAttribute>()?.MaximumLength,
            MinLength = prop.GetCustomAttribute<StringLengthAttribute>()?.MinimumLength
        };

        propertyMetadata.Add(metadata);
    }

    return Ok(new Dictionary<string, List<object>> { { type.Name, propertyMetadata } });
}

       
    }
}