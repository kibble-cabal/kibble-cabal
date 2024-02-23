using System.Collections.Generic;
using System.Linq;
using Godot;
using Query;

namespace Query
{
    public interface IFilter<Input>
    {
        bool Filter(Input input);
    }

    public interface ITransformation<Input, Output>
    {
        Output Transform(Input input);
    }

    public partial class Query<Caller, ResultType> : Resource where Caller : GodotObject
    {
        public List<IFilter<ResultType>> Filters = [];
        public List<ITransformation<dynamic, dynamic>> Transformations = [];

        public virtual IEnumerable<ResultType> Search(Caller caller) => [];

        public virtual Output Run<Output>(Caller caller)
        {
            var results = Search(caller);
            foreach (var filter in Filters)
                results = results.Where(filter.Filter);
            object transformedResults = results;
            foreach (var transformation in Transformations)
                transformedResults = transformation.Transform(results);
            return (Output)transformedResults;
        }
    }
}

public static class QueryExtensions
{
    public static ITransformation<dynamic, dynamic> AsDynamic<Input, Output>(this ITransformation<Input, Output> transformation) => (transformation as ITransformation<dynamic, dynamic>)!;
}