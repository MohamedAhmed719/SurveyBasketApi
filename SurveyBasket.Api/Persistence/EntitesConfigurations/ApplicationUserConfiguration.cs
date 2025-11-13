
using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Persistence.EntitesConfigurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.FirstName).HasMaxLength(100);
        builder.Property(x => x.LastName).HasMaxLength(100);

        builder.OwnsMany(x => x.RefreshTokens).ToTable("RefreshTokens")
            .WithOwner()
            .HasForeignKey("UserId");

        // Default data
        builder.HasData(new ApplicationUser
        {
            Id = DefaultUsers.AdminId,
            FirstName = DefaultUsers.FirstName,
            LastName = DefaultUsers.LastName,
            Email = DefaultUsers.AdminEmail,
            UserName = DefaultUsers.AdminEmail,
            NormalizedEmail = DefaultUsers.AdminEmail.ToUpper(),
            NormalizedUserName = DefaultUsers.AdminEmail.ToUpper(),
            EmailConfirmed = true,
            ConcurrencyStamp = DefaultUsers.AdminConcurrencyStamp,
            SecurityStamp = DefaultUsers.AdminSecurityStamp,
            PasswordHash = DefaultUsers.AdminHashedPassword
        });


    }
}
