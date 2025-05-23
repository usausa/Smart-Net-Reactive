namespace Smart.ComponentModel;

using System.ComponentModel;
using System.Reactive.Linq;

public static class NotifyPropertyChangedExtensions
{
    public static IObservable<PropertyChangedEventArgs> PropertyChangedAsObservable(
        this INotifyPropertyChanged source)
    {
        return Observable.FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
            h => (_, e) => h(e),
            h => source.PropertyChanged += h,
            h => source.PropertyChanged -= h);
    }

    public static IObservable<T> AsObservable<T>(
        this T source,
        string propertyName)
        where T : INotifyPropertyChanged
    {
        return source
            .PropertyChangedAsObservable()
            .Where(x => x.PropertyName == propertyName)
            .Select(_ => source);
    }

    public static IObservable<T> AsObservable<T>(
        this T source)
        where T : INotifyPropertyChanged
    {
        return source
            .PropertyChangedAsObservable()
            .Select(_ => source);
    }

    public static IObservable<T> AsValueObservable<T>(
        this NotificationValue<T> source)
    {
        return source
            .PropertyChangedAsObservable()
            .Where(x => x.PropertyName == nameof(NotificationValue<>.Value))
            .Select(_ => source.Value);
    }
}
