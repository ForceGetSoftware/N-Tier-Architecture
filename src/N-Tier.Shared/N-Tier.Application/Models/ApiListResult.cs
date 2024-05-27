namespace N_Tier.Application.Models;

public class ApiListResult<T>
{
    private ApiListResult()
    {
    }
    
    private ApiListResult(bool succeeded, T result, int total, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Result = result;
        Total = total;
        Errors = errors;
    }
    
    public bool Succeeded { get; set; }
    
    public T Result { get; set; }
    public int Total { get; set; }
    
    public IEnumerable<string> Errors { get; set; }
    
    public static ApiListResult<T> Success(T result, int total)
    {
        return new ApiListResult<T>(true, result, total, new List<string>());
    }
    
    public static ApiListResult<T> Failure(IEnumerable<string> errors)
    {
        return new ApiListResult<T>(false, default, -1, errors);
    }
}
