using SurveyBasket.Api.Contracts.Questions;
using SurveyBasket.Api.Contracts.Users;

namespace SurveyBasket.Api.Mapping;

public class MappingConfigurations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<QuestionRequest, Question>()
            .Map(dest => dest.Answers, src => src.Answers.Select(answer => new Answer { Content = answer }));

        config.NewConfig<CreateUserRequest, ApplicationUser>()
            .Map(dest => dest.EmailConfirmed, src => true)
            .Map(dest => dest.UserName, src => src.Email);

        config.NewConfig<UpdateUserRequest, ApplicationUser>()
            .Map(dest => dest.NormalizedUserName, src => src.Email.ToUpper())
            .Map(dest => dest.UserName, src => src.Email);
    }
}
