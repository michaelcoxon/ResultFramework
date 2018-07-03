using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultFramework
{
    public class ErrorWhenTaskBuilder<TPreviousValue, TNextValue, TError> : ITaskBuilder<TPreviousValue, TNextValue>
    {
        private readonly ITaskBuilder<TPreviousValue> _previousTaskBuilder;
        private readonly Func<TPreviousValue, Task<bool>> _predicateAsync;
        private readonly Func<TPreviousValue, Task<TNextValue>> _onSuccess;
        private readonly Func<TPreviousValue, Task<TError>> _onError;
        private readonly string _errorMessage;

        public ErrorWhenTaskBuilder(
            ITaskBuilder<TPreviousValue> previousTaskBuilder,
            Func<TPreviousValue, Task<bool>> predicateAsync,
            Func<TPreviousValue, Task<TNextValue>> onSuccess,
            Func<TPreviousValue, Task<TError>> onError,
            string errorMessage = null)
        {
            this._previousTaskBuilder = previousTaskBuilder;
            this._predicateAsync = predicateAsync;
            this._onSuccess = onSuccess;
            this._onError = onError;
            this._errorMessage = errorMessage;
        }
        public ErrorWhenTaskBuilder(
            ITaskBuilder<TPreviousValue> previousTaskBuilder,
            Func<TPreviousValue, bool> predicate,
            Func<TPreviousValue, Task<TNextValue>> onSuccess,
            Func<TPreviousValue, Task<TError>> onError,
            string errorMessage = null)
        {
            this._previousTaskBuilder = previousTaskBuilder;
            this._predicateAsync = (p) => Task.Run(() => predicate(p));
            this._onSuccess = onSuccess;
            this._onError = onError;
            this._errorMessage = errorMessage;
        }

        public async Task<IResult<TNextValue>> ExecuteAsync()
        {
            var previousResult = await this._previousTaskBuilder.ExecuteAsync();

            if (previousResult.Success && await this._predicateAsync(previousResult.Value))
            {
                return new ErrorResult<TNextValue, TError>(await this._onError(previousResult.Value), this._errorMessage, false, previousResult);
            }
            else
            {
                return new SuccessResult<TNextValue>(await this._onSuccess(previousResult.Value), previousResult);
            }
        }
    }

    public class ErrorWhenTaskBuilder<TPreviousValue, TError> : ITaskBuilder<TPreviousValue>
    {
        private readonly ITaskBuilder<TPreviousValue> _previousTaskBuilder;
        private readonly Func<TPreviousValue, Task<bool>> _predicateAsync;
        private readonly Func<TPreviousValue, Task<TError>> _onError;
        private readonly string _errorMessage;

        public ErrorWhenTaskBuilder(
            ITaskBuilder<TPreviousValue> previousTaskBuilder,
            Func<TPreviousValue, Task<bool>> predicateAsync,
            Func<TPreviousValue, Task<TError>> onError,
            string errorMessage = null)
        {
            this._previousTaskBuilder = previousTaskBuilder;
            this._predicateAsync = predicateAsync;
            this._onError = onError;
            this._errorMessage = errorMessage;
        }
        public ErrorWhenTaskBuilder(
            ITaskBuilder<TPreviousValue> previousTaskBuilder,
            Func<TPreviousValue, bool> predicate,
            Func<TPreviousValue, Task<TError>> onError,
            string errorMessage = null)
        {
            this._previousTaskBuilder = previousTaskBuilder;
            this._predicateAsync = (p) => Task.Run(() => predicate(p));
            this._onError = onError;
            this._errorMessage = errorMessage;
        }

        public async Task<IResult<TPreviousValue>> ExecuteAsync()
        {
            var previousResult = await this._previousTaskBuilder.ExecuteAsync();

            if (previousResult.Success && await this._predicateAsync(previousResult.Value))
            {
                return new ErrorResult<TPreviousValue, TError>(await this._onError(previousResult.Value), this._errorMessage, false, previousResult);
            }
            else
            {
                return previousResult;
            }
        }
    }
}
