namespace LoginNet.Application.Common.Enums
{
    public enum RegistrationError
    {
        InvalidUsernameOrPassword,
        UsernameAlreadyExists,
        RoleRequired,
        RoleNotFound,
        InsufficientPermissions,
        InvalidRoleId
    }
}
