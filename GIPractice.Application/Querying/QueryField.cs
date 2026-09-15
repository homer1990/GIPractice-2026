using System.Linq.Expressions;

namespace GIPractice.Application.Querying;

/// <summary>
/// A canonical semantic field and an expression EF can translate. Reuse the same
/// field through different relationships instead of duplicating DTO-to-entity maps.
/// </summary>
public sealed class QueryField<TEntity, TValue>(
    string name,
    Expression<Func<TEntity, TValue>> selector) : IQueryField<TEntity>
{
    public string Name { get; } = name;
    public Type ValueType => typeof(TValue);
    public Expression<Func<TEntity, TValue>> Selector { get; } = selector;
    public LambdaExpression SelectorExpression => Selector;

    public QueryField<TParent, TValue> Through<TParent>(
        Expression<Func<TParent, TEntity>> relationship)
    {
        var body = new ReplaceExpressionVisitor(
                Selector.Parameters[0],
                relationship.Body)
            .Visit(Selector.Body)
            ?? throw new InvalidOperationException("Could not compose field selector.");

        return new QueryField<TParent, TValue>(
            Name,
            Expression.Lambda<Func<TParent, TValue>>(body, relationship.Parameters[0]));
    }

    private sealed class ReplaceExpressionVisitor(
        Expression source,
        Expression replacement) : ExpressionVisitor
    {
        public override Expression? Visit(Expression? node) =>
            ReferenceEquals(node, source) ? replacement : base.Visit(node);
    }
}
