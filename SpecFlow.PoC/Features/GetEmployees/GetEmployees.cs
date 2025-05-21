using MediatR;

using Microsoft.EntityFrameworkCore;

using NuGet.Protocol.Plugins;

namespace SpecFlow.PoC.Features.GetEmployees;

public static class GetEmployees
{
    public record Request(Guid id) : IRequest<Employee[]>;
    public class Handler : IRequestHandler<Request,Employee[]>
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Employee[]> Handle(Request request, CancellationToken cancellationToken)
        {
            var employees =  await _context.Employees.Where(x => x.Id == request.id).ToArrayAsync(cancellationToken);
            return employees;
        }
    }
}



