using System.Threading.Tasks;

namespace ResultFramework
{
    public interface ITaskBuilder<TNextValue>
    {
        Task<IResult<TNextValue>> ExecuteAsync();
    }

    public interface ITaskBuilder<TPreviousValue, TNextValue> : ITaskBuilder<TNextValue>
    {
    }
}