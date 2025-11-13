using SurveyBasket.Api.ValidationAttributes;
using System.ComponentModel.DataAnnotations;

namespace SurveyBasket.Api.Entites;

public class Student
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    [MinAge(26),Display(Name ="Date Of Birth")]
    public DateTime? DateOfBirth { get; set; }
}
