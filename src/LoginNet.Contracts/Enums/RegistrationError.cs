namespace LoginNet.Contracts.Enums
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
