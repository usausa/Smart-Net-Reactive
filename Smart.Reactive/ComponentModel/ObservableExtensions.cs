namespace Smart.ComponentModel;

using System;

public static class ObservableExtensions
{
    public static IDisposable SubscribeValue<T>(
        this IObservable<T> observable,
        IValueHolder<T> holder)
    {
        return observable.Subscribe(x => holder.Value = x);
    }

    public static IDisposable SubscribeValue<TSource, TResult>(
        this IObservable<TSource> observable,
        Func<TSource, TResult> selector,
        IValueHolder<TResult> holder)
    {
        return observable.Subscribe(x => holder.Value = selector(x));
    }
}
