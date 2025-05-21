using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SpecFlow.PoC.Features;
using SpecFlow.PoC.Features.GetEmployees;

namespace SpecFlow.PoC.Controllers;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]s")]
public class EmployeeController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmployeeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Employee[]))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<Results<Ok<Employee[]>, NotFound>> Get(Guid id)
    {
        var employeesResult = await _mediator.Send( new GetEmployees.Request(id) );
        if (!employeesResult.Any())
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(employeesResult);
    }
}
