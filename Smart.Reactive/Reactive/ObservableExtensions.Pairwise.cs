namespace Smart.Reactive;

using System.Reactive.Disposables;
using System.Reactive.Linq;

public static class ObservablePairwiseExtensions
{
#pragma warning disable CA1031
    public static IObservable<TResult> Pairwise<T, TResult>(this IObservable<T> source, Func<T, T, TResult> selector)
    {
        return Observable.Create<TResult>(observer =>
        {
            var subscription = new SingleAssignmentDisposable();
            var prev = default(T);
            var isFirst = true;

            subscription.Disposable = source.Subscribe(
                x =>
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
                            subscription.Dispose();
                            observer.OnError(ex);
                            return;
                        }

                        observer.OnNext(value);
                    }
                },
                observer.OnError,
                observer.OnCompleted);

            return subscription;
        });
    }
#pragma warning restore CA1031
}
