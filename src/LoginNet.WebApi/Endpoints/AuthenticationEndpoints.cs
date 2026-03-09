using LoginNet.Application.UseCases.Auth;
using LoginNet.Contracts.DTOs;
using AppEnums = LoginNet.Application.Common.Enums;
using LoginNet.Application.Common.Interfaces;
using LoginNet.WebApi.Mappers;

namespace LoginNet.WebApi.Endpoints
{
    public static class AuthenticationEndpoints
    {
        public static RouteGroupBuilder MapAuthenticationEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/auth");
            
            group.MapPost("/register", async (RegisterPostDTO req, IMediator mediator) =>
            {
                var result = await mediator.Send(new RegisterUserCommand(req.Username, req.Password, req.RoleId));
                
                if (!result.IsSuccess)
                {
                    if (result.ErrorCode is AppEnums.RegistrationError regError)
                    {
                        return regError switch
                        {
                            AppEnums.RegistrationError.InvalidUsernameOrPassword => Results.BadRequest(result.ErrorMessage),
                            AppEnums.RegistrationError.UsernameAlreadyExists => Results.BadRequest(result.ErrorMessage),
                            AppEnums.RegistrationError.RoleRequired => Results.BadRequest(result.ErrorMessage),
                            AppEnums.RegistrationError.InvalidRoleId => Results.BadRequest(result.ErrorMessage),
                            AppEnums.RegistrationError.InsufficientPermissions => Results.Forbid(),
                            _ => Results.BadRequest(result.ErrorMessage ?? "An unexpected error occurred.")
                        };
                    }
                    return Results.BadRequest(result.ErrorMessage ?? "An unexpected error occurred.");
                }
                
                return Results.Ok(result.Value!.ToGetDTO());
            });

            group.MapPost("/login", async (LoginPostDTO req, IMediator mediator) =>
            {
                var result = await mediator.Send(new LoginUserCommand(req.Username, req.Password));
                if (!result.IsSuccess)
                    return Results.Unauthorized();
                return Results.Ok(new { token = result.Value });
            });

            return group;
        }
    }
}
