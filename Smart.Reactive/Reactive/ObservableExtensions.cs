namespace Smart.Reactive;

using System.Reactive.Linq;
using System.Runtime.CompilerServices;

public static class ObservableExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IObservable<TSource> WhereNotNull<TSource>(this IObservable<TSource?> source)
        where TSource : class =>
        source.Where(static x => x is not null)!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IObservable<TSource> WhereNotNull<TSource>(this IObservable<TSource?> source)
        where TSource : struct =>
        source.Where(static x => x.HasValue).Select(static x => x!.Value);

    public static IObservable<TSource> ObserveOnCurrentContext<TSource>(this IObservable<TSource> source) =>
        source.ObserveOn(SynchronizationContext.Current!);

#pragma warning disable CA1031
    public static IObservable<TResult> Pairwise<T, TResult>(this IObservable<T> source, Func<T, T, TResult> selector)
    {
        return Observable.Create<TResult>(observer =>
        {
            var prev = default(T);
            var isFirst = true;

            return source.Subscribe(x =>
            {
                if (isFirst)
                {
                    isFirst = false;
                    prev = x;
                }
                else
                {
                    TResult value;
                    try
                    {
                        value = selector(prev!, x);
                        prev = x;
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                        return;
                    }

                    observer.OnNext(value);
                }
            }, observer.OnError, observer.OnCompleted);
        });
    }
#pragma warning disable CA1031
}
