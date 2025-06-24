using System.Collections.Immutable;
using DotMaybe;

namespace ExpenseExplorer.Application;

public static class MaybeExtensions
{
    public static Maybe<ImmutableArray<T>> TraverseMaybeToImmutableArray<T>(this IEnumerable<Maybe<T>> source)
        => source.Aggregate(
                Some.With(Enumerable.Empty<T>()),
                (acc, opt) => acc.Bind(accList => opt.Map(accList.Append)))
            .Map(col => col.ToImmutableArray());

    public static Maybe<ImmutableList<T>> TraverseMaybeToImmutableList<T>(this IEnumerable<Maybe<T>> source)
        => source.Aggregate(
                Some.With(Enumerable.Empty<T>()),
                (acc, opt) => acc.Bind(accList => opt.Map(accList.Append)))
            .Map(col => col.ToImmutableList());
}