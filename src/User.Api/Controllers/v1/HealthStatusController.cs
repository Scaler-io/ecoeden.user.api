using Asp.Versioning;
using Ecoeden.Swagger;
using Ecoeden.Swagger.Examples;
using Ecoeden.Swagger.Examples.HealthCheck;
using Ecoeden.User.Application.Features.HealthCheck.Queries.GetHealthCheckStatus;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Responses.HealthCheck;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Net;

namespace User.Api.Controllers.v1
{
    [ApiVersion("1")]
    public class HealthStatusController : ApiBaseController
    {
        private readonly IMediator _mediator;

        public HealthStatusController(ILogger logger, IMediator mediator)
            : base(logger)
        {
            _mediator = mediator;
        }

        [HttpGet("status")]
        [SwaggerHeader("CorrelationId", Description = "expects unique correlation id")]
        [SwaggerOperation(OperationId = "CheckApiHealth", Description = "checks for api health")]
        // 200
        [ProducesResponseType(typeof(HealthCheckResponse), (int)HttpStatusCode.OK)]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(HealthCheckResponseExample))]
        // 500
        [ProducesResponseType(typeof(ApiExceptionResponse), (int)HttpStatusCode.InternalServerError)]
        [SwaggerResponseExample((int)HttpStatusCode.InternalServerError, typeof(InternalServerResponseExample))]
        public async Task<IActionResult> CheckApiHealth()
        {
            var request = new GetHealthCheckStatusQuery(RequestInformation.CorrelationId);
            var result = await _mediator.Send(request);
            return OkOrFailure(result);
        }
    }
}
