namespace MsHuyenLC.Application.Exceptions;

public class ValidationException : Exception
{
    public List<string> Errors { get; }

    public ValidationException(List<string> errors) 
        : base("Validation failed")
    {
        Errors = errors;
    }

    public ValidationException(string error) 
        : base(error)
    {
        Errors = new List<string> { error };
    }
}
