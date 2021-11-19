namespace Smart.ComponentModel;

using System;
using System.ComponentModel;
using System.Reactive.Linq;

public static class NotifyDataErrorInfoExtensions
{
    public static IObservable<DataErrorsChangedEventArgs> ErrorsChangedAsObservable(
        this INotifyDataErrorInfo source)
    {
        return Observable.FromEvent<EventHandler<DataErrorsChangedEventArgs>, DataErrorsChangedEventArgs>(
            h => (_, e) => h(e),
            h => source.ErrorsChanged += h,
            h => source.ErrorsChanged -= h);
    }

    public static IObservable<T> ErrorsChangedAsObservable<T>(
        this T source,
        string propertyName)
        where T : INotifyDataErrorInfo
    {
        return source
            .ErrorsChangedAsObservable()
            .Where(x => x.PropertyName == propertyName)
            .Select(_ => source);
    }
}
