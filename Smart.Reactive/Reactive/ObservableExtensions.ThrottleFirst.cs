namespace Smart.Reactive;

using System.Reactive.Concurrency;
using System.Reactive.Linq;

public static class ObservableThrottleFirstExtensions
{
    public static IObservable<TSource> ThrottleFirst<TSource>(
        this IObservable<TSource> source,
        TimeSpan window) =>
        source.ThrottleFirst(window, DefaultScheduler.Instance);

    public static IObservable<TSource> ThrottleFirst<TSource>(
        this IObservable<TSource> source,
        TimeSpan window,
        IScheduler scheduler)
    {
        return Observable.Create<TSource>(observer =>
        {
            var gate = new object();
            var open = true;

            return source.Subscribe(
                value =>
                {
                    bool emit;
                    lock (gate)
                    {
                        emit = open;
                        if (open)
                        {
                            open = false;
                        }
                    }

                    if (!emit)
                    {
                        return;
                    }

                    observer.OnNext(value);

                    scheduler.Schedule(window, () =>
                    {
                        lock (gate)
                        {
                            open = true;
                        }
                    });
                },
                observer.OnError,
                observer.OnCompleted);
        });
    }
}
