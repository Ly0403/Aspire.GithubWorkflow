using Carter;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderApi.Database;
using Shared.Abstracts;

namespace OrderApi.Modules.Orders;

public class GetOrderById
{
    public sealed record GetOrderByIdRequest(Guid Id): IRequest<Result<OrderResponse>>;

    public sealed record OrderResponse(Guid Id, decimal Total);

    public sealed class GetOrderByIdValidator : AbstractValidator<GetOrderByIdRequest>
    {
        public GetOrderByIdValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    internal sealed class GetOrderByIdRequestHandler 
        (OrderDbContext dbContext)
        : IRequestHandler<GetOrderByIdRequest, Result<OrderResponse>>
    {
        public async Task<Result<OrderResponse>> Handle(GetOrderByIdRequest request, CancellationToken cancellationToken)
        {
            Order? order = await dbContext.Orders.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (order is null)
                return new Result<OrderResponse>($"Order with id {request.Id} not found");

            OrderResponse response = new(order.Id, order.Total);

            return new Result<OrderResponse>(response);
        }
    }

    public sealed class GetOrderByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/{id}", async (Guid id, [FromServices] ISender sender, CancellationToken cancellationToken) =>
            {
                Result<OrderResponse> response = await sender.Send(new GetOrderByIdRequest(id),cancellationToken);

                if (response.IsFailure)
                    return Results.NotFound(
                        new HttpCustomResponse<OrderResponse>
                        {
                            Details = response.ErrorMessage,
                            IsSuccess = response.IsSuccess,
                            Status = StatusCodes.Status404NotFound,
                            Title = "Error"
                        }
                    );

                return Results.Ok(
                        new HttpCustomResponse<OrderResponse>
                        {
                            Data = response.Data,
                            Details = response.ErrorMessage,
                            IsSuccess = response.IsSuccess,
                            Status = StatusCodes.Status200OK,
                            Title = "Order"
                        }
                    );
            });
        }
    }
}
