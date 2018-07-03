using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultFramework
{
    public interface IResult
    {
        IResult PreviousResult { get; }

        object Value { get; }

        bool Success { get; }
    }

    public interface IErrorResult : IResult
    {
        string ErrorMessage { get; }

        object Error { get; }

        bool CanRetry { get; }
    }

    public interface IResult<out TValue> : IResult
    {
        new TValue Value { get; }
    }

    public interface IResult<out TValue, out TPreviousResult> : IResult<TValue>
    {
        new TPreviousResult PreviousResult { get; }

        new TValue Value { get; }
    }

    public interface IErrorResult<out TResultValue, out TError> : IErrorResult, IResult<TResultValue>
    {
        new TError Error { get; }
    }
}
