using LoginNet.Infrastructure.Persistence;
using LoginNet.Domain.Entities;
using LoginNet.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LoginNet.Tests.Helpers
{
    /// <summary>
    /// Base class for database tests with in-memory database
    /// </summary>
    public class DatabaseTestBase : IDisposable
    {
        protected readonly AppDbContext _db;
        protected readonly EfUserRepository _userRepository;
        protected readonly EfRoleRepository _roleRepository;
        protected readonly EfNoteRepository _noteRepository;

        public DatabaseTestBase()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _db = new AppDbContext(options);
            _userRepository = new EfUserRepository(_db);
            _roleRepository = new EfRoleRepository(_db);
            _noteRepository = new EfNoteRepository(_db);
        }

        /// <summary>
        /// Create a role in the database
        /// </summary>
        protected async Task<Role> CreateRoleAsync(string name, bool canRegisterUsers = false, bool canCreateRoles = false, int? ownerId = null)
        {
            var role = new Role
            {
                Name = name,
                CanRegisterUsers = canRegisterUsers,
                CanCreateRoles = canCreateRoles,
                OwnerId = ownerId
            };
            _db.Roles.Add(role);
            await _db.SaveChangesAsync();
            return role;
        }

        /// <summary>
        /// Create a user in the database
        /// </summary>
        protected async Task<User> CreateUserAsync(string username, int roleId, string password = "Password123!")
        {
            var user = User.Create(username, roleId, password);
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            
            // Reload with role
            await _db.Entry(user).Reference(u => u.Role).LoadAsync();
            return user;
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
