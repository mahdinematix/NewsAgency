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

public class OperationResult<TData> : OperationResult
{
    public TData? Data { get; private set; }

    public OperationResult<TData> Succeeded(
        TData data,
        string message = "Operation successfully done!")
    {
        base.Succeeded(message);
        Data = data;
        return this;
    }

    public new OperationResult<TData> Failed(
        string message = "Operation failed! Try again.")
    {
        base.Failed(message);
        Data = default;
        return this;
    }
}