using System.Linq.Expressions;

namespace GIPractice.Application.Querying;

public interface IQueryField<TEntity>
{
    string Name { get; }
    Type ValueType { get; }
    LambdaExpression SelectorExpression { get; }
}
