
using SurveyBasket.Api.Abstractions.Consts;

namespace SurveyBasket.Api.Persistence.EntitesConfigurations;

public class RoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {

        //Default data

        builder.HasData(
            [
            new ApplicationRole{
                Id = DefaultRoles.AdminRoleId,
                Name = DefaultRoles.Admin,
                ConcurrencyStamp = DefaultRoles.AdminRoleConcurrencyStamp,
                NormalizedName = DefaultRoles.Admin.ToUpper()
            },

            new ApplicationRole{
                Id = DefaultRoles.MemberRoleId,
                Name = DefaultRoles.Member,
                ConcurrencyStamp = DefaultRoles.MemberRoleConcurrencyStamp,
                NormalizedName = DefaultRoles.Member.ToUpper(),
                IsDefault = true
            }

            ]);
    }
}
