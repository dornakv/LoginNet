using LoginNet.Application.UseCases.Roles;
using LoginNet.Contracts.DTOs;
using AppEnums = LoginNet.Application.Common.Enums;
using LoginNet.Application.Common.Interfaces;
using LoginNet.WebApi.Mappers;

namespace LoginNet.WebApi.Endpoints
{
    public static class RoleEndpoints
    {
        public static RouteGroupBuilder MapRoleEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/role");

            group.MapPost("", async (RolePostDTO req, IMediator mediator) =>
            {
                var result = await mediator.Send(new CreateRoleCommand(req.Name, req.CanRegisterUsers, req.CanCreateRoles, req.OwnerId));

                if (!result.IsSuccess)
                {
                    if (result.ErrorCode is AppEnums.RoleCreationError roleError)
                    {
                        return roleError switch
                        {
                            AppEnums.RoleCreationError.UserNotFound => Results.Forbid(),
                            AppEnums.RoleCreationError.InsufficientPermissions => Results.Forbid(),
                            AppEnums.RoleCreationError.InvalidName => Results.BadRequest(result.ErrorMessage),
                            AppEnums.RoleCreationError.DuplicateName => Results.BadRequest(result.ErrorMessage),
                            _ => Results.BadRequest(result.ErrorMessage ?? "An unexpected error occurred.")
                        };
                    }
                    return Results.BadRequest(result.ErrorMessage ?? "An unexpected error occurred.");
                }

                return Results.Created($"/role/{result.Value!.Id}", result.Value.ToGetDTO());
            });

            group.MapGet("/{id:int}", async (int id, IMediator mediator) =>
            {
                var result = await mediator.Send(new GetRoleByIdQuery(id));
                if (!result.IsSuccess)
                    return Results.NotFound();
                return Results.Ok(result.Value?.ToGetDTO());
            });

            return group;
        }
    }
}
