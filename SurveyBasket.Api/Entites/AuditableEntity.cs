namespace SurveyBasket.Api.Entites;

public class AuditableEntity
{
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime ?UpdatedOn { get; set; }
    public string? UpdatedById { get; set; }
    public string CreatedById { get; set; } = string.Empty;
    public ApplicationUser CreatedBy { get; set; } = default!;
    public ApplicationUser UpdatedBy { get; set; } = default!;
}
