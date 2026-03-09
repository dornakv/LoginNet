namespace LoginNet.Domain.Entities
{
    public class User
    {
        public int Id { get; private set; }
        public string Username { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public int RoleId { get; private set; }
        public Role? Role { get; private set; }

        private User() { } // Required for EF Core

        public static User Create(string username, int roleId, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty.", nameof(username));

            return new User
            {
                Username = username,
                RoleId = roleId,
                PasswordHash = passwordHash
            };
        }

        public void UpdatePasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash cannot be empty.", nameof(passwordHash));
            
            PasswordHash = passwordHash;
        }
    }
}
