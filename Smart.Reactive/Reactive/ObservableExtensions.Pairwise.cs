#pragma warning disable SA1649 // File name should match first type name
namespace Smart.Reactive;

// Use case examples:
//
// // 直前の値と現在の値を組み合わせて差分を計算する
// priceStream
//     .Pairwise((prev, curr) => curr - prev)
//     .Subscribe(diff => Console.WriteLine($"差分: {diff}"));
//
// // 直前と現在の値をタプルで受け取る
// valueStream
//     .Pairwise((prev, curr) => (prev, curr))
//     .Subscribe(pair => Console.WriteLine($"{pair.prev} -> {pair.curr}"));
using System.Reactive.Linq;

public static class ObservablePairwiseExtensions
{
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
#pragma warning restore CA1031
}
