namespace Smart.Reactive;

using System.Reactive.Linq;

public static class ObservableObserveOnCurrentContextExtensions
{
    public static IObservable<TSource> ObserveOnCurrentContext<TSource>(this IObservable<TSource> source)
    {
        var context = SynchronizationContext.Current ?? throw new InvalidOperationException("Current synchronization context is null.");
        return source.ObserveOn(context);
    }
}
