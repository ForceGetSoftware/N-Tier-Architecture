namespace N_Tier.Application.Models;

public class ApiListResponse<T>
{
    private ApiListResponse()
    {
    }
    
    private ApiListResponse(bool succeeded, List<T> result, dynamic total, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Result = result;
        Total = total;
        Errors = errors;
    }
    
    public bool Succeeded { get; set; }
    
    public List<T> Result { get; set; }
    public dynamic Total { get; set; }
    
    public IEnumerable<string> Errors { get; set; }
    
    public static ApiListResponse<T> Success(List<T> result, dynamic total)
    {
        return new ApiListResponse<T>(true, result, total, new List<string>());
    }
    
    public static ApiListResponse<T> Failure(IEnumerable<string> errors)
    {
        return new ApiListResponse<T>(false, default, -1, errors);
    }
}
