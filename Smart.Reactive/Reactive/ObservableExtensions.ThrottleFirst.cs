#pragma warning disable SA1649 // File name should match first type name
namespace Smart.Reactive;

// Use case examples:
//
// // ボタン連打防止: 最初のクリックを即時発行し、1秒間は後続を無視する
// buttonClick
//     .ThrottleFirst(TimeSpan.FromSeconds(1))
//     .Subscribe(OnClicked);
//
// // スケジューラ指定あり (テスト用など)
// buttonClick
//     .ThrottleFirst(TimeSpan.FromSeconds(1), TestScheduler)
//     .Subscribe(OnClicked);
//
// Rx 標準の Throttle/Debounce は最後の要素を遅延発行するが、
// ThrottleFirst は最初の要素を即時発行して以降のウィンドウ内要素を無視する。
using System.Reactive.Concurrency;
using System.Reactive.Linq;

public static class ObservableThrottleFirstExtensions
{
    /// <summary>
    /// Emits the first item in each time window and suppresses subsequent items until the window expires.
    /// </summary>
    public static IObservable<TSource> ThrottleFirst<TSource>(
        this IObservable<TSource> source,
        TimeSpan window) =>
        source.ThrottleFirst(window, DefaultScheduler.Instance);

    /// <summary>
    /// Emits the first item in each time window and suppresses subsequent items until the window expires.
    /// </summary>
    public static IObservable<TSource> ThrottleFirst<TSource>(
        this IObservable<TSource> source,
        TimeSpan window,
        IScheduler scheduler)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(scheduler);

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
