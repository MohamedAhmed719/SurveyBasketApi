
using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Persistence.EntitesConfigurations;

public class RoleClaimsConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<string>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
    {
        var adminClaims = new List<IdentityRoleClaim<string>>();

        var allPermissions = DefaultPermissions.GetAllPermissions();

        for(int i = 0; i < allPermissions.Count; i++)
        {
            adminClaims.Add(new IdentityRoleClaim<string>
            {
                Id = i + 1,
                ClaimType = DefaultPermissions.Type,
                ClaimValue = allPermissions[i],
                RoleId = DefaultRoles.AdminRoleId
            });
        }

        builder.HasData(adminClaims);
        
    }
}
