namespace Smart.Reactive;

using System.Reactive.Linq;
using System.Runtime.CompilerServices;

public static class ObservableWhereNotNullExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IObservable<TSource> WhereNotNull<TSource>(this IObservable<TSource?> source)
        where TSource : class =>
        source.Where(static x => x is not null)!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IObservable<TSource> WhereNotNull<TSource>(this IObservable<TSource?> source)
        where TSource : struct =>
        source.Where(static x => x.HasValue).Select(static x => x!.Value);
}
