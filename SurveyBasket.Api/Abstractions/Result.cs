namespace SurveyBasket.Api.Abstractions;

public class Result
{
    public Error Error { get; } = default!;
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Result(bool isSuccess, Error error)
    {
        if ((isSuccess && error != Error.none) || (!isSuccess && error == Error.none))
            throw new InvalidOperationException("");
        Error = error;
        IsSuccess = isSuccess;
    }

    public static Result Success() => new(true, Error.none);
    public static Result Failure(Error error) => new(false, error);
    public static Result<Tvalue> Success<Tvalue>(Tvalue value) => new(value, true, Error.none);
    public static Result<Tvalue> Failure<Tvalue>(Error error) => new(default, false, error);
}

public class Result<Tvalue> : Result
{
    private readonly Tvalue? _value;
    public Result(Tvalue? value,bool isSuccess,Error error):base(isSuccess,error)
    {
        _value = value;
    }

    public Tvalue? Value => IsSuccess ?
        _value : throw new InvalidOperationException("Failure results can't have value");
}
