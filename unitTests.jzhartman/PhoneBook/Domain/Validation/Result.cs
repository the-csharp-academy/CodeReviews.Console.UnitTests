namespace PhoneBook.Domain.Validation;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public List<Error> Errors { get; }

    protected Result(bool isSuccess, IEnumerable<Error> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors.ToList();
    }

    public static Result Success() => new(true, new List<Error>());
    public static Result Failure(params Error[] errors) => new(false, errors);
    public static Result Failure(IEnumerable<Error> errors) => new(false, errors);
}

public class Result<T> : Result
{
    public T? Value { get; set; }
    private Result(bool isSuccess, T? value, IEnumerable<Error> errors) : base(isSuccess, errors)
    {
        Value = value;
    }

    public static Result<T> Success(T? value) => new(true, value, new List<Error>());
    public static new Result<T> Failure(params Error[] errors) => new(false, default, errors);
    public static new Result<T> Failure(IEnumerable<Error> errors) => new(false, default, errors);
}