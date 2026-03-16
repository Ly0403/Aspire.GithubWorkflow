using Carter;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Database;
using Shared.Abstracts;
using static OrderApi.Modules.Orders.GetOrderById;

namespace OrderApi.Modules.Orders;

public class CreateOrder
{
    public sealed record CreateOrderCommand(decimal Total): IRequest<Result<Guid>>;

    public sealed class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderValidator()
        {
            RuleFor(x => x.Total).NotEmpty();
        }
    }

    internal sealed class CreateOrderCommandHandler
        (OrderDbContext dbContext)
        : IRequestHandler<CreateOrderCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            Order order = new()
            {
                Total = request.Total,
                Id = Guid.CreateVersion7()
            };

            await dbContext.Orders.AddAsync(order, cancellationToken);

            return new Result<Guid>(order.Id);
        }
    }

    public sealed class CreateOrderEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/", async (CreateOrderCommand request, [FromServices] ISender sender, CancellationToken cancellationToken) =>
            {
                Result<Guid> response = await sender.Send(request, cancellationToken);

                return Results.Ok(
                        new HttpCustomResponse<Guid>
                        {
                            Data = response.Data,
                            Details = response.ErrorMessage,
                            IsSuccess = response.IsSuccess,
                            Status = StatusCodes.Status200OK,
                            Title = "Create Order"
                        }
                    );
            });
        }
    }
}
