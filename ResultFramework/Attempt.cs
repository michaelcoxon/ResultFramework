using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultFramework
{
    public static class Attempt
    {
        public static IResult<TValue> Run<TValue>(Func<TValue> func)
        {
            try
            {
                return new SuccessResult<TValue>(func());
            }
            catch (Exception ex)
            {
                return new ErrorResult<TValue, Exception>(ex);
            }
        }

        public static IResult<TValue> Run<TValue, TPreviousValue>(Func<TValue> func, IResult<TPreviousValue> previousResult)
        {
            try
            {
                return new SuccessResult<TValue>(func(), previousResult);
            }
            catch (Exception ex)
            {
                return new ErrorResult<TValue, Exception>(ex, previousResult: previousResult);
            }
        }

        public static IResult<TReturnValue> Then<TValue, TReturnValue>(this IResult<TValue> previousResult, Func<IResult<TValue>, TReturnValue> next)
        {
            if (previousResult.Success)
            {
                return Run(() => next(previousResult), previousResult);
            }
            else
            {
                if (previousResult is IErrorResult errorResult)
                {
                    return (IResult<TReturnValue>)errorResult;
                }
                else
                {
                    return new ErrorResult<TReturnValue, Exception>(new NotSupportedException("Setting succes true and having an error is not supported"));
                }
            }
        }

        public static IResult<TResultValue> ErrorIf<TResultValue, TValue, TError>(
            this IResult<TValue> previousResult,
            Func<TValue, bool> predicate,
            Func<TValue, TResultValue> success,
            Func<TValue, TError> error,
            string errorMessage = null)
        {
            if (previousResult.Success && predicate(previousResult.Value))
            {
                return new SuccessResult<TResultValue>(success(previousResult.Value), previousResult);
            }
            else
            {
                return new ErrorResult<TResultValue, TError>(error(previousResult.Value), errorMessage, false, previousResult);
            }
        }

        public static async Task<IResult<TValue>> Run<TValue>(Func<Task<TValue>> func)
        {
            try
            {
                return new SuccessResult<TValue>(await func());
            }
            catch (Exception ex)
            {
                return new ErrorResult<TValue, Exception>(ex);
            }
        }

        public static async Task<IResult<TValue>> Run<TValue, TPreviousValue>(Func<Task<TValue>> func, Task<IResult<TPreviousValue>> previousResult)
        {
            try
            {
                return new SuccessResult<TValue>(await func(), await previousResult);
            }
            catch (Exception ex)
            {
                return new ErrorResult<TValue, Exception>(ex, previousResult: await previousResult);
            }
        }


        public static async Task<IResult<TReturnValue>> Then<TValue, TReturnValue>(this Task<IResult<TValue>> previousResultTask, Func<IResult<TValue>, Task<TReturnValue>> next)
        {
            var previousResult = await previousResultTask;
            if (previousResult.Success)
            {
                return await Run(async () => await next(previousResult));
            }
            else
            {
                if (previousResult is IErrorResult errorResult)
                {
                    return (IResult<TReturnValue>)errorResult;
                }
                else
                {
                    return new ErrorResult<TReturnValue, Exception>(new NotSupportedException("Setting succes true and having an error is not supported"));
                }
            }
        }

        public static async Task<IResult<TResultValue>> ErrorWhen<TResultValue, TValue, TError>(
            this Task<IResult<TValue>> previousResultTask,
            Func<TValue, Task<bool>> predicate,
            Func<TValue, Task<TResultValue>> success,
            Func<TValue, Task<TError>> error,
            string errorMessage = null)
        {
            var previousResult = await previousResultTask;

            if (previousResult.Success && await predicate(previousResult.Value))
            {
                return new SuccessResult<TResultValue>(await success(previousResult.Value), previousResult);
            }
            else
            {
                return new ErrorResult<TResultValue, TError>(await error(previousResult.Value), errorMessage, false, previousResult);
            }
        }
    }
}
