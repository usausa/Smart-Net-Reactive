#pragma warning disable SA1649 // File name should match first type name
namespace Smart.Reactive;

// Use case examples:
//
// // null を除外して非null型のストリームを得る (class)
// IObservable<string?> maybeStrings = ...;
// IObservable<string> strings = maybeStrings.WhereNotNull();
//
// // null を除外して値型のストリームを得る (struct)
// IObservable<int?> maybeInts = ...;
// IObservable<int> ints = maybeInts.WhereNotNull();
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
