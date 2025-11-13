
using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Persistence.EntitesConfigurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {
        //Default data

        builder.HasData(new IdentityUserRole<string>
        {
             UserId = DefaultUsers.AdminId,
             RoleId = DefaultRoles.AdminRoleId
        });
    }
}
