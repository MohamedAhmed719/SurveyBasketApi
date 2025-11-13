namespace SurveyBasket.Api.Errors;

public static class QuestionErrors
{
    public static readonly Error DuplicatedQuestionContent = new("Question.DuplicatedContent", "Another question with the same content is already exists", StatusCodes.Status409Conflict);
    public static readonly Error QuestionNotFound = new("Question.QuestionNotFound", "Question not found.", StatusCodes.Status404NotFound);
}
