using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResultFramework.Builder;

namespace ResultFramework
{
    public static class AttemptBuilder
    {
        public static ITaskBuilder<TValue> Run<TValue>(Func<TValue> method)
        {
            return new RunTaskBuilder<TValue>(method);
        }

        public static ITaskBuilder<TValue> Run<TValue>(Func<Task<TValue>> methodAsync)
        {
            return new RunTaskBuilder<TValue>(methodAsync);
        }

        public static ITaskBuilder<TNextValue> Run<TPreviousValue, TNextValue>(ITaskBuilder<TPreviousValue> previousTaskBuilder, Func<TNextValue> method)
        {
            return new RunTaskBuilder<TPreviousValue, TNextValue>(previousTaskBuilder, method);
        }

        public static ITaskBuilder<TPreviousValue, TNextValue> ErrorWhen<TPreviousValue, TNextValue, TError>(
            this ITaskBuilder<TPreviousValue> previousTaskBuilder,
            Func<TPreviousValue, Task<bool>> predicateAsync,
            Func<TPreviousValue, Task<TNextValue>> onSuccess,
            Func<TPreviousValue, Task<TError>> onError,
            string errorMessage = null)
        {
            return new ErrorWhenTaskBuilder<TPreviousValue, TNextValue, TError>(previousTaskBuilder, predicateAsync, onSuccess, onError, errorMessage);
        }
        public static ITaskBuilder<TPreviousValue> ErrorWhen<TPreviousValue, TError>(
           this ITaskBuilder<TPreviousValue> previousTaskBuilder,
           Func<TPreviousValue, Task<bool>> predicateAsync,
           Func<TPreviousValue, Task<TError>> onError,
           string errorMessage = null)
        {
            return new ErrorWhenTaskBuilder<TPreviousValue, TError>(previousTaskBuilder, predicateAsync, onError, errorMessage);
        }

        public static ITaskBuilder<TPreviousValue, TNextValue> ErrorWhen<TPreviousValue, TNextValue, TError>(
            this ITaskBuilder<TPreviousValue> previousTaskBuilder,
            Func<TPreviousValue, bool> predicate,
            Func<TPreviousValue, Task<TNextValue>> onSuccess,
            Func<TPreviousValue, Task<TError>> onError,
            string errorMessage = null)
        {
            return new ErrorWhenTaskBuilder<TPreviousValue, TNextValue, TError>(previousTaskBuilder, predicate, onSuccess, onError, errorMessage);
        }

        public static ITaskBuilder<TPreviousValue> ErrorWhen<TPreviousValue, TError>(
            this ITaskBuilder<TPreviousValue> previousTaskBuilder,
            Func<TPreviousValue, bool> predicate,
            Func<TPreviousValue, Task<TError>> onError,
            string errorMessage = null)
        {
            return new ErrorWhenTaskBuilder<TPreviousValue, TError>(previousTaskBuilder, predicate, onError, errorMessage);
        }

        public static ITaskBuilder<TPreviousValue, TNextValue> Then<TPreviousValue, TNextValue>(ITaskBuilder<TPreviousValue> previousTaskBuilder, Func<TPreviousValue, TNextValue> method)
        {
            return new RunTaskBuilder<TPreviousValue, TNextValue>(previousTaskBuilder, method);
        }

        public static ITaskBuilder<TPreviousValue, TNextValue> Then<TPreviousValue, TNextValue>(ITaskBuilder<TPreviousValue> previousTaskBuilder, Func<TPreviousValue, Task<TNextValue>> method)
        {
            return new RunTaskBuilder<TPreviousValue, TNextValue>(previousTaskBuilder, method);
        }
    }
}
