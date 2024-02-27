using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using Query;

namespace Query
{
    public interface IFilter<in In>
    {
        bool Filter(In input);
    }

    public interface ITransformation
    {
        public const string TransformInterfaceName = "ITransformation";
        public const string TransformMethodName = "Transform";
        object? TransformDynamic(object input)
        {
            foreach (var impl in GetType().GetInterfaces().Where(impl => impl.Name.StartsWith(TransformInterfaceName) && impl.GenericTypeArguments.Length > 0))
            {
                var method = impl.GetMethod(TransformMethodName, [input?.GetType()]);
                if (method is not null) return method.Invoke(this, [input]);
            }
            throw new System.NotImplementedException($"Cannot perform transformation {GetType()} on type {input?.GetType()}.");
        }
    }

    public interface ITransformation<in In, out Output> : ITransformation
    {
        Output? Transform(In input);
    }

    public partial class Query<Caller, ResultType> : Resource where Caller : GodotObject
    {
        public List<IFilter<ResultType>> Filters = [];
        public List<ITransformation> Transformations = [];

        protected virtual IEnumerable<ResultType> Search(Caller caller) => [];

        public Output? Run<Output>(Caller caller)
        {
            var results = Filters.Aggregate(Search(caller), (current, filter) => current.Where(filter.Filter));
            object transformedResults = results;
            foreach (var transformation in Transformations)
            {
                if (transformation.TransformDynamic(transformedResults) is { } newResults)
                    transformedResults = newResults;
                else break;
            }
            return (Output?)transformedResults;
        }
    }
}