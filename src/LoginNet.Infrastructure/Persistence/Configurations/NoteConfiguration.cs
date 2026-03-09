using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LoginNet.Domain.Entities;

namespace LoginNet.Infrastructure.Persistence.Configurations
{
    public class NoteConfiguration : IEntityTypeConfiguration<Note>
    {
        public void Configure(EntityTypeBuilder<Note> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Title).IsRequired();
            builder.Property(e => e.Content).IsRequired();
            builder.Property(e => e.CreatedAt).IsRequired();
            
            builder.Property(e => e.ReadUserAcl).IsRequired();
            builder.Property(e => e.WriteUserAcl).IsRequired();
            builder.Property(e => e.ReadRoleAcl).IsRequired();
            builder.Property(e => e.WriteRoleAcl).IsRequired();
        }
    }
}
