using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ResultFramework.Builder
{
    public class RunTaskBuilder<TNextValue> : ITaskBuilder<TNextValue>
    {
        private readonly Func<Task<TNextValue>> _methodAsync;

        public RunTaskBuilder(Func<TNextValue> method)
        {
            this._methodAsync = () => Task.Run(method);
        }

        public RunTaskBuilder(Func<Task<TNextValue>> methodAsync)
        {
            this._methodAsync = methodAsync;
        }

        public async Task<IResult<TNextValue>> ExecuteAsync()
        {
            try
            {
                return new SuccessResult<TNextValue>(await this._methodAsync());
            }
            catch (Exception ex)
            {
                return new ErrorResult<TNextValue, Exception>(ex);
            }
        }
    }

    public class RunTaskBuilder<TPreviousValue, TNextValue> : ITaskBuilder<TPreviousValue, TNextValue>
    {
        private readonly ITaskBuilder<TPreviousValue> _previousTaskBuilder;
        private readonly Func<TPreviousValue, Task<TNextValue>> _methodAsync;

        public RunTaskBuilder(ITaskBuilder<TPreviousValue> previousTaskBuilder, Func<TNextValue> method)
        {
            this._previousTaskBuilder = previousTaskBuilder;
            this._methodAsync = (p) => Task.Run(method);
        }

        public RunTaskBuilder(ITaskBuilder<TPreviousValue> previousTaskBuilder, Func<Task<TNextValue>> methodAsync)
        {
            this._previousTaskBuilder = previousTaskBuilder;
            this._methodAsync = (p) => methodAsync();
        }

        public RunTaskBuilder(ITaskBuilder<TPreviousValue> previousTaskBuilder, Func<TPreviousValue, TNextValue> method)
        {
            this._previousTaskBuilder = previousTaskBuilder;
            this._methodAsync = (p) => Task.Run(() => method(p));
        }

        public RunTaskBuilder(ITaskBuilder<TPreviousValue> previousTaskBuilder, Func<TPreviousValue, Task<TNextValue>> methodAsync)
        {
            this._previousTaskBuilder = previousTaskBuilder;
            this._methodAsync = methodAsync;
        }

        public async Task<IResult<TNextValue>> ExecuteAsync()
        {
            var previousResult = await this._previousTaskBuilder.ExecuteAsync();

            try
            {
                return new SuccessResult<TNextValue>(await this._methodAsync(previousResult.Value));
            }
            catch (Exception ex)
            {
                return new ErrorResult<TNextValue, Exception>(ex);
            }
        }
    }
}
