namespace NewsAgency.Application.Contracts.Common;

public class OperationResult
{
    public bool IsSuccess { get; private set; }
    public string Message { get; private set; }


    public OperationResult Succeeded(string message = "Operation successfully done!")
    {
        IsSuccess = true;
        Message = message;
        return this;
   }
    public OperationResult Failed(string message = "Operation failed! try again.")
    {
        IsSuccess = false;
        Message = message;
        return this;
    }
}

