namespace NewsAgency.Application.Contracts.Common;

public class OperationResult
{
    public bool IsSuccess { get; private set; }
    public string Message { get; private set; }
    public OperationErrorCode ErrorCode { get; private set; }


    public OperationResult Succeeded(string message = "Operation successfully done!")
    {
        IsSuccess = true;
        Message = message;
        ErrorCode = OperationErrorCode.NoError;
        return this;
   }
    public OperationResult Failed(OperationErrorCode errorCode, string message = "Operation failed! try again.")
    {
        IsSuccess = false;
        if (errorCode == OperationErrorCode.NoError)
        {
            throw new ArgumentOutOfRangeException(
                nameof(errorCode),
                "A failed operation must have an error code!");
        }
        ErrorCode = errorCode;
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

    public new OperationResult<TData> Failed(OperationErrorCode errorCode, 
        string message = "Operation failed! Try again.")
    {
        base.Failed(errorCode, message);
        Data = default;
        return this;
    }
}