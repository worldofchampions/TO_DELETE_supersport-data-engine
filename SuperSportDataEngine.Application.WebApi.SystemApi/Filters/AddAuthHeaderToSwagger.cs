using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Description;

namespace SuperSportDataEngine.Application.WebApi.SystemApi.Filters
{
    /// <summary>
    /// Class to allow swagger use Authorization header
    /// </summary>
    public class AddAuthHeaderToSwagger : IOperationFilter
    {
        /// <summary>
        /// Method that applies the header for use in swagger
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="schemaRegistry"></param>
        /// <param name="apiDescription"></param>
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.parameters != null)
            {
                operation.parameters.Add(new Parameter
                {
                    name = "Authorization",
                    @in = "header",
                    description = "access token",
                    required = false,
                    type = "string"
                });
            }
        }
    }
}