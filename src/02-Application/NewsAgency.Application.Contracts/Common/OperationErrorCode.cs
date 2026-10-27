namespace NewsAgency.Application.Contracts.Common;

public enum OperationErrorCode
{
    NoError,
    Validation,
    NotFound,
    Conflict,
    LogicError
}