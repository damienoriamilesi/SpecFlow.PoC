using System.Collections.Concurrent;

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
    private int maxThreadsAllowed = 1;
    //private static Mutex _mutex = new(true, "UniqueAppId");

    public EmployeeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private static readonly ConcurrentDictionary<string, SemaphoreSlim> _userLocks = new();
    private const string userId = $"bulk_user_TOTO_{nameof(Get)}";

    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Employee[]))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    public async Task<Results<Ok<Employee[]>, BadRequest>> Get(Guid id)
    {
        //Application logic here
        var semaphore = _userLocks.GetOrAdd(userId, _ => new SemaphoreSlim(1, 1));
        if (semaphore.CurrentCount > maxThreadsAllowed)
        {
            //return TypedResults.BadRequest<BadRequest>();
            //425 : Too Early
            throw new Exception("Concurrent attempt not allowed");
        }
        //await semaphore.WaitAsync();

        //return Results.BadRequest(new{ Error = "Too many threads"  });

        try
        {
            // Simulate some async work
            var employeesResult = await _mediator.Send( new GetEmployees.Request(id) );
            return TypedResults.Ok(employeesResult);
        }
        finally
        {
            await Task.Delay(5000);

            // Optional: Clean up if no one is waiting
            if (semaphore.CurrentCount == 1)
            {
                _userLocks.TryRemove(userId, out _);
            }
            semaphore.Release();
        }
    }
}
