using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultFramework
{
    public abstract class Result<TValue> : IResult<TValue>
    {
        public Result(TValue value, bool success, IResult previousResult = null)
        {
            this.Value = value;
            this.Success = success;
            this.PreviousResult = previousResult;
        }

        public IResult PreviousResult { get; private set; }
        public TValue Value { get; private set; }
        public bool Success { get; private set; }
        object IResult.Value => this.Value;

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }

    public sealed class SuccessResult<TValue> : Result<TValue>, IResult<TValue>
    {
        public SuccessResult(TValue value, IResult previousResult = null) : base(value, true, previousResult) { }
    }

    public sealed class ErrorResult<TResultValue, TError> : Result<TResultValue>, IErrorResult<TResultValue, TError>
    {
        public ErrorResult(TError error, string errorMessage = null, bool canRetry = false, IResult previousResult = null)
            : base(default, false, previousResult)
        {
            this.Error = error;
            this.ErrorMessage = errorMessage;
            this.CanRetry = canRetry;
        }

        public TError Error { get; private set; }
        public string ErrorMessage { get; private set; }
        public bool CanRetry { get; private set; }
        object IErrorResult.Error => this.Error;

        public override string ToString()
        {
            return this.ErrorMessage ?? this.Error.ToString();
        }
    }
}
